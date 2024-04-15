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
using System.IO;
using Stimulsoft.Report.Components;
using Stimulsoft.Base;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Licenses;
using Stimulsoft.Base.Services;
using Stimulsoft.Base.Zip;
using System.Threading;
using System.Collections.Generic;
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
using System.Security.Cryptography;
#endif

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Image = Stimulsoft.Drawing.Image;
using ImageFormat = Stimulsoft.Drawing.Imaging.ImageFormat;
using Metafile = Stimulsoft.Drawing.Imaging.Metafile;
#endif

namespace Stimulsoft.Report.Export
{
    /// <summary>
    /// A class for the Word 2007 Export.
    /// </summary>
    [StiServiceBitmap(typeof(StiExportService), "Stimulsoft.Report.Images.Dictionary.ResourceWord.png")]
    public class StiWord2007ExportService : StiExportService
    {
        #region StiExportService override
        /// <summary>
		/// Gets or sets a default extension of export. 
		/// </summary>
		public override string DefaultExtension => "docx";

        public override StiExportFormat ExportFormat => StiExportFormat.Word2007;

        /// <summary>
        /// Gets a group of the export in the context menu.
        /// </summary>
        public override string GroupCategory => "Word";

        /// <summary>
        /// Gets a position of the export in the context menu.
        /// </summary>
        public override int Position => (int)StiExportPosition.Word2007;

        /// <summary>
        /// Gets an export name in the context menu.
        /// </summary>
        public override string ExportNameInMenu => StiLocalization.Get("Export", "ExportTypeWord2007File");

        /// <summary>
        /// Exports a document to the stream without dialog of the saving file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream in which report will be exported.</param>
        /// <param name="settings">A settings for the report exporting.</param>
        public override void ExportTo(StiReport report, Stream stream, StiExportSettings settings)
        {
            ExportWord(report, stream, settings as StiWord2007ExportSettings);
            InvokeExporting(100, 100, 1, 1);
        }

