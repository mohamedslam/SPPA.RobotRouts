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
using System.Xml;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Stimulsoft.Base;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Licenses;
using Stimulsoft.Base.Services;
using Stimulsoft.Base.Zip;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.BarCodes;
using System.Threading;
using Stimulsoft.Report.Dashboard;
using System.Drawing;
using System.Drawing.Imaging;
using Stimulsoft.Report.Helpers;

#if CLOUD
using Stimulsoft.Base.Plans;
#endif

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
using Stimulsoft.System.Security.Cryptography;
#else
using System.Windows.Forms;
using System.Security.Cryptography;
#endif

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
using ImageFormat = Stimulsoft.Drawing.Imaging.ImageFormat;
#endif

namespace Stimulsoft.Report.Export
{
    /// <summary>
    /// A class for the Excel 2007 Export.
    /// </summary>
    [StiServiceBitmap(typeof(StiExportService), "Stimulsoft.Report.Images.Dictionary.ResourceExcel.png")]
    public class StiExcel2007ExportService : StiExportService
    {
        #region StiExportService override
        /// <summary>
		/// Gets or sets a default extension of export. 
		/// </summary>
		public override string DefaultExtension => "xlsx";

        public override StiExportFormat ExportFormat => StiExportFormat.Excel2007;

        /// <summary>
        /// Gets a group of the export in the context menu.
        /// </summary>
        public override string GroupCategory => "Excel";

        /// <summary>
        /// Gets a position of the export in the context menu.
        /// </summary>
        public override int Position => (int)StiExportPosition.Excel2007;

        /// <summary>
        /// Gets an export name in the context menu.
        /// </summary>
        public override string ExportNameInMenu => StiLocalization.Get("Export", "ExportTypeExcel2007File");