        /// <summary>
        /// Exports a rendered report to the Word file.
        /// Also exported document can be sent via e-mail.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="fileName">A name of the file for exporting a rendered report.</param>
        /// <param name="sendEMail">A parameter indicating whether the exported report will be sent via e-mail.</param>
        public override void Export(StiReport report, string fileName, bool sendEMail, StiGuiMode guiMode)
        {
            using (var form = StiGuiOptions.GetExportFormRunner("StiWord2007ExportSetupForm", guiMode, this.OwnerWindow))
            {
                form["CurrentPage"] = report.CurrentPrintPage;
                form["OpenAfterExportEnabled"] = !sendEMail;

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
        /// Returns a filter for the Word files.
        /// </summary>
        /// <returns>Returns a filter for the Word files.</returns>
        public override string GetFilter() => StiLocalization.Get("FileFilters", "Word2007Files");
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

                            var settings = new StiWord2007ExportSettings
                            {
                                PageRange = form["PagesRange"] as StiPagesRange,
                                UsePageHeadersAndFooters = (bool)form["UsePageHeadersAndFooters"],
                                ImageQuality = (float)form["ImageQuality"],
                                ImageResolution = (float)form["Resolution"],
                                RemoveEmptySpaceAtBottom = (bool)form["RemoveEmptySpaceAtBottom"],
                                RestrictEditing = (StiWord2007RestrictEditing)form["RestrictEditing"]
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

        #region Fiedls
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

        internal bool RemoveEmptySpaceAtBottom { get; private set; } = StiOptions.Export.Word.RemoveEmptySpaceAtBottom;

        private Hashtable fontList;
        private ArrayList styleList;
        private StiImageCache imageCache;
        private Hashtable bookmarkList;
        private Hashtable hyperlinkList;
        private ArrayList embedsList;

        private int xmlIndentation = 1;

        private float imageQuality = 0.75f;
        private float imageResolution = 96;	//dpi
        private string lineSpace;
        private string lineSpace2;
        private bool usePageHeadersAndFooters;
        private StiWord2007RestrictEditing restrictEditing = StiWord2007RestrictEditing.No;

        private ArrayList headersData;
        private ArrayList headersRels;
        private ArrayList footersData;
        private ArrayList footersRels;

        private string docCompanyString;
        private string docLastModifiedString;

        private Hashtable fontsToCorrectHeight = new Hashtable() {
            { "Arial Unicode MS", null },
            {"Cascadia Code ExtraLight", null },
            {"Cascadia Code SemiBold", null },
            { "Cascadia Code SemiLight", null },
            { "Cascadia Mono ExtraLight", null },
            { "Cascadia Mono Light", null },
            { "Cascadia Mono SemiBold", null },
            { "Cascadia Mono SemiLight", null },
            { "Malgun Gothic", null },
            { "Malgun Gothic Semilight", null },
            { "Microsoft JhengHei", null },
            { "Microsoft JhengHei Light", null },
            { "Microsoft JhengHei UI", null },
            { "Microsoft JhengHei UI Light", null },
            { "Microsoft YaHei", null },
            { "Microsoft YaHei Light", null },
            { "Microsoft YaHei UI", null },
            { "Microsoft YaHei UI Light", null },
            { "MingLiU_HKSCS-ExtB", null },
            { "MingLiU-ExtB", null },
            { "MS Gothic", null },
            { "MS PGothic", null },
            { "MS UI Gothic", null },
            { "NSimSun", null },
            { "PMingLiU-ExtB", null },
            { "Segoe MDL2 Assets", null },
            { "SimSun", null },
            { "SimSun-ExtB", null },
            { "Yu Gothic", null },
            { "Yu Gothic Light", null },
            { "Yu Gothic Medium", null },
            { "Yu Gothic UI", null },
            { "Yu Gothic UI Light", null },
            { "Yu Gothic UI Semibold", null },
            { "Yu Gothic UI Semilight", null },
            { "\u5FAE\u8F6F\u96C5\u9ED1", null }   // 微软雅黑, Microsoft YaHei
        };

        #region struct StiWord2007StyleInfo
        private struct StiWord2007StyleInfo
        {
            public string Name;
            public StiTextHorAlignment Alignment;
            public bool RightToLeft;
            public string FontName;
            public int FontSize;
            public bool Bold;
            public bool Italic;
            public bool Underline;
            public Color TextColor;
        }
        #endregion

        #region Utils

        #region GetImageExt
        private static string GetImageExt(ImageFormat format)
        {
            string fileExt = "jpeg";
            if (format == ImageFormat.Emf) fileExt = "emf";
            if (format == ImageFormat.Png) fileExt = "png";
            return fileExt;
        }
        #endregion

        #region GetLineStyle
        private string GetLineStyle(StiPenStyle penStyle)
        {
            switch (penStyle)
            {
                case StiPenStyle.Solid:
                    return "single";

                case StiPenStyle.Dot:
                    return "dotted";

                case StiPenStyle.Dash:
                    return "dashSmallGap";

                case StiPenStyle.DashDot:
                    return "dotDash";

                case StiPenStyle.DashDotDot:
                    return "dotDotDash";

                case StiPenStyle.Double:
                    return "double";

                default:
                    return string.Empty;
            }
        }
        #endregion

        #region GetColorString
        private string GetColorString(Color color)
        {
            if (color.A == 0) return "auto";
            if (color.A < 32) return "FFFFFF";
            return color.ToArgb().ToString("X8").Substring(2);
        }
        #endregion

        #region GetStyleNumber
        private int GetStyleNumber(ArrayList tmpStyleList, StiWord2007StyleInfo styleInfo)
        {
            if (tmpStyleList.Count > 0)
            {
                for (int index = 0; index < tmpStyleList.Count; index++)
                {
                    var tmpStyle = (StiWord2007StyleInfo)tmpStyleList[index];
                    if ((tmpStyle.Alignment == styleInfo.Alignment) &&
                        (tmpStyle.Name == styleInfo.Name) &&
                        (tmpStyle.FontName == styleInfo.FontName) &&
                        (tmpStyle.FontSize == styleInfo.FontSize) &&
                        (tmpStyle.Bold == styleInfo.Bold) &&
                        (tmpStyle.Italic == styleInfo.Italic) &&
                        (tmpStyle.Underline == styleInfo.Underline) &&
                        (tmpStyle.TextColor == styleInfo.TextColor) &&
                        (tmpStyle.RightToLeft == styleInfo.RightToLeft))
                    {
                        //style is already in table, return number
                        return index;
                    }
                }
            }
            //add style to table, return style number
            tmpStyleList.Add(styleInfo);
            int temp = tmpStyleList.Count - 1;
            return temp;
        }
        #endregion

        #region GetStyleFromComponent
        private int GetStyleFromComponent(StiComponent component)
        {
            if ((component != null) && (!string.IsNullOrEmpty(component.ComponentStyle)))
            {
                var mFont = component as IStiFont;
                var mTextBrush = component as IStiTextBrush;
                var mTextHorAlign = component as IStiTextHorAlignment;
                var textOpt = component as IStiTextOptions;

                var style = new StiWord2007StyleInfo();
                style.Name = component.ComponentStyle;
                if (mFont != null)
                {
                    style.FontName = mFont.Font.Name;
                    style.FontSize = (int)Math.Round(mFont.Font.SizeInPoints * 2, 0);
                    style.Bold = mFont.Font.Bold;
                    style.Italic = mFont.Font.Italic;
                    style.Underline = mFont.Font.Underline;
                }
                if (mTextBrush != null)
                {
                    style.TextColor = StiBrush.ToColor(mTextBrush.TextBrush);
                }
                if (mTextHorAlign != null)
                {
                    style.Alignment = mTextHorAlign.HorAlignment;
                }
                if (textOpt != null)
                {
                    style.RightToLeft = textOpt.TextOptions.RightToLeft;
                }
                return GetStyleNumber(styleList, style);
            }
            return -1;
        }
        #endregion

        #region StringToUrl
        private string StringToUrl(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            var output = new StringBuilder();
            if (input.StartsWith("file:"))
            {
                foreach (char ch in input)
                {
                    if ((ch < 0x20) || (ch == '"'))
                    {
                        output.Append(string.Format("%{0:x2}", ch));
                    }
                    else
                    {
                        output.Append(ch);
                    }
                }
            }
            else
            {
                foreach (char ch in input)
                {
                    if ((ch < 0x20) || (wrongUrlSymbols.IndexOf(ch) != -1))
                    {
                        output.Append('_');
                    }
                    else
                    {
                        output.Append(ch);
                    }
                }
            }

            return output.ToString();
        }
        //                                  space "   #   %   &   '   *   ,   :   ;   <   >   ?   [   ^   `   {   |   }   
        //private string wrongUrlSymbols = "\x20\x22\x23\x25\x26\x27\x2a\x2c\x3a\x3b\x3c\x3e\x3f\x5b\x5e\x60\x7b\x7c\x7d";
        private string wrongUrlSymbols = "\x20\x22\x27\x2a\x2c\x3b\x3c\x3e\x5b\x5e\x60\x7b\x7c\x7d";
        #endregion

        private int GetHyperlinkRefId(string url)
        {
            if (url.StartsWith("www."))
            {
                url = @"http:\" + url;
            }
            int hypRefId;
            if (hyperlinkList.ContainsKey(url))
            {
                hypRefId = (int)hyperlinkList[url];
            }
            else
            {
                hypRefId = hyperlinkList.Count;
                hyperlinkList.Add(url, hypRefId);
            }
            return hypRefId;
        }

        //conversion from hundredths of inch to twips
        private static double HiToTwips
        {
            get
            {
                //				return 14.4 / 20 * 1.028;
                return 14.4 * 0.995;
            }
        }

        private int Convert(double x)
        {
            return (int)Math.Round((decimal)(x * HiToTwips));
        }
        private int ConvertHiToTwips(double x)
        {
            return (int)Math.Round((decimal)(x * 14.4));
        }

        // 1 inch = 914400 EMU
        private int ConvertTwipsToEmu(double x)
        {
            return (int)Math.Round((decimal)((x / HiToTwips) / 100 * 914400));
        }

        private string ConvertStringToBookmark(string inputString)
        {
            var sbOutput = new StringBuilder();
            foreach (char ch in inputString)
            {
                if (char.IsLetterOrDigit(ch)) sbOutput.Append(ch);
            }
            if (sbOutput.Length > 0 && char.IsDigit(sbOutput[0])) sbOutput.Insert(0, 'b');
            return sbOutput.ToString();
        }
        #endregion

        #region WriteFromMatrix
        private void WriteFromMatrix(XmlTextWriter writer, int startLine, int endLine, bool outHeadersAndFooters)
        {
            int maxCoordX = Matrix.CoordX.Count;
            //if (maxCoordX > 64) maxCoordX = 64;

            #region recalculate matrix coordinates
            var wordCoordX = new int[Matrix.CoordX.Count];
            for (int indexColumn = 0; indexColumn < Matrix.CoordX.Count; indexColumn++)
            {
                double columnX = (double)Matrix.CoordX.GetByIndex(indexColumn);
                wordCoordX[indexColumn] = Convert(columnX);
            }
            var wordCoordY = new int[Matrix.CoordY.Count];
            for (int indexRow = 0; indexRow < Matrix.CoordY.Count; indexRow++)
            {
                double columnY = (double)Matrix.CoordY.GetByIndex(indexRow);
                wordCoordY[indexRow] = Convert(columnY);
            }
            #endregion

            bool isTable = false;
            int skipCounter = 0;

            #region rows
            var readyCells = new bool[Matrix.CoordY.Count, Matrix.CoordX.Count];
            var readyCellsVert = new bool[Matrix.CoordY.Count, Matrix.CoordX.Count];
            var lastHeaderName = string.Empty;

            CurrentPassNumber = 2 + (StiOptions.Export.Word.DivideSegmentPages ? 1 : 0);
            double progressScale = Math.Max(Matrix.CoordY.Count / 200f, 1f);
            int progressValue = 0;

            for (int indexRow = 1; indexRow < Matrix.CoordY.Count; indexRow++)
            {
                int currentProgress = (int)(indexRow / progressScale);
                if (currentProgress > progressValue)
                {
                    progressValue = currentProgress;
                    InvokeExporting(indexRow, matrix.CoordY.Count, CurrentPassNumber, MaximumPassNumber);
                }

                if (matrix.isPaginationMode && matrix.HorizontalPageBreaksHash.ContainsKey(indexRow)) continue; //skip pagination space

                bool needOutLine = ((indexRow - 1) >= startLine) && ((indexRow - 1) <= endLine);
                if (outHeadersAndFooters == false)
                {
                    if ((matrix.LinePlacement[indexRow - 1] == StiMatrix.StiTableLineInfo.PageHeader) ||
                        (matrix.LinePlacement[indexRow - 1] == StiMatrix.StiTableLineInfo.PageFooter) ||
                        (matrix.LinePlacement[indexRow - 1] == StiMatrix.StiTableLineInfo.Trash))
                    {
                        needOutLine = false;
                    }
                }

                if (skipCounter > 0)
                {
                    skipCounter--;
                    needOutLine = false;
                }

                double maxTopMargin = 100005;

                #region check for rtfparagraph, rtfnewpage and CanBreak
                var paragraphList = new ArrayList();
                bool needNewPage = false;
                if (needOutLine)
                {
                    for (int columnIndex = 1; columnIndex < maxCoordX; columnIndex++)
                    {
                        var cell = Matrix.Cells[indexRow - 1, columnIndex - 1];
                        if ((readyCells[indexRow, columnIndex] == false) && (cell != null) && (cell.Component != null))
                        {
                            if (cell.Component.TagValue != null)
                            {
                                string cellTag = cell.Component.TagValue.ToString().ToLower();
                                if (cellTag.IndexOf("rtfparagraph", StringComparison.InvariantCulture) != -1)
                                {
                                    paragraphList.Add(cell);
                                    needOutLine = false;
                                }
                                if (cellTag.IndexOf("rtfnewpage", StringComparison.InvariantCulture) != -1) needNewPage = true;
                            }

                            StiMargins margins = null;
                            if (cell.Component is StiText) margins = (cell.Component as StiText).Margins;
                            if (cell.Component is StiRichText) margins = (cell.Component as StiRichText).Margins;
                            if (cell.Component is StiImage) margins = (cell.Component as StiImage).Margins;
                            if (margins != null)
                            {
                                if (margins.Top < maxTopMargin) maxTopMargin = margins.Top;
                                if (cell.Height > 0)
                                {
                                    maxTopMargin = 0;
                                    //maxBottomMargin = 0;
                                }
                            }
                        }
                    }
                }
                #endregion

                //if (maxTopMargin > 100000) maxTopMargin = 0;
                maxTopMargin *= HiToTwips;
                int maxTopMarginInt = (int)Math.Round(maxTopMargin);

                if (needNewPage)
                {
                    if (isTable)
                    {
                        writer.WriteFullEndElement();
                        isTable = false;
                    }
                    writer.WriteStartElement("w:p");
                    writer.WriteStartElement("w:r");
                    writer.WriteStartElement("w:br");
                    writer.WriteAttributeString("w:type", "page");
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }

                if (needOutLine)
                {
                    if ((usePageHeadersAndFooters) && (matrix.LinePlacement[indexRow - 1] == StiMatrix.StiTableLineInfo.HeaderAP))
                    {
                        string headerName = matrix.ParentBandName[indexRow - 1];
                        int symPos = headerName.IndexOf('\x1f');
                        if (symPos != -1)
                        {
                            headerName = headerName.Substring(0, symPos);
                        }
                        if (headerName != lastHeaderName)
                        {
                            lastHeaderName = headerName;
                            if (isTable)
                            {
                                writer.WriteFullEndElement();
                            }
                            writer.WriteStartElement("w:p");
                            writer.WriteEndElement();
                            writer.WriteStartElement("w:tbl");
                            writeTableInfo(writer, wordCoordX, maxCoordX);
                            isTable = true;
                        }
                    }

                    if (!isTable)
                    {
                        writer.WriteStartElement("w:tbl");
                        writeTableInfo(writer, wordCoordX, maxCoordX);
                        isTable = true;
                    }

                    writer.WriteStartElement("w:tr");

                    #region row info
                    int rowHeight = wordCoordY[indexRow] - wordCoordY[indexRow - 1];
                    writer.WriteStartElement("w:trPr");
                    if (!usePageHeadersAndFooters || StiOptions.Export.Word.ForceLineHeight)
                    {
                        writer.WriteStartElement("w:trHeight");
                        if ((!usePageHeadersAndFooters) && (StiOptions.Export.Word.LineHeightExactly) ||
                            usePageHeadersAndFooters && StiOptions.Export.Word.LineHeightExactlyForPHFMode)
                        {
                            writer.WriteAttributeString("w:hRule", "exact");
                        }
                        writer.WriteAttributeString("w:val", string.Format("{0}", rowHeight));
                        writer.WriteEndElement();
                    }
                    if ((usePageHeadersAndFooters) && (matrix.LinePlacement[indexRow - 1] == StiMatrix.StiTableLineInfo.HeaderAP))
                    {
                        writer.WriteStartElement("w:tblHeader");
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                    #endregion
                }

                int curCoordX = 0;
                for (int indexColumn = 1; indexColumn < maxCoordX; indexColumn++)
                {
                    var cell = Matrix.Cells[indexRow - 1, indexColumn - 1];

                    if ((!readyCells[indexRow, indexColumn]) || (readyCellsVert[indexRow, indexColumn]))
                    {
                        curCoordX++;
                        bool needContent = true;
                        if (readyCellsVert[indexRow, indexColumn])
                        {
                            int tempIndexRow = indexRow;
                            while (readyCellsVert[tempIndexRow, indexColumn]) tempIndexRow--;
                            cell = Matrix.Cells[tempIndexRow - 1, indexColumn - 1];
                            needContent = false;
                        }

                        if ((cell != null) && (needContent))
                        {
                            #region Range
                            for (int yy = 0; yy <= cell.Height; yy++)
                            {
                                for (int xx = 0; xx <= cell.Width; xx++)
                                {
                                    readyCells[indexRow + yy, indexColumn + xx] = true;
                                }
                            }
                            if (cell.Height > 0)
                            {
                                for (int yy = 1; yy <= cell.Height; yy++)
                                {
                                    readyCellsVert[indexRow + yy, indexColumn] = true;
                                }
                            }
                            #endregion
                        }

                        if (needOutLine)
                        {
                            writer.WriteStartElement("w:tc");

                            if (cell != null)
                            {
                                #region cell properties
                                writer.WriteStartElement("w:tcPr");

                                #region cell width
                                int cellWidth = 0;
                                for (int indexMerg = 0; indexMerg < cell.Width + 1; indexMerg++)
                                {
                                    cellWidth += wordCoordX[indexColumn + indexMerg] - wordCoordX[indexColumn - 1 + indexMerg];
                                }
                                writer.WriteStartElement("w:tcW");
                                writer.WriteAttributeString("w:w", string.Format("{0}", cellWidth));
                                writer.WriteAttributeString("w:type", "dxa");
                                writer.WriteEndElement();
                                #endregion

                                #region merged cells
                                if (cell.Width > 0)
                                {
                                    //merged width
                                    writer.WriteStartElement("w:gridSpan");
                                    writer.WriteAttributeString("w:val", string.Format("{0}", cell.Width + 1));
                                    writer.WriteEndElement();
                                }
                                if (cell.Height > 0)
                                {
                                    //merged height
                                    if (!readyCellsVert[indexRow, indexColumn])
                                    {
                                        //on first line
                                        writer.WriteStartElement("w:vMerge");
                                        writer.WriteAttributeString("w:val", "restart");
                                        writer.WriteEndElement();
                                    }
                                    else
                                    {
                                        //on other lines
                                        writer.WriteStartElement("w:vMerge");
                                        writer.WriteEndElement();
                                    }
                                }
                                #endregion

                                #region text direction
                                if (cell.CellStyle.TextOptions != null)
                                {
                                    float textAngle = cell.CellStyle.TextOptions.Angle;
                                    string stTextAngle = string.Empty;
                                    if ((textAngle > 45) && (textAngle < 135)) stTextAngle = "btLr";
                                    if ((textAngle > 225) && (textAngle < 315)) stTextAngle = "tbRl";
                                    if (stTextAngle != string.Empty)
                                    {
                                        writer.WriteStartElement("w:textDirection");
                                        writer.WriteAttributeString("w:val", stTextAngle);
                                        writer.WriteEndElement();
                                    }
                                }
                                #endregion

                                #region cell margins
                                bool isEmbedRtfFlag = cell.Component is StiText && (cell.Component as StiText).CheckAllowHtmlTags() && !StiOptions.Export.Word.RenderHtmlTagsAsImage;
                                if (needContent && (cell.Component is StiText) && (!cell.Component.IsExportAsImage(StiExportFormat.Word2007) || isEmbedRtfFlag))
                                {
                                    var stiText = cell.Component as StiText;
                                    int mLeft = Convert(stiText.Margins.Left);
                                    int mRight = Convert(stiText.Margins.Right - StiOptions.Export.Word.RightMarginCorrection);
                                    int mTop = Convert(stiText.Margins.Top);
                                    if (mTop > maxTopMarginInt) mTop = maxTopMarginInt;
                                    if ((mLeft > 0) || (mRight > 0) || (mTop > 0))
                                    {
                                        writer.WriteStartElement("w:tcMar");
                                        if (mTop > 0)
                                        {
                                            writer.WriteStartElement("w:top");
                                            writer.WriteAttributeString("w:w", string.Format("{0}", mTop));
                                            writer.WriteAttributeString("w:type", "dxa");
                                            writer.WriteEndElement();
                                        }
                                        if (mLeft > 0)
                                        {
                                            writer.WriteStartElement("w:left");
                                            writer.WriteAttributeString("w:w", string.Format("{0}", mLeft));
                                            writer.WriteAttributeString("w:type", "dxa");
                                            writer.WriteEndElement();
                                        }
                                        if (mRight > 0)
                                        {
                                            writer.WriteStartElement("w:right");
                                            writer.WriteAttributeString("w:w", string.Format("{0}", mRight));
                                            writer.WriteAttributeString("w:type", "dxa");
                                            writer.WriteEndElement();
                                        }
                                        writer.WriteEndElement();
                                    }
                                }
                                #endregion

                                #region vertical align
                                string stVertAlign = string.Empty;
                                switch (cell.CellStyle.VertAlignment)
                                {
                                    case StiVertAlignment.Center:
                                        stVertAlign = "center";
                                        break;

                                    case StiVertAlignment.Bottom:
                                        stVertAlign = "bottom";
                                        break;
                                }
                                if (stVertAlign != string.Empty)
                                {
                                    writer.WriteStartElement("w:vAlign");
                                    writer.WriteAttributeString("w:val", stVertAlign);
                                    writer.WriteEndElement();
                                }
                                #endregion

                                WriteBorders(writer, cell.Top, cell.Left, cell.Height + 1, cell.Width + 1);

                                #region fill color
                                if ((cell.Component != null) && !cell.Component.IsExportAsImage(StiExportFormat.Word2007))
                                {
                                    writer.WriteStartElement("w:shd");
                                    writer.WriteAttributeString("w:val", "clear");
                                    writer.WriteAttributeString("w:color", "auto");
                                    writer.WriteAttributeString("w:fill", GetColorString(cell.CellStyle.Color));
                                    writer.WriteEndElement();
                                }
                                #endregion

                                writer.WriteEndElement();
                                #endregion

                                bool needPerm = (restrictEditing == StiWord2007RestrictEditing.ExceptEditableFields) && (cell.Component != null) && (cell.Component is StiText) && (cell.Component as StiText).Editable;
                                if (needPerm)
                                {
                                    writer.WriteStartElement("w:permStart");
                                    writer.WriteAttributeString("w:id", "0");
                                    writer.WriteEndElement();
                                }

                                bool needEmptyParagraph = true;
                                if (needContent)
                                {
                                    #region cell contents
                                    WriteCellContent(writer, cell, ref needEmptyParagraph, indexRow, indexColumn, wordCoordX, wordCoordY);
                                    #endregion
                                }
                                if (needEmptyParagraph)
                                {
                                    writer.WriteStartElement("w:p");
                                    writer.WriteFullEndElement();
                                }

                                if (needPerm)
                                {
                                    writer.WriteStartElement("w:permEnd");
                                    writer.WriteAttributeString("w:id", "0");
                                    writer.WriteEndElement();
                                }
                            }
                            else
                            {
                                #region prepare data
                                string baseSt = null;
                                var baseSides = RenderBorder2TableGetValues(indexRow, indexColumn, ref baseSt);
                                int newWidth = 0;
                                if ((baseSides & StiBorderSides.Right) == 0)
                                {
                                    while ((indexColumn + newWidth + 1 < maxCoordX) && (Matrix.Cells[indexRow - 1, indexColumn + newWidth + 1 - 1] == null) &&
                                        (readyCells[indexRow, indexColumn + newWidth + 1] == false) && (readyCellsVert[indexRow, indexColumn + newWidth + 1] == false))
                                    {
                                        string newSt = null;
                                        var newSides = RenderBorder2TableGetValues(indexRow, indexColumn + newWidth + 1, ref newSt);
                                        if ((newSides & StiBorderSides.Left) > 0) break;
                                        if ((newSides & (StiBorderSides.Top | StiBorderSides.Bottom)) != (baseSides & (StiBorderSides.Top | StiBorderSides.Bottom))) break;
                                        if (baseSt != newSt) break;
                                        newWidth++;
                                        if ((newSides & StiBorderSides.Right) > 0) break;
                                    }
                                    if (newWidth > 0)
                                    {
                                        for (int xx = 1; xx <= newWidth; xx++)
                                        {
                                            readyCells[indexRow, indexColumn + xx] = true;
                                        }
                                    }
                                }
                                #endregion

                                #region empty cell
                                writer.WriteStartElement("w:tcPr");
                                int cellWidth = wordCoordX[indexColumn + newWidth] - wordCoordX[indexColumn - 1];
                                writer.WriteStartElement("w:tcW");
                                writer.WriteAttributeString("w:w", string.Format("{0}", cellWidth));
                                writer.WriteAttributeString("w:type", "dxa");
                                writer.WriteEndElement();
                                if (newWidth > 0)
                                {
                                    //merged width
                                    writer.WriteStartElement("w:gridSpan");
                                    writer.WriteAttributeString("w:val", string.Format("{0}", newWidth + 1));
                                    writer.WriteEndElement();
                                }
                                WriteBorders(writer, indexRow - 1, indexColumn - 1, 1, newWidth + 1, usePageHeadersAndFooters);
                                writer.WriteEndElement();

                                writer.WriteStartElement("w:p");
                                writer.WriteFullEndElement();
                                #endregion
                            }
                            writer.WriteFullEndElement();
                        }
                    }
                    if (curCoordX >= 64) break;
                }

                if (needOutLine)
                {
                    writer.WriteFullEndElement();   //w:tr
                }

                foreach (StiCell paragraphCell in paragraphList)
                {
                    if (isTable)
                    {
                        writer.WriteEndElement();	//w:tbl
                    }
                    isTable = false;
                    skipCounter = paragraphCell.Height;
                    bool needEmptyParagraph2 = false;
                    WriteCellContent(writer, paragraphCell, ref needEmptyParagraph2, paragraphCell.Top + 1, paragraphCell.Left + 1, wordCoordX, wordCoordY);
                }
            }
            #endregion

            if (isTable)
            {
                writer.WriteEndElement();	//w:tbl
            }
        }

        private void WriteCellContent(XmlTextWriter writer, StiCell cell, ref bool needEmptyParagraph, int indexRow, int indexColumn, int[] wordCoordX, int[] wordCoordY)
        {
            #region get bookmarks, hyperlinks info
            string bkmRefText = null;
            int bkmRefId = -1;
            string bkm = Matrix.Bookmarks[indexRow - 1, indexColumn - 1];
            if ((bkm != null) && (!bookmarkList.ContainsKey(bkm)))
            {
                bkmRefId = bookmarkList.Count;
                bkmRefText = ConvertStringToBookmark(bkm);
                bookmarkList[bkm] = bkmRefId;
            }

            string hypRefText = null;
            int hypRefId = -1;
            if (cell.Component != null && cell.Component.HyperlinkValue != null)
            {
                string hyperlink = cell.Component.HyperlinkValue.ToString().Trim();
                if (hyperlink.Length > 0 && !hyperlink.StartsWith("javascript:"))
                {
                    if (hyperlink.StartsWith("#", StringComparison.InvariantCulture))
                    {
                        hypRefText = ConvertStringToBookmark(hyperlink.Substring(1));
                    }
                    else
                    {
                        hypRefText = StringToUrl(hyperlink);
                        hypRefId = GetHyperlinkRefId(hypRefText);
                    }
                }
            }
            #endregion

            #region cell contents
            if ((cell.Component is StiText) && (!cell.Component.IsExportAsImage(StiExportFormat.Word2007)) && !(cell.Component as StiText).CheckAllowHtmlTags())
            {
                #region StiText
                var sb = new StringBuilder(cell.Text);
                if (usePageHeadersAndFooters)
                {
                    var expr = cell.Component.TagValue as string;
                    if (!string.IsNullOrEmpty(expr))
                    {
                        if (expr.ToLowerInvariant().IndexOf("rtfnewpage", StringComparison.InvariantCulture) == -1 &&
                            (expr.IndexOf("#PageNumber#", StringComparison.InvariantCulture) != -1 ||
                             expr.IndexOf("#TotalPageCount#", StringComparison.InvariantCulture) != -1 ||
                             expr.IndexOf("#PageRef", StringComparison.InvariantCulture) != -1))
                        {
                            sb = new StringBuilder(expr);
                        }
                    }
                }

                #region count lines and make stringList
                var stringList = new ArrayList();
                var st = string.Empty;
                foreach (char ch in sb.ToString())
                {
                    if (char.IsControl(ch) && (ch != '\t'))
                    {
                        if (ch == '\n')
                        {
                            stringList.Add(st);
                            st = string.Empty;
                        }
                    }
                    else
                    {
                        st += ch;
                    }
                }
                if (st != string.Empty) stringList.Add(st);
                if (stringList.Count == 0) stringList.Add(st);
                #endregion

                if (stringList.Count > 0) needEmptyParagraph = false;

                int styleIndex = GetStyleFromComponent(cell.Component);

                for (int indexLine = 0; indexLine < stringList.Count; indexLine++)
                {
                    var textLine = (string)stringList[indexLine];
                    writer.WriteStartElement("w:p");

                    #region paragraph properties
                    writer.WriteStartElement("w:pPr");

                    var lineHeight = (cell.Component as StiText).LineSpacing;
                    if (lineSpace != null || lineHeight != 1)
                    {
                        string stLineSpace = lineSpace;
                        if (lineHeight != 1)
                        {
                            stLineSpace = ((int)Math.Round(StiOptions.Export.Word.LineSpacing * 240 * lineHeight)).ToString();
                        }
                        var font = cell.Component as IStiFont;
                        if (font != null && font.Font != null)
                        {
                            string fontName = font.Font.Name;
                            if (fontsToCorrectHeight.ContainsKey(fontName))
                            {
                                stLineSpace = lineSpace2;
                                if (lineHeight != 1)
                                {
                                    stLineSpace = ((int)Math.Round(StiOptions.Export.Word.LineSpacing * 240 * 0.772 * lineHeight)).ToString();
                                }
                            }
                        }

                        writer.WriteStartElement("w:spacing");
                        writer.WriteAttributeString("w:line", stLineSpace);
                        writer.WriteEndElement();
                    }

                    if (styleIndex != -1)
                    {
                        writer.WriteStartElement("w:pStyle");
                        writer.WriteAttributeString("w:val", string.Format("Style{0}", styleIndex));
                        writer.WriteEndElement();
                    }

                    #region horizontal align
                    var horAlignment = cell.CellStyle.HorAlignment;
                    bool rightToLeft = false;
                    if (cell.CellStyle.TextOptions != null) rightToLeft = cell.CellStyle.TextOptions.RightToLeft;

                    //string stHorAlignment = "left";
                    string stHorAlignment = string.Empty;
                    if (((horAlignment == StiTextHorAlignment.Left) &&
                        (rightToLeft == true)) ||
                        ((horAlignment == StiTextHorAlignment.Right) &&
                        (rightToLeft == false)))
                    {
                        stHorAlignment = "right";
                    }
                    if (horAlignment == StiTextHorAlignment.Center) stHorAlignment = "center";
                    if (horAlignment == StiTextHorAlignment.Width) stHorAlignment = "both";

                    if (stHorAlignment != string.Empty)
                    {
                        writer.WriteStartElement("w:jc");
                        writer.WriteAttributeString("w:val", stHorAlignment);
                        writer.WriteEndElement();
                    }
                    #endregion

                    if (styleIndex == -1)
                    {
                        writeRunProperties(writer, cell);
                    }

                    writer.WriteEndElement();
                    #endregion

                    if (hypRefText != null)
                    {
                        writer.WriteStartElement("w:hyperlink");
                        if (hypRefId == -1)
                        {
                            writer.WriteAttributeString("w:anchor", hypRefText);
                        }
                        else
                        {
                            writer.WriteAttributeString("r:id", string.Format("hId{0}", hypRefId));
                        }
                    }
                    if (bkmRefId != -1)
                    {
                        writer.WriteStartElement("w:bookmarkStart");
                        writer.WriteAttributeString("w:id", bkmRefId.ToString(CultureInfo.InvariantCulture));
                        writer.WriteAttributeString("w:name", bkmRefText);
                        writer.WriteEndElement();
                    }

                    if (textLine != string.Empty)
                    {
                        if (usePageHeadersAndFooters)
                        {
                            #region write by runs
                            textLine = textLine.Replace("#PageNumber#", "\x01").Replace("#TotalPageCount#", "\x02");

                            #region count lines and make runList
                            var runList = new ArrayList();
                            var sbr = new StringBuilder();
                            foreach (char ch in textLine)
                            {
                                if ((ch == '\x01') || (ch == '\x02'))
                                {
                                    if (sbr.Length > 0) runList.Add(sbr.ToString());
                                    runList.Add(ch.ToString(CultureInfo.InvariantCulture));
                                    sbr = new StringBuilder();
                                }
                                else
                                {
                                    sbr.Append(ch);
                                }
                            }
                            if (sbr.Length > 0) runList.Add(sbr.ToString());
                            if (runList.Count == 0) runList.Add(textLine);
                            #endregion

                            for (int indexRun = 0; indexRun < runList.Count; indexRun++)
                            {
                                var run = (string)runList[indexRun];
                                if ((run.Length > 0) && ((run[0] == '\x01') || (run[0] == '\x02')))
                                {
                                    #region write field
                                    writer.WriteStartElement("w:r");
                                    if (styleIndex == -1)
                                    {
                                        writeRunProperties(writer, cell);
                                    }
                                    writer.WriteStartElement("w:fldChar");
                                    writer.WriteAttributeString("w:fldCharType", "begin");
                                    writer.WriteEndElement();
                                    writer.WriteEndElement();	//w:r

                                    writer.WriteStartElement("w:r");
                                    writeRunProperties(writer, cell);
                                    writer.WriteStartElement("w:instrText");
                                    switch (run[0])
                                    {
                                        case '\x01':
                                            writer.WriteString("PAGE");
                                            break;
                                        case '\x02':
                                            writer.WriteString("NUMPAGES");
                                            break;
                                    }
                                    writer.WriteEndElement();
                                    writer.WriteEndElement();	//w:r

                                    writer.WriteStartElement("w:r");
                                    writeRunProperties(writer, cell);
                                    writer.WriteStartElement("w:fldChar");
                                    writer.WriteAttributeString("w:fldCharType", "end");
                                    writer.WriteEndElement();
                                    writer.WriteEndElement();	//w:r
                                    #endregion
                                }
                                else
                                {
                                    writer.WriteStartElement("w:r");
                                    if (styleIndex == -1)
                                    {
                                        writeRunProperties(writer, cell);
                                    }
                                    writer.WriteStartElement("w:t");
                                    writer.WriteAttributeString("xml:space", "preserve");
                                    writer.WriteString(run);
                                    writer.WriteEndElement();
                                    writer.WriteEndElement();	//w:r
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            writer.WriteStartElement("w:r");
                            if (styleIndex == -1)
                            {
                                writeRunProperties(writer, cell);
                            }
                            writer.WriteStartElement("w:t");
                            writer.WriteAttributeString("xml:space", "preserve");
                            writer.WriteString(textLine);
                            writer.WriteEndElement();
                            writer.WriteEndElement();	//w:r
                        }
                    }

                    if (bkmRefId != -1)
                    {
                        writer.WriteStartElement("w:bookmarkEnd");
                        writer.WriteAttributeString("w:id", bkmRefId.ToString(CultureInfo.InvariantCulture));
                        writer.WriteEndElement();
                    }
                    if (hypRefText != null)
                    {
                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();	//w:p
                }
                #endregion
            }
            else if (cell.Component is StiRichText && !StiOptions.Export.Word.RenderRichTextAsImage)
            {
                #region StiRichText
                var rich = cell.Component as StiRichText;
                string rtfText = rich.RtfText;

                int rtfPos = rtfText.TrimStart().ToLowerInvariant().IndexOf("rtf");
                if (rtfPos == -1 || rtfPos > 30)
                {
                    var richTextBox = new Controls.StiRichTextBox(false);
                    rtfText = rich.GetPreparedText(richTextBox);
                }

                if (StiOptions.Export.Word.AllowCorrectFontSize11Problem && !string.IsNullOrWhiteSpace(rtfText))
                {
                    #region Scan stylesheet table
                    int indexStylesheet = rtfText.IndexOf("{\\stylesheet", StringComparison.InvariantCulture);
                    int indexStylesheetEnd = 0;
                    if (indexStylesheet != -1)
                    {
                        int kolSkob = 0;
                        indexStylesheetEnd = indexStylesheet;
                        try
                        {
                            do
                            {
                                if (rtfText[indexStylesheetEnd] == '{') kolSkob++;
                                if (rtfText[indexStylesheetEnd] == '}') kolSkob--;
                                indexStylesheetEnd++;
                            } while (kolSkob > 0);
                        }
                        catch
                        {
                        }
                    }
                    #endregion

                    #region Font size correction
                    //rtfText = rtfText.Replace("\\fs22", "\\fs21");  //fix error for fontsize=11, set it to 10,5

                    var sbb1 = new StringBuilder(rtfText);
                    int indexFs = -1;
                    //bool hasChanges = false;
                    while ((indexFs = rtfText.IndexOf("\\fs22", indexFs + 1)) != -1)
                    {
                        if (indexFs > indexStylesheet && indexFs < indexStylesheetEnd)
                        {
                            sbb1[indexFs + 4] = '3';    //stylesheet table, 11.5
                        }
                        else
                        {
                            sbb1[indexFs + 4] = '1';    //10.5
                        }
                        //hasChanges = true;
                    }
                    //if (hasChanges)
                    //{
                    //    rtfText = sbb1.ToString();
                    //}
                    #endregion

                    #region Correct line height
                    bool hasChanges2 = false;
                    int indexSl = -1;
                    while ((indexSl = rtfText.IndexOf("\\sl", indexSl + 1)) != -1)
                    {
                        int indexSl2 = indexSl + 3;
                        while (char.IsDigit(rtfText, indexSl2)) indexSl2++;
                        int digLen = indexSl2 - indexSl;
                        if (digLen == 0) continue;

                        int res;
                        if (int.TryParse(rtfText.Substring(indexSl, indexSl2 - indexSl), out res))
                        {
                            int newRes = (int)(res * StiOptions.Export.Word.RichTextLineSpacingCorrection);
                            string newSt = newRes.ToString() + "   ";
                            for (int indexDig = 0; indexDig < digLen; indexDig++)
                            {
                                sbb1[indexSl + 3 + indexDig] = newSt[indexDig];
                            }
                            hasChanges2 = true;
                        }
                    }
                    if (!hasChanges2)
                    {
                        var slPard = string.Format("\\sl{0}\\slmult1", ((int)(240 * StiOptions.Export.Word.RichTextLineSpacingCorrection)).ToString());
                        var expnd = string.Format("\\expndtw{0}", StiOptions.Export.RichText.SpaceBetweenCharacters);
                        int indexPard = rtfText.Length - 1;
                        while ((indexPard = rtfText.LastIndexOf("\\pard\\", indexPard)) != -1)
                        {
                            sbb1.Insert(indexPard + 5, slPard + expnd);
                        }
                    }
                    #endregion

                    //if (hasChanges || hasChanges2)
                    //{
                    rtfText = sbb1.ToString();
                    //}

                    #region Correct Normal style name
                    if (indexStylesheet != -1)
                    {
                        int posName = rtfText.IndexOf(" Normal;}", indexStylesheet);
                        if ((posName != -1) && (posName < indexStylesheetEnd))
                        {
                            sbb1.Insert(posName + 7, "1");
                            rtfText = sbb1.ToString();
                        }
                    }
                    #endregion

                }

                embedsList.Add(rtfText);

                if (hypRefText != null)
                {
                    writer.WriteStartElement("w:hyperlink");
                    if (hypRefId == -1)
                    {
                        writer.WriteAttributeString("w:anchor", hypRefText);
                    }
                    else
                    {
                        writer.WriteAttributeString("r:id", string.Format("hId{0}", hypRefId));
                    }
                }

                writer.WriteStartElement("w:altChunk");
                writer.WriteAttributeString("r:id", string.Format("reId{0}", embedsList.Count));
                writer.WriteEndElement();

                if (hypRefText != null)
                {
                    writer.WriteEndElement();
                }
                #endregion
            }
            else if (cell.Component is StiText && (cell.Component as StiText).CheckAllowHtmlTags() && !StiOptions.Export.Word.RenderHtmlTagsAsImage)
            {
                if (!string.IsNullOrWhiteSpace(cell.Text))
                {
                    if (StiOptions.Export.Word.RenderHtmlTagsAsEmbeddedRichText)
                    {
                        #region write Html-tags as RichText embedded file
                        string st = StiRtfExportService.GetRtfFileFromHtmlTags(cell.Component as StiText, cell.Text);
                        if (StiOptions.Export.Word.AllowCorrectFontSize11Problem)
                        {
                            st = st.Replace("\\fs22", "\\fs21");  //fix error for fontsize=11, set it to 10,5
                        }
                        embedsList.Add(st);
                        writer.WriteStartElement("w:altChunk");
                        writer.WriteAttributeString("r:id", string.Format("reId{0}", embedsList.Count));
                        writer.WriteEndElement();
                        #endregion
                    }
                    else
                    {
                        writeHtmlTags(writer, cell);
                    }
                }
            }
            else if ((cell.Component != null) && cell.Component.IsExportAsImage(StiExportFormat.Word2007))
            {
                #region Image
                var exportImage = cell.Component as IStiExportImage;
                if (exportImage != null)
                {
                    float rsImageResolution = imageResolution;
                    var exportImageExtended = exportImage as IStiExportImageExtended;

                    using (var image = (exportImageExtended != null && exportImageExtended.IsExportAsImage(StiExportFormat.Word2007)) ?
                        exportImageExtended.GetImage(ref rsImageResolution, StiExportFormat.Word2007) : exportImage.GetImage(ref rsImageResolution))
                    {
                        if (image != null)
                        {
                            var img = Matrix.GetRealImageData(cell, image);

                            needEmptyParagraph = false;
                            int imageIndex = 0;
                            if (img != null) imageIndex = imageCache.AddImageInt(img);
                            else imageIndex = imageCache.AddImageInt(image);

                            int richWidth = -1;
                            int richHeight = -1;

                            if ((cell.Component is StiRichText) && StiOptions.Engine.FullTrust)
                            {
                                var rich = cell.Component as StiRichText;
                                rich.RenderMetafile();
                                richWidth = Convert(rich.Image.PhysicalDimension.Width / 25.4);
                                richHeight = Convert(rich.Image.PhysicalDimension.Height / 25.4);

                                imageCache.ImagePackedStore[imageIndex] = ConvertRichTextToImageInFullTrust(rich);
                                imageCache.ImageFormatStore[imageIndex] = ImageFormat.Emf;
                            }
                            string fileExt = GetImageExt((ImageFormat)imageCache.ImageFormatStore[imageIndex]);

                            #region write image info
                            writer.WriteStartElement("w:p");

                            #region paragraph properties
                            writer.WriteStartElement("w:pPr");

                            #region horizontal align
                            var horAlignment = cell.CellStyle.HorAlignment;
                            bool rightToLeft = false;
                            if (cell.CellStyle.TextOptions != null) rightToLeft = cell.CellStyle.TextOptions.RightToLeft;

                            if ((cell.Component as IStiTextHorAlignment) == null)
                            {
                                var horAlignComp = cell.Component as IStiHorAlignment;
                                if (horAlignComp != null)
                                {
                                    if (horAlignComp.HorAlignment == StiHorAlignment.Left) horAlignment = StiTextHorAlignment.Left;
                                    if (horAlignComp.HorAlignment == StiHorAlignment.Center) horAlignment = StiTextHorAlignment.Center;
                                    if (horAlignComp.HorAlignment == StiHorAlignment.Right) horAlignment = StiTextHorAlignment.Right;
                                }
                            }

                            //string stHorAlignment = "left";
                            var stHorAlignment = string.Empty;
                            if (((horAlignment == StiTextHorAlignment.Left) &&
                                (rightToLeft == true)) ||
                                ((horAlignment == StiTextHorAlignment.Right) &&
                                (rightToLeft == false)))
                            {
                                stHorAlignment = "right";
                            }
                            if (horAlignment == StiTextHorAlignment.Center) stHorAlignment = "center";
                            if (horAlignment == StiTextHorAlignment.Width) stHorAlignment = "both";

                            if (stHorAlignment != string.Empty)
                            {
                                writer.WriteStartElement("w:jc");
                                writer.WriteAttributeString("w:val", stHorAlignment);
                                writer.WriteEndElement();
                            }
                            #endregion

                            writer.WriteEndElement();
                            #endregion

                            //// this code work in 2007 beta, but not in release
                            //if (hypRefText != null)
                            //{
                            //    writer.WriteStartElement("w:hyperlink");
                            //    if (hypRefId == -1)
                            //    {
                            //        writer.WriteAttributeString("w:anchor", hypRefText);
                            //    }
                            //    else
                            //    {
                            //        writer.WriteAttributeString("r:id", string.Format("hId{0}", hypRefId));
                            //    }
                            //}

                            if (bkmRefId != -1)
                            {
                                writer.WriteStartElement("w:bookmarkStart");
                                writer.WriteAttributeString("w:id", bkmRefId.ToString(CultureInfo.InvariantCulture));
                                writer.WriteAttributeString("w:name", bkmRefText);
                                writer.WriteEndElement();
                            }

                            writer.WriteStartElement("w:r");

                            #region Image

                            writer.WriteStartElement("w:rPr");
                            writer.WriteElementString("w:noProof", "");
                            writer.WriteEndElement();

                            #region drawing
                            writer.WriteStartElement("w:drawing");

                            writer.WriteStartElement("wp:inline");
                            writer.WriteAttributeString("distT", "0");
                            writer.WriteAttributeString("distB", "0");
                            writer.WriteAttributeString("distL", "0");
                            writer.WriteAttributeString("distR", "0");

                            int imageHeight = wordCoordY[indexRow - 1 + cell.Height + 1] - wordCoordY[indexRow - 1];
                            int imageWidth = wordCoordX[indexColumn - 1 + cell.Width + 1] - wordCoordX[indexColumn - 1];
                            if (richWidth != -1)
                            {
                                imageWidth = richWidth;
                                imageHeight = richHeight;
                            }

                            #region drawing properties
                            writer.WriteStartElement("wp:extent");
                            writer.WriteAttributeString("cx", string.Format("{0}", ConvertTwipsToEmu(imageWidth)));
                            writer.WriteAttributeString("cy", string.Format("{0}", ConvertTwipsToEmu(imageHeight)));
                            writer.WriteEndElement();

                            writer.WriteStartElement("wp:effectExtent");
                            writer.WriteAttributeString("l", "0");
                            writer.WriteAttributeString("t", "0");
                            writer.WriteAttributeString("r", "0");
                            writer.WriteAttributeString("b", "0");
                            writer.WriteEndElement();

                            writer.WriteStartElement("wp:docPr");
                            writer.WriteAttributeString("id", string.Format("{0}", imageIndex + 2));
                            writer.WriteAttributeString("name", string.Format("Picture {0}", imageIndex + 1));
                            writer.WriteAttributeString("descr", string.Format("Image{0:D5}.{1}", imageIndex + 1, fileExt));
                            if (hypRefText != null)
                            {
                                if (hypRefId == -1)
                                {
                                    hypRefText = "#" + hypRefText;
                                    hypRefId = GetHyperlinkRefId(hypRefText);
                                }
                                writer.WriteStartElement("a:hlinkClick");
                                writer.WriteAttributeString("xmlns:a", "http://schemas.openxmlformats.org/drawingml/2006/main");
                                writer.WriteAttributeString("r:id", string.Format("hId{0}", hypRefId));
                                writer.WriteEndElement();
                            }
                            writer.WriteEndElement();

                            writer.WriteStartElement("wp:cNvGraphicFramePr");
                            writer.WriteStartElement("a:graphicFrameLocks");
                            writer.WriteAttributeString("xmlns:a", "http://schemas.openxmlformats.org/drawingml/2006/main");
                            writer.WriteAttributeString("noChangeAspect", "1");
                            writer.WriteEndElement();
                            writer.WriteEndElement();
                            #endregion

                            writer.WriteStartElement("a:graphic");
                            writer.WriteAttributeString("xmlns:a", "http://schemas.openxmlformats.org/drawingml/2006/main");
                            writer.WriteStartElement("a:graphicData");
                            writer.WriteAttributeString("uri", "http://schemas.openxmlformats.org/drawingml/2006/picture");
                            writer.WriteStartElement("pic:pic");
                            writer.WriteAttributeString("xmlns:pic", "http://schemas.openxmlformats.org/drawingml/2006/picture");

                            writer.WriteStartElement("pic:nvPicPr");
                            writer.WriteStartElement("pic:cNvPr");
                            writer.WriteAttributeString("id", "0");
                            writer.WriteAttributeString("name", string.Format("Image{0:D5}.{1}", imageIndex + 1, fileExt));
                            writer.WriteEndElement();
                            writer.WriteStartElement("pic:cNvPicPr");
                            writer.WriteEndElement();
                            writer.WriteEndElement();

                            writer.WriteStartElement("pic:blipFill");
                            writer.WriteStartElement("a:blip");
                            writer.WriteAttributeString("r:embed", string.Format("rId{0}", 5 + imageIndex));
                            writer.WriteAttributeString("cstate", "print");
                            writer.WriteEndElement();
                            writer.WriteStartElement("a:stretch");
                            writer.WriteStartElement("a:fillRect");
                            writer.WriteEndElement();
                            writer.WriteEndElement();
                            writer.WriteEndElement();

                            writer.WriteStartElement("pic:spPr");
                            writer.WriteStartElement("a:xfrm");
                            writer.WriteStartElement("a:off");
                            writer.WriteAttributeString("x", "0");
                            writer.WriteAttributeString("y", "0");
                            writer.WriteEndElement();
                            writer.WriteStartElement("a:ext");
                            writer.WriteAttributeString("cx", string.Format("{0}", ConvertTwipsToEmu(imageWidth)));
                            writer.WriteAttributeString("cy", string.Format("{0}", ConvertTwipsToEmu(imageHeight)));
                            writer.WriteEndElement();
                            writer.WriteEndElement();
                            writer.WriteStartElement("a:prstGeom");
                            writer.WriteAttributeString("prst", "rect");
                            writer.WriteStartElement("a:avLst");
                            writer.WriteEndElement();
                            writer.WriteEndElement();
                            writer.WriteEndElement();

                            writer.WriteEndElement();	//pic:pic
                            writer.WriteEndElement();	//a:graphicData
                            writer.WriteEndElement();	//a:graphic

                            writer.WriteEndElement();	//wp:inline
                            writer.WriteEndElement();	//w:drawing
                            #endregion

                            #endregion

                            writer.WriteEndElement();	//w:r

                            if (bkmRefId != -1)
                            {
                                writer.WriteStartElement("w:bookmarkEnd");
                                writer.WriteAttributeString("w:id", bkmRefId.ToString(CultureInfo.InvariantCulture));
                                writer.WriteEndElement();
                            }

                            //// this code work in 2007 beta, but not in release
                            //if (hypRefText != null)
                            //{
                            //    writer.WriteEndElement();
                            //}

                            writer.WriteEndElement();	//w:p
                            #endregion
                        }
                    }
                }
                #endregion
            }
            else
            {
                #region Bookmarks
                if (bkmRefId != -1)
                {
                    writer.WriteStartElement("w:bookmarkStart");
                    writer.WriteAttributeString("w:id", bkmRefId.ToString(CultureInfo.InvariantCulture));
                    writer.WriteAttributeString("w:name", bkmRefText);
                    writer.WriteEndElement();

                    writer.WriteStartElement("w:bookmarkEnd");
                    writer.WriteAttributeString("w:id", bkmRefId.ToString(CultureInfo.InvariantCulture));
                    writer.WriteEndElement();
                }
                #endregion
            }
            #endregion
        }

        private byte[] ConvertRichTextToImageInFullTrust(StiRichText rich)
        {
            var ms = new MemoryStream();

            Image bmp = null;
            using (var bmpTemp = new Bitmap(1, 1))
            using (var grfx = Graphics.FromImage(bmpTemp))
            {
                var ipHdc = grfx.GetHdc();
                bmp = new Metafile(ms, ipHdc);
                grfx.ReleaseHdc(ipHdc);
            }

            using (var gr = Graphics.FromImage(bmp))
            {
                gr.DrawImage(rich.Image, 0, 0);
            }
            ms.Flush();

            return ms.ToArray();
        }

        private void writeTableInfo(XmlTextWriter writer, int[] wordCoordX, int maxCoordX)
        {
            #region table properties
            writer.WriteStartElement("w:tblPr");
            writer.WriteStartElement("w:tblStyle");
            writer.WriteAttributeString("w:val", "a1");
            writer.WriteEndElement();
            writer.WriteStartElement("w:tblW");
            writer.WriteAttributeString("w:w", "0");
            writer.WriteAttributeString("w:type", "dxa");
            writer.WriteEndElement();
            writer.WriteStartElement("w:tblLayout");
            writer.WriteAttributeString("w:type", "fixed");
            writer.WriteEndElement();
            writer.WriteStartElement("w:tblCellMar");
            writer.WriteStartElement("w:top");
            writer.WriteAttributeString("w:w", "0");
            writer.WriteAttributeString("w:type", "dxa");
            writer.WriteEndElement();
            writer.WriteStartElement("w:left");
            writer.WriteAttributeString("w:w", "0");
            writer.WriteAttributeString("w:type", "dxa");
            writer.WriteEndElement();
            writer.WriteStartElement("w:bottom");
            writer.WriteAttributeString("w:w", "0");
            writer.WriteAttributeString("w:type", "dxa");
            writer.WriteEndElement();
            writer.WriteStartElement("w:right");
            writer.WriteAttributeString("w:w", "0");
            writer.WriteAttributeString("w:type", "dxa");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("w:tblLook");
            writer.WriteAttributeString("w:val", "04A0");
            writer.WriteEndElement();
            writer.WriteEndElement();   //w:tblPr
            #endregion

            #region columns info
            writer.WriteStartElement("w:tblGrid");
            for (int indexColumn = 1; indexColumn < maxCoordX; indexColumn++)
            {
                double columnWidth = wordCoordX[indexColumn] - wordCoordX[indexColumn - 1];
                writer.WriteStartElement("w:gridCol");
                writer.WriteAttributeString("w:w", string.Format("{0}", columnWidth));
                writer.WriteEndElement();
            }
            writer.WriteFullEndElement();
            #endregion
        }

        private void writeHtmlTags(XmlTextWriter writer, StiCell cell)
        {
            var stiText = cell.Component as StiText;
            var inputText = cell.Text;

            #region Prepare states
            var baseTagsState = new StiTextRenderer.StiHtmlTagsState(
                stiText.Font.Bold,
                stiText.Font.Italic,
                stiText.Font.Underline,
                stiText.Font.Strikeout,
                stiText.Font.SizeInPoints,
                stiText.Font.Name,
                StiBrush.ToColor(stiText.TextBrush),
                StiBrush.ToColor(stiText.Brush),
                false,
                false,
                0,
                0,
                stiText.LineSpacing,
                stiText.HorAlignment);
            var baseState = new StiTextRenderer.StiHtmlState(
                baseTagsState,
                0);
            var statesList = StiTextRenderer.ParseHtmlToStates(inputText, baseState);
            #endregion

            int styleIndex = GetStyleFromComponent(cell.Component);
            bool isRtl = StiBidirectionalConvert.StringContainArabicOrHebrew(inputText);
            bool needRightToLeft = cell.CellStyle.TextOptions != null ? cell.CellStyle.TextOptions.RightToLeft : false;

            writeParagraphBegin(writer, cell, styleIndex, statesList, 0, needRightToLeft);

            for (int index = 0; index < statesList.Count; index++)
            {
                var htmlState = statesList[index];
                var state = htmlState.TS;

                if ((htmlState.Text.ToString() == "\n") && (index < statesList.Count - 1))
                {
                    writer.WriteEndElement();   //w:p
                    writeParagraphBegin(writer, cell, styleIndex, statesList, index + 1, needRightToLeft);
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(state.Href))
                {
                    int hypRefId = GetHyperlinkRefId(StringToUrl(state.Href));
                    writer.WriteStartElement("w:hyperlink");
                    writer.WriteAttributeString("r:id", string.Format("hId{0}", hypRefId));
                }

                writer.WriteStartElement("w:r");

                writer.WriteStartElement("w:rPr");
                if (state.Bold)
                {
                    writer.WriteStartElement("w:b");
                    writer.WriteEndElement();
                }
                if (state.Italic)
                {
                    writer.WriteStartElement("w:i");
                    writer.WriteEndElement();
                }
                if (state.Underline)
                {
                    writer.WriteStartElement("w:u");
                    writer.WriteAttributeString("w:val", "single");
                    writer.WriteEndElement();
                }
                if (state.Strikeout)
                {
                    writer.WriteStartElement("w:strike");
                    writer.WriteEndElement();
                }
                if (state.Superscript)
                {
                    writer.WriteStartElement("w:vertAlign");
                    writer.WriteAttributeString("w:val", "superscript");
                    writer.WriteEndElement();
                }
                if (state.Subscript)
                {
                    writer.WriteStartElement("w:vertAlign");
                    writer.WriteAttributeString("w:val", "subscript");
                    writer.WriteEndElement();
                }
                //if (state.FontColor.ToArgb() != baseTagsState.FontColor.ToArgb())
                //{
                    writer.WriteStartElement("w:color");
                    writer.WriteAttributeString("w:val", GetColorString(state.FontColor));
                    writer.WriteEndElement();
                //}
                if (state.BackColor.ToArgb() != baseTagsState.BackColor.ToArgb())
                {
                    writer.WriteStartElement("w:shd");
                    writer.WriteAttributeString("w:val", "clear");
                    writer.WriteAttributeString("w:color", "auto");
                    writer.WriteAttributeString("w:fill", GetColorString(state.BackColor));
                    writer.WriteEndElement();
                }
                //if (state.FontSize != baseTagsState.FontSize)
                //{
                    writer.WriteStartElement("w:sz");
                    writer.WriteAttributeString("w:val", string.Format("{0}", state.FontSize * 2));
                    writer.WriteEndElement();
                    writer.WriteStartElement("w:szCs");
                    writer.WriteAttributeString("w:val", string.Format("{0}", state.FontSize * 2));
                    writer.WriteEndElement();
                //}
                //if (state.FontName != baseTagsState.FontName)
                //{
                    writer.WriteStartElement("w:rFonts");
                    string fontName = state.FontName;
                    fontList[fontName] = fontName;
                    writer.WriteAttributeString("w:ascii", fontName);
                    writer.WriteAttributeString("w:hAnsi", fontName);
                    writer.WriteAttributeString("w:eastAsia", fontName);
                    writer.WriteAttributeString("w:cs", fontName);
                    writer.WriteEndElement();
                //}
                //if (state.WordSpacing != baseTagsState.WordSpacing)
                //{
                //    fontStyle.Append(string.Format("word-spacing:{0}em;", state.WordSpacing).Replace(",", "."));
                //}
                double letterSpacing = state.LetterSpacing * state.FontSize * 18.8 + StiOptions.Export.Word.SpaceBetweenCharacters;
                if (letterSpacing != 0)
                {
                    writer.WriteStartElement("w:spacing");
                    writer.WriteAttributeString("w:val", string.Format("{0}", letterSpacing));
                    writer.WriteEndElement();
                }
                if (isRtl)
                {
                    writer.WriteStartElement("w:rtl");
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();  //w:rPr

                //remove control symbols
                var st2 = StiTextRenderer.PrepareStateText(htmlState.Text).ToString();
                var sbb2 = new StringBuilder();
                foreach (char ch in st2)
                {
                    if (!char.IsControl(ch) || (ch == '\t'))
                    {
                        sbb2.Append(ch);
                    }
                }

                writer.WriteStartElement("w:t");
                writer.WriteAttributeString("xml:space", "preserve");
                writer.WriteString(sbb2.ToString());
                writer.WriteEndElement();

                writer.WriteEndElement();	//w:r

                if (!string.IsNullOrWhiteSpace(state.Href))
                {
                    writer.WriteEndElement();
                }
            }

            writer.WriteEndElement();	//w:p
        }

        private void writeParagraphBegin(XmlTextWriter writer, StiCell cell, int styleIndex, List<StiTextRenderer.StiHtmlState> states, int stateIndex, bool isRtl)
        {
            writer.WriteStartElement("w:p");

            #region paragraph properties
            writer.WriteStartElement("w:pPr");

            if (styleIndex != -1)
            {
                writer.WriteStartElement("w:pStyle");
                writer.WriteAttributeString("w:val", string.Format("Style{0}", styleIndex));
                writer.WriteEndElement();
            }
            else
            {
                writeRunProperties(writer, cell);
            }

            #region Find last state in current paragraph
            var lastState = states[stateIndex];
            if (lastState.Text.ToString() != "\n")
            {
                int indexS = stateIndex + 1;
                while (indexS < states.Count)
                {
                    if (states[indexS].Text.ToString() == "\n") break;
                    if ((states[indexS].Text.ToString() == " ") && (indexS == states.Count - 1) && (states[indexS].TS.LineHeight != states[indexS - 1].TS.LineHeight)) break;
                    lastState = states[indexS];
                    indexS++;
                }
            }
            #endregion

            #region LineHeight
            double lineSpacing = StiOptions.Export.Word.LineSpacing * lastState.TS.LineHeight;
            if (lastState.TS.LineHeight < 1)
            {
                //correction
                lineSpacing *= 1 - (1 - lastState.TS.LineHeight) * 0.2;
            }
            if (fontsToCorrectHeight.ContainsKey(lastState.TS.FontName))
            {
                lineSpacing *= 0.772;
            }
            if (lineSpacing != 1)
            {
                writer.WriteStartElement("w:spacing");
                writer.WriteAttributeString("w:line", ((int)(lineSpacing * 240)).ToString());
                writer.WriteEndElement();
            }
            #endregion

            #region Horizontal align
            string stHorAlignment = null;
            if (lastState.TS.TextAlign == StiTextHorAlignment.Center) stHorAlignment = "center";
            if (lastState.TS.TextAlign == StiTextHorAlignment.Right && !isRtl || lastState.TS.TextAlign == StiTextHorAlignment.Left && isRtl) stHorAlignment = "right";
            if (lastState.TS.TextAlign == StiTextHorAlignment.Width) stHorAlignment = "both";

            if (stHorAlignment != null)
            {
                writer.WriteStartElement("w:jc");
                writer.WriteAttributeString("w:val", stHorAlignment);
                writer.WriteEndElement();
            }
            #endregion

            writer.WriteEndElement(); //w:pPr
            #endregion
        }


        private void writeRunProperties(XmlTextWriter writer, StiCell cell)
        {
            #region run properties
            writer.WriteStartElement("w:rPr");
            writer.WriteStartElement("w:rFonts");
            string fontName = cell.CellStyle.Font.Name;
            fontList[fontName] = fontName;
            writer.WriteAttributeString("w:ascii", fontName);
            writer.WriteAttributeString("w:hAnsi", fontName);
            writer.WriteAttributeString("w:eastAsia", fontName);
            writer.WriteAttributeString("w:cs", fontName);
            writer.WriteEndElement();
            if (cell.CellStyle.Font.Bold)
            {
                writer.WriteStartElement("w:b");
                writer.WriteEndElement();
            }
            if (cell.CellStyle.Font.Italic)
            {
                writer.WriteStartElement("w:i");
                writer.WriteEndElement();
            }
            if (cell.CellStyle.Font.Underline)
            {
                writer.WriteStartElement("w:u");
                writer.WriteAttributeString("w:val", "single");
                writer.WriteEndElement();
            }
            writer.WriteStartElement("w:color");
            writer.WriteAttributeString("w:val", GetColorString(cell.CellStyle.TextColor));
            writer.WriteEndElement();
            writer.WriteStartElement("w:sz");
            writer.WriteAttributeString("w:val", string.Format("{0}", cell.CellStyle.Font.SizeInPoints * 2));
            writer.WriteEndElement();
            if (StiOptions.Export.Word.SpaceBetweenCharacters != 0)
            {
                writer.WriteStartElement("w:spacing");
                writer.WriteAttributeString("w:val", string.Format("{0}", StiOptions.Export.Word.SpaceBetweenCharacters));
                writer.WriteEndElement();
            }

            if (StiBidirectionalConvert.StringContainArabicOrHebrew(cell.Text))
            {
                if (cell.CellStyle.Font.Bold)
                {
                    writer.WriteStartElement("w:bCs");
                    writer.WriteEndElement();
                }
                if (cell.CellStyle.Font.Italic)
                {
                    writer.WriteStartElement("w:iCs");
                    writer.WriteEndElement();
                }
                writer.WriteStartElement("w:szCs");
                writer.WriteAttributeString("w:val", string.Format("{0}", cell.CellStyle.Font.SizeInPoints * 2));
                writer.WriteEndElement();
                writer.WriteStartElement("w:rtl");
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            #endregion
        }

        private StiBorderSides RenderBorder2TableGetValues(int rowIndex, int columnIndex, ref string styles)
        {
            var sides = new StiBorderSides();
            var sb = new StringBuilder();
            if (matrix.BordersY[rowIndex - 1, columnIndex - 1] != null)
            {
                //needBorderLeft = true;
                //sb.Append("\\clbrdrl" + GetLineStyle(matrix.BordersY[rowIndex - 1, columnIndex - 1], colorList));
                sides |= StiBorderSides.Left;
            }
            if (matrix.BordersY[rowIndex - 1, columnIndex - 1 + 1] != null)
            {
                //needBorderRight = true;
                //sb.Append("\\clbrdrr" + GetLineStyle(matrix.BordersY[rowIndex - 1, columnIndex - 1 + width + 1], colorList));
                sides |= StiBorderSides.Right;
            }
            if (matrix.BordersX[rowIndex - 1, columnIndex - 1] != null)
            {
                //needBorderTop = true;
                sb.Append("\\t" + GetLineStyle2TableGetValues(matrix.BordersX[rowIndex - 1, columnIndex - 1]));
                sides |= StiBorderSides.Top;
            }
            if (matrix.BordersX[rowIndex - 1 + 1, columnIndex - 1] != null)
            {
                //needBorderBottom = true;
                sb.Append("\\b" + GetLineStyle2TableGetValues(matrix.BordersX[rowIndex - 1 + 1, columnIndex - 1]));
                sides |= StiBorderSides.Bottom;
            }
            styles = sb.ToString();
            return sides;
        }
        private string GetLineStyle2TableGetValues(StiBorderSide border)
        {
            var sb = new StringBuilder();
            sb.Append(GetLineStyle(border.Style));
            //sb.Append("\\brdrw");
            sb.Append((int)(border.Size * 15));
            sb.Append(string.Format("c{0}", GetColorString(border.Color)));
            return sb.ToString();
        }
        #endregion

        #region WriteDocument
        private MemoryStream WriteDocument(StiPagesCollection pages, StiReport report, bool hasDividedPages)
        {
            //MemoryStream ms = new MemoryStream();
            var ms = new Tools.StiCachedStream();
            var writer = new XmlTextWriter(ms, Encoding.UTF8);
            writer.Indentation = (xmlIndentation < 0 ? 0 : xmlIndentation);
            writer.Formatting = (xmlIndentation < 0 ? Formatting.None : Formatting.Indented);
            writer.WriteStartDocument();
            writer.WriteStartElement("w:document");
            writer.WriteAttributeString("xmlns:ve", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            writer.WriteAttributeString("xmlns:o", "urn:schemas-microsoft-com:office:office");
            writer.WriteAttributeString("xmlns:r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            writer.WriteAttributeString("xmlns:m", "http://schemas.openxmlformats.org/officeDocument/2006/math");
            writer.WriteAttributeString("xmlns:v", "urn:schemas-microsoft-com:vml");
            writer.WriteAttributeString("xmlns:wp", "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing");
            writer.WriteAttributeString("xmlns:w10", "urn:schemas-microsoft-com:office:word");
            writer.WriteAttributeString("xmlns:w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
            writer.WriteAttributeString("xmlns:wne", "http://schemas.microsoft.com/office/word/2006/wordml");

            //set backColor if it equal for all pages
            var backColor = GetPagesBackColor(pages);
            if (!(backColor.Equals(Color.White) || (backColor.A == 0)))
            {
                writer.WriteStartElement("w:background");
                writer.WriteAttributeString("w:color", GetColorString(backColor));
                writer.WriteEndElement();
            }

            writer.WriteStartElement("w:body");

            var allPages = pages;
            pages = null;

            int indexPage = 0;
            int partIndex = 0;
            while (indexPage < allPages.Count)
            {
                pages = new StiPagesCollection(report, allPages);
                pages.CacheMode = report.RenderedPages.CacheMode;
                pages.AddV2Internal(allPages.GetPageWithoutCache(indexPage));

                while ((indexPage < allPages.Count - 1) && ComparePages(allPages.GetPageWithoutCache(indexPage + 1), allPages.GetPageWithoutCache(indexPage)))
                {
                    indexPage++;
                    pages.AddV2Internal(allPages.GetPageWithoutCache(indexPage));
                }

                var firstPage = pages[0];
                if (firstPage != null)
                {
                    pages.GetPage(firstPage);
                    if (usePageHeadersAndFooters)
                    {
                        #region write header and footer
                        var pages2 = new StiPagesCollection(report, report.RenderedPages);
                        pages2.CacheMode = report.RenderedPages.CacheMode;
                        pages2.AddV2Internal(pages[0]);
                        matrix = new StiMatrix(pages2, StiOptions.Export.Word.DivideBigCells, this);
                        if (IsStopped) return null;
                        matrix.ScanComponentsPlacement(false);

                        int startLine = 0;
                        int endLine = 0;

                        bool needHeader = false;
                        endLine = (matrix.CoordY.Count - 1) - 1;
                        while ((endLine > 0) && (matrix.LinePlacement[endLine] != StiMatrix.StiTableLineInfo.PageHeader)) endLine--;
                        if (endLine >= 0 && matrix.LinePlacement[endLine] == StiMatrix.StiTableLineInfo.PageHeader)
                        {
                            startLine = 0;
                            while (matrix.LinePlacement[startLine] != StiMatrix.StiTableLineInfo.PageHeader) startLine++;
                            needHeader = true;
                        }
                        headersData.Add(WriteHeader(startLine, endLine, needHeader));
                        if (imageCache.ImagePackedStore.Count > 0)
                        {
                            headersRels.Add(WriteHeaderFooterRels());
                        }
                        else headersRels.Add(null);

                        bool needFooter = false;
                        startLine = 0;
                        while ((matrix.LinePlacement[startLine] != StiMatrix.StiTableLineInfo.PageFooter) && (startLine < (matrix.CoordY.Count - 1) - 1)) startLine++;
                        if (matrix.LinePlacement[startLine] == StiMatrix.StiTableLineInfo.PageFooter)
                        {
                            endLine = (matrix.CoordY.Count - 1) - 1;
                            while (matrix.LinePlacement[endLine] != StiMatrix.StiTableLineInfo.PageFooter) endLine--;
                            needFooter = true;
                        }
                        footersData.Add(WriteFooter(startLine, endLine, needFooter));
                        if (imageCache.ImagePackedStore.Count > 0)
                        {
                            footersRels.Add(WriteHeaderFooterRels());
                        }
                        else footersRels.Add(null);
                        #endregion

                        matrix = new StiMatrix(pages, StiOptions.Export.Word.DivideBigCells, this);
                        matrix.ScanComponentsPlacement(true);
                        endLine = (matrix.CoordY.Count - 1) - 1;
                        WriteFromMatrix(writer, 0, endLine, false);
                    }
                    else
                    {
                        matrix = new StiMatrix(pages, StiOptions.Export.Word.DivideBigCells, this, null, StiDataExportMode.AllBands, hasDividedPages);
                        if (IsStopped) return null;
                        WriteFromMatrix(writer, 0, (matrix.CoordY.Count - 1) - 1, true);
                    }

                    if (indexPage < allPages.Count - 1)
                    {
                        writer.WriteStartElement("w:p");
                        writer.WriteStartElement("w:pPr");
                        //<w:widowControl /> 
                        //<w:autoSpaceDE /> 
                        //<w:autoSpaceDN /> 
                        //<w:adjustRightInd /> 
                        WritePageInfo(writer, pages[0], partIndex);
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                    }
                }
                indexPage++;
                partIndex++;
            }

            if (pages.Count > 0)
            {
                WritePageInfo(writer, pages[0], partIndex - 1);
            }

            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return ms;
        }

        //Return backColor if it equal for all pages
        private Color GetPagesBackColor(StiPagesCollection pages)
        {
            var backColor = Color.Transparent;
            if (pages.Count > 0)
            {
                var page1 = pages.GetPageWithoutCache(0);
                backColor = StiBrush.ToColor(page1.Brush);
                for (int index2 = 1; index2 < pages.Count; index2++)
                {
                    var pageN = pages.GetPageWithoutCache(index2);
                    var colorN = StiBrush.ToColor(pageN.Brush);
                    if (!backColor.Equals(colorN))
                    {
                        backColor = Color.Transparent;
                        break;
                    }
                }
            }
            return backColor;
        }

        private bool ComparePages(StiPage page1, StiPage page2)
        {
            //ExcellSheetNames
            string st1 = page1.ExcelSheetValue;
            if (string.IsNullOrEmpty(st1)) st1 = string.Empty;
            string st2 = page2.ExcelSheetValue;
            if (string.IsNullOrEmpty(st2)) st2 = string.Empty;
            if (st1 != st2) return false;

            if (page1.Orientation != page2.Orientation) return false;
            if (!page1.Margins.Equals(page2.Margins)) return false;
            if (page1.UnlimitedHeight != page2.UnlimitedHeight) return false;

            return true;
        }

        private void WritePageInfo(XmlTextWriter writer, StiPage page, int partIndex)
        {
            int pageHeight = ConvertHiToTwips(Math.Ceiling(page.Unit.ConvertToHInches(page.PageHeight * (usePageHeadersAndFooters ? 1 : page.SegmentPerHeight))));
            int pageWidth = ConvertHiToTwips(page.Unit.ConvertToHInches(page.PageWidth * (usePageHeadersAndFooters ? 1 : page.SegmentPerWidth)));
            int mgLeft = ConvertHiToTwips(page.Unit.ConvertToHInches(page.Margins.Left));
            int mgRight = ConvertHiToTwips(page.Unit.ConvertToHInches(page.Margins.Right));
            int mgTop = ConvertHiToTwips(page.Unit.ConvertToHInches(page.Margins.Top));
            int mgBottom = ConvertHiToTwips(page.Unit.ConvertToHInches(page.Margins.Bottom)) - StiOptions.Export.Word.BottomMarginCorrection;
            if (mgBottom < 0) mgBottom = 0;
            if (pageWidth > 31500) pageWidth = 31500;
            if (pageHeight > 31500) pageHeight = 31500;

            writer.WriteStartElement("w:sectPr");

            if (usePageHeadersAndFooters)
            {
                writer.WriteStartElement("w:headerReference");
                writer.WriteAttributeString("w:type", "default");
                writer.WriteAttributeString("r:id", string.Format("rIdh{0}", partIndex + 1));
                writer.WriteEndElement();
                writer.WriteStartElement("w:footerReference");
                writer.WriteAttributeString("w:type", "default");
                writer.WriteAttributeString("r:id", string.Format("rIdf{0}", partIndex + 1));
                writer.WriteEndElement();
            }

            #region Demo
            else
            {
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
                    writer.WriteStartElement("w:headerReference");
                    writer.WriteAttributeString("w:type", "default");
                    writer.WriteAttributeString("r:id", "rIdh1");
                    writer.WriteEndElement();
                }
            }
            #endregion

            writer.WriteStartElement("w:pgSz");
            writer.WriteAttributeString("w:w", string.Format("{0}", pageWidth));
            writer.WriteAttributeString("w:h", string.Format("{0}", pageHeight));
            if (page.Orientation == StiPageOrientation.Landscape)
            {
                writer.WriteAttributeString("w:orient", "landscape");
            }
            writer.WriteEndElement();
            writer.WriteStartElement("w:pgMar");
            writer.WriteAttributeString("w:top", string.Format("{0}", mgTop));
            writer.WriteAttributeString("w:right", string.Format("{0}", mgRight));
            writer.WriteAttributeString("w:bottom", string.Format("{0}", mgBottom));
            writer.WriteAttributeString("w:left", string.Format("{0}", mgLeft));
            writer.WriteAttributeString("w:header", string.Format("{0}", mgTop));
            writer.WriteAttributeString("w:footer", string.Format("{0}", mgBottom));
            writer.WriteAttributeString("w:gutter", "0");
            writer.WriteEndElement();
            //			writer.WriteStartElement("w:cols");
            //			writer.WriteAttributeString("w:space", "708");
            //			writer.WriteEndElement();
            //			writer.WriteStartElement("w:docGrid");
            //			writer.WriteAttributeString("w:linePitch", "360");
            //			writer.WriteEndElement();
            writer.WriteEndElement();
        }

        private void WriteBorders(XmlTextWriter writer, int indexRow, int indexColumn, int height, int width, bool checkHeaderAP = false)
        {
            bool needBorderLeft = true;
            bool needBorderRight = true;
            for (int index = 0; index < height; index++)
            {
                if (matrix.BordersY[indexRow + index, indexColumn] == null) needBorderLeft = false;
                if (matrix.BordersY[indexRow + index, indexColumn + width] == null) needBorderRight = false;
            }
            bool needBorderTop = true;
            bool needBorderBottom = true;
            for (int index = 0; index < width; index++)
            {
                if (matrix.BordersX[indexRow, indexColumn + index] == null) needBorderTop = false;
                if (matrix.BordersX[indexRow + height, indexColumn + index] == null) needBorderBottom = false;
            }

            if (checkHeaderAP)
            {
                if ((indexRow + 1 < Matrix.CoordY.Count) && (matrix.LinePlacement[indexRow + 1] == StiMatrix.StiTableLineInfo.HeaderAP))
                {
                    needBorderBottom = false;
                }
            }

            if (needBorderTop || needBorderLeft || needBorderBottom || needBorderRight)
            {
                writer.WriteStartElement("w:tcBorders");
                if (needBorderTop) WriteBorderData(writer, "w:top", Matrix.BordersX[indexRow, indexColumn]);
                if (needBorderLeft) WriteBorderData(writer, "w:left", Matrix.BordersY[indexRow, indexColumn]);
                if (needBorderBottom) WriteBorderData(writer, "w:bottom", Matrix.BordersX[indexRow + height, indexColumn]);
                if (needBorderRight) WriteBorderData(writer, "w:right", Matrix.BordersY[indexRow, indexColumn + width]);
                writer.WriteEndElement();
            }
        }

        private void WriteBorderData(XmlTextWriter writer, string side, StiBorderSide border)
        {
            if (border != null)
            {
                string style = GetLineStyle(border.Style);
                if (style != string.Empty)
                {
                    writer.WriteStartElement(side);
                    writer.WriteAttributeString("w:val", style);
                    writer.WriteAttributeString("w:sz", string.Format("{0}", (int)(border.Size * 5)));
                    writer.WriteAttributeString("w:space", "0");
                    writer.WriteAttributeString("w:color", GetColorString(border.Color));
                    writer.WriteEndElement();
                }
            }
        }
        #endregion

        #region WriteFootNotes
        private MemoryStream WriteFootNotes()
        {
            var ms = new MemoryStream();
            var writer = new XmlTextWriter(ms, Encoding.UTF8);
            writer.Indentation = (xmlIndentation < 0 ? 0 : xmlIndentation);
            writer.Formatting = (xmlIndentation < 0 ? Formatting.None : Formatting.Indented);
            writer.WriteStartDocument();
            writer.WriteStartElement("w:footnotes");
            writer.WriteAttributeString("xmlns:ve", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            writer.WriteAttributeString("xmlns:o", "urn:schemas-microsoft-com:office:office");
            writer.WriteAttributeString("xmlns:r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            writer.WriteAttributeString("xmlns:m", "http://schemas.openxmlformats.org/officeDocument/2006/math");
            writer.WriteAttributeString("xmlns:v", "urn:schemas-microsoft-com:vml");
            writer.WriteAttributeString("xmlns:wp", "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing");
            writer.WriteAttributeString("xmlns:w10", "urn:schemas-microsoft-com:office:word");
            writer.WriteAttributeString("xmlns:w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
            writer.WriteAttributeString("xmlns:wne", "http://schemas.microsoft.com/office/word/2006/wordml");

            writer.WriteStartElement("w:footnote");
            writer.WriteAttributeString("w:type", "separator");
            writer.WriteAttributeString("w:id", "0");
            writer.WriteStartElement("w:p");
            writer.WriteStartElement("w:r");
            writer.WriteStartElement("w:separator");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteFullEndElement();

            writer.WriteStartElement("w:footnote");
            writer.WriteAttributeString("w:type", "continuationSeparator");
            writer.WriteAttributeString("w:id", "1");
            writer.WriteStartElement("w:p");
            writer.WriteStartElement("w:r");
            writer.WriteStartElement("w:continuationSeparator");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteFullEndElement();

            writer.WriteFullEndElement();   //footnotes
            writer.WriteEndDocument();
            writer.Flush();
            return ms;
        }
        #endregion

        #region WriteEndNotes
        private MemoryStream WriteEndNotes()
        {
            var ms = new MemoryStream();
            var writer = new XmlTextWriter(ms, Encoding.UTF8);
            writer.Indentation = (xmlIndentation < 0 ? 0 : xmlIndentation);
            writer.Formatting = (xmlIndentation < 0 ? Formatting.None : Formatting.Indented);
            writer.WriteStartDocument();
            writer.WriteStartElement("w:endnotes");
            writer.WriteAttributeString("xmlns:ve", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            writer.WriteAttributeString("xmlns:o", "urn:schemas-microsoft-com:office:office");
            writer.WriteAttributeString("xmlns:r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            writer.WriteAttributeString("xmlns:m", "http://schemas.openxmlformats.org/officeDocument/2006/math");
            writer.WriteAttributeString("xmlns:v", "urn:schemas-microsoft-com:vml");
            writer.WriteAttributeString("xmlns:wp", "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing");
            writer.WriteAttributeString("xmlns:w10", "urn:schemas-microsoft-com:office:word");
            writer.WriteAttributeString("xmlns:w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
            writer.WriteAttributeString("xmlns:wne", "http://schemas.microsoft.com/office/word/2006/wordml");

            writer.WriteStartElement("w:endnote");
            writer.WriteAttributeString("w:type", "separator");
            writer.WriteAttributeString("w:id", "0");
            writer.WriteStartElement("w:p");
            writer.WriteStartElement("w:r");
            writer.WriteStartElement("w:separator");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteFullEndElement();

            writer.WriteStartElement("w:endnote");
            writer.WriteAttributeString("w:type", "continuationSeparator");
            writer.WriteAttributeString("w:id", "1");
            writer.WriteStartElement("w:p");
            writer.WriteStartElement("w:r");
            writer.WriteStartElement("w:continuationSeparator");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteFullEndElement();

            writer.WriteFullEndElement();   //endnotes
            writer.WriteEndDocument();
            writer.Flush();
            return ms;
        }
        #endregion

        #region WriteHeader
        private MemoryStream WriteHeader(int startLine, int endLine, bool needHeader)
        {
            var ms = new MemoryStream();
            var writer = new XmlTextWriter(ms, Encoding.UTF8);
            writer.Indentation = (xmlIndentation < 0 ? 0 : xmlIndentation);
            writer.Formatting = (xmlIndentation < 0 ? Formatting.None : Formatting.Indented);
            writer.WriteStartDocument();
            writer.WriteStartElement("w:hdr");
            writer.WriteAttributeString("xmlns:ve", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            writer.WriteAttributeString("xmlns:o", "urn:schemas-microsoft-com:office:office");
            writer.WriteAttributeString("xmlns:r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            writer.WriteAttributeString("xmlns:m", "http://schemas.openxmlformats.org/officeDocument/2006/math");
            writer.WriteAttributeString("xmlns:v", "urn:schemas-microsoft-com:vml");
            writer.WriteAttributeString("xmlns:wp", "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing");
            writer.WriteAttributeString("xmlns:w10", "urn:schemas-microsoft-com:office:word");
            writer.WriteAttributeString("xmlns:w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
            writer.WriteAttributeString("xmlns:wne", "http://schemas.microsoft.com/office/word/2006/wordml");

            if (needHeader)
            {
                WriteFromMatrix(writer, startLine, endLine, true);
            }
            else
            {
                writer.WriteStartElement("w:p");
                writer.WriteStartElement("w:r");
                writer.WriteStartElement("w:cr");
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndElement();
            }

            writer.WriteFullEndElement();   //hdr
            writer.WriteEndDocument();
            writer.Flush();
            return ms;
        }
        #endregion

        #region WriteFooter
        private MemoryStream WriteFooter(int startLine, int endLine, bool needFooter)
        {
            var ms = new MemoryStream();
            var writer = new XmlTextWriter(ms, Encoding.UTF8);
            writer.Indentation = (xmlIndentation < 0 ? 0 : xmlIndentation);
            writer.Formatting = (xmlIndentation < 0 ? Formatting.None : Formatting.Indented);
            writer.WriteStartDocument();
            writer.WriteStartElement("w:ftr");
            writer.WriteAttributeString("xmlns:ve", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            writer.WriteAttributeString("xmlns:o", "urn:schemas-microsoft-com:office:office");
            writer.WriteAttributeString("xmlns:r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            writer.WriteAttributeString("xmlns:m", "http://schemas.openxmlformats.org/officeDocument/2006/math");
            writer.WriteAttributeString("xmlns:v", "urn:schemas-microsoft-com:vml");
            writer.WriteAttributeString("xmlns:wp", "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing");
            writer.WriteAttributeString("xmlns:w10", "urn:schemas-microsoft-com:office:word");
            writer.WriteAttributeString("xmlns:w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
            writer.WriteAttributeString("xmlns:wne", "http://schemas.microsoft.com/office/word/2006/wordml");

            if (needFooter)
            {
                WriteFromMatrix(writer, startLine, endLine, true);
            }
            else
            {
                writer.WriteStartElement("w:p");
                writer.WriteStartElement("w:r");
                writer.WriteStartElement("w:cr");
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndElement();
            }

            writer.WriteFullEndElement();   //ftr
            writer.WriteEndDocument();
            writer.Flush();
            return ms;
        }
        #endregion

        #region WriteContentTypes
        private MemoryStream WriteContentTypes()
        {
            var ms = new MemoryStream();
            var writer = new XmlTextWriter(ms, Encoding.UTF8);
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
            writer.WriteAttributeString("Extension", "emf");
            writer.WriteAttributeString("ContentType", "image/x-emf");
            writer.WriteEndElement();
            writer.WriteStartElement("Default");
            writer.WriteAttributeString("Extension", "rtf");
            writer.WriteAttributeString("ContentType", "application/rtf");
            writer.WriteEndElement();
            writer.WriteStartElement("Default");
            writer.WriteAttributeString("Extension", "png");
            writer.WriteAttributeString("ContentType", "image/png");
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
            writer.WriteAttributeString("PartName", "/word/document.xml");
            writer.WriteAttributeString("ContentType", "application/vnd.openxmlformats-officedocument.wordprocessingml.document.main+xml");
            writer.WriteEndElement();
            writer.WriteStartElement("Override");
            writer.WriteAttributeString("PartName", "/word/styles.xml");
            writer.WriteAttributeString("ContentType", "application/vnd.openxmlformats-officedocument.wordprocessingml.styles+xml");
            writer.WriteEndElement();
            writer.WriteStartElement("Override");
            writer.WriteAttributeString("PartName", "/word/settings.xml");
            writer.WriteAttributeString("ContentType", "application/vnd.openxmlformats-officedocument.wordprocessingml.settings+xml");
            writer.WriteEndElement();
            writer.WriteStartElement("Override");
            writer.WriteAttributeString("PartName", "/word/webSettings.xml");
            writer.WriteAttributeString("ContentType", "application/vnd.openxmlformats-officedocument.wordprocessingml.webSettings+xml");
            writer.WriteEndElement();
            writer.WriteStartElement("Override");
            writer.WriteAttributeString("PartName", "/word/fontTable.xml");
            writer.WriteAttributeString("ContentType", "application/vnd.openxmlformats-officedocument.wordprocessingml.fontTable+xml");
            writer.WriteEndElement();

            if (usePageHeadersAndFooters)
            {
                writer.WriteStartElement("Override");
                writer.WriteAttributeString("PartName", "/word/footnotes.xml");
                writer.WriteAttributeString("ContentType", "application/vnd.openxmlformats-officedocument.wordprocessingml.footnotes+xml");
                writer.WriteEndElement();
                writer.WriteStartElement("Override");
                writer.WriteAttributeString("PartName", "/word/endnotes.xml");
                writer.WriteAttributeString("ContentType", "application/vnd.openxmlformats-officedocument.wordprocessingml.endnotes+xml");
                writer.WriteEndElement();
                for (int index = 0; index < headersData.Count; index++)
                {
                    writer.WriteStartElement("Override");
                    writer.WriteAttributeString("PartName", string.Format("/word/footer{0}.xml", index + 1));
                    writer.WriteAttributeString("ContentType", "application/vnd.openxmlformats-officedocument.wordprocessingml.footer+xml");
                    writer.WriteEndElement();
                    writer.WriteStartElement("Override");
                    writer.WriteAttributeString("PartName", string.Format("/word/header{0}.xml", index + 1));
                    writer.WriteAttributeString("ContentType", "application/vnd.openxmlformats-officedocument.wordprocessingml.header+xml");
                    writer.WriteEndElement();
                }
            }

            #region Trial
            else
            {
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
                    writer.WriteStartElement("Override");
                    writer.WriteAttributeString("PartName", "/word/footnotes.xml");
                    writer.WriteAttributeString("ContentType", "application/vnd.openxmlformats-officedocument.wordprocessingml.footnotes+xml");
                    writer.WriteEndElement();
                    writer.WriteStartElement("Override");
                    writer.WriteAttributeString("PartName", "/word/endnotes.xml");
                    writer.WriteAttributeString("ContentType", "application/vnd.openxmlformats-officedocument.wordprocessingml.endnotes+xml");
                    writer.WriteEndElement();
                    writer.WriteStartElement("Override");
                    writer.WriteAttributeString("PartName", "/word/headerAdditional.xml");
                    writer.WriteAttributeString("ContentType", "application/vnd.openxmlformats-officedocument.wordprocessingml.header+xml");
                    writer.WriteEndElement();
                }
            }
            #endregion

            writer.WriteFullEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return ms;
        }
        #endregion

        #region WriteMainRels
        private MemoryStream WriteMainRels()
        {
            var ms = new MemoryStream();
            var writer = new XmlTextWriter(ms, Encoding.UTF8);
            writer.Indentation = (xmlIndentation < 0 ? 0 : xmlIndentation);
            writer.Formatting = (xmlIndentation < 0 ? Formatting.None : Formatting.Indented);
            writer.WriteStartDocument();
            writer.WriteStartElement("Relationships");
            writer.WriteAttributeString("xmlns", "http://schemas.openxmlformats.org/package/2006/relationships");

            writer.WriteStartElement("Relationship");
            writer.WriteAttributeString("Id", "rId1");
            writer.WriteAttributeString("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument");
            writer.WriteAttributeString("Target", "word/document.xml");
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
            var ms = new MemoryStream();
            var writer = new XmlTextWriter(ms, Encoding.UTF8);
            writer.Indentation = (xmlIndentation < 0 ? 0 : xmlIndentation);
            writer.Formatting = (xmlIndentation < 0 ? Formatting.None : Formatting.Indented);
            writer.WriteStartDocument();
            writer.WriteStartElement("Properties");
            writer.WriteAttributeString("xmlns", "http://schemas.openxmlformats.org/officeDocument/2006/extended-properties");
            writer.WriteAttributeString("xmlns:vt", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes");

            writer.WriteElementString("Application", "Microsoft Office Word");
            writer.WriteElementString("Company", docCompanyString == null ? "" : docCompanyString);
            writer.WriteElementString("Template", "Normal.dotm");
            writer.WriteElementString("TotalTime", "0");
            writer.WriteElementString("Pages", "1");
            writer.WriteElementString("Words", "1");
            writer.WriteElementString("Characters", "1");
            writer.WriteElementString("DocSecurity", restrictEditing == StiWord2007RestrictEditing.No ? "0" : "8");
            writer.WriteElementString("Lines", "1");
            writer.WriteElementString("Paragraphs", "1");
            writer.WriteElementString("ScaleCrop", "false");
            writer.WriteElementString("LinksUpToDate", "false");
            writer.WriteElementString("CharactersWithSpaces", "1");
            writer.WriteElementString("SharedDoc", "false");
            writer.WriteElementString("HyperlinksChanged", "false");
            writer.WriteElementString("AppVersion", "12.0000");

            writer.WriteFullEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return ms;
        }
        #endregion

        #region WriteDocPropsCore
        private MemoryStream WriteDocPropsCore()
        {
            var ms = new MemoryStream();
            var writer = new XmlTextWriter(ms, Encoding.UTF8);
            writer.Indentation = (xmlIndentation < 0 ? 0 : xmlIndentation);
            writer.Formatting = (xmlIndentation < 0 ? Formatting.None : Formatting.Indented);
            writer.WriteStartDocument();
            writer.WriteStartElement("cp:coreProperties");
            writer.WriteAttributeString("xmlns:cp", "http://schemas.openxmlformats.org/package/2006/metadata/core-properties");
            writer.WriteAttributeString("xmlns:dc", "http://purl.org/dc/elements/1.1/");
            writer.WriteAttributeString("xmlns:dcterms", "http://purl.org/dc/terms/");
            writer.WriteAttributeString("xmlns:dcmitype", "http://purl.org/dc/dcmitype/");
            writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");

            var dateTime = string.Format("{0}", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            //string creator = Stimulsoft.Report.Export.StiExportUtils.GetReportVersion();

            writer.WriteElementString("dc:title", report.ReportName == null ? "" : report.ReportName);
            writer.WriteElementString("dc:subject", report.ReportAlias == null ? "" : report.ReportAlias);
            writer.WriteElementString("dc:creator", report.ReportAuthor == null ? "" : report.ReportAuthor);
            writer.WriteElementString("cp:keywords", "");
            writer.WriteElementString("dc:description", report.ReportDescription == null ? "" : report.ReportDescription);
            writer.WriteElementString("cp:lastModifiedBy", docLastModifiedString == null ? "" : docLastModifiedString);
            writer.WriteElementString("cp:revision", "1");
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

        #region WriteSettings
        private MemoryStream WriteSettings(StiPagesCollection pages)
        {
            var ms = new MemoryStream();
            var writer = new XmlTextWriter(ms, Encoding.UTF8);
            writer.Indentation = (xmlIndentation < 0 ? 0 : xmlIndentation);
            writer.Formatting = (xmlIndentation < 0 ? Formatting.None : Formatting.Indented);
            writer.WriteStartDocument();
            writer.WriteStartElement("w:settings");
            writer.WriteAttributeString("xmlns:o", "urn:schemas-microsoft-com:office:office");
            writer.WriteAttributeString("xmlns:r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            writer.WriteAttributeString("xmlns:m", "http://schemas.openxmlformats.org/officeDocument/2006/math");
            writer.WriteAttributeString("xmlns:v", "urn:schemas-microsoft-com:vml");
            writer.WriteAttributeString("xmlns:w10", "urn:schemas-microsoft-com:office:word");
            writer.WriteAttributeString("xmlns:w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
            writer.WriteAttributeString("xmlns:sl", "http://schemas.openxmlformats.org/schemaLibrary/2006/main");

            if (restrictEditing != StiWord2007RestrictEditing.No)
            {
                writer.WriteStartElement("w:documentProtection");
                writer.WriteAttributeString("w:edit", "readOnly");
                writer.WriteAttributeString("w:enforcement", "1");
                writer.WriteAttributeString("w:cryptProviderType", "rsaFull");
                writer.WriteAttributeString("w:cryptAlgorithmClass", "hash");
                writer.WriteAttributeString("w:cryptAlgorithmType", "typeAny");
                writer.WriteAttributeString("w:cryptAlgorithmSid", "4");
                writer.WriteAttributeString("w:cryptSpinCount", "50000");
                writer.WriteAttributeString("w:hash", "aw5VYrxGrQVOl7/SJDI9GvbcwaE=");  //*TestPassword*
                writer.WriteAttributeString("w:salt", "974T1u8C/8p0OAB+3ev3nQ==");
                writer.WriteEndElement();
            }

            writer.WriteStartElement("w:zoom");
            writer.WriteAttributeString("w:percent", "100");
            writer.WriteEndElement();

            var backColor = GetPagesBackColor(pages);
            if (!(backColor.Equals(Color.White) || (backColor.A == 0)))
            {
                writer.WriteStartElement("w:displayBackgroundShape");
                writer.WriteEndElement();
            }

            writer.WriteStartElement("w:proofState");
            writer.WriteAttributeString("w:spelling", "clean");
            writer.WriteAttributeString("w:grammar", "clean");
            writer.WriteEndElement();
            writer.WriteStartElement("w:defaultTabStop");
            writer.WriteAttributeString("w:val", "708");
            writer.WriteEndElement();
            writer.WriteStartElement("w:characterSpacingControl");
            writer.WriteAttributeString("w:val", "doNotCompress");
            writer.WriteEndElement();

            if (usePageHeadersAndFooters)
            {
                writer.WriteStartElement("w:footnotePr");
                writer.WriteStartElement("w:footnote");
                writer.WriteAttributeString("w:id", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("w:footnote");
                writer.WriteAttributeString("w:id", "1");
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteStartElement("w:endnotePr");
                writer.WriteStartElement("w:endnote");
                writer.WriteAttributeString("w:id", "0");
                writer.WriteEndElement();
                writer.WriteStartElement("w:endnote");
                writer.WriteAttributeString("w:id", "1");
                writer.WriteEndElement();
                writer.WriteEndElement();

            }

            writer.WriteStartElement("w:compat");
            if (StiOptions.Export.Word.CompatibilityModeValue > 12)
            {
                writer.WriteStartElement("w:compatSetting");
                writer.WriteAttributeString("w:name", "compatibilityMode");
                writer.WriteAttributeString("w:uri", "http://schemas.microsoft.com/office/word");
                writer.WriteAttributeString("w:val", StiOptions.Export.Word.CompatibilityModeValue.ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteStartElement("m:mathPr");
            writer.WriteStartElement("m:mathFont");
            writer.WriteAttributeString("m:val", "Cambria Math");
            writer.WriteEndElement();
            writer.WriteStartElement("m:brkBin");
            writer.WriteAttributeString("m:val", "before");
            writer.WriteEndElement();
            writer.WriteStartElement("m:brkBinSub");
            writer.WriteAttributeString("m:val", "--");
            writer.WriteEndElement();
            writer.WriteStartElement("m:smallFrac");
            writer.WriteAttributeString("m:val", "off");
            writer.WriteEndElement();
            writer.WriteStartElement("m:dispDef");
            writer.WriteEndElement();
            writer.WriteStartElement("m:lMargin");
            writer.WriteAttributeString("m:val", "0");
            writer.WriteEndElement();
            writer.WriteStartElement("m:rMargin");
            writer.WriteAttributeString("m:val", "0");
            writer.WriteEndElement();
            writer.WriteStartElement("m:defJc");
            writer.WriteAttributeString("m:val", "centerGroup");
            writer.WriteEndElement();
            writer.WriteStartElement("m:wrapIndent");
            writer.WriteAttributeString("m:val", "1440");
            writer.WriteEndElement();
            writer.WriteStartElement("m:intLim");
            writer.WriteAttributeString("m:val", "subSup");
            writer.WriteEndElement();
            writer.WriteStartElement("m:naryLim");
            writer.WriteAttributeString("m:val", "undOvr");
            writer.WriteEndElement();
            writer.WriteEndElement();

            writer.WriteStartElement("w:themeFontLang");
            writer.WriteAttributeString("w:val", "en-EN");
            writer.WriteEndElement();

            writer.WriteStartElement("w:clrSchemeMapping");
            writer.WriteAttributeString("w:bg1", "light1");
            writer.WriteAttributeString("w:t1", "dark1");
            writer.WriteAttributeString("w:bg2", "light2");
            writer.WriteAttributeString("w:t2", "dark2");
            writer.WriteAttributeString("w:accent1", "accent1");
            writer.WriteAttributeString("w:accent2", "accent2");
            writer.WriteAttributeString("w:accent3", "accent3");
            writer.WriteAttributeString("w:accent4", "accent4");
            writer.WriteAttributeString("w:accent5", "accent5");
            writer.WriteAttributeString("w:accent6", "accent6");
            writer.WriteAttributeString("w:hyperlink", "hyperlink");
            writer.WriteAttributeString("w:followedHyperlink", "followedHyperlink");
            writer.WriteEndElement();

            writer.WriteStartElement("w:shapeDefaults");
            writer.WriteStartElement("o:shapedefaults");
            writer.WriteAttributeString("v:ext", "edit");
            writer.WriteAttributeString("spidmax", "2050");
            writer.WriteEndElement();
            writer.WriteStartElement("o:shapelayout");
            writer.WriteAttributeString("v:ext", "edit");
            writer.WriteStartElement("o:idmap");
            writer.WriteAttributeString("v:ext", "edit");
            writer.WriteAttributeString("data", "1");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();

            writer.WriteStartElement("w:decimalSymbol");
            writer.WriteAttributeString("w:val", ",");
            writer.WriteEndElement();
            writer.WriteStartElement("w:listSeparator");
            writer.WriteAttributeString("w:val", ";");
            writer.WriteEndElement();

            writer.WriteFullEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return ms;
        }
        #endregion

        #region WriteWebSettings
        private MemoryStream WriteWebSettings()
        {
            var ms = new MemoryStream();
            var writer = new XmlTextWriter(ms, Encoding.UTF8);
            writer.Indentation = (xmlIndentation < 0 ? 0 : xmlIndentation);
            writer.Formatting = (xmlIndentation < 0 ? Formatting.None : Formatting.Indented);
            writer.WriteStartDocument();
            writer.WriteStartElement("w:webSettings");
            writer.WriteAttributeString("xmlns:r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            writer.WriteAttributeString("xmlns:w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");

            writer.WriteStartElement("w:optimizeForBrowser");
            writer.WriteEndElement();

            writer.WriteFullEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return ms;
        }
        #endregion

        #region WriteFontTable
        private MemoryStream WriteFontTable()
        {
            var ms = new MemoryStream();
            var writer = new XmlTextWriter(ms, Encoding.UTF8);
            writer.Indentation = (xmlIndentation < 0 ? 0 : xmlIndentation);
            writer.Formatting = (xmlIndentation < 0 ? Formatting.None : Formatting.Indented);
            writer.WriteStartDocument();
            writer.WriteStartElement("w:fonts");
            writer.WriteAttributeString("xmlns:r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            writer.WriteAttributeString("xmlns:w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");

            fontList["Calibri"] = "Calibri";
            fontList["Cambria"] = "Cambria";

            foreach (string fontName in fontList.Values)
            {
                writer.WriteStartElement("w:font");
                writer.WriteAttributeString("w:name", fontName);
                //writer.WriteStartElement("w:panose1");
                //writer.WriteAttributeString("w:val", "02040503050406030204");
                //writer.WriteEndElement();
                //writer.WriteStartElement("w:charset");
                //writer.WriteAttributeString("w:val", "CC");
                //writer.WriteEndElement();
                //writer.WriteStartElement("w:family");
                //writer.WriteAttributeString("w:val", "roman");
                //writer.WriteEndElement();
                //writer.WriteStartElement("w:pitch");
                //writer.WriteAttributeString("w:val", "variable");
                //writer.WriteEndElement();
                //writer.WriteStartElement("w:sig");
                //writer.WriteAttributeString("w:usb0", "A00002EF");
                //writer.WriteAttributeString("w:usb1", "4000007B");
                //writer.WriteAttributeString("w:usb2", "00000000");
                //writer.WriteAttributeString("w:usb3", "00000000");
                //writer.WriteAttributeString("w:csb0", "0000009F");
                //writer.WriteAttributeString("w:csb1", "00000000");
                //writer.WriteEndElement();
                writer.WriteFullEndElement();
            }

            writer.WriteFullEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return ms;
        }
        #endregion

        #region WriteDocumentRels
        private MemoryStream WriteDocumentRels()
        {
            var ms = new MemoryStream();
            var writer = new XmlTextWriter(ms, Encoding.UTF8);
            writer.Indentation = (xmlIndentation < 0 ? 0 : xmlIndentation);
            writer.Formatting = (xmlIndentation < 0 ? Formatting.None : Formatting.Indented);
            writer.WriteStartDocument();
            writer.WriteStartElement("Relationships");
            writer.WriteAttributeString("xmlns", "http://schemas.openxmlformats.org/package/2006/relationships");

            writer.WriteStartElement("Relationship");
            writer.WriteAttributeString("Id", "rId1");
            writer.WriteAttributeString("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles");
            writer.WriteAttributeString("Target", "styles.xml");
            writer.WriteEndElement();
            writer.WriteStartElement("Relationship");
            writer.WriteAttributeString("Id", "rId2");
            writer.WriteAttributeString("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/settings");
            writer.WriteAttributeString("Target", "settings.xml");
            writer.WriteEndElement();
            writer.WriteStartElement("Relationship");
            writer.WriteAttributeString("Id", "rId3");
            writer.WriteAttributeString("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/webSettings");
            writer.WriteAttributeString("Target", "webSettings.xml");
            writer.WriteEndElement();
            writer.WriteStartElement("Relationship");
            writer.WriteAttributeString("Id", "rId4");
            writer.WriteAttributeString("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/fontTable");
            writer.WriteAttributeString("Target", "fontTable.xml");
            writer.WriteEndElement();

            for (int index = 0; index < imageCache.ImagePackedStore.Count; index++)
            {
                writer.WriteStartElement("Relationship");
                writer.WriteAttributeString("Id", string.Format("rId{0}", 5 + index));
                writer.WriteAttributeString("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/image");
                writer.WriteAttributeString("Target", string.Format("media/image{0:D5}.{1}", index + 1, GetImageExt((ImageFormat)imageCache.ImageFormatStore[index])));
                writer.WriteEndElement();
            }

            if (usePageHeadersAndFooters)
            {
                writer.WriteStartElement("Relationship");
                writer.WriteAttributeString("Id", string.Format("rId{0}", 5 + imageCache.ImagePackedStore.Count + 0));
                writer.WriteAttributeString("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/footnotes");
                writer.WriteAttributeString("Target", "footnotes.xml");
                writer.WriteEndElement();
                writer.WriteStartElement("Relationship");
                writer.WriteAttributeString("Id", string.Format("rId{0}", 5 + imageCache.ImagePackedStore.Count + 1));
                writer.WriteAttributeString("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/endnotes");
                writer.WriteAttributeString("Target", "endnotes.xml");
                writer.WriteEndElement();
                for (int index = 0; index < headersData.Count; index++)
                {
                    writer.WriteStartElement("Relationship");
                    writer.WriteAttributeString("Id", string.Format("rIdh{0}", index + 1));
                    writer.WriteAttributeString("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/header");
                    writer.WriteAttributeString("Target", string.Format("header{0}.xml", index + 1));
                    writer.WriteEndElement();
                    writer.WriteStartElement("Relationship");
                    writer.WriteAttributeString("Id", string.Format("rIdf{0}", index + 1));
                    writer.WriteAttributeString("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/footer");
                    writer.WriteAttributeString("Target", string.Format("footer{0}.xml", index + 1));
                    writer.WriteEndElement();
                }
            }

            #region Trial
            else
            {
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
                    writer.WriteStartElement("Relationship");
                    writer.WriteAttributeString("Id", string.Format("rId{0}", 5 + imageCache.ImagePackedStore.Count + 0));
                    writer.WriteAttributeString("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/footnotes");
                    writer.WriteAttributeString("Target", "footnotes.xml");
                    writer.WriteEndElement();
                    writer.WriteStartElement("Relationship");
                    writer.WriteAttributeString("Id", string.Format("rId{0}", 5 + imageCache.ImagePackedStore.Count + 1));
                    writer.WriteAttributeString("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/endnotes");
                    writer.WriteAttributeString("Target", "endnotes.xml");
                    writer.WriteEndElement();
                    writer.WriteStartElement("Relationship");
                    writer.WriteAttributeString("Id", "rIdh1");
                    writer.WriteAttributeString("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/header");
                    writer.WriteAttributeString("Target", "headerAdditional.xml");
                    writer.WriteEndElement();
                }
            }
            #endregion

            if (hyperlinkList.Count > 0)
            {
                foreach (DictionaryEntry de in hyperlinkList)
                {
                    writer.WriteStartElement("Relationship");
                    writer.WriteAttributeString("Id", string.Format("hId{0}", (int)de.Value));
                    writer.WriteAttributeString("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/hyperlink");
                    writer.WriteAttributeString("Target", (string)de.Key);
                    writer.WriteAttributeString("TargetMode", "External");
                    writer.WriteEndElement();
                }
            }

            if (embedsList.Count > 0)
            {
                for (int index = 0; index < embedsList.Count; index++)
                {
                    writer.WriteStartElement("Relationship");
                    writer.WriteAttributeString("Id", string.Format("reId{0}", 1 + index));
                    writer.WriteAttributeString("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/aFChunk");
                    writer.WriteAttributeString("Target", string.Format("embeddings/richtext{0:D5}.rtf", index + 1));
                    writer.WriteAttributeString("TargetMode", "Internal");
                    writer.WriteEndElement();
                }
            }

            writer.WriteFullEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return ms;
        }
        #endregion

        #region WriteHeaderFooterRels
        private MemoryStream WriteHeaderFooterRels()
        {
            var ms = new MemoryStream();
            var writer = new XmlTextWriter(ms, Encoding.UTF8);
            writer.Indentation = (xmlIndentation < 0 ? 0 : xmlIndentation);
            writer.Formatting = (xmlIndentation < 0 ? Formatting.None : Formatting.Indented);
            writer.WriteStartDocument();
            writer.WriteStartElement("Relationships");
            writer.WriteAttributeString("xmlns", "http://schemas.openxmlformats.org/package/2006/relationships");

            for (int index = 0; index < imageCache.ImagePackedStore.Count; index++)
            {
                writer.WriteStartElement("Relationship");
                writer.WriteAttributeString("Id", string.Format("rId{0}", 5 + index));
                writer.WriteAttributeString("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/image");
                writer.WriteAttributeString("Target", string.Format("media/image{0:D5}.jpeg", index + 1));
                writer.WriteEndElement();
            }

            writer.WriteFullEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return ms;
        }
        #endregion

        #region WriteStyles
        private MemoryStream WriteStyles()
        {
            var ms = new MemoryStream();
            var writer = new XmlTextWriter(ms, Encoding.UTF8);
            writer.Indentation = (xmlIndentation < 0 ? 0 : xmlIndentation);
            writer.Formatting = (xmlIndentation < 0 ? Formatting.None : Formatting.Indented);
            writer.WriteStartDocument();
            writer.WriteStartElement("w:styles");
            writer.WriteAttributeString("xmlns:r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            writer.WriteAttributeString("xmlns:w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");

            writer.WriteStartElement("w:style");
            writer.WriteAttributeString("w:type", "paragraph");
            writer.WriteAttributeString("w:default", "1");
            writer.WriteAttributeString("w:styleId", "a");
            writer.WriteStartElement("w:name");
            writer.WriteAttributeString("w:val", "Normal");
            writer.WriteEndElement();
            writer.WriteStartElement("w:qFormat");
            writer.WriteEndElement();
            writer.WriteStartElement("w:pPr");
            writer.WriteStartElement("w:spacing");
            writer.WriteAttributeString("w:after", "0");
            writer.WriteAttributeString("w:line", "240");
            writer.WriteAttributeString("w:lineRule", "auto");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteStartElement("w:rPr");
            writer.WriteStartElement("w:sz");
            writer.WriteAttributeString("w:val", string.Format("{0}", (int)Math.Round(StiOptions.Export.Word.NormalStyleDefaultFontSize * 2)));
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();

            writer.WriteStartElement("w:style");
            writer.WriteAttributeString("w:type", "character");
            writer.WriteAttributeString("w:default", "1");
            writer.WriteAttributeString("w:styleId", "a0");
            writer.WriteStartElement("w:name");
            writer.WriteAttributeString("w:val", "Default Paragraph Font");
            writer.WriteEndElement();
            writer.WriteStartElement("w:uiPriority");
            writer.WriteAttributeString("w:val", "1");
            writer.WriteEndElement();
            writer.WriteStartElement("w:semiHidden");
            writer.WriteEndElement();
            writer.WriteStartElement("w:unhideWhenUsed");
            writer.WriteEndElement();
            writer.WriteEndElement();

            writer.WriteStartElement("w:style");
            writer.WriteAttributeString("w:type", "table");
            writer.WriteAttributeString("w:default", "1");
            writer.WriteAttributeString("w:styleId", "a1");
            writer.WriteStartElement("w:name");
            writer.WriteAttributeString("w:val", "Normal Table");
            writer.WriteEndElement();
            writer.WriteStartElement("w:uiPriority");
            writer.WriteAttributeString("w:val", "99");
            writer.WriteEndElement();
            writer.WriteStartElement("w:semiHidden");
            writer.WriteEndElement();
            writer.WriteStartElement("w:unhideWhenUsed");
            writer.WriteEndElement();
            writer.WriteStartElement("w:qFormat");
            writer.WriteEndElement();
            writer.WriteStartElement("w:tblPr");
            writer.WriteStartElement("w:tblInd");
            writer.WriteAttributeString("w:w", "0");
            writer.WriteAttributeString("w:type", "dxa");
            writer.WriteEndElement();
            writer.WriteStartElement("w:tblCellMar");
            writer.WriteStartElement("w:top");
            writer.WriteAttributeString("w:w", "0");
            writer.WriteAttributeString("w:type", "dxa");
            writer.WriteEndElement();
            writer.WriteStartElement("w:left");
            writer.WriteAttributeString("w:w", "0");
            writer.WriteAttributeString("w:type", "dxa");
            writer.WriteEndElement();
            writer.WriteStartElement("w:bottom");
            writer.WriteAttributeString("w:w", "0");
            writer.WriteAttributeString("w:type", "dxa");
            writer.WriteEndElement();
            writer.WriteStartElement("w:right");
            writer.WriteAttributeString("w:w", "0");
            writer.WriteAttributeString("w:type", "dxa");
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();

            writer.WriteStartElement("w:style");
            writer.WriteAttributeString("w:type", "numbering");
            writer.WriteAttributeString("w:default", "1");
            writer.WriteAttributeString("w:styleId", "a2");
            writer.WriteStartElement("w:name");
            writer.WriteAttributeString("w:val", "No List");
            writer.WriteEndElement();
            writer.WriteStartElement("w:uiPriority");
            writer.WriteAttributeString("w:val", "99");
            writer.WriteEndElement();
            writer.WriteStartElement("w:semiHidden");
            writer.WriteEndElement();
            writer.WriteStartElement("w:unhideWhenUsed");
            writer.WriteEndElement();
            writer.WriteEndElement();

            for (int index = 0; index < styleList.Count; index++)
            {
                var style = (StiWord2007StyleInfo)styleList[index];
                writer.WriteStartElement("w:style");
                writer.WriteAttributeString("w:type", "paragraph");
                writer.WriteAttributeString("w:customStyle", "1");
                writer.WriteAttributeString("w:styleId", string.Format("Style{0}", index));
                writer.WriteStartElement("w:name");
                writer.WriteAttributeString("w:val", style.Name);
                writer.WriteEndElement();
                writer.WriteStartElement("w:basedOn");
                writer.WriteAttributeString("w:val", "a");
                writer.WriteEndElement();
                writer.WriteStartElement("w:rPr");
                writer.WriteStartElement("w:rFonts");
                writer.WriteAttributeString("w:ascii", style.FontName);
                writer.WriteAttributeString("w:hAnsi", style.FontName);
                writer.WriteAttributeString("w:cs", style.FontName);
                writer.WriteEndElement();
                if (style.Bold)
                {
                    writer.WriteStartElement("w:b");
                    writer.WriteEndElement();
                }
                if (style.Italic)
                {
                    writer.WriteStartElement("w:i");
                    writer.WriteEndElement();
                }
                if (style.Underline)
                {
                    writer.WriteStartElement("w:u");
                    writer.WriteAttributeString("w:val", "single");
                    writer.WriteEndElement();
                }
                writer.WriteStartElement("w:color");
                writer.WriteAttributeString("w:val", GetColorString(style.TextColor));
                writer.WriteEndElement();
                writer.WriteStartElement("w:sz");
                writer.WriteAttributeString("w:val", string.Format("{0}", style.FontSize));
                writer.WriteEndElement();
                if (StiOptions.Export.Word.SpaceBetweenCharacters != 0)
                {
                    writer.WriteStartElement("w:spacing");
                    writer.WriteAttributeString("w:val", string.Format("{0}", StiOptions.Export.Word.SpaceBetweenCharacters));
                    writer.WriteEndElement();
                }
                if (style.RightToLeft)
                {
                    if (style.Bold)
                    {
                        writer.WriteStartElement("w:bCs");
                        writer.WriteEndElement();
                    }
                    if (style.Italic)
                    {
                        writer.WriteStartElement("w:iCs");
                        writer.WriteEndElement();
                    }
                    writer.WriteStartElement("w:szCs");
                    writer.WriteAttributeString("w:val", string.Format("{0}", style.FontSize));
                    writer.WriteEndElement();
                    writer.WriteStartElement("w:rtl");
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndElement();
            }

            writer.WriteFullEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return ms;
        }
        #endregion

        #region WriteImage
        private MemoryStream WriteImage(int number)
        {
            var ms = new MemoryStream();
            var buf = (byte[])imageCache.ImagePackedStore[number];
            ms.Write(buf, 0, buf.Length);
            return ms;
        }
        #endregion

        #region WriteEmbed
        private MemoryStream WriteEmbed(int number)
        {
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms, Encoding.GetEncoding(1252));

            var st = (string)embedsList[number];

            var st2 = StiExportUtils.CorrectEncoding(st);
            if (st2 != null) st = st2;

            sw.Write(st);
            sw.Flush();
            return ms;
        }
        #endregion

        #region WriteAdditionalData
        private MemoryStream WriteAdditionalData(string st, bool base64)
        {
            var ms = new MemoryStream();
            var bytes = base64
                ? global::System.Convert.FromBase64String(st)
                : Encoding.ASCII.GetBytes(st);

            ms.Write(bytes, 0, bytes.Length);
            return ms;
        }
        #endregion

        /// <summary>
        /// Exports rendered report to an Word file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="fileName">A name of the file for exporting a rendered report.</param>
        public void ExportWord(StiReport report, string fileName)
        {
            FileStream stream = null;
            try
            {
                StiFileUtils.ProcessReadOnly(fileName);
                stream = new FileStream(fileName, FileMode.Create);
                ExportWord(report, stream);
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
        /// Exports rendered report to an Word file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream for export of a document.</param>
        public void ExportWord(StiReport report, Stream stream)
        {
            ExportWord(report, stream, new StiWord2007ExportSettings());
        }


        /// <summary>
        /// Exports rendered report to an Word file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream for export of a document.</param>
        /// <param name="pageRange">Describes range of pages of the document for the export.</param>
        public void ExportWord(StiReport report, Stream stream, StiPagesRange pageRange)
        {
            var settings = new StiWord2007ExportSettings
            {
                PageRange = pageRange
            };

            ExportWord(report, stream, settings);
        }



        public void ExportWord(StiReport report, Stream stream, StiWord2007ExportSettings settings)
        {
            StiLogService.Write(this.GetType(), "Export report to Word 2007 format");

#if NETSTANDARD || NETCOREAPP
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif

            string reportVersion = StiExportUtils.GetReportVersion();
            string reportVersion2 = StiExportUtils.GetReportVersion(report);

            #region Read settings
            if (settings == null)
                throw new ArgumentNullException("The 'settings' argument cannot be equal in null.");

            var pageRange = settings.PageRange;
            usePageHeadersAndFooters = settings.UsePageHeadersAndFooters;
            RemoveEmptySpaceAtBottom = settings.RemoveEmptySpaceAtBottom;
            imageResolution = settings.ImageResolution;
            imageQuality = settings.ImageQuality;
            docCompanyString = settings.CompanyString == reportVersion ? reportVersion2 : settings.CompanyString;
            docLastModifiedString = settings.LastModifiedString == reportVersion ? reportVersion2 : settings.LastModifiedString;
            restrictEditing = settings.RestrictEditing;
            #endregion

            xmlIndentation = -1;

            if (imageQuality < 0) imageQuality = 0;
            if (imageQuality > 1) imageQuality = 1;
            if (imageResolution < 10) imageResolution = 10;
            imageResolution = imageResolution / 100;

            if (StiOptions.Export.Word.LineSpacing != 1)
            {
                lineSpace = ((int)Math.Round(StiOptions.Export.Word.LineSpacing * 240)).ToString();
                lineSpace2 = ((int)Math.Round(StiOptions.Export.Word.LineSpacing * 0.772 * 240)).ToString();
            }
            if (StiOptions.Export.Word.RestrictEditing > restrictEditing)
            {
                restrictEditing = StiOptions.Export.Word.RestrictEditing;
            }

            this.report = report;

            var currentCulture = Thread.CurrentThread.CurrentCulture;
            try
            {
                //StiExportUtils.DisableFontSmoothing();
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

                styleList = new ArrayList();
                fontList = new Hashtable();
                imageCache = new StiImageCache(StiOptions.Export.Word.AllowImageComparer, ImageFormat.Jpeg, imageQuality);
                bookmarkList = new Hashtable();
                hyperlinkList = new Hashtable();
                embedsList = new ArrayList();

                CurrentPassNumber = 0;
                MaximumPassNumber = 3 + (StiOptions.Export.Word.DivideSegmentPages ? 1 : 0);

                bool hasDividedPages = false;
                var pages = pageRange.GetSelectedPages(report.RenderedPages);
                if (StiOptions.Export.Word.DivideSegmentPages)
                {
                    bool needDivideSegments = true;
                    if (usePageHeadersAndFooters && (pages.Count > 0) && pages[0].UnlimitedHeight && (pages[0].SegmentPerWidth == 1)) needDivideSegments = false;
                    if (needDivideSegments)
                    {
                        var newPages = StiSegmentPagesDivider.Divide(pages, this);
                        if (pages != newPages) hasDividedPages = true;
                        pages = newPages;
                        CurrentPassNumber++;
                    }
                }
                if (IsStopped) return;

                StatusString = StiLocalization.Get("Export", "ExportingCreatingDocument");

                var zip = new StiZipWriter20();
                zip.Begin(stream, true);

                if (usePageHeadersAndFooters)
                {
                    headersData = new ArrayList();
                    headersRels = new ArrayList();
                    footersData = new ArrayList();
                    footersRels = new ArrayList();
                }

                #region write document
                zip.AddFile("word/document.xml", WriteDocument(pages, report, hasDividedPages), true);
                if (IsStopped) return;

                if (usePageHeadersAndFooters)
                {
                    for (int index = 0; index < headersData.Count; index++)
                    {
                        zip.AddFile(string.Format("word/header{0}.xml", index + 1), (MemoryStream)headersData[index]);
                        if (headersRels[index] != null) zip.AddFile(string.Format("word/_rels/header{0}.xml.rels", index + 1), (MemoryStream)headersRels[index]);
                        zip.AddFile(string.Format("word/footer{0}.xml", index + 1), (MemoryStream)footersData[index]);
                        if (footersRels[index] != null) zip.AddFile(string.Format("word/_rels/footer{0}.xml.rels", index + 1), (MemoryStream)footersRels[index]);
                    }
                }
                #endregion

                if (usePageHeadersAndFooters)
                {
                    zip.AddFile("word/footnotes.xml", WriteFootNotes());
                    zip.AddFile("word/endnotes.xml", WriteEndNotes());
                }
                zip.AddFile("[Content_Types].xml", WriteContentTypes());
                zip.AddFile("_rels/.rels", WriteMainRels());
                zip.AddFile("docProps/app.xml", WriteDocPropsApp());
                zip.AddFile("docProps/core.xml", WriteDocPropsCore());
                zip.AddFile("word/settings.xml", WriteSettings(pages));
                zip.AddFile("word/webSettings.xml", WriteWebSettings());
                zip.AddFile("word/fontTable.xml", WriteFontTable());
                zip.AddFile("word/_rels/document.xml.rels", WriteDocumentRels());
                zip.AddFile("word/styles.xml", WriteStyles());
                if (imageCache.ImagePackedStore.Count > 0)
                {
                    for (int index = 0; index < imageCache.ImagePackedStore.Count; index++)
                    {
                        zip.AddFile(string.Format("word/media/image{0:D5}.{1}", index + 1, GetImageExt((ImageFormat)imageCache.ImageFormatStore[index])), WriteImage(index));
                    }
                }
                if (embedsList.Count > 0)
                {
                    for (int index = 0; index < embedsList.Count; index++)
                    {
                        zip.AddFile(string.Format("word/embeddings/richtext{0:D5}.rtf", index + 1), WriteEmbed(index));
                    }
                }

                #region Trial
#if CLOUD
                var isTrial = StiCloudPlan.IsTrial(report.ReportGuid);
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
                if (!usePageHeadersAndFooters && isTrial)
                {
                    zip.AddFile("word/footnotes.xml", WriteFootNotes());
                    zip.AddFile("word/endnotes.xml", WriteEndNotes());
                    zip.AddFile("word/media/imageAdditional.png", WriteAdditionalData(StiExportUtils.AdditionalData, true));
                    zip.AddFile("word/headerAdditional.xml", WriteAdditionalData(
                        "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>\r\n<w:hdr xmlns:ve=\"http://schemas.openxmlformats.org/markup-compatibility/2006\" xmlns:o=\"urn:schemas-microsoft-com:office:office\" " +
                        "xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\" xmlns:m=\"http://schemas.openxmlformats.org/officeDocument/2006/math\" xmlns:v=\"urn:schemas-microsoft-com:vml\" " +
                        "xmlns:wp=\"http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing\" xmlns:w10=\"urn:schemas-microsoft-com:office:word\" xmlns:w=\"http://schemas.openxmlformats.org/wordprocessingml/2006/main\" " +
                        "xmlns:wne=\"http://schemas.microsoft.com/office/word/2006/wordml\"><w:p><w:pPr><w:pStyle w:val=\"a3\"/></w:pPr><w:r><w:rPr><w:noProof/><w:lang w:eastAsia=\"ru-RU\"/></w:rPr><w:pict>" +
                        "<v:shapetype id=\"_x0000_t75\" coordsize=\"21600,21600\" o:spt=\"75\" o:preferrelative=\"t\" path=\"m@4@5l@4@11@9@11@9@5xe\" filled=\"f\" stroked=\"f\"> <v:stroke joinstyle=\"miter\"/> <v:formulas>" +
                        "<v:f eqn=\"if lineDrawn pixelLineWidth 0\"/><v:f eqn=\"sum @0 1 0\"/><v:f eqn=\"sum 0 0 @1\"/><v:f eqn=\"prod @2 1 2\"/><v:f eqn=\"prod @3 21600 pixelWidth\"/><v:f eqn=\"prod @3 21600 pixelHeight\"/><v:f eqn=\"sum @0 0 1\"/>" +
                        "<v:f eqn=\"prod @6 1 2\"/><v:f eqn=\"prod @7 21600 pixelWidth\"/><v:f eqn=\"sum @8 21600 0\"/><v:f eqn=\"prod @7 21600 pixelHeight\"/><v:f eqn=\"sum @10 21600 0\"/></v:formulas><v:path o:extrusionok=\"f\" gradientshapeok=\"t\" o:connecttype=\"rect\"/>" +
                        "<o:lock v:ext=\"edit\" aspectratio=\"t\"/></v:shapetype><v:shape id=\"WordPictureWatermark11777093\" o:spid=\"_x0000_s2050\" type=\"#_x0000_t75\" " +
                        "style=\"position:absolute;margin-left:0;margin-top:0;width:467.45pt;height:381pt;z-index:-251657216;mso-position-horizontal:center;mso-position-horizontal-relative:margin;mso-position-vertical:center;mso-position-vertical-relative:margin\" " +
                        "o:allowincell=\"f\"><v:imagedata r:id=\"rId1\" o:title=\"additional\"/></v:shape></w:pict></w:r></w:p></w:hdr>", false));
                    zip.AddFile("word/_rels/headerAdditional.xml.rels", WriteAdditionalData(
                        "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>\r\n<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\"><Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/image\" Target=\"media/imageAdditional.png\" /></Relationships>", false));
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
                fontList.Clear();
                fontList = null;
                imageCache.Clear();
                imageCache = null;
                styleList.Clear();
                styleList = null;
                bookmarkList.Clear();
                bookmarkList = null;
                hyperlinkList.Clear();
                hyperlinkList = null;
                embedsList.Clear();
                embedsList = null;
                if (usePageHeadersAndFooters)
                {
                    headersData.Clear();
                    headersData = null;
                    headersRels.Clear();
                    headersRels = null;
                    footersData.Clear();
                    footersData = null;
                    footersRels.Clear();
                    footersRels = null;
                }

                if (report.RenderedPages.CacheMode) StiMatrix.GCCollect();
            }
        }
        #endregion
    }
}