        /// <summary>
        /// Exports a document to the stream without dialog of the saving file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream in which report will be exported.</param>
        /// <param name="settings">A settings for the report exporting.</param>
        public override void ExportTo(StiReport report, Stream stream, StiExportSettings settings)
        {
            ExportExcel(report, stream, settings as StiExcelExportSettings);
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
                form["ExportFormat"] = StiExportFormat.Excel2007;

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
        public override string GetFilter() => StiLocalization.Get("FileFilters", "Excel2007Files");
        #endregion

        #region Handlers
        private void Form_Complete(IStiFormRunner form, StiShowDialogCompleteEvetArgs e)
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

                            var settings = new StiExcelExportSettings
                            {
                                ExcelType = StiExcelType.Excel2007,
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
        #endregion

        #region Fields
        private StiReport report;
        private string fileName;
        private bool sendEMail;
        private StiGuiMode guiMode;
        #endregion

        #region this
        private StiMatrix matrix;
        internal StiMatrix Matrix
        {
            get
            {
                return matrix;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating maximum sheet height in rows.
        /// </summary>
        public static int MaximumSheetHeight { get; set; } = 1048574;

        public static bool AllowNativeImageBorders { get; set; } = false;

        private ArrayList fontList = null;
        private ArrayList fillList = null;
        private ArrayList borderList = null;
        private ArrayList xfList = null;
        private ArrayList sstList = null;
        private Hashtable sstHash = null;
        private Hashtable sstHashIsTags = null;
        private ArrayList imageList = null;
        private ArrayList formatList = null;
        private int sstCount = 0;
        private ArrayList sheetNameList = null;
        private int imageListOffset = 0;
        private ArrayList printAreasList = null;
        private ArrayList matrixList = null;
        private ArrayList firstPageIndexList = null;
        private ArrayList hyperlinkList = null;
        private ArrayList minRowList = null;
        private ArrayList maxRowList = null;
        private List<BarCodeLineData> barCodeLinesList = null;

        private bool useOnePageHeaderAndFooter = false;
        private StiDataExportMode dataExportMode = StiDataExportMode.AllBands;
        private bool exportObjectFormatting = true;
        private bool exportEachPageToSheet = false;
        private bool exportHorizontalPageBreaks = false;
        private float imageResolution = 1;
        private float imageQuality = 0.75f;
        private ImageFormat imageFormat = ImageFormat.Jpeg;
        private StiImageCache imageCache = null;
        private StiExcel2007RestrictEditing restrictEditing = StiExcel2007RestrictEditing.No;

        private CultureInfo reportCulture = null;

        private string docCompanyString = null;
        private string docLastModifiedString = null;

        private int xmlIndentation = 1;

        #region struct DataFont
        private struct DataFont
        {
            public string Name;
            public bool Bold;
            public bool Italic;
            public bool Underlined;
            public bool Strikeout;
            public int Height;
            public Color Color;
            public int Charset;
            public int Family;

            public DataFont(
                string name,
                bool bold,
                bool italic,
                bool underlined,
                bool strikeout,
                int height,
                Color color,
                int charset,
                int family)
            {
                this.Name = name;
                this.Bold = bold;
                this.Italic = italic;
                this.Underlined = underlined;
                this.Strikeout = strikeout;
                this.Height = height;
                this.Color = color;
                this.Charset = charset;
                this.Family = family;
            }
        }
        #endregion

        #region struct DataFill
        private struct DataFill
        {
            public string Type;
            public Color FgColor;
            public Color BgColor;

            public DataFill(
                string type,
                Color fgColor,
                Color bgColor)
            {
                this.Type = type;
                this.FgColor = fgColor;
                this.BgColor = bgColor;
            }
        }
        #endregion

        #region struct DataBorder
        private struct DataBorder
        {
            public StiBorderSide BorderLeft;
            public StiBorderSide BorderRight;
            public StiBorderSide BorderTop;
            public StiBorderSide BorderBottom;

            public bool EqualDataBorder(DataBorder db)
            {
                if (Equals(this.BorderLeft, db.BorderLeft) &&
                    Equals(this.BorderRight, db.BorderRight) &&
                    Equals(this.BorderTop, db.BorderTop) &&
                    Equals(this.BorderBottom, db.BorderBottom)) return true;

                return false;
            }

            public DataBorder(
                StiBorderSide borderLeft,
                StiBorderSide borderRight,
                StiBorderSide borderTop,
                StiBorderSide borderBottom)
            {
                this.BorderLeft = borderLeft;
                this.BorderRight = borderRight;
                this.BorderTop = borderTop;
                this.BorderBottom = borderBottom;
            }
        }
        #endregion

        #region struct DataXF
        private struct DataXF
        {
            public int FormatIndex;
            public int FontIndex;
            public int FillIndex;
            public int BorderIndex;
            public int XFId;
            public StiTextHorAlignment HorAlign;
            public StiVertAlignment VertAlign;
            public int TextRotationAngle;
            public bool TextWrapped;
            public bool RightToLeft;
            public bool Editable;

            public bool EqualDataXF(DataXF xf)
            {
                if ((FormatIndex != xf.FormatIndex) ||
                    (FontIndex != xf.FontIndex) ||
                    (FillIndex != xf.FillIndex) ||
                    (BorderIndex != xf.BorderIndex) ||
                    (XFId != xf.XFId) ||
                    (HorAlign != xf.HorAlign) ||
                    (VertAlign != xf.VertAlign) ||
                    (TextRotationAngle != xf.TextRotationAngle) ||
                    (TextWrapped != xf.TextWrapped) ||
                    (RightToLeft != xf.RightToLeft) ||
                    (Editable != xf.Editable)) return false;

                return true;
            }

            public DataXF(
                int FormatIndex,
                int FontIndex,
                int FillIndex,
                int BorderIndex,
                int XFId,
                StiTextHorAlignment HorAlign,
                StiVertAlignment VertAlign,
                int TextRotationAngle,
                bool TextWrapped,
                bool RightToLeft,
                bool Editable)
            {
                this.FormatIndex = FormatIndex;
                this.FontIndex = FontIndex;
                this.FillIndex = FillIndex;
                this.BorderIndex = BorderIndex;
                this.XFId = XFId;
                this.HorAlign = HorAlign;
                this.VertAlign = VertAlign;
                this.TextRotationAngle = TextRotationAngle;
                this.TextWrapped = TextWrapped;
                this.RightToLeft = RightToLeft;
                this.Editable = Editable;
            }
        }
        #endregion

        #region struct ExcelImageData
        public struct ExcelImageData
        {
            public int FirstRowIndex;
            public int FirstRowOffset;
            public int FirstColumnIndex;
            public int FirstColumnOffset;
            public int LastRowIndex;
            public int LastRowOffset;
            public int LastColumnIndex;
            public int LastColumnOffset;
            public int ImageIndex;
            public string Hyperlink;
            public StiBorder Border;

            public ExcelImageData(
                int FirstRowIndex,
                int FirstRowOffset,
                int FirstColumnIndex,
                int FirstColumnOffset,
                int LastRowIndex,
                int LastRowOffset,
                int LastColumnIndex,
                int LastColumnOffset,
                int ImageIndex,
                string Hyperlink,
                StiBorder border)
            {
                this.FirstRowIndex = FirstRowIndex;
                this.FirstRowOffset = FirstRowOffset;
                this.FirstColumnIndex = FirstColumnIndex;
                this.FirstColumnOffset = FirstColumnOffset;
                this.LastRowIndex = LastRowIndex;
                this.LastRowOffset = LastRowOffset;
                this.LastColumnIndex = LastColumnIndex;
                this.LastColumnOffset = LastColumnOffset;
                this.ImageIndex = ImageIndex;
                this.Hyperlink = Hyperlink;
                this.Border = border;
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

        #region struct BarcodeLineData
        internal struct BarCodeLineData
        {
            public int Row;
            public int Column;
            public PointD Position;
            public SizeD Size;
            public Color Color;
        }
        #endregion


        #region GetLineStyle
        private string GetLineStyle(StiBorderSide border)
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
                    if (width > thickWidth) return "thick";
                    if (width > mediumWidth) return "medium";
                    return "thin";

                case StiPenStyle.Dot:
                    if (width > mediumWidth) return "dotted";
                    return "hair";

                case StiPenStyle.Dash:
                    if (width > mediumWidth) return "mediumDashed";
                    return "dashed";

                case StiPenStyle.DashDot:
                    if (width > mediumWidth) return "mediumDashDot";
                    return "dashDot";

                case StiPenStyle.DashDotDot:
                    if (width > mediumWidth) return "mediumDashDotDot";
                    return "dashDotDot";

                case StiPenStyle.Double:
                    return "double";

                default:
                    return null;
            }
        }
        #endregion

        #region GetRefString
        private string GetRefString(int column, int row)
        {
            StringBuilder output = new StringBuilder();
            output.Append(GetRefColumnName(column));
            output.Append((row + 1).ToString());
            return output.ToString();
        }
        private string GetRefAbsoluteString(int column, int row)
        {
            StringBuilder output = new StringBuilder();
            output.Append("$");
            output.Append(GetRefColumnName(column));
            output.Append("$");
            output.Append((row + 1).ToString());
            return output.ToString();
        }
        private string GetRefColumnName(int column)
        {
            StringBuilder output = new StringBuilder();
            int column2 = 0;
            int column3 = 0;
            if (column >= 702)
            {
                int tempColumn = column - 26;
                int column1 = tempColumn / (26 * 26);
                output.Append((char)((byte)'A' + column1 - 1));
                tempColumn -= column1 * 26 * 26;
                column2 = tempColumn / 26;
                column3 = tempColumn % 26;
                output.Append((char)((byte)'A' + column2));
            }
            else
            {
                column2 = column / 26;
                column3 = column % 26;
                if (column2 > 0)
                {
                    output.Append((char)((byte)'A' + column2 - 1));
                }
            }
            output.Append((char)((byte)'A' + column3));
            return output.ToString();
        }
        #endregion

        #region FloatToString
        private string FloatToString(double number)
        {
            string output = number.ToString().Replace(",", ".");
            return output;
        }
        #endregion

        #region StringToUrl
        private string StringToUrl(string input)
        {
            StringBuilder output = new StringBuilder();

            //System.Text.UTF8Encoding enc = new UTF8Encoding();
            //byte[] buf = enc.GetBytes(input);
            //foreach (byte byt in buf)
            //{
            //    if ((byt < 0x20) || (byt > 0x7f) || (wrongUrlSymbols.IndexOf((char)byt) != -1))
            //    {
            //        output.Append(string.Format("%{0:x2}", byt));
            //    }
            //    else
            //    {
            //        output.Append((char)byt);
            //    }
            //}

            foreach (char byt in input)
            {
                if (byt == '\'')
                {
                    continue;
                }
                else
                {
                    if ((byt < 0x20) || (wrongUrlSymbols.IndexOf(byt) != -1))
                    {
                        output.Append(string.Format("%{0:x2}", (int)byt));
                    }
                    else
                    {
                        output.Append(byt);
                    }
                }
            }

            return output.ToString();
        }
        //                                  space "   #   %   &   '   *   ,   :   ;   <   >   ?   [   ^   `   {   |   }   
        //private string wrongUrlSymbols = "\x20\x22\x23\x25\x26\x27\x2a\x2c\x3a\x3b\x3c\x3e\x3f\x5b\x5e\x60\x7b\x7c\x7d";
        private readonly string wrongUrlSymbols = "\x20\x22\x27\x2a\x2c\x3b\x3c\x3e\x5b\x5e\x60\x7b\x7c\x7d";
        #endregion


        #region GetFontNumber
        private int GetFontNumber(DataFont dataIn)
        {
            if (fontList.Count > 0)
            {
                for (int index = 0; index < fontList.Count; index++)
                {
                    if (Equals((DataFont)fontList[index], dataIn))
                    {
                        //is already in table, return number 
                        return index;
                    }
                }
            }
            //add to table, return number 
            fontList.Add(dataIn);
            int temp = fontList.Count - 1;
            return temp;
        }
        #endregion

        #region GetFillNumber
        private int GetFillNumber(DataFill dataIn)
        {
            if (fillList.Count > 0)
            {
                for (int index = 0; index < fillList.Count; index++)
                {
                    if (Equals((DataFill)fillList[index], dataIn))
                    {
                        //is already in table, return number 
                        return index;
                    }
                }
            }
            //add to table, return number 
            fillList.Add(dataIn);
            int temp = fillList.Count - 1;
            return temp;
        }
        #endregion

        #region GetBorderNumber
        private int GetBorderNumber(DataBorder dataIn)
        {
            if (borderList.Count > 0)
            {
                for (int index = 0; index < borderList.Count; index++)
                {
                    if (((DataBorder)borderList[index]).EqualDataBorder(dataIn))
                    {
                        //is already in table, return number 
                        return index;
                    }
                }
            }
            //add to table, return number 
            borderList.Add(dataIn);
            int temp = borderList.Count - 1;
            return temp;
        }
        #endregion

        #region GetXFNumber
        private int GetXFNumber(DataXF dataIn)
        {
            if (xfList.Count > 0)
            {
                for (int index = 0; index < xfList.Count; index++)
                {
                    if (((DataXF)xfList[index]).EqualDataXF(dataIn))
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

        #region GetSSTNumber
        private int GetSSTNumber(string dataIn)
        {
            sstCount++;
            if (sstList.Count > 0)
            {
                if (sstHash.ContainsKey(dataIn))
                {
                    //is already in table, return number 
                    return (int)sstHash[dataIn];
                }
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

        #region ConvertRotatedAlignment
        internal static void ConvertRotatedAlignment(float angle, ref StiTextHorAlignment horAlign, ref StiVertAlignment vertAlign)
        {
            if (angle == 0) return;

            var oldHorAlign = horAlign;
            var oldVertAlign = vertAlign;

            horAlign = StiTextHorAlignment.Center;
            vertAlign = StiVertAlignment.Center;

            if (angle == 90)
            {
                switch (oldHorAlign)
                {
                    case StiTextHorAlignment.Left:  vertAlign = StiVertAlignment.Bottom; break;
                    case StiTextHorAlignment.Right: vertAlign = StiVertAlignment.Top; break;
                }
                switch (oldVertAlign)
                {
                    case StiVertAlignment.Top: horAlign = StiTextHorAlignment.Left; break;
                    case StiVertAlignment.Bottom: horAlign = StiTextHorAlignment.Right; break;
                }
            }
            if (angle == 180)
            {
                switch (oldHorAlign)
                {
                    case StiTextHorAlignment.Left: vertAlign = StiVertAlignment.Top; break;
                    case StiTextHorAlignment.Right: vertAlign = StiVertAlignment.Bottom; break;
                }
                switch (oldVertAlign)
                {
                    case StiVertAlignment.Top: horAlign = StiTextHorAlignment.Right; break;
                    case StiVertAlignment.Bottom: horAlign = StiTextHorAlignment.Left; break;
                }
            }
        }
        #endregion

        private string GetImageFormatExtension(ImageFormat currImageFormat)
        {
            if (currImageFormat == ImageFormat.Png) return "png";
            return "jpeg";
        }

        //conversion from hundredths of inch to twips
        private static double HiToTwips
        {
            get
            {
                //return 14.4 / 20 * 1.028; //correction 2019.10.29
                return 14.4 / 20 * 1.01;
            }
        }

        // 1 point = 1/72 inch
        // 20 twips = 1 point 
        // 75 point = 100 pixels
        // 75 pixels = 10 width of column
        // 100 pixels = 3657 colinfo width
        //
        // 1 twips = (1/20 point) * (100/75 pixels) * (3657/100 width) = 100*3657 / 20*75*100 = 3657 / 1500 = 2.438 colinfo width

        //conversion from twips to units of column
        //const double TwipsToColinfo = 2.438 * 0.921 * 0.075 * 1.03 * 0.99 * 1.01; //correction 2019.10.29
        const double TwipsToColinfo = 2.438 * 0.976 * 0.075;

        private double Convert(double x)
        {
            return (x * HiToTwips);
        }

        private double ConvertToEMU(double x)
        {
            return Math.Round(x * 914400 / 100 * 0.93385);   // 0.96 / 1.028 = 0.93385
        }

        private bool CompareExcellSheetNames(string name1, string name2)
        {
            string st1 = name1;
            if ((st1 == null) || (st1.Length == 0)) st1 = string.Empty;
            string st2 = name2;
            if ((st2 == null) || (st2.Length == 0)) st2 = string.Empty;
            return (st1 == st2);
        }

        private string RemoveControlCharacters(string input)
        {
            StringBuilder sbb = new StringBuilder();
            foreach (char ch in input)
            {
                if (!char.IsControl(ch) || (ch == '\t') || (ch == '\n'))
                {
                    sbb.Append(ch);
                }
            }
            return sbb.ToString();
        }

        #region prepare data
        private void PrepareData()
        {
            fontList = new ArrayList();
            fillList = new ArrayList();
            borderList = new ArrayList();
            xfList = new ArrayList();
            sstList = new ArrayList();
            sstHash = new Hashtable();
            sstHashIsTags = new Hashtable();
            imageList = new ArrayList();
            formatList = new ArrayList();
            sheetNameList = new ArrayList();
            sstCount = 0;
            printAreasList = new ArrayList();
            matrixList = new ArrayList();
            firstPageIndexList = new ArrayList();
            minRowList = new ArrayList();
            maxRowList = new ArrayList();

            imageCache = new StiImageCache(StiOptions.Export.Excel.AllowImageComparer, imageFormat, imageQuality);

            #region add default
            DataFont tempFont = new DataFont("Arial", false, false, false, false, 10, Color.Black, 0x01, 0);
            GetFontNumber(tempFont);

            DataFill tempFill = new DataFill("none", Color.Transparent, Color.Black);
            GetFillNumber(tempFill);
            tempFill = new DataFill("gray125", Color.Transparent, Color.Black);
            GetFillNumber(tempFill);

            DataBorder tempBorder = new DataBorder(null, null, null, null);
            GetBorderNumber(tempBorder);

            DataXF tempXF = new DataXF(0, 0, 0, 0, 0, StiTextHorAlignment.Left, StiVertAlignment.Bottom, 0, false, false, false);
            GetXFNumber(tempXF);
            #endregion

        }
        #endregion


        #region WriteContentTypes
        private MemoryStream WriteContentTypes()
        {
            MemoryStream ms = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(ms, Encoding.UTF8);
            writer.Indentation = (xmlIndentation < 0 ? 0 : xmlIndentation);
            writer.Formatting = (xmlIndentation < 0 ? Formatting.None : Formatting.Indented);
            writer.WriteStartDocument();
            writer.WriteStartElement("Types");
            writer.WriteAttributeString("xmlns", "http://schemas.openxmlformats.org/package/2006/content-types");

            writer.WriteStartElement("Default");
            writer.WriteAttributeString("Extension", "rels");
            writer.WriteAttributeString("ContentType", "application/vnd.openxmlformats-package.relationships+xml");
            writer.WriteEndElement();
            writer.WriteStartElement("Default");
            writer.WriteAttributeString("Extension", "xml");
            writer.WriteAttributeString("ContentType", "application/xml");
            writer.WriteEndElement();
            writer.WriteStartElement("Default");
            writer.WriteAttributeString("Extension", "jpeg");
            writer.WriteAttributeString("ContentType", "image/jpeg");
            writer.WriteEndElement();

            writer.WriteStartElement("Default");
            writer.WriteAttributeString("Extension", "png");
            writer.WriteAttributeString("ContentType", "image/png");
            writer.WriteEndElement();
            writer.WriteStartElement("Default");
            writer.WriteAttributeString("Extension", "vml");
            writer.WriteAttributeString("ContentType", "application/vnd.openxmlformats-officedocument.vmlDrawing");
            writer.WriteEndElement();

            writer.WriteStartElement("Override");
            writer.WriteAttributeString("PartName", "/docProps/app.xml");
            writer.WriteAttributeString("ContentType", "application/vnd.openxmlformats-officedocument.extended-properties+xml");
            writer.WriteEndElement();
            writer.WriteStartElement("Override");
            writer.WriteAttributeString("PartName", "/docProps/core.xml");
            writer.WriteAttributeString("ContentType", "application/vnd.openxmlformats-package.core-properties+xml");
            writer.WriteEndElement();
            writer.WriteStartElement("Override");
            writer.WriteAttributeString("PartName", "/xl/workbook.xml");
            writer.WriteAttributeString("ContentType", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml");
            writer.WriteEndElement();
            for (int index = 0; index < sheetNameList.Count; index++)
            {
                writer.WriteStartElement("Override");
                writer.WriteAttributeString("PartName", string.Format("/xl/worksheets/sheet{0}.xml", index + 1));
                writer.WriteAttributeString("ContentType", "application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml");
                writer.WriteEndElement();
            }
            //			writer.WriteStartElement("Override");
            //			writer.WriteAttributeString("PartName", "/xl/theme/theme1.xml" );
            //			writer.WriteAttributeString("ContentType", "application/vnd.openxmlformats-officedocument.theme+xml");
            //			writer.WriteEndElement();
            writer.WriteStartElement("Override");
            writer.WriteAttributeString("PartName", "/xl/styles.xml");
            writer.WriteAttributeString("ContentType", "application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml");
            writer.WriteEndElement();
            for (int index = 0; index < sheetNameList.Count; index++)
            {
                writer.WriteStartElement("Override");
                writer.WriteAttributeString("PartName", string.Format("/xl/drawings/drawing{0}.xml", index + 1));
                writer.WriteAttributeString("ContentType", "application/vnd.openxmlformats-officedocument.drawing+xml");
                writer.WriteEndElement();
            }
            if (sstList.Count > 0)
            {
                writer.WriteStartElement("Override");
                writer.WriteAttributeString("PartName", "/xl/sharedStrings.xml");
                writer.WriteAttributeString("ContentType", "application/vnd.openxmlformats-officedocument.spreadsheetml.sharedStrings+xml");
                writer.WriteEndElement();
            }

            writer.WriteFullEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return ms;
        }
        #endregion

        #region WriteMainRels
        private MemoryStream WriteMainRels()
        {
            MemoryStream ms = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(ms, Encoding.UTF8);
            writer.Indentation = (xmlIndentation < 0 ? 0 : xmlIndentation);
            writer.Formatting = (xmlIndentation < 0 ? Formatting.None : Formatting.Indented);
            writer.WriteStartDocument();
            writer.WriteStartElement("Relationships");
            writer.WriteAttributeString("xmlns", "http://schemas.openxmlformats.org/package/2006/relationships");

            writer.WriteStartElement("Relationship");
            writer.WriteAttributeString("Id", "rId1");
            writer.WriteAttributeString("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument");
            writer.WriteAttributeString("Target", "xl/workbook.xml");
            writer.WriteEndElement();
            writer.WriteStartElement("Relationship");
            writer.WriteAttributeString("Id", "rId2");
            writer.WriteAttributeString("Type", "http://schemas.openxmlformats.org/package/2006/relationships/metadata/core-properties");
            writer.WriteAttributeString("Target", "docProps/core.xml");
            writer.WriteEndElement();
            writer.WriteStartElement("Relationship");
            writer.WriteAttributeString("Id", "rId3");
            writer.WriteAttributeString("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/extended-properties");
            writer.WriteAttributeString("Target", "docProps/app.xml");
            writer.WriteEndElement();

            writer.WriteFullEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return ms;
        }
        #endregion

        #region WriteDocPropsApp
        private MemoryStream WriteDocPropsApp()
        {
            MemoryStream ms = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(ms, Encoding.UTF8);
            writer.Indentation = (xmlIndentation < 0 ? 0 : xmlIndentation);
            writer.Formatting = (xmlIndentation < 0 ? Formatting.None : Formatting.Indented);
            writer.WriteStartDocument();
            writer.WriteStartElement("Properties");
            writer.WriteAttributeString("xmlns", "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties");
            writer.WriteAttributeString("xmlns:vt", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes");

            writer.WriteElementString("Application", "Microsoft Excel");
            writer.WriteElementString("DocSecurity", "0");
            writer.WriteElementString("ScaleCrop", "false");
            writer.WriteElementString("Company", docCompanyString ?? "");
            writer.WriteElementString("LinksUpToDate", "false");
            writer.WriteElementString("SharedDoc", "false");
            writer.WriteElementString("HyperlinksChanged", "false");
            writer.WriteElementString("AppVersion", "12.0000");

            writer.WriteStartElement("HeadingPairs");
            writer.WriteStartElement("vt:vector");
            writer.WriteAttributeString("size", "4");
            writer.WriteAttributeString("baseType", "variant");
            writer.WriteStartElement("vt:variant");
            writer.WriteElementString("vt:lpstr", "Worksheets");
            writer.WriteEndElement();
            writer.WriteStartElement("vt:variant");
            writer.WriteElementString("vt:i4", string.Format("{0}", sheetNameList.Count));
            writer.WriteEndElement();
            writer.WriteStartElement("vt:variant");
            writer.WriteElementString("vt:lpstr", "Named Ranges");
            writer.WriteEndElement();
            writer.WriteStartElement("vt:variant");
            writer.WriteElementString("vt:i4", string.Format("{0}", sheetNameList.Count));
            writer.WriteEndElement();   //vt:variant
            writer.WriteEndElement();   //vt:vector
            writer.WriteEndElement();   //HeadingPair

            writer.WriteStartElement("TitlesOfParts");
            writer.WriteStartElement("vt:vector");
            writer.WriteAttributeString("size", string.Format("{0}", sheetNameList.Count * 2));
            writer.WriteAttributeString("baseType", "lpstr");
            for (int index = 0; index < sheetNameList.Count; index++)
            {
                writer.WriteElementString("vt:lpstr", (string)sheetNameList[index]);
            }
            for (int index = 0; index < sheetNameList.Count; index++)
            {
                writer.WriteElementString("vt:lpstr", string.Format("'{0}'!Print_Area", (string)sheetNameList[index]));
            }
            writer.WriteEndElement();
            writer.WriteEndElement();

            writer.WriteFullEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return ms;
        }
        #endregion

        #region WriteDocPropsCore
        private MemoryStream WriteDocPropsCore()
        {
            MemoryStream ms = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(ms, Encoding.UTF8);
            writer.Indentation = (xmlIndentation < 0 ? 0 : xmlIndentation);
            writer.Formatting = (xmlIndentation < 0 ? Formatting.None : Formatting.Indented);
            writer.WriteStartDocument();
            writer.WriteStartElement("cp:coreProperties");
            writer.WriteAttributeString("xmlns:cp", "http://schemas.openxmlformats.org/package/2006/metadata/core-properties");
            writer.WriteAttributeString("xmlns:dc", "http://purl.org/dc/elements/1.1/");
            writer.WriteAttributeString("xmlns:dcterms", "http://purl.org/dc/terms/");
            writer.WriteAttributeString("xmlns:dcmitype", "http://purl.org/dc/dcmitype/");
            writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");

            string dateTime = string.Format("{0}", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            //string creator = Stimulsoft.Report.Export.StiExportUtils.GetReportVersion();

            writer.WriteElementString("dc:title", report.ReportName ?? "");
            writer.WriteElementString("dc:subject", report.ReportAlias ?? "");
            writer.WriteElementString("dc:creator", report.ReportAuthor ?? "");
            writer.WriteElementString("dc:description", report.ReportDescription ?? "");
            writer.WriteElementString("cp:lastModifiedBy", docLastModifiedString ?? "");
            writer.WriteStartElement("dcterms:created");
            writer.WriteAttributeString("xsi:type", "dcterms:W3CDTF");
            writer.WriteString(dateTime);
            writer.WriteEndElement();
            writer.WriteStartElement("dcterms:modified");
            writer.WriteAttributeString("xsi:type", "dcterms:W3CDTF");
            writer.WriteString(dateTime);
            writer.WriteEndElement();

            writer.WriteFullEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return ms;
        }
        #endregion

        #region WriteWorkbookRels
        private MemoryStream WriteWorkbookRels()
        {
            MemoryStream ms = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(ms, Encoding.UTF8);
            writer.Indentation = (xmlIndentation < 0 ? 0 : xmlIndentation);
            writer.Formatting = (xmlIndentation < 0 ? Formatting.None : Formatting.Indented);
            writer.WriteStartDocument();
            writer.WriteStartElement("Relationships");
            writer.WriteAttributeString("xmlns", "http://schemas.openxmlformats.org/package/2006/relationships");

            for (int index = 0; index < sheetNameList.Count; index++)
            {
                writer.WriteStartElement("Relationship");
                writer.WriteAttributeString("Id", string.Format("rId{0}", index + 1));
                writer.WriteAttributeString("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet");
                writer.WriteAttributeString("Target", string.Format("worksheets/sheet{0}.xml", index + 1));
                writer.WriteEndElement();
            }
            //			writer.WriteStartElement("Relationship");
            //			writer.WriteAttributeString("Id", string.Format("rId{0}", sheetNameList.Count + 1));
            //			writer.WriteAttributeString("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/theme" );
            //			writer.WriteAttributeString("Target", "theme/theme1.xml");
            //			writer.WriteEndElement();
            writer.WriteStartElement("Relationship");
            writer.WriteAttributeString("Id", string.Format("rId{0}", sheetNameList.Count + 2));
            writer.WriteAttributeString("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles");
            writer.WriteAttributeString("Target", "styles.xml");
            writer.WriteEndElement();

            if (sstCount > 0)
            {
                writer.WriteStartElement("Relationship");
                writer.WriteAttributeString("Id", string.Format("rId{0}", sheetNameList.Count + 3));
                writer.WriteAttributeString("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/sharedStrings");
                writer.WriteAttributeString("Target", "sharedStrings.xml");
                writer.WriteEndElement();
            }

            writer.WriteFullEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return ms;
        }
        #endregion

        #region WriteWorkbook
        private MemoryStream WriteWorkbook()
        {
            MemoryStream ms = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(ms, Encoding.UTF8);
            writer.Indentation = (xmlIndentation < 0 ? 0 : xmlIndentation);
            writer.Formatting = (xmlIndentation < 0 ? Formatting.None : Formatting.Indented);
            writer.WriteStartDocument();
            writer.WriteStartElement("workbook");
            writer.WriteAttributeString("xmlns", "http://schemas.openxmlformats.org/spreadsheetml/2006/main");
            writer.WriteAttributeString("xmlns:r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");

            writer.WriteStartElement("fileVersion");
            writer.WriteAttributeString("appName", "xl");
            writer.WriteAttributeString("lastEdited", "4");
            writer.WriteAttributeString("lowestEdited", "4");
            writer.WriteAttributeString("rupBuild", "4505");
            writer.WriteEndElement();

            writer.WriteStartElement("workbookPr");
            writer.WriteAttributeString("defaultThemeVersion", "124226");
            writer.WriteEndElement();

            writer.WriteStartElement("bookViews");
            writer.WriteStartElement("workbookView");
            writer.WriteAttributeString("xWindow", "120");
            writer.WriteAttributeString("yWindow", "15");
            writer.WriteAttributeString("windowWidth", "18975");
            writer.WriteAttributeString("windowHeight", "11955");
            writer.WriteEndElement();
            writer.WriteEndElement();

            writer.WriteStartElement("sheets");
            for (int index = 0; index < sheetNameList.Count; index++)
            {
                writer.WriteStartElement("sheet");
                writer.WriteAttributeString("name", (string)sheetNameList[index]);
                writer.WriteAttributeString("sheetId", string.Format("{0}", index + 1));
                writer.WriteAttributeString("r:id", string.Format("rId{0}", index + 1));
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteStartElement("definedNames");
            for (int index = 0; index < sheetNameList.Count; index++)
            {
                Size area = (Size)printAreasList[index];
                if (area.Height > 1048575) area.Height = 1048575;
                if (area.Width > 16383) area.Width = 16383;
                string sref = GetRefAbsoluteString(area.Width, area.Height);

                writer.WriteStartElement("definedName");
                writer.WriteAttributeString("name", "_xlnm.Print_Area");
                writer.WriteAttributeString("localSheetId", string.Format("{0}", index));
                writer.WriteString(string.Format("'{0}'!$A$1:{1}", (string)sheetNameList[index], sref));
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteStartElement("calcPr");
            writer.WriteAttributeString("calcId", "124519");
            writer.WriteEndElement();

            writer.WriteFullEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return ms;
        }
        #endregion

        #region WriteSheetRels
        private MemoryStream WriteSheetRels(int indexSheet)
        {
            MemoryStream ms = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(ms, Encoding.UTF8);
            writer.Indentation = (xmlIndentation < 0 ? 0 : xmlIndentation);
            writer.Formatting = (xmlIndentation < 0 ? Formatting.None : Formatting.Indented);
            writer.WriteStartDocument();
            writer.WriteStartElement("Relationships");
            writer.WriteAttributeString("xmlns", "http://schemas.openxmlformats.org/package/2006/relationships");

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

            if ((imageList.Count - imageListOffset > 0) || (barCodeLinesList.Count > 0) || isTrial)
            {
                writer.WriteStartElement("Relationship");
                //writer.WriteAttributeString("Id", string.Format("rId{0}", indexSheet + 1));
                writer.WriteAttributeString("Id", string.Format("rId{0}", 1));
                writer.WriteAttributeString("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/drawing");
                writer.WriteAttributeString("Target", string.Format("../drawings/drawing{0}.xml", indexSheet + 1));
                writer.WriteEndElement();
            }

            if (hyperlinkList.Count > 0)
            {
                for (int index = 0; index < hyperlinkList.Count; index++)
                {
                    writer.WriteStartElement("Relationship");
                    writer.WriteAttributeString("Id", string.Format("rId{0}", 2 + index));
                    writer.WriteAttributeString("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/hyperlink");
                    writer.WriteAttributeString("Target", (string)hyperlinkList[index]);
                    writer.WriteAttributeString("TargetMode", "External");
                    writer.WriteEndElement();
                }
            }

            if (isTrial)
            {
                writer.WriteStartElement("Relationship");
                writer.WriteAttributeString("Id", "dId1");
                writer.WriteAttributeString("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/vmlDrawing");
                writer.WriteAttributeString("Target", "../drawings/vmlDrawingAdditional.vml");
                writer.WriteEndElement();
            }
            #endregion

            writer.WriteFullEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return ms;
        }
        #endregion

        #region WriteSheet
        private MemoryStream WriteSheet(int indexSheet, StiPage page)
        {
            MemoryStream ms = new Tools.StiCachedStream();
            XmlTextWriter writer = new XmlTextWriter(ms, Encoding.UTF8);
            writer.Indentation = (xmlIndentation < 0 ? 0 : xmlIndentation);
            writer.Formatting = (xmlIndentation < 0 ? Formatting.None : Formatting.Indented);
            writer.WriteStartDocument();
            writer.WriteStartElement("worksheet");
            writer.WriteAttributeString("xmlns", "http://schemas.openxmlformats.org/spreadsheetml/2006/main");
            writer.WriteAttributeString("xmlns:r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            //writer.WriteAttributeString("xmlns:mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            //writer.WriteAttributeString("xmlns:x14ac", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac");
            //writer.WriteAttributeString("mc:Ignorable", "x14ac");

            if (StiOptions.Export.Excel.NumberOfPagesInWideToFit > 0 || StiOptions.Export.Excel.NumberOfPagesInHeightToFit > 0)
            {
                writer.WriteStartElement("sheetPr");
                writer.WriteStartElement("pageSetUpPr");
                writer.WriteAttributeString("fitToPage", "1");
                writer.WriteEndElement();
                writer.WriteEndElement();
            }

            matrix = (StiMatrix)matrixList[indexSheet];
            int minRowIndex = (int)minRowList[indexSheet];
            int maxRowIndex = (int)maxRowList[indexSheet];

            Size printAreaSize = new Size(Matrix.CoordX.Count - 2, maxRowIndex - minRowIndex - 1);
            if (printAreaSize.Width < 0) printAreaSize.Width = 0;
            if (printAreaSize.Height < 0) printAreaSize.Height = 0;
            printAreasList.Add(printAreaSize);

            ArrayList numberStoredAsText = new ArrayList();

            writer.WriteStartElement("dimension");
            writer.WriteAttributeString("ref", string.Format("{0}:{1}",
                GetRefString(0, 0),
                GetRefString(printAreaSize.Width, printAreaSize.Height)));
            writer.WriteEndElement();

            #region check for locked components for FreezePanes feature
            int paneX = 0;
            int paneY = 0;
            if (StiOptions.Export.Excel.AllowFreezePanes)
            {
                for (int indexRow = minRowIndex; indexRow < maxRowIndex; indexRow++)
                {
                    for (int indexColumn = 0; indexColumn < Matrix.CoordX.Count - 1; indexColumn++)
                    {
                        StiCell cell = Matrix.Cells[indexRow, indexColumn];
                        if (cell != null && cell.Component != null && (cell.Component.Locked || (cell.Component.TagValue != null && cell.Component.TagValue.ToString().Contains("excelfreezepanes"))))
                        {
                            string stPlacement = cell.Component.ComponentPlacement;
                            if (stPlacement != null && stPlacement.Length > 0 && (stPlacement.StartsWith("rt") || stPlacement.StartsWith("ph") || stPlacement.StartsWith("h")))
                            {
                                //use the bottom left corner of the component
                                paneX = cell.Left;
                                paneY = cell.Top + cell.Height + 1;
                                break;
                            }
                        }
                    }
                }
            }
            #endregion

            #region sheetViews
            writer.WriteStartElement("sheetViews");
            writer.WriteStartElement("sheetView");
            if (StiOptions.Export.Excel.ColumnsRightToLeft)
            {
                writer.WriteAttributeString("rightToLeft", "1");
            }
            if (!StiOptions.Export.Excel.ShowGridLines)
            {
                writer.WriteAttributeString("showGridLines", "0");
            }
            if (StiOptions.Export.Excel.SheetViewMode == StiExcelSheetViewMode.PageBreakPreview)
            {
                writer.WriteAttributeString("view", "pageBreakPreview");
                writer.WriteAttributeString("zoomScale", "60");
                writer.WriteAttributeString("zoomScaleNormal", "100");
            }
            if (StiOptions.Export.Excel.SheetViewMode == StiExcelSheetViewMode.PageLayout)
            {
                writer.WriteAttributeString("view", "pageLayout");
                writer.WriteAttributeString("zoomScaleNormal", "100");
            }
            if (indexSheet == 0)
            {
                writer.WriteAttributeString("tabSelected", "1");
            }
            writer.WriteAttributeString("workbookViewId", "0");
            if (paneX == 0 && paneY == 0)
            {
                writer.WriteStartElement("selection");
                writer.WriteAttributeString("activeCell", "A1");
                writer.WriteAttributeString("sqref", "A1");
                writer.WriteEndElement();
            }
            else
            {
                if (paneX == 0)
                {
                    writer.WriteStartElement("pane");
                    writer.WriteAttributeString("ySplit", paneY.ToString());
                    writer.WriteAttributeString("topLeftCell", GetRefString(paneX, paneY));
                    writer.WriteAttributeString("activePane", "bottomLeft");
                    writer.WriteAttributeString("state", "frozen");
                    writer.WriteEndElement();
                    writer.WriteStartElement("selection");
                    writer.WriteAttributeString("pane", "bottomLeft");
                    writer.WriteAttributeString("activeCell", GetRefString(0, paneY));
                    writer.WriteAttributeString("sqref", GetRefString(0, paneY));
                    writer.WriteEndElement();
                }
                else
                {
                    writer.WriteStartElement("pane");
                    writer.WriteAttributeString("xSplit", paneX.ToString());
                    writer.WriteAttributeString("ySplit", paneY.ToString());
                    writer.WriteAttributeString("topLeftCell", GetRefString(paneX, paneY));
                    writer.WriteAttributeString("activePane", "bottomRight");
                    writer.WriteAttributeString("state", "frozen");
                    writer.WriteEndElement();
                    writer.WriteStartElement("selection");
                    writer.WriteAttributeString("pane", "topRight");
                    writer.WriteAttributeString("activeCell", GetRefString(paneX, 0));
                    writer.WriteAttributeString("sqref", GetRefString(paneX, 0));
                    writer.WriteEndElement();
                    writer.WriteStartElement("selection");
                    writer.WriteAttributeString("pane", "bottomLeft");
                    writer.WriteAttributeString("activeCell", GetRefString(0, paneY));
                    writer.WriteAttributeString("sqref", GetRefString(0, paneY));
                    writer.WriteEndElement();
                    writer.WriteStartElement("selection");
                    writer.WriteAttributeString("pane", "bottomRight");
                    writer.WriteAttributeString("activeCell", GetRefString(paneX, paneY));
                    writer.WriteAttributeString("sqref", GetRefString(paneX, paneY));
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();
            writer.WriteEndElement();
            #endregion

            writer.WriteStartElement("sheetFormatPr");
            writer.WriteAttributeString("defaultRowHeight", string.Format("{0}", 15));
            writer.WriteEndElement();

            #region Background color
            int backgroundStyle = 0;
            Color backgroundColor = StiBrush.ToColor(page.Brush);
            if (!(backgroundColor.Equals(Color.White) || backgroundColor.A == 0))
            {
                DataFill tempFill = new DataFill("solid", backgroundColor, Color.Transparent);
                int fillIndex = GetFillNumber(tempFill);

                DataXF tempXF = new DataXF(
                                0,
                                0,
                                fillIndex,
                                0,
                                0,
                                StiTextHorAlignment.Left,
                                StiVertAlignment.Bottom,
                                0,
                                false,
                                false,
                                false);
                backgroundStyle = GetXFNumber(tempXF);
            }
            #endregion

            #region columns widths
            if (exportObjectFormatting && (Matrix.CoordX.Count > 1))
            {
                writer.WriteStartElement("cols");
                for (int indexCol = 0; indexCol < Matrix.CoordX.Count - 1; indexCol++)
                {
                    double value2 = (double)Matrix.CoordX.GetByIndex(indexCol + 1);
                    double value1 = (double)Matrix.CoordX.GetByIndex(indexCol);
                    double colWidth = Convert(value2 - value1) * TwipsToColinfo;
                    writer.WriteStartElement("col");
                    writer.WriteAttributeString("min", string.Format("{0}", indexCol + 1));
                    writer.WriteAttributeString("max", string.Format("{0}", indexCol + 1));
                    writer.WriteAttributeString("width", string.Format("{0}", colWidth));
                    writer.WriteAttributeString("customWidth", "1");
                    if (backgroundStyle > 0)
                    {
                        writer.WriteAttributeString("style", $"{backgroundStyle}");
                    }
                    writer.WriteEndElement();
                }
                if (backgroundStyle > 0)
                {
                    writer.WriteStartElement("col");
                    writer.WriteAttributeString("min", string.Format("{0}", Matrix.CoordX.Count));
                    writer.WriteAttributeString("max", string.Format("{0}", 16384));
                    writer.WriteAttributeString("width", "8.7265625");
                    writer.WriteAttributeString("style", $"{backgroundStyle}");
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            #endregion

            #region rows
            RichTextBox richtextForConvert = null;
            bool[,] readyCells = new bool[maxRowIndex + 1, Matrix.CoordX.Count];
            int[,] mergedCellsStyle = new int[maxRowIndex + 1, Matrix.CoordX.Count];
            ArrayList mergedCells = new ArrayList();
            ArrayList hlinks = new ArrayList();

            double progressScale = Math.Max(Matrix.CoordY.Count / 200f, 1f);
            int progressValue = 0;

            writer.WriteStartElement("sheetData");
            for (int indexRow = minRowIndex; indexRow < maxRowIndex; indexRow++)
            {
                int currentProgress = (int)(indexRow / progressScale);
                if (currentProgress > progressValue)
                {
                    progressValue = currentProgress;
                    InvokeExporting(indexRow, matrix.CoordY.Count, CurrentPassNumber, MaximumPassNumber);
                }

                double height = ((double)Matrix.CoordY.GetByIndex(indexRow + 1)
                    - (double)Matrix.CoordY.GetByIndex(indexRow));

                writer.WriteStartElement("row");
                writer.WriteAttributeString("r", string.Format("{0}", indexRow + 1 - minRowIndex));
                writer.WriteAttributeString("spans", string.Format("{0}:{1}", 1, Matrix.CoordX.Count - 1));
                if (exportObjectFormatting)
                {
                    writer.WriteAttributeString("ht", string.Format("{0}", Convert(height)));
                    writer.WriteAttributeString("customHeight", "1");
                }

                for (int indexColumn = 0; indexColumn < Matrix.CoordX.Count - 1; indexColumn++)
                {
                    StiCell cell = Matrix.Cells[indexRow, indexColumn];

                    if ((!readyCells[indexRow, indexColumn]) && (cell != null))
                    {
                        #region readyCells & cells exists
                        readyCells[indexRow, indexColumn] = true;
                        StiRichText rtf = cell.Component as StiRichText;
                        StiText textComp = cell.Component as StiText;

                        StiBarCode barCode = cell.Component as StiBarCode;

                        string str = cell.Text;
                        if ((rtf != null) && (rtf.RtfText != string.Empty))
                        {
                            if (richtextForConvert == null) richtextForConvert = new Controls.StiRichTextBox(false);
                            rtf.GetPreparedText(richtextForConvert);
                            str = richtextForConvert.Text;
                        }

                        StiCheckBox checkComp = cell.Component as StiCheckBox;
                        bool hasCheckBoxExcelDataValue = false;
                        bool isCheckBox = false;
                        if (checkComp != null)
                        {
                            if ((checkComp.ExcelDataValue != null) && (checkComp.ExcelDataValue.Length > 0))
                            {
                                hasCheckBoxExcelDataValue = true;
                                str = checkComp.ExcelDataValue;
                            }
                        }

                        #region Hyperlink
                        string hyperlink = null;
                        if (cell.Component != null && cell.Component.HyperlinkValue != null)
                        {
                            hyperlink = cell.Component.HyperlinkValue.ToString().Trim();
                            if (hyperlink.Length > 0 && !hyperlink.StartsWith("javascript:"))
                            {
                                string description = str;
                                if (description == null || description.Length == 0) description = hyperlink;
                                CellRangeAddress range = new CellRangeAddress(
                                    indexRow - minRowIndex,
                                    indexRow - minRowIndex + cell.Height,
                                    indexColumn,
                                    indexColumn + cell.Width);
                                HlinkData hl = new HlinkData(range, description, hyperlink);
                                hlinks.Add(hl);
                            }
                        }
                        #endregion

                        bool isProcessed = false;

                        #region BarCode
                        bool isBarCode = false;     // barCode != null;
                        if (isBarCode)
                        {
                            StiExcel2007GeomWriter excelGeomWriter = new StiExcel2007GeomWriter(barCodeLinesList, indexRow, indexColumn);
                            StiBarCodeExportPainter barCodePainter = new StiBarCodeExportPainter(excelGeomWriter);
                            if (!string.IsNullOrEmpty(barCode.CodeValue) && barCode.Page != null)
                            {
                                RectangleF rectf = report.Unit.ConvertToHInches(barCode.ClientRectangle).ToRectangleF();
                                barCode.BarCodeType.Draw(barCodePainter, barCode, rectf, 1);
                            }
                            isProcessed = true;
                        }
                        #endregion

                        #region Image
                        bool isImage = false;
                        IStiExportImage exportImage = cell.Component as IStiExportImage;
                        if (!isProcessed && !isCheckBox && exportImage != null)
                        {
                            IStiExportImageExtended exportImageExtended = exportImage as IStiExportImageExtended;

                            float zoom = imageResolution;

                            if (StiOptions.Export.Excel.UseImageResolution &&
                                (exportImage as StiImage) != null && (exportImage as StiImage).ExistImageToDraw())
                            {
                                using (var gdiImage = (exportImage as StiImage).TryTakeGdiImageToDraw())
                                {
                                    float dpix = gdiImage.HorizontalResolution;
                                    if (dpix >= 50 && dpix <= 600) zoom *= dpix / 100f;
                                }
                            }

                            Image image = null;
                            object imageTag = cell.Component.TagValue;
                            if (cell.Component.IsExportAsImage(StiExportFormat.Excel2007))
                            {
                                if (imageTag == null)
                                {
                                    cell.Component.TagValue = Painters.StiViewGdiPainter.StiGetRawFormatTag;
                                }

                                Thread.CurrentThread.CurrentCulture = reportCulture;

                                if (exportImageExtended != null && exportImageExtended.IsExportAsImage(StiExportFormat.Excel2007))
                                    image = exportImageExtended.GetImage(ref zoom, imageFormat == null || imageFormat == ImageFormat.Png ? StiExportFormat.ImagePng : StiExportFormat.Excel);
                                else image = exportImage.GetImage(ref zoom);

                                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
                                if (imageTag == null)
                                {
                                    imageTag = cell.Component.TagValue;
                                    cell.Component.TagValue = null;
                                }
                            }

                            if (image != null)
                            {
                                Image img = Matrix.GetRealImageData(cell, image);
                                if (img != null) image = img;

                                var currImageFormat = imageFormat;
                                if (imageFormat == null)
                                {
                                    if (ImageFormat.MemoryBmp.Equals(image.RawFormat) || ImageFormat.Png.Equals(image.RawFormat) || ImageFormat.Gif.Equals(image.RawFormat) || ImageFormat.Bmp.Equals(image.RawFormat))
                                        currImageFormat = ImageFormat.Png;
                                    else
                                        currImageFormat = ImageFormat.Jpeg;

                                    //temporary check while the GetImage method redraws the pictures and accordingly changes its type to MemoryBitmap.
                                    //when there is a native transfer of images, it will be necessary to rewrite this check.
                                    var stiImage = cell.Component as StiImage;
                                    if (stiImage != null && ImageFormat.MemoryBmp.Equals(image.RawFormat))
                                    {
                                        if (imageTag is ImageFormat)
                                        {
                                            currImageFormat = ImageFormat.Png.Equals(imageTag) ? ImageFormat.Png : ImageFormat.Jpeg;
                                        }
                                        else
                                        {
                                            currImageFormat = ImageFormat.Jpeg;
                                        }
                                    }
                                }
                                int imageIndex = imageCache.AddImageInt(image, currImageFormat);

                                ExcelImageData imageData = new ExcelImageData(
                                    indexRow - minRowIndex,
                                    0,
                                    indexColumn,
                                    0,
                                    indexRow - minRowIndex + 1 + cell.Height,
                                    0,
                                    indexColumn + 1 + cell.Width,
                                    0,
                                    imageIndex,
                                    hyperlink,
                                    AllowNativeImageBorders ? (cell.Component as IStiBorder)?.Border : null);
                                imageList.Add(imageData);

                                image.Dispose();

                                isImage = true;
                                isProcessed = true;
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
                                numberStoredAsText.Add(GetRefString(indexColumn, indexRow));
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
                        string currencySymbol = reportCulture.NumberFormat.CurrencySymbol;
                        bool currencyPositionBefore = reportCulture.NumberFormat.CurrencyPositivePattern == 0 || reportCulture.NumberFormat.CurrencyPositivePattern == 2;
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
                                        else
                                        {
                                            currencySymbol = string.Empty;
                                        }
                                    }
                                    #endregion
                                }
                            }
                        }
                        #endregion

                        if (!(isFormatCurrency || isFormatNumeric || isFormatPercent || isFormatDate || isFormatTime))
                        {
                            isDefaultFormat = true;
                        }

                        if (isFormatCurrency && isDefaultFormat) isDefaultFormat = false;	//Excel2007 do not have default format for currency

                        if (hideZeros && !string.IsNullOrEmpty(inputFormat))
                        {
                            if (isDefaultFormat && isFormatNumeric) groupDigits = 3;
                            isDefaultFormat = false;
                        }

                        if (isExcelText) isDefaultFormat = true;

                        #region make format string
                        if (isFormatDate || isFormatTime)
                        {
                            #region DateTime
                            string dtFormat = inputFormat.Substring(1);
                            switch (dtFormat)
                            {
                                case "d":
                                    outputFormat = reportCulture.DateTimeFormat.ShortDatePattern;
                                    break;
                                case "D":
                                    outputFormat = reportCulture.DateTimeFormat.LongDatePattern;
                                    break;
                                case "f":
                                    outputFormat = reportCulture.DateTimeFormat.LongDatePattern + " " + reportCulture.DateTimeFormat.ShortTimePattern;
                                    break;
                                case "F":
                                    outputFormat = reportCulture.DateTimeFormat.FullDateTimePattern;
                                    break;
                                case "g":
                                    outputFormat = reportCulture.DateTimeFormat.ShortDatePattern + " " + reportCulture.DateTimeFormat.ShortTimePattern;
                                    break;
                                case "G":
                                    outputFormat = reportCulture.DateTimeFormat.ShortDatePattern + " " + reportCulture.DateTimeFormat.LongTimePattern;
                                    break;
                                case "m":
                                case "M":
                                    outputFormat = reportCulture.DateTimeFormat.MonthDayPattern;
                                    break;
                                case "s":
                                    outputFormat = reportCulture.DateTimeFormat.SortableDateTimePattern;
                                    break;
                                case "t":
                                    outputFormat = reportCulture.DateTimeFormat.ShortTimePattern;
                                    break;
                                case "T":
                                    outputFormat = reportCulture.DateTimeFormat.LongTimePattern;
                                    break;
                                case "u":
                                    outputFormat = reportCulture.DateTimeFormat.UniversalSortableDateTimePattern;
                                    break;
                                case "y":
                                case "Y":
                                    outputFormat = reportCulture.DateTimeFormat.YearMonthPattern;
                                    break;
                                default:
                                    outputFormat = dtFormat;
                                    break;
                            }
                            outputFormat.Replace(" ", "\\ ");
                            int pos1 = outputFormat.IndexOf('\'');
                            while (pos1 != -1)
                            {
                                int pos2 = outputFormat.IndexOf('\'', pos1 + 1);
                                if (pos2 == -1) break;
                                outputFormat = outputFormat.Substring(0, pos1) + outputFormat.Substring(pos1 + 1, pos2 - pos1 - 1) + outputFormat.Substring(pos2 + 1);
                                pos1 = outputFormat.IndexOf('\'', pos2 - 2);
                            }
                            #endregion
                        } 
                        else if (!isDefaultFormat)
                        {
                            #region Currency, Numeric, Percent
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
                                if (isFormatCurrency && currencyPositionBefore)
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
                                if (isFormatCurrency && !currencyPositionBefore)
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
                                if (hideZeros)
                                {
                                    outputFormat = outputFormat + ";" + negativePattern + ";";
                                }
                                else
                                {
                                    if (negativeBraces) outputFormat = outputFormat + ";" + negativePattern;
                                }
                            }
                            #endregion
                        }
                        #endregion

                        int formatIndex = 0;    //general
                        if (isDefaultFormat)
                        {
                            if (isFormatNumeric)
                            {
                                formatIndex = 4;    //decimal	# ##0.00
                            }
                            //							if (isFormatCurrency)
                            //							{
                            //								formatIndex = 7;	//currency  # ##0.00
                            //							}
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
                        int indexStyle = 0; //default style
                        if (exportObjectFormatting)
                        {
                            StiCellStyle style = cell.CellStyle;

                            DataFont tempFont = new DataFont(
                                style.Font.Name,
                                style.Font.Bold,
                                style.Font.Italic,
                                style.Font.Underline,
                                style.Font.Strikeout,
                                (int)(style.Font.SizeInPoints),
                                style.TextColor,
                                style.Font.GdiCharSet,
                                0);	//fontFamily auto
                            if (hasCheckBoxExcelDataValue)
                            {
                                tempFont = new DataFont(
                                    StiOptions.Export.CheckBoxReplacementForExcelValue.Font.Name,
                                    StiOptions.Export.CheckBoxReplacementForExcelValue.Font.Bold,
                                    StiOptions.Export.CheckBoxReplacementForExcelValue.Font.Italic,
                                    StiOptions.Export.CheckBoxReplacementForExcelValue.Font.Underline,
                                    StiOptions.Export.CheckBoxReplacementForExcelValue.Font.Strikeout,
                                    (int)(StiOptions.Export.CheckBoxReplacementForExcelValue.Font.SizeInPoints),
                                    style.TextColor,
                                    StiOptions.Export.CheckBoxReplacementForExcelValue.Font.GdiCharSet,
                                    0);	//fontFamily auto
                            }

                            DataFill tempFill = new DataFill(
                                "solid",
                                style.Color,
                                style.Color);
                            if (isImage)
                            {
                                var color = StiBrush.ToColor(cell.Component?.Page?.Brush);
                                if (color.Equals(Color.Empty) || color.Equals(Color.White)) color = Color.Transparent;
                                tempFill = new DataFill(
                                    "solid",
                                    color,
                                    color);
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

                            DataBorder tempBorder = new DataBorder(
                                (needBorderLeft ? matrix.BordersY[cell.Top, cell.Left] : null),
                                (needBorderRight ? matrix.BordersY[cell.Top, cell.Left + cell.Width + 1] : null),
                                (needBorderTop ? matrix.BordersX[cell.Top, cell.Left] : null),
                                (needBorderBottom ? matrix.BordersX[cell.Top + cell.Height + 1, cell.Left] : null));

                            bool rightToLeft = false;
                            if (style.TextOptions != null)
                            {
                                rightToLeft = style.TextOptions.RightToLeft;
                            }

                            bool textWordWrap = false;
                            if (style.TextOptions != null) textWordWrap = style.TextOptions.WordWrap;
                            if (!string.IsNullOrEmpty(str))
                            {
                                if ((str.IndexOf("\r", StringComparison.InvariantCulture) != -1) ||
                                (str.IndexOf("\n", StringComparison.InvariantCulture) != -1)) textWordWrap = true;
                                if ((textComp != null) && textComp.CheckAllowHtmlTags() &&
                                    ((str.IndexOf("<br", StringComparison.InvariantCulture) != -1) || (str.IndexOf("<ul", StringComparison.InvariantCulture) != -1) || (str.IndexOf("<ol", StringComparison.InvariantCulture) != -1) || (str.IndexOf("<p", StringComparison.InvariantCulture) != -1))) textWordWrap = true;
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

                            bool editable = (restrictEditing == StiExcel2007RestrictEditing.ExceptEditableFields) && (cell.Component != null) && (cell.Component is StiText) && (cell.Component as StiText).Editable;

                            StiTextHorAlignment horAlign = style.HorAlignment;
                            if (rightToLeft)
                            {
                                if (horAlign == StiTextHorAlignment.Left) horAlign = StiTextHorAlignment.Right;
                                else if (horAlign == StiTextHorAlignment.Right) horAlign = StiTextHorAlignment.Left;
                            }

                            DataXF tempXF = new DataXF(
                                formatIndex,
                                GetFontNumber(tempFont),
                                GetFillNumber(tempFill),
                                GetBorderNumber(tempBorder),
                                0,	//xfId
                                hasCheckBoxExcelDataValue ? StiOptions.Export.CheckBoxReplacementForExcelValue.HorAlignment : horAlign,
                                hasCheckBoxExcelDataValue ? StiOptions.Export.CheckBoxReplacementForExcelValue.VertAlignment : style.VertAlignment,
                                rotationAngle,
                                textWordWrap,
                                rightToLeft,
                                editable);

                            indexStyle = GetXFNumber(tempXF);
                        }
                        #endregion

                        writer.WriteStartElement("c");
                        writer.WriteAttributeString("r", GetRefString(indexColumn, indexRow - minRowIndex));
                        writer.WriteAttributeString("s", indexStyle.ToString());

                        #region Range
                        if (exportObjectFormatting)
                        {
                            for (int xx = 0; xx <= cell.Width; xx++)
                            {
                                for (int yy = 0; yy <= cell.Height; yy++)
                                {
                                    readyCells[indexRow + yy, indexColumn + xx] = true;
                                    mergedCellsStyle[indexRow + yy, indexColumn + xx] = indexStyle;
                                }
                            }
                            if ((cell.Width > 0) || (cell.Height > 0))
                            {
                                CellRangeAddress tempCellRange = new CellRangeAddress(
                                    indexRow - minRowIndex,
                                    indexRow - minRowIndex + cell.Height,
                                    indexColumn,
                                    indexColumn + cell.Width);
                                mergedCells.Add(tempCellRange);
                            }
                        }
                        else
                        {
                            readyCells[indexRow, indexColumn] = true;
                            mergedCellsStyle[indexRow, indexColumn] = indexStyle;
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

                        if (!isProcessed &&
                            (!string.IsNullOrEmpty(str) || !string.IsNullOrEmpty(excelDataValue)))
                        {
                            bool isFormula = false;
                            bool isNumber = false;
                            double Number = 0;
                            if (!string.IsNullOrEmpty(excelDataValue) && (excelDataValue != "-") && !isExcelText)
                            {
                                string value = excelDataValue;

                                //try to found and remove group separator
                                string value2 = value;
                                int posDot = value2.IndexOf(".");
                                int posComma = value2.IndexOf(",");
                                if ((posDot != -1) && (posComma != -1))
                                {
                                    value2 = value2.Replace(posDot > posComma ? "," : ".", "");
                                }
                                string sep = reportCulture.NumberFormat.NumberDecimalSeparator;
                                value2 = value2.Replace(".", ",").Replace(",", sep);

                                if (!string.IsNullOrEmpty(value2))
                                {
                                    isNumber = true;
                                    try
                                    {
                                        if (isFormatDate || isFormatTime)
                                        {
                                            isNumber = false;
                                            if (StiOptions.Export.Excel.AllowExportDateTime && !(outputFormat == "Q" || outputFormat == "QI" || outputFormat == "YQ" || outputFormat == "YQI"))
                                            {
                                                DateTime dt2;
                                                if (DateTime.TryParse(value, reportCulture, DateTimeStyles.None, out dt2))
                                                {
                                                    TimeSpan ts = dt2.Subtract(new DateTime(1900, 1, 1, 0, 0, 0));
                                                    Number = ts.TotalDays + 1;
                                                    if (Number > 59) Number++; //leap 1900
                                                    isNumber = true;
                                                }
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

                                if (value != str && value.StartsWith("="))
                                {
                                    isFormula = true;
                                }

                                if ((!isNumber) && (str == null))
                                {
                                    str = value;
                                }
                            }

                            if (isFormula)
                            {
                                string strs = excelDataValue.Replace("\r", "").Replace('\t', ' ').Substring(1);
                                if (StiOptions.Export.Excel.TrimTrailingSpaces) strs = StiExportUtils.TrimEndWhiteSpace(strs);
                                writer.WriteElementString("f", strs);
                            }
                            else
                            {
                                if (isNumber)
                                {
                                    //writer.WriteElementString("v", Number.ToString().Replace('.', ','));
                                    writer.WriteElementString("v", Number.ToString().Replace(',', '.'));
                                }
                                else
                                {
                                    string strs = str.Replace("\r", "").Replace('\t', ' ');
                                    if (StiOptions.Export.Excel.TrimTrailingSpaces) strs = StiExportUtils.TrimEndWhiteSpace(strs);
                                    strs = RemoveControlCharacters(strs);

                                    int indexSST = 0;
                                    if ((textComp != null) && (textComp.CheckAllowHtmlTags()))
                                    {
                                        strs = ConvertAllowHtmlTagsToExcelString(textComp, strs);
                                        indexSST = GetSSTNumber(strs);
                                        sstHashIsTags[indexSST] = null;
                                    }
                                    else
                                    {
                                        indexSST = GetSSTNumber(strs);
                                    }
                                    writer.WriteAttributeString("t", "s");
                                    writer.WriteElementString("v", indexSST.ToString());
                                }
                            }
                        }
                        #endregion

                        writer.WriteEndElement();
                        #endregion
                    }
                    else
                    {
                        //int indexStyle = 0; //default
                        int indexStyle = backgroundStyle;
                        if (readyCells[indexRow, indexColumn])
                        {
                            indexStyle = mergedCellsStyle[indexRow, indexColumn];
                        }

                        bool needBorderLeft = matrix.BordersY[indexRow + 0, indexColumn + 0] != null;
                        bool needBorderRight = matrix.BordersY[indexRow + 0, indexColumn + 1] != null;
                        bool needBorderTop = matrix.BordersX[indexRow + 0, indexColumn + 0] != null;
                        bool needBorderBottom = matrix.BordersX[indexRow + 1, indexColumn + 0] != null;

                        if (needBorderLeft || needBorderRight || needBorderTop || needBorderBottom || (indexStyle != 0))
                        {
                            #region Style
                            DataBorder tempBorder = new DataBorder(
                                (needBorderLeft ? matrix.BordersY[indexRow + 0, indexColumn + 0] : null),
                                (needBorderRight ? matrix.BordersY[indexRow + 0, indexColumn + 1] : null),
                                (needBorderTop ? matrix.BordersX[indexRow + 0, indexColumn + 0] : null),
                                (needBorderBottom ? matrix.BordersX[indexRow + 1, indexColumn + 0] : null));

                            DataXF parentXF = (DataXF)xfList[indexStyle];

                            DataXF tempXF = new DataXF(
                                parentXF.FormatIndex,
                                parentXF.FontIndex,
                                parentXF.FillIndex,
                                GetBorderNumber(tempBorder),
                                parentXF.XFId,
                                parentXF.HorAlign,
                                parentXF.VertAlign,
                                parentXF.TextRotationAngle,
                                parentXF.TextWrapped,
                                parentXF.RightToLeft,
                                parentXF.Editable);
                            int newIndexStyle = GetXFNumber(tempXF);
                            if (!exportObjectFormatting) newIndexStyle = 0; //default style
                            #endregion

                            writer.WriteStartElement("c");
                            writer.WriteAttributeString("r", GetRefString(indexColumn, indexRow - minRowIndex));
                            writer.WriteAttributeString("s", newIndexStyle.ToString());
                            writer.WriteEndElement();
                        }
                    }
                }
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            if (richtextForConvert != null) richtextForConvert.Dispose();
            #endregion

            #region sheet protection
            if (restrictEditing != StiExcel2007RestrictEditing.No)
            {
                writer.WriteStartElement("sheetProtection");
                writer.WriteAttributeString("password", "F05F");    //*TestPassword*
                writer.WriteAttributeString("sheet", "1");
                writer.WriteAttributeString("objects", "1");
                writer.WriteAttributeString("scenarios", "1");
                writer.WriteEndElement();
            }
            #endregion

            #region merged cells
            if ((mergedCells.Count > 0) && ((dataExportMode == StiDataExportMode.AllBands) || exportObjectFormatting))
            {
                writer.WriteStartElement("mergeCells");
                writer.WriteAttributeString("count", string.Format("{0}", mergedCells.Count));
                for (int index = 0; index < mergedCells.Count; index++)
                {
                    CellRangeAddress tempCellRange = (CellRangeAddress)mergedCells[index];
                    writer.WriteStartElement("mergeCell");
                    writer.WriteAttributeString("ref", string.Format("{0}:{1}",
                        GetRefString(tempCellRange.FirstColumn, tempCellRange.FirstRow),
                        GetRefString(tempCellRange.LastColumn, tempCellRange.LastRow)));
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            #endregion

            #region Hyperlinks
            if (hlinks.Count > 0)
            {
                #region Check for at least one hyperlink
                bool founded = false;
                for (int indexHlink = 0; indexHlink < hlinks.Count; indexHlink++)
                {
                    HlinkData hl = (HlinkData)hlinks[indexHlink];
                    string hyperlink = hl.Bookmark;
                    if (hyperlink.StartsWith("#", StringComparison.InvariantCulture))
                    {
                        #region find bookmark
                        hyperlink = hyperlink.Substring(1);
                        for (int indexMatrix = 0; indexMatrix < matrixList.Count; indexMatrix++)
                        {
                            Hashtable bookmarks = ((StiMatrix)matrixList[indexMatrix]).Bookmarks.BookmarksTable;
                            if (bookmarks != null)
                            {
                                object obj = bookmarks[hyperlink];
                                if (obj != null)
                                {
                                    int minRow = (int)minRowList[indexMatrix];
                                    int maxRow = (int)maxRowList[indexMatrix];
                                    Size pos = (Size)obj;
                                    if ((pos.Height >= minRow) && (pos.Height < maxRow))
                                    {
                                        founded = true;
                                        break;
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        founded = true;
                    }
                    if (founded) break;
                }
                #endregion

                if (founded)
                {
                    writer.WriteStartElement("hyperlinks");
                    for (int indexHlink = 0; indexHlink < hlinks.Count; indexHlink++)
                    {
                        HlinkData hl = (HlinkData)hlinks[indexHlink];
                        string hyperlink = hl.Bookmark;
                        if (hyperlink.StartsWith("#", StringComparison.InvariantCulture))
                        {
                            #region find bookmark
                            hyperlink = hyperlink.Substring(1);
                            bool isFounded = false;
                            for (int indexMatrix = 0; indexMatrix < matrixList.Count; indexMatrix++)
                            {
                                Hashtable bookmarks = ((StiMatrix)matrixList[indexMatrix]).Bookmarks.BookmarksTable;
                                if (bookmarks != null)
                                {
                                    object obj = bookmarks[hyperlink];
                                    if (obj != null)
                                    {
                                        int minRow = (int)minRowList[indexMatrix];
                                        int maxRow = (int)maxRowList[indexMatrix];
                                        Size pos = (Size)obj;
                                        if ((pos.Height >= minRow) && (pos.Height < maxRow))
                                        {
                                            hyperlink = string.Format("'{0}'!{1}", (string)sheetNameList[indexMatrix], GetRefString(pos.Width, pos.Height - minRow));
                                            isFounded = true;
                                            break;
                                        }
                                    }
                                }
                            }
                            #endregion

                            if (isFounded)
                            {
                                writer.WriteStartElement("hyperlink");
                                writer.WriteAttributeString("ref", GetRefString(hl.Range.FirstColumn, hl.Range.FirstRow));
                                writer.WriteAttributeString("location", hyperlink);
                                writer.WriteAttributeString("display", hl.Description);
                                writer.WriteEndElement();
                            }
                        }
                        else
                        {
                            string display = hyperlink;
                            if (hyperlink.StartsWith("file:\\\\\\", StringComparison.InvariantCulture))
                            {
                                display = hyperlink.Substring(8);
                                hyperlink = "file:///" + display;
                            }
                            if (hyperlink.StartsWith("http:", StringComparison.InvariantCulture))
                            {
                                hyperlink = hyperlink.Replace(" ", "");
                            }
                            writer.WriteStartElement("hyperlink");
                            writer.WriteAttributeString("ref", GetRefString(hl.Range.FirstColumn, hl.Range.FirstRow));
                            writer.WriteAttributeString("r:id", string.Format("rId{0}", 2 + hyperlinkList.Count));
                            writer.WriteAttributeString("display", display);
                            writer.WriteEndElement();
                            hyperlinkList.Add(StringToUrl(hyperlink));
                        }
                    }
                    writer.WriteEndElement();
                }
            }
            #endregion

            #region page info
            writer.WriteStartElement("pageMargins");
            writer.WriteAttributeString("left", FloatToString(page.Unit.ConvertToHInches(page.Margins.Left) / 100));
            writer.WriteAttributeString("right", FloatToString(page.Unit.ConvertToHInches(page.Margins.Right) / 100));
            writer.WriteAttributeString("top", FloatToString(page.Unit.ConvertToHInches(page.Margins.Top) / 100));
            writer.WriteAttributeString("bottom", FloatToString(page.Unit.ConvertToHInches(page.Margins.Bottom) / 100));
            writer.WriteAttributeString("header", FloatToString(0.0));
            writer.WriteAttributeString("footer", FloatToString(0.0));
            writer.WriteEndElement();

            writer.WriteStartElement("pageSetup");
            writer.WriteAttributeString("paperSize", string.Format("{0}", (int)page.PaperSize));
            writer.WriteAttributeString("orientation", (page.Orientation == StiPageOrientation.Portrait ? "portrait" : "landscape"));
            if (StiOptions.Export.Excel.NumberOfPagesInWideToFit > 0 || StiOptions.Export.Excel.NumberOfPagesInHeightToFit > 0)
            {
                if (StiOptions.Export.Excel.NumberOfPagesInWideToFit != 1)
                {
                    writer.WriteAttributeString("fitToWidth", StiOptions.Export.Excel.NumberOfPagesInWideToFit.ToString());
                }
                if (StiOptions.Export.Excel.NumberOfPagesInHeightToFit != 1)
                {
                    writer.WriteAttributeString("fitToHeight", StiOptions.Export.Excel.NumberOfPagesInHeightToFit.ToString());
                }
            }
            writer.WriteEndElement();
            #endregion

            #region Trial
#if CLOUD
            var isTrial = StiCloudPlan.IsTrial(report != null ? report.ReportGuid : null);
#elif SERVER
		    var isTrial = StiVersionX.IsSvr;
#else
		    var key = StiLicenseKeyValidator.GetLicenseKey();
		    var isTrial = !StiLicenseKeyValidator.IsValidOnNetFramework(key);
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
                bool needAddSpace = page.Unit.ConvertToHInches(page.PageHeight) > 400;
                writer.WriteStartElement("headerFooter");
                writer.WriteStartElement("oddHeader");
                writer.WriteRaw("&amp;C\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n" + (needAddSpace ? "\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n" : "") + "&amp;G");
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
            #endregion

            #region Page breaks
            int horizontalPageBreaksCount = Math.Min(1024, Matrix.HorizontalPageBreaks.Count);
            if ((exportHorizontalPageBreaks) && (horizontalPageBreaksCount > 0))
            {
                int maxCol = Math.Max(0, Matrix.CoordX.Count - 1 - 1);
                writer.WriteStartElement("rowBreaks");
                writer.WriteAttributeString("count", horizontalPageBreaksCount.ToString());
                writer.WriteAttributeString("manualBreakCount", horizontalPageBreaksCount.ToString());
                for (int indexBreak = 0; indexBreak < horizontalPageBreaksCount; indexBreak++)
                {
                    writer.WriteStartElement("brk");
                    writer.WriteAttributeString("id", string.Format("{0}", (int)Matrix.HorizontalPageBreaks[indexBreak]));
                    writer.WriteAttributeString("max", string.Format("{0}", maxCol));
                    writer.WriteAttributeString("man", "1");
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            #endregion

            if ((imageList.Count > imageListOffset) || (barCodeLinesList.Count > 0) || isTrial)
            {
                writer.WriteStartElement("drawing");
                //writer.WriteAttributeString("r:id", string.Format("rId{0}", indexSheet + 1));
                writer.WriteAttributeString("r:id", string.Format("rId{0}", 1));
                writer.WriteEndElement();
            }

            if (numberStoredAsText.Count > 0)
            {
                writer.WriteStartElement("ignoredErrors");
                foreach (string sqref in numberStoredAsText)
                {
                    writer.WriteStartElement("ignoredError");
                    writer.WriteAttributeString("sqref", sqref);
                    writer.WriteAttributeString("numberStoredAsText", "1");
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }

            #region Trial
            if (isTrial)
            {
                writer.WriteStartElement("legacyDrawingHF");
                writer.WriteAttributeString("r:id", "dId1");
                writer.WriteEndElement();
            }
            #endregion

            writer.WriteFullEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return ms;
        }

        private void PrepareMatrix(StiPagesCollection pages)
        {
            StiDataExportMode dataMode = dataExportMode;
#pragma warning disable 612, 618
            if (StiOptions.Export.Excel.AllowExportFootersInDataOnlyMode) dataMode |= StiDataExportMode.Footers;
#pragma warning restore 612, 618

            matrix = new StiMatrix(pages, StiOptions.Export.Excel.DivideBigCells, this, null, dataMode);
            if (IsStopped) return;

            if (useOnePageHeaderAndFooter)
            {
                matrix.ScanComponentsPlacement(true, exportObjectFormatting);

                #region prepare line info

                //array must hold only first page header
                int tempOffset = 0;
                //find first header
                while ((matrix.LinePlacement[tempOffset] != StiMatrix.StiTableLineInfo.PageHeader) &&
                    (tempOffset < matrix.CoordY.Count - 1)) tempOffset++;
                //header exist
                if (matrix.LinePlacement[tempOffset] == StiMatrix.StiTableLineInfo.PageHeader)
                {
                    //find first line after header
                    while ((matrix.LinePlacement[tempOffset] == StiMatrix.StiTableLineInfo.PageHeader) &&
                        (tempOffset < matrix.CoordY.Count - 1)) tempOffset++;
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
                    (tempOffset > 0)) tempOffset--;
                //header exist
                if (matrix.LinePlacement[tempOffset] == StiMatrix.StiTableLineInfo.PageFooter)
                {
                    //find first line before header
                    while ((matrix.LinePlacement[tempOffset] == StiMatrix.StiTableLineInfo.PageFooter) &&
                        (tempOffset > 0)) tempOffset--;
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
                int lineOffset = 0;
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
                    else
                    {
                        lineOffset++;
                    }
                    if (exportHorizontalPageBreaks)
                    {
                        for (int indexHPB = 0; indexHPB < matrix.HorizontalPageBreaks.Count; indexHPB++)
                        {
                            if (matrix.HorizontalPageBreaks[indexHPB] == rowIndex)
                            {
                                matrix.HorizontalPageBreaks[indexHPB] = matrix.HorizontalPageBreaks[indexHPB] - (lineOffset - 1);
                            }
                        }
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
                bool lastIsHeader = false;
                for (int rowIndex = 0; rowIndex < Matrix.CoordY.Count - 1; rowIndex++)
                {
                    bool isHeader = false;
                    if ((Matrix.LinePlacement[rowIndex] == StiMatrix.StiTableLineInfo.HeaderD) || (Matrix.LinePlacement[rowIndex] == StiMatrix.StiTableLineInfo.HeaderAP))
                    {
                        string currentParentBandName = Matrix.ParentBandName[rowIndex];
                        if (lastIsHeader && (GetParentBandName(rowIndex) == GetParentBandName(rowIndex - 1)))
                        {
                            isHeader = true;
                        }
                        else
                        {
                            //check for new header component 
                            int symPos = currentParentBandName.IndexOf('\x1f');
                            if (symPos != -1)
                            {
                                string parentBandName = currentParentBandName.Substring(0, symPos);
                                if (parentBandName != lastParentBandName)
                                {
                                    lastParentBandName = parentBandName;
                                    headerNames.Clear();
                                }
                            }
                            //check for repeated lines
                            if (!headerNames.ContainsKey(currentParentBandName))
                            {
                                isHeader = true;
                                headerNames.Add(currentParentBandName, currentParentBandName);
                            }
                        }
                    }
                    lastIsHeader = isHeader;

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
        }

        private string GetParentBandName(int rowIndex)
        {
            string st = Matrix.ParentBandName[rowIndex];
            int symPos = st.IndexOf('\x1f');
            if (symPos == -1) return st;
            return st.Substring(0, symPos);
        }

        private string ConvertAllowHtmlTagsToExcelString(StiText textBox, string input)
        {
            var output = new StringBuilder(input.Length * 2);
            var sw = new StringWriter(output);
            var writer = new XmlTextWriter(sw);
            writer.Indentation = (xmlIndentation < 0 ? 0 : xmlIndentation);
            writer.Formatting = (xmlIndentation < 0 ? Formatting.None : Formatting.Indented);

            var baseTagsState = new StiTextRenderer.StiHtmlTagsState(
                textBox.Font.Bold,
                textBox.Font.Italic,
                textBox.Font.Underline,
                textBox.Font.Strikeout,
                textBox.Font.SizeInPoints,
                textBox.Font.Name,
                StiBrush.ToColor(textBox.TextBrush),
                StiBrush.ToColor(textBox.Brush),
                false,
                false,
                0,
                0,
                1,
                textBox.HorAlignment);

            var baseState = new StiTextRenderer.StiHtmlState(baseTagsState, 0);

            var states = StiTextRenderer.ParseHtmlToStates(input, baseState);

            foreach (StiTextRenderer.StiHtmlState state in states)
            {
                writer.WriteStartElement("r");

                writer.WriteStartElement("rPr");

                if (state.TS.Bold)
                {
                    writer.WriteElementString("b", null);
                }
                if (state.TS.Italic)
                {
                    writer.WriteElementString("i", null);
                }
                if (state.TS.Underline)
                {
                    writer.WriteElementString("u", null);
                }
                if (state.TS.Strikeout)
                {
                    writer.WriteElementString("strike", null);
                }

                if (state.TS.Superscript)
                {
                    writer.WriteStartElement("vertAlign");
                    writer.WriteAttributeString("val", "superscript");
                    writer.WriteEndElement();
                }
                if (state.TS.Subscript)
                {
                    writer.WriteStartElement("vertAlign");
                    writer.WriteAttributeString("val", "subscript");
                    writer.WriteEndElement();
                }

                writer.WriteStartElement("sz");
                writer.WriteAttributeString("val", state.TS.FontSize.ToString());
                writer.WriteEndElement();

                writer.WriteStartElement("color");
                writer.WriteAttributeString("rgb", string.Format("{0:X8}", state.TS.FontColor.ToArgb()));
                writer.WriteEndElement();

                writer.WriteStartElement("rFont");
                writer.WriteAttributeString("val", state.TS.FontName);
                writer.WriteEndElement();

                //writer.WriteStartElement("charset");
                //writer.WriteAttributeString("val", string.Format("{0}", state.TS.FontCharset));
                //writer.WriteEndElement();

                writer.WriteEndElement();   //rPr

                var st = StiTextRenderer.PrepareStateText(state.Text).ToString();
                if (string.IsNullOrEmpty(st)) st = " "; //fix
                ConvertTextToExcelString(writer, st);

                writer.WriteEndElement();   //r
            }

            writer.Flush();
            writer.Close();

            return output.ToString();
        }

        private void ConvertTextToExcelString(XmlTextWriter writer, string input)
        {
            writer.WriteStartElement("t");
            if ((input.Length > 0) && (char.IsWhiteSpace(input, input.Length - 1) || char.IsWhiteSpace(input, 0) || (input.IndexOf('\n') != -1)))
            {
                writer.WriteAttributeString("xml:space", "preserve");
            }
            writer.WriteString(input);
            writer.WriteFullEndElement();
        }
        #endregion

        #region WriteDrawingRels
        private MemoryStream WriteDrawingRels(int indexSheet)
        {
            MemoryStream ms = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(ms, Encoding.UTF8);
            writer.Indentation = (xmlIndentation < 0 ? 0 : xmlIndentation);
            writer.Formatting = (xmlIndentation < 0 ? Formatting.None : Formatting.Indented);
            writer.WriteStartDocument();
            writer.WriteStartElement("Relationships");
            writer.WriteAttributeString("xmlns", "http://schemas.openxmlformats.org/package/2006/relationships");

            for (int index = imageListOffset; index < imageList.Count; index++)
            {
                ExcelImageData image = (ExcelImageData)imageList[index];

                writer.WriteStartElement("Relationship");
                writer.WriteAttributeString("Id", string.Format("rId{0}", index - imageListOffset + 1));
                writer.WriteAttributeString("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/image");
                writer.WriteAttributeString("Target", string.Format("../media/image{0:D5}.{1}", image.ImageIndex + 1, GetImageFormatExtension(imageCache.ImageFormatStore[image.ImageIndex])));
                writer.WriteEndElement();

                if (!string.IsNullOrWhiteSpace(image.Hyperlink))
                {
                    writer.WriteStartElement("Relationship");
                    writer.WriteAttributeString("Id", string.Format("hId{0}", index - imageListOffset + 1));
                    writer.WriteAttributeString("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/hyperlink");
                    writer.WriteAttributeString("Target", image.Hyperlink);
                    writer.WriteAttributeString("TargetMode", "External");
                    writer.WriteEndElement();
                }
            }

            writer.WriteFullEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return ms;
        }
        #endregion

        #region WriteDrawing
        private MemoryStream WriteDrawing(int indexSheet, StiPage page)
        {
            MemoryStream ms = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(ms, Encoding.UTF8);
            writer.Indentation = (xmlIndentation < 0 ? 0 : xmlIndentation);
            writer.Formatting = (xmlIndentation < 0 ? Formatting.None : Formatting.Indented);
            writer.WriteStartDocument();
            writer.WriteStartElement("xdr:wsDr");
            writer.WriteAttributeString("xmlns:xdr", "http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing");
            writer.WriteAttributeString("xmlns:a", "http://schemas.openxmlformats.org/drawingml/2006/main");

            #region Images
            for (int index = imageListOffset; index < imageList.Count; index++)
            {
                ExcelImageData image = (ExcelImageData)imageList[index];

                writer.WriteStartElement("xdr:twoCellAnchor");
                if (!StiOptions.Export.Excel.ImageMoveAndSizeWithCells)
                {
                    writer.WriteAttributeString("editAs", "oneCell");
                }

                writer.WriteStartElement("xdr:from");
                writer.WriteElementString("xdr:col", string.Format("{0}", image.FirstColumnIndex));
                writer.WriteElementString("xdr:colOff", string.Format("{0}", image.FirstColumnOffset));
                writer.WriteElementString("xdr:row", string.Format("{0}", image.FirstRowIndex));
                writer.WriteElementString("xdr:rowOff", string.Format("{0}", image.FirstRowOffset));
                writer.WriteEndElement();
                writer.WriteStartElement("xdr:to");
                writer.WriteElementString("xdr:col", string.Format("{0}", image.LastColumnIndex));
                writer.WriteElementString("xdr:colOff", string.Format("{0}", image.LastColumnOffset));
                writer.WriteElementString("xdr:row", string.Format("{0}", image.LastRowIndex));
                writer.WriteElementString("xdr:rowOff", string.Format("{0}", image.LastRowOffset));
                writer.WriteEndElement();

                writer.WriteStartElement("xdr:pic");

                writer.WriteStartElement("xdr:nvPicPr");
                writer.WriteStartElement("xdr:cNvPr");
                writer.WriteAttributeString("id", string.Format("{0}", index - imageListOffset + 2));
                writer.WriteAttributeString("name", string.Format("Picture {0}", index - imageListOffset + 1));
                writer.WriteAttributeString("descr", string.Format("image{0:D5}", index + 1));
                if (!string.IsNullOrWhiteSpace(image.Hyperlink))
                {
                    writer.WriteStartElement("a:hlinkClick");
                    writer.WriteAttributeString("xmlns:r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
                    writer.WriteAttributeString("r:id", string.Format("hId{0}", index - imageListOffset + 1));
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteStartElement("xdr:cNvPicPr");
                writer.WriteStartElement("a:picLocks");
                writer.WriteAttributeString("noChangeAspect", "1");
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndElement();

                writer.WriteStartElement("xdr:blipFill");
                writer.WriteStartElement("a:blip");
                writer.WriteAttributeString("xmlns:r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
                writer.WriteAttributeString("r:embed", string.Format("rId{0}", index - imageListOffset + 1));
                writer.WriteEndElement();
                writer.WriteStartElement("a:stretch");
                writer.WriteStartElement("a:fillRect");
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndElement();

                writer.WriteStartElement("xdr:spPr");
                writer.WriteStartElement("a:xfrm");
                writer.WriteStartElement("a:off");
                writer.WriteAttributeString("x", "1");
                writer.WriteAttributeString("y", "1");
                writer.WriteEndElement();
                writer.WriteStartElement("a:ext");
                writer.WriteAttributeString("cx", "2");
                writer.WriteAttributeString("cy", "2");
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteStartElement("a:prstGeom");
                writer.WriteAttributeString("prst", "rect");
                writer.WriteStartElement("a:avLst");
                writer.WriteEndElement();
                writer.WriteEndElement();
                if (AllowNativeImageBorders && (image.Border != null) && !(image.Border is StiAdvancedBorder) && image.Border.IsAllBorderSidesPresent && !image.Border.Color.Equals(Color.Transparent) && !(image.Border.Style == StiPenStyle.None))
                {
                    writer.WriteStartElement("a:ln");
                    writer.WriteAttributeString("w", string.Format("{0}", ConvertToEMU(image.Border.GetSize())));
                    writer.WriteStartElement("a:solidFill");
                    writer.WriteStartElement("a:srgbClr");
                    writer.WriteAttributeString("val", string.Format("{0:X8}", image.Border.Color.ToArgb()).Substring(2));
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    if (image.Border.Style != StiPenStyle.Solid)
                    {
                        string style = GetLineStyle(new StiBorderSide(image.Border.Color, image.Border.Size, image.Border.Style));
                        writer.WriteStartElement("a:prstDash");
                        writer.WriteAttributeString("val", style);
                        writer.WriteEndElement();
                    }
                    writer.WriteStartElement("a:round");
                    writer.WriteEndElement();
                    writer.WriteStartElement("a:headEnd");
                    writer.WriteEndElement();
                    writer.WriteStartElement("a:tailEnd");
                    writer.WriteEndElement();
                    writer.WriteEndElement();   //ln
                }
                writer.WriteEndElement();

                writer.WriteEndElement();

                writer.WriteStartElement("xdr:clientData");
                writer.WriteEndElement();

                writer.WriteEndElement();
            }
            #endregion

            #region Barcodes
            for (int index = 0; index < barCodeLinesList.Count; index++)
            {
                BarCodeLineData line = barCodeLinesList[index];

                var pointStart = GetRealRowCol(line, true);
                var pointEnd = GetRealRowCol(line, false);

                writer.WriteStartElement("xdr:twoCellAnchor");
                //writer.WriteAttributeString("editAs", "oneCell");

                writer.WriteStartElement("xdr:from");
                writer.WriteElementString("xdr:col", string.Format("{0}", pointStart.Column));
                writer.WriteElementString("xdr:colOff", string.Format("{0}", ConvertToEMU(pointStart.Position.X)));
                writer.WriteElementString("xdr:row", string.Format("{0}", pointStart.Row));
                writer.WriteElementString("xdr:rowOff", string.Format("{0}", ConvertToEMU(pointStart.Position.Y)));
                writer.WriteEndElement();
                writer.WriteStartElement("xdr:to");
                writer.WriteElementString("xdr:col", string.Format("{0}", pointEnd.Column));
                writer.WriteElementString("xdr:colOff", string.Format("{0}", ConvertToEMU(pointEnd.Position.X)));
                writer.WriteElementString("xdr:row", string.Format("{0}", pointEnd.Row));
                writer.WriteElementString("xdr:rowOff", string.Format("{0}", ConvertToEMU(pointEnd.Position.Y)));
                writer.WriteEndElement();

                writer.WriteStartElement("xdr:sp");
                writer.WriteAttributeString("macro", "");
                writer.WriteAttributeString("textlink", "");

                writer.WriteStartElement("xdr:nvSpPr");
                writer.WriteStartElement("xdr:cNvPr");
                writer.WriteAttributeString("id", string.Format("{0}", imageList.Count + index + 2));
                writer.WriteAttributeString("name", string.Format("Line {0}", imageList.Count + index + 2));
                writer.WriteEndElement();
                writer.WriteStartElement("xdr:cNvSpPr");
                writer.WriteStartElement("a:picLocks");
                writer.WriteAttributeString("noChangeShapeType", "1");
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndElement();

                writer.WriteStartElement("xdr:spPr");
                writer.WriteAttributeString("bwMode", "auto");

                writer.WriteStartElement("a:xfrm");
                writer.WriteStartElement("a:off");
                writer.WriteAttributeString("x", string.Format("{0}", ConvertToEMU(line.Position.X + line.Size.Width / 2)));
                writer.WriteAttributeString("y", string.Format("{0}", ConvertToEMU(line.Position.Y)));
                writer.WriteEndElement();
                writer.WriteStartElement("a:ext");
                writer.WriteAttributeString("cx", string.Format("{0}", ConvertToEMU(line.Position.X + line.Size.Width / 2)));
                writer.WriteAttributeString("cy", string.Format("{0}", ConvertToEMU(line.Position.Y + line.Size.Height)));
                writer.WriteEndElement();
                writer.WriteEndElement();

                writer.WriteStartElement("a:prstGeom");
                writer.WriteAttributeString("prst", "line");
                writer.WriteStartElement("a:avLst");
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteStartElement("a:noFill");
                writer.WriteEndElement();

                writer.WriteStartElement("a:ln");
                writer.WriteAttributeString("w", string.Format("{0}", ConvertToEMU(line.Size.Width)));
                writer.WriteStartElement("a:solidFill");
                writer.WriteStartElement("a:srgbClr");
                writer.WriteAttributeString("val", string.Format("{0:X8}", line.Color.ToArgb()).Substring(2));
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteStartElement("a:round");
                writer.WriteEndElement();
                writer.WriteStartElement("a:headEnd");
                writer.WriteEndElement();
                writer.WriteStartElement("a:tailEnd");
                writer.WriteEndElement();
                writer.WriteEndElement();   //ln

                writer.WriteEndElement();   //spPr
                writer.WriteEndElement();   //sp
                writer.WriteStartElement("xdr:clientData");
                writer.WriteEndElement();

                writer.WriteEndElement();
            }
            #endregion

            if (page != null)
            {
                #region Trial
                double pageWidth = page.Unit.ConvertToHInches(page.PageWidth - page.Margins.Left - page.Margins.Right);
                double pageHeight = page.Unit.ConvertToHInches(page.PageHeight - page.Margins.Top - page.Margins.Bottom);
                double pos = pageHeight * 0.45;

                //find nearest row and vertical offset from it
                int minRow = (int)minRowList[indexSheet];
                int posRow = minRow;
                double minRowValue = (double)matrix.CoordY.GetByIndex(minRow);
                double currVal = minRowValue;
                for (int index = minRow + 1; index < matrix.CoordY.Count - 1; index++)
                {
                    double val = (double)matrix.CoordY.GetByIndex(index);
                    if ((val - minRowValue) > pos) break;
                    posRow = index;
                    currVal = val;
                }
                double posDY = pos - (currVal - minRowValue);

                string drawing = "<xdr:oneCellAnchor><xdr:from><xdr:col>0</xdr:col><xdr:colOff>0</xdr:colOff><xdr:row>" + posRow.ToString() + "</xdr:row><xdr:rowOff>" + ((int)ConvertToEMU(posDY)).ToString() + "</xdr:rowOff></xdr:from>" + 
                    "<xdr:ext cx=\"" + ConvertToEMU(pageWidth).ToString() + "\" cy=\"" + ((int)ConvertToEMU(pageHeight * 0.1)).ToString() + "\"/>" +
                    "<xdr:sp macro=\"\" textlink=\"\"><xdr:nvSpPr><xdr:cNvPr id=\"" + (imageList.Count + barCodeLinesList.Count + indexSheet + 2).ToString() + "\" name=\"TextBox 1\"/><xdr:cNvSpPr txBox=\"1\"/></xdr:nvSpPr><xdr:spPr>" +
                    "<a:xfrm rot=\"-2700000\"><a:off x=\"0\" y=\"0\"/><a:ext cx=\"" + ConvertToEMU(pageWidth).ToString() + "\" cy=\"" + ConvertToEMU(pageHeight * 0.1).ToString() + "\"/></a:xfrm>" +
                    "<a:prstGeom prst=\"rect\"><a:avLst/></a:prstGeom><a:noFill/><a:ln w=\"9525\" cmpd=\"sng\"><a:noFill/></a:ln>" +
                    "</xdr:spPr><xdr:style><a:lnRef idx=\"0\"><a:scrgbClr r=\"0\" g=\"0\" b=\"0\"/></a:lnRef><a:fillRef idx=\"0\"><a:scrgbClr r=\"0\" g=\"0\" b=\"0\"/></a:fillRef><a:effectRef idx=\"0\"><a:scrgbClr r=\"0\" g=\"0\" b=\"0\"/></a:effectRef><a:fontRef idx=\"minor\"><a:schemeClr val=\"dk1\"/></a:fontRef></xdr:style>" +
                    "<xdr:txBody><a:bodyPr vertOverflow=\"overflow\" horzOverflow=\"overflow\" wrap=\"square\" rtlCol=\"0\" anchor=\"ctr\"/><a:lstStyle/>" +
                    "<a:p><a:pPr algn=\"ctr\"/><a:r><a:rPr lang=\"en-US\" sz=\"9600\" b=\"1\"><a:solidFill><a:schemeClr val=\"dk1\"><a:alpha val=\"18000\"/></a:schemeClr></a:solidFill>" +
                    "<a:latin typeface=\"Arial\" panose=\"020B0604020202020204\" pitchFamily=\"34\" charset=\"0\"/><a:cs typeface=\"Arial\" panose=\"020B0604020202020204\" pitchFamily=\"34\" charset=\"0\"/>" +
                    "</a:rPr><a:t>Trial</a:t></a:r></a:p></xdr:txBody></xdr:sp><xdr:clientData/></xdr:oneCellAnchor>";
                writer.WriteRaw(drawing);
                #endregion
            }

            writer.WriteFullEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return ms;
        }

        private BarCodeLineData GetRealRowCol(BarCodeLineData line, bool start)
        {
            var real = new BarCodeLineData
            {
                Row = line.Row,
                Column = line.Column,
                Position = line.Position
            };
            real.Position.X += line.Size.Width / 2;
            if (!start) real.Position.Y += line.Size.Height;

            //remove first offset
            real.Position.X -= (double)Matrix.CoordX.GetByIndex(real.Column);
            real.Position.Y -= (double)Matrix.CoordY.GetByIndex(real.Row);

            bool flag = true;
            while (flag)
            {
                double cellWidth = (double)Matrix.CoordX.GetByIndex(real.Column + 1) - (double)Matrix.CoordX.GetByIndex(real.Column);
                if (real.Position.X > cellWidth)
                {
                    real.Column++;
                    real.Position.X -= cellWidth;
                }
                else
                {
                    flag = false;
                }
            }

            flag = true;
            while (flag)
            {
                double cellHeight = (double)Matrix.CoordY.GetByIndex(real.Row + 1) - (double)Matrix.CoordY.GetByIndex(real.Row);
                if (real.Position.Y > cellHeight)
                {
                    real.Row++;
                    real.Position.Y -= cellHeight;
                }
                else
                {
                    flag = false;
                }
            }

            return real;
        }
        #endregion

        #region WriteStyles
        private MemoryStream WriteStyles()
        {
            MemoryStream ms = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(ms, Encoding.UTF8);
            writer.Indentation = (xmlIndentation < 0 ? 0 : xmlIndentation);
            writer.Formatting = (xmlIndentation < 0 ? Formatting.None : Formatting.Indented);
            writer.WriteStartDocument();
            writer.WriteStartElement("styleSheet");
            writer.WriteAttributeString("xmlns", "http://schemas.openxmlformats.org/spreadsheetml/2006/main");

            #region NumFmts
            if (formatList.Count > 0)
            {
                writer.WriteStartElement("numFmts");
                writer.WriteAttributeString("count", string.Format("{0}", formatList.Count));
                for (int index = 0; index < formatList.Count; index++)
                {
                    writer.WriteStartElement("numFmt");
                    writer.WriteAttributeString("numFmtId", string.Format("{0}", 164 + index));
                    writer.WriteAttributeString("formatCode", (string)formatList[index]);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            #endregion

            #region Fonts
            writer.WriteStartElement("fonts");
            writer.WriteAttributeString("count", string.Format("{0}", fontList.Count));
            for (int index = 0; index < fontList.Count; index++)
            {
                DataFont tempFont = (DataFont)fontList[index];
                writer.WriteStartElement("font");
                if (tempFont.Bold)
                {
                    writer.WriteElementString("b", "");
                }
                if (tempFont.Italic)
                {
                    writer.WriteElementString("i", "");
                }
                if (tempFont.Underlined)
                {
                    writer.WriteElementString("u", "");
                }
                if (tempFont.Strikeout)
                {
                    writer.WriteElementString("strike", "");
                }
                writer.WriteStartElement("sz");
                writer.WriteAttributeString("val", string.Format("{0}", tempFont.Height));
                writer.WriteEndElement();
                writer.WriteStartElement("color");
                writer.WriteAttributeString("rgb", string.Format("{0:X8}", tempFont.Color.ToArgb()));
                writer.WriteEndElement();
                writer.WriteStartElement("name");
                writer.WriteAttributeString("val", tempFont.Name);
                writer.WriteEndElement();
                //				writer.WriteStartElement("family");
                //				writer.WriteAttributeString("val", string.Format("{0}", 0));
                //				writer.WriteEndElement();
                writer.WriteStartElement("charset");
                writer.WriteAttributeString("val", string.Format("{0}", tempFont.Charset));
                writer.WriteEndElement();
                writer.WriteEndElement();   //font
            }
            writer.WriteEndElement();
            #endregion

            #region Fills
            writer.WriteStartElement("fills");
            writer.WriteAttributeString("count", string.Format("{0}", fillList.Count));
            for (int index = 0; index < fillList.Count; index++)
            {
                DataFill tempFill = (DataFill)fillList[index];
                writer.WriteStartElement("fill");
                writer.WriteStartElement("patternFill");
                writer.WriteAttributeString("patternType", tempFill.Type);
                if (tempFill.Type == "solid")
                {
                    writer.WriteStartElement("fgColor");
                    writer.WriteAttributeString("rgb", tempFill.FgColor.ToArgb().ToString("X8"));
                    writer.WriteEndElement();
                    writer.WriteStartElement("bgColor");
                    writer.WriteAttributeString("indexed", string.Format("{0}", 64));
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            #endregion

            #region Borders
            writer.WriteStartElement("borders");
            writer.WriteAttributeString("count", string.Format("{0}", borderList.Count));
            for (int index = 0; index < borderList.Count; index++)
            {
                DataBorder tempBorder = (DataBorder)borderList[index];
                writer.WriteStartElement("border");
                WriteBorderData(writer, "left", tempBorder.BorderLeft);
                WriteBorderData(writer, "right", tempBorder.BorderRight);
                WriteBorderData(writer, "top", tempBorder.BorderTop);
                WriteBorderData(writer, "bottom", tempBorder.BorderBottom);
                WriteBorderData(writer, "diagonal", null);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            #endregion

            #region CellStyleXfs
            writer.WriteStartElement("cellStyleXfs");
            writer.WriteAttributeString("count", string.Format("{0}", 1));
            writer.WriteStartElement("xf");
            writer.WriteAttributeString("numFmtId", string.Format("{0}", 0));
            writer.WriteAttributeString("fontId", string.Format("{0}", 0));
            writer.WriteAttributeString("fillId", string.Format("{0}", 0));
            writer.WriteAttributeString("borderId", string.Format("{0}", 0));
            writer.WriteEndElement();
            writer.WriteEndElement();
            #endregion

            #region CellXfs
            writer.WriteStartElement("cellXfs");
            writer.WriteAttributeString("count", string.Format("{0}", xfList.Count));
            for (int index = 0; index < xfList.Count; index++)
            {
                DataXF tempXF = (DataXF)xfList[index];
                DataFill tempFill = (DataFill)fillList[tempXF.FillIndex];
                int fillIndex = tempXF.FillIndex;
                if (tempFill.FgColor.A == 0) fillIndex = 0;

                writer.WriteStartElement("xf");
                writer.WriteAttributeString("numFmtId", string.Format("{0}", tempXF.FormatIndex));
                writer.WriteAttributeString("fontId", string.Format("{0}", tempXF.FontIndex));
                writer.WriteAttributeString("fillId", string.Format("{0}", fillIndex));
                writer.WriteAttributeString("borderId", string.Format("{0}", tempXF.BorderIndex));
                writer.WriteAttributeString("xfId", string.Format("{0}", tempXF.XFId));
                if (tempXF.FormatIndex != 0)
                {
                    writer.WriteAttributeString("applyNumberFormat", "1");
                }
                if (tempXF.FontIndex != 0)
                {
                    writer.WriteAttributeString("applyFont", "1");
                }
                if (tempXF.FillIndex != 0)
                {
                    writer.WriteAttributeString("applyFill", "1");
                }
                if (tempXF.BorderIndex != 0)
                {
                    writer.WriteAttributeString("applyBorder", "1");
                }

                writer.WriteAttributeString("applyAlignment", "1");

                if (tempXF.Editable)
                {
                    writer.WriteAttributeString("applyProtection", "1");
                }

                #region alignment
                StiTextHorAlignment horAlignment = tempXF.HorAlign;
                StiVertAlignment vertAlignment = tempXF.VertAlign;
                float textAngle = tempXF.TextRotationAngle;
                ConvertRotatedAlignment(textAngle, ref horAlignment, ref vertAlignment);

                writer.WriteStartElement("alignment");
                string horAlign = "left";
                switch (horAlignment)
                {
                    case StiTextHorAlignment.Right: horAlign = "right"; break;
                    case StiTextHorAlignment.Center: horAlign = "center"; break;
                    case StiTextHorAlignment.Width: horAlign = "justify"; break;
                }
                writer.WriteAttributeString("horizontal", horAlign);

                if (vertAlignment != StiVertAlignment.Bottom)
                {
                    string vertAlign = string.Empty;
                    switch (vertAlignment)
                    {
                        case StiVertAlignment.Top: vertAlign = "top"; break;
                        case StiVertAlignment.Center: vertAlign = "center"; break;
                    }
                    writer.WriteAttributeString("vertical", vertAlign);
                }
                if (textAngle != 0)
                {
                    writer.WriteAttributeString("textRotation", textAngle.ToString());
                }
                if (tempXF.TextWrapped)
                {
                    writer.WriteAttributeString("wrapText", "1");
                }
                if (tempXF.RightToLeft)
                {
                    writer.WriteAttributeString("readingOrder", "2");
                }
                writer.WriteEndElement();
                #endregion

                #region
                if (tempXF.Editable)
                {
                    writer.WriteStartElement("protection");
                    writer.WriteAttributeString("locked", "0");
                    writer.WriteEndElement();
                }
                #endregion

                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            #endregion

            #region CellStyles
            writer.WriteStartElement("cellStyles");
            writer.WriteAttributeString("count", string.Format("{0}", 1));
            writer.WriteStartElement("cellStyle");
            writer.WriteAttributeString("name", "Normal");
            writer.WriteAttributeString("xfId", string.Format("{0}", 0));
            writer.WriteAttributeString("builtinId", string.Format("{0}", 0));
            writer.WriteEndElement();
            //			writer.WriteStartElement("cellStyle");
            //			writer.WriteAttributeString("name", "Hyperlink");
            //			writer.WriteAttributeString("xfId", string.Format("{0}", 1));
            //			writer.WriteAttributeString("builtinId", string.Format("{0}", 8));
            //			writer.WriteEndElement();
            writer.WriteEndElement();
            #endregion

            #region Dxfs
            writer.WriteStartElement("dxfs");
            writer.WriteAttributeString("count", string.Format("{0}", 0));
            writer.WriteEndElement();
            #endregion

            #region TableStyles
            writer.WriteStartElement("tableStyles");
            writer.WriteAttributeString("count", string.Format("{0}", 0));
            writer.WriteAttributeString("defaultTableStyle", "TableStyleMedium9");
            writer.WriteAttributeString("defaultPivotStyle", "PivotStyleLight16");
            writer.WriteEndElement();
            #endregion

            writer.WriteFullEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return ms;
        }

        private void WriteBorderData(XmlTextWriter writer, string side, StiBorderSide border)
        {
            writer.WriteStartElement(side);
            if (border != null)
            {
                string style = GetLineStyle(border);
                Color color = border.Color;
                if (style != string.Empty)
                {
                    writer.WriteAttributeString("style", style);
                    writer.WriteStartElement("color");
                    writer.WriteAttributeString("rgb", color.ToArgb().ToString("X8"));
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();
        }
        #endregion

        #region WriteSST
        private MemoryStream WriteSST()
        {
            MemoryStream ms = new Tools.StiCachedStream();
            XmlTextWriter writer = new XmlTextWriter(ms, Encoding.UTF8);
            writer.Indentation = (xmlIndentation < 0 ? 0 : xmlIndentation);
            writer.Formatting = (xmlIndentation < 0 ? Formatting.None : Formatting.Indented);
            writer.WriteStartDocument();
            writer.WriteStartElement("sst");
            writer.WriteAttributeString("xmlns", "http://schemas.openxmlformats.org/spreadsheetml/2006/main");
            writer.WriteAttributeString("count", string.Format("{0}", sstCount));
            writer.WriteAttributeString("uniqueCount", string.Format("{0}", sstList.Count));

            for (int index = 0; index < sstList.Count; index++)
            {
                writer.WriteStartElement("si");

                string st = (string)sstList[index];

                //remove codes 0x00-0x1f
                StringBuilder sb = new StringBuilder();
                foreach (char ch in st)
                {
                    if (((int)ch >= 32) || (ch == '\n') || (ch == '\t')) sb.Append(ch);
                }
                st = sb.ToString();

                if (sstHashIsTags.ContainsKey(index))
                {
                    writer.WriteRaw(st);
                }
                else
                {
                    ConvertTextToExcelString(writer, st);
                }

                writer.WriteEndElement();
            }

            writer.WriteFullEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return ms;
        }
        #endregion

        #region WriteAdditionalData
        private MemoryStream WriteAdditionalData(string st, bool base64)
        {
            MemoryStream ms = new MemoryStream();
            byte[] bytes = null;
            if (base64)
            {
                bytes = global::System.Convert.FromBase64String(st);
            }
            else
            {
                bytes = Encoding.ASCII.GetBytes(st);
            }
            ms.Write(bytes, 0, bytes.Length);
            return ms;
        }
        #endregion

        #region WriteImage
        private MemoryStream WriteImage(int number)
        {
            MemoryStream ms = new MemoryStream();
            //			ExcelImageData image = (ExcelImageData)imageList[number];
            byte[] bytes = (byte[])imageCache.ImagePackedStore[number];
            ms.Write(bytes, 0, bytes.Length);
            return ms;
        }
        #endregion

        /// <summary>
        /// Exports rendered report to an Excel file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="fileName">A name of the file for exporting a rendered report.</param>
        public void ExportExcel(StiReport report, string fileName)
        {
            FileStream stream = null;
            try
            {
                StiFileUtils.ProcessReadOnly(fileName);
                stream = new FileStream(fileName, FileMode.Create);
                ExportExcel(report, stream);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Flush();
                    stream.Close();
                }
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



        public void ExportExcel(StiReport report, Stream stream, StiExcelExportSettings settings)
        {
            StiLogService.Write(this.GetType(), "Export report to Excel 2007 format");

            #region Export Dashboard
            if (!report.IsDocument && report.GetCurrentPage() is IStiDashboard)
            {
                StiDashboardExport.Export(report, stream, settings);
                return;
            }
            #endregion

#if NETSTANDARD || NETCOREAPP
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif

            string reportVersion = StiExportUtils.GetReportVersion();
            string reportVersion2 = StiExportUtils.GetReportVersion(report);

            #region Read settings
            if (settings == null)
                throw new ArgumentNullException("The 'settings' argument cannot be equal in null.");

            StiPagesRange pageRange = settings.PageRange;
            this.useOnePageHeaderAndFooter = settings.UseOnePageHeaderAndFooter;
            this.dataExportMode = settings.DataExportMode;
            this.exportObjectFormatting = settings.ExportObjectFormatting;
            this.exportEachPageToSheet = settings.ExportEachPageToSheet;
            this.exportHorizontalPageBreaks = settings.ExportPageBreaks;
            this.imageResolution = settings.ImageResolution;
            this.imageQuality = settings.ImageQuality;
            this.imageFormat = settings.ImageFormat;
            this.docCompanyString = settings.CompanyString == reportVersion ? reportVersion2 : settings.CompanyString;
            this.docLastModifiedString = settings.LastModifiedString == reportVersion ? reportVersion2 : settings.LastModifiedString;
            this.restrictEditing = settings.RestrictEditing;
            #endregion

            xmlIndentation = -1;
            if (imageResolution < 10) imageResolution = 10;
            imageResolution /= 100;
            if (imageFormat != null && imageFormat != ImageFormat.Png) imageFormat = ImageFormat.Jpeg;

            if (this.dataExportMode != StiDataExportMode.AllBands)
            {
                this.useOnePageHeaderAndFooter = false;
            }
            else
            {
                this.exportObjectFormatting = true;
            }

            if (StiOptions.Export.Excel.RestrictEditing > restrictEditing)
            {
                restrictEditing = StiOptions.Export.Excel.RestrictEditing;
            }

            this.report = report;

            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
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

                var allPages = pageRange.GetSelectedPages(report.RenderedPages);
                if (IsStopped) return;

                CurrentPassNumber = 0;
                MaximumPassNumber = 3;

                StatusString = StiLocalization.Get("Export", "ExportingCreatingDocument");
                PrepareData();

                //prepare matrixList
                int indexPage = 0;
                while (indexPage < allPages.Count)
                {
                    var pages = new StiPagesCollection(report, report.RenderedPages)
                    {
                        CacheMode = report.RenderedPages.CacheMode
                    };
                    pages.AddV2Internal(allPages.GetPageWithoutCache(indexPage));
                    int firstPageIndex = indexPage;
                    if (!exportEachPageToSheet)
                    {
                        string pageName = allPages.GetPageWithoutCache(indexPage).ExcelSheetValue;
                        while ((indexPage < allPages.Count - 1) && CompareExcellSheetNames(allPages.GetPageWithoutCache(indexPage + 1).ExcelSheetValue, pageName))
                        {
                            indexPage++;
                            pages.AddV2Internal(allPages.GetPageWithoutCache(indexPage));
                        }
                    }

                    string sheetName = pages[0].ExcelSheetValue;
                    if ((sheetName == null) || (sheetName == string.Empty))
                    {
                        sheetName = string.Format("Page {0}", sheetNameList.Count + 1);
                    }
                    sheetName = sheetName.Replace("'", "");
                    string sheetSuffix = string.Empty;
                    int sheetIndex = 1;

                    PrepareMatrix(pages);
                    int minRowIndex = 0;

                    do
                    {
                        firstPageIndexList.Add(firstPageIndex);
                        matrixList.Add(matrix);
                        int maxRowIndex = matrix.CoordY.Count - 1;
                        if (maxRowIndex - minRowIndex > MaximumSheetHeight)
                        {
                            maxRowIndex = minRowIndex + MaximumSheetHeight;
                        }
                        else
                        {
                            matrix = null;
                        }
                        minRowList.Add(minRowIndex);
                        maxRowList.Add(maxRowIndex);
                        minRowIndex = maxRowIndex;
                        if ((matrix != null) || (sheetSuffix.Length > 0))
                        {
                            sheetSuffix = string.Format(" part{0}", sheetIndex++);
                            if (sheetName.Length > 24) sheetName = sheetName.Substring(0, 24);
                        }
                        else
                        {
                            if (sheetName.Length > 30) sheetName = sheetName.Substring(0, 30);
                        }
                        sheetNameList.Add(sheetName + sheetSuffix);
                        if (IsStopped) return;
                    }
                    while (matrix != null);

                    indexPage++;
                }

                #region check sheet names
                var ht = new Hashtable();
                for (int index = 0; index < sheetNameList.Count; index++)
                {
                    string titleString = (string)sheetNameList[index];
                    titleString = titleString.Replace("*", "_").Replace("\\", "_").Replace("/", "_").Replace("[", "_").Replace("]", "_").Replace(":", "_").Replace("?", "_");
                    //if ((titleString == null) || (titleString == string.Empty))
                    //{
                    //    titleString = string.Format("Page {0}", index + 1);
                    //}
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
                    sheetNameList[index] = titleString;
                }
                #endregion

                var zip = new StiZipWriter20();
                zip.Begin(stream, true);

                CurrentPassNumber = 2;

                #region Trial
#if SERVER
                var isTrial = StiVersionX.IsSvr;
#else
			    var key = StiLicenseKeyValidator.GetLicenseKey();
			    var isTrial = !StiLicenseKeyValidator.IsValidOnNetFramework(key);
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
                #endregion

                imageListOffset = 0;
                for (int indexSheet = 0; indexSheet < matrixList.Count; indexSheet++)
                {
                    barCodeLinesList = new List<BarCodeLineData>();
                    hyperlinkList = new ArrayList();
                    zip.AddFile(string.Format("xl/worksheets/sheet{0}.xml", indexSheet + 1), WriteSheet(indexSheet, allPages[(int)firstPageIndexList[indexSheet]]));

                    if ((hyperlinkList.Count > 0) || (imageList.Count - imageListOffset > 0) || (barCodeLinesList.Count > 0) || isTrial)
                    {
                        zip.AddFile(string.Format("xl/worksheets/_rels/sheet{0}.xml.rels", indexSheet + 1), WriteSheetRels(indexSheet));
                    }
                    if ((imageList.Count - imageListOffset > 0) || (barCodeLinesList.Count > 0) || isTrial)
                    {
                        zip.AddFile(string.Format("xl/drawings/_rels/drawing{0}.xml.rels", indexSheet + 1), WriteDrawingRels(indexSheet));
                        zip.AddFile(string.Format("xl/drawings/drawing{0}.xml", indexSheet + 1), WriteDrawing(indexSheet, isTrial ? allPages[(int)firstPageIndexList[indexSheet]] : null));
                    }
                    imageListOffset = imageList.Count;

                    barCodeLinesList.Clear();
                    barCodeLinesList = null;
                    if (IsStopped) return;
                }

                zip.AddFile("[Content_Types].xml", WriteContentTypes());
                zip.AddFile("_rels/.rels", WriteMainRels());
                zip.AddFile("docProps/app.xml", WriteDocPropsApp());
                zip.AddFile("docProps/core.xml", WriteDocPropsCore());
                zip.AddFile("xl/_rels/workbook.xml.rels", WriteWorkbookRels());
                zip.AddFile("xl/workbook.xml", WriteWorkbook());
                zip.AddFile("xl/styles.xml", WriteStyles());
                if (sstList.Count > 0)
                {
                    zip.AddFile("xl/sharedStrings.xml", WriteSST());
                }
                if (imageCache.ImagePackedStore.Count > 0)
                {
                    for (int index = 0; index < imageCache.ImagePackedStore.Count; index++)
                    {
                        zip.AddFile(string.Format("xl/media/image{0:D5}.{1}", index + 1, GetImageFormatExtension(imageCache.ImageFormatStore[index])), WriteImage(index));
                    }
                }

                #region Trial
                if (isTrial)
                {
                    zip.AddFile("xl/media/imageAdditional.png", WriteAdditionalData(StiExportUtils.AdditionalData, true));
                    zip.AddFile("xl/drawings/_rels/vmlDrawingAdditional.vml.rels", WriteAdditionalData("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>\r\n<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\"><Relationship Id=\"dId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/image\" Target=\"../media/imageAdditional.png\"/></Relationships>", false));
                    zip.AddFile("xl/drawings/vmlDrawingAdditional.vml", WriteAdditionalData(
                        "<xml xmlns:v=\"urn:schemas-microsoft-com:vml\"\r\n xmlns:o=\"urn:schemas-microsoft-com:office:office\"\r\n xmlns:x=\"urn:schemas-microsoft-com:office:excel\">\r\n <o:shapelayout v:ext=\"edit\">\r\n  <o:idmap v:ext=\"edit\" data=\"1\"/>\r\n" +
                        " </o:shapelayout><v:shapetype id=\"_x0000_t75\" coordsize=\"21600,21600\" o:spt=\"75\"\r\n  o:preferrelative=\"t\" path=\"m@4@5l@4@11@9@11@9@5xe\" filled=\"f\" stroked=\"f\">\r\n  <v:stroke joinstyle=\"miter\"/>\r\n  <v:formulas>" +
                        "\r\n   <v:f eqn=\"if lineDrawn pixelLineWidth 0\"/>\r\n   <v:f eqn=\"sum @0 1 0\"/>\r\n   <v:f eqn=\"sum 0 0 @1\"/>\r\n   <v:f eqn=\"prod @2 1 2\"/>\r\n   <v:f eqn=\"prod @3 21600 pixelWidth\"/>\r\n   <v:f eqn=\"prod @3 21600 pixelHeight\"/>\r\n   <v:f eqn=\"sum @0 0 1\"/>" +
                        "\r\n   <v:f eqn=\"prod @6 1 2\"/>\r\n   <v:f eqn=\"prod @7 21600 pixelWidth\"/>\r\n   <v:f eqn=\"sum @8 21600 0\"/>\r\n   <v:f eqn=\"prod @7 21600 pixelHeight\"/>\r\n   <v:f eqn=\"sum @10 21600 0\"/>\r\n  </v:formulas>\r\n  <v:path o:extrusionok=\"f\" gradientshapeok=\"t\" o:connecttype=\"rect\"/>" +
                        "\r\n  <o:lock v:ext=\"edit\" aspectratio=\"t\"/>\r\n </v:shapetype><v:shape id=\"CH\" o:spid=\"_x0000_s1027\" type=\"#_x0000_t75\"\r\n  style='position:absolute;margin-left:0;margin-top:0;width:365pt;height:298pt;\r\n  z-index:1'>" +
                        "\r\n  <v:imagedata o:relid=\"dId1\" o:title=\"additional\"/>\r\n  <o:lock v:ext=\"edit\" rotation=\"t\"/>\r\n </v:shape></xml>", false));
                }
                #endregion

                zip.End();
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
                fontList = null;
                fillList = null;
                borderList = null;
                xfList = null;
                sstList.Clear();
                sstList = null;
                sstHash.Clear();
                sstHash = null;
                sstHashIsTags.Clear();
                sstHashIsTags = null;
                formatList = null;
                sheetNameList = null;
                imageList.Clear();
                imageList = null;
                imageCache.Clear();
                printAreasList = null;
                matrixList = null;
                firstPageIndexList = null;
                hyperlinkList = null;
                minRowList = null;
                maxRowList = null;

                if (report.RenderedPages.CacheMode) StiMatrix.GCCollect();
            }
        }
        #endregion
    }
}