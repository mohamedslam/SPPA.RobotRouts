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
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Stimulsoft.Base;
using Stimulsoft.Base.Licenses;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Services;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Helpers;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
using Stimulsoft.System.Security.Cryptography;
#else
using System.Windows.Forms;
using System.Security.Cryptography;
#endif

namespace Stimulsoft.Report.Export
{
    /// <summary>
    /// A class for the text export.
    /// </summary>
    [StiServiceBitmap(typeof(StiExportService), "Stimulsoft.Report.Images.Dictionary.ResourceTxt.png")]
    public class StiTxtExportService : StiExportService
    {
        #region struct StiTxtStyleInfo
        private struct StiTxtStyleInfo
        {
            public bool Bold;
            public bool Italic;
            public bool Underline;
            public string Codes;
        }
        #endregion

        #region StiExportService override
        /// <summary>
		/// Gets or sets a default extension of export. 
		/// </summary>
		public override string DefaultExtension => "txt";

        public override StiExportFormat ExportFormat => StiExportFormat.Text;

        /// <summary>
        /// Gets a group of the export in the context menu.
        /// </summary>
        public override string GroupCategory => "Word";

        /// <summary>
        /// Gets a position of the export in the context menu.
        /// </summary>
        public override int Position => (int)StiExportPosition.Txt;

        /// <summary>
        /// Gets a name of the export in the context menu.
        /// </summary>
        public override string ExportNameInMenu => StiLocalization.Get("Export", "ExportTypeTxtFile");

        /// <summary>
        /// Gets a value indicating a number of files in exported document as a result of export
        /// of one page of the rendered report.
        /// </summary>        
        public override bool MultipleFiles => false;

        /// <summary>
        /// Returns a filter for text files.
        /// </summary>
        /// <returns>Returns a filter for text files.</returns>		
        public override string GetFilter()
        {
            return StiLocalization.Get("FileFilters", "TxtFiles");
        }

        /// <summary>
        /// Exports a document to the stream without dialog of the saving file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream in which report will be exported.</param>
        /// <param name="settings">A settings for the report exporting.</param>
        public override void ExportTo(StiReport report, Stream stream, StiExportSettings settings)
        {
            ExportTxt(report, stream, settings as StiTxtExportSettings);
            InvokeExporting(100, 100, 1, 1);
        }

        /// <summary>
        /// Exports a rendered report to the file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="fileName">A name of the file for exporting a rendered report.</param>
        /// <param name="sendEMail">If true then the result of the exporting will be sent via e-mail.</param>
        public override void Export(StiReport report, string fileName, bool sendEMail, StiGuiMode guiMode)
        {
            using (var form = StiGuiOptions.GetExportFormRunner("StiTxtExportSetupForm", guiMode, this.OwnerWindow))
            {
                form["CurrentPage"] = report.CurrentPrintPage;
                form["OpenAfterExportEnabled"] = !sendEMail;

                this.report = report;
                this.fileName = fileName;
                this.sendEMail = sendEMail;
                this.guiMode = guiMode;
                form.Complete += FormComplete;
                form.ShowDialog();
            }
        }
        #endregion
        
        #region Consts
        /// <summary>
        /// Symbols code of the border for the inner representation.
        /// </summary>
        private const string borderCodes = "\x06\x09\x0c\x0a\x05\x03\x0d\x0b\x0e\x07\x0f\x01\x02\x04\x08";
        private const int firstEscapeCodeIndex = 0xF8FE;
        private const string ltrMark = "\x200E"; //Left-to-Right mark
        #endregion

        #region Fields
        private StiReport report;
        private string fileName;
        private bool sendEMail;
        private StiGuiMode guiMode;
        private bool[] needVerticalBorders;
        private bool[] needHorizontalBorders;
        private bool useFullTextBoxWidth;
        private bool useFullVerticalBorder = true;
        private bool useFullHorizontalBorder = true;
        private bool useEscapeCodes;
        private List<StiTxtStyleInfo> styleList;
        private List<string> escapeCodesList;

        internal bool putFeedPageCode;
        #endregion

        #region Methods
        private void FormComplete(IStiFormRunner form, StiShowDialogCompleteEvetArgs e)
        {
            if (!e.DialogResult)
            {
                IsStopped = true;
                return;
            }

            if (string.IsNullOrEmpty(fileName))
                fileName = base.GetFileName(report, sendEMail);

            if (fileName == null) return;

            StiFileUtils.ProcessReadOnly(fileName);
            try
            {
                using (var stream = new FileStream(fileName, FileMode.Create))
                {
                    StartProgress(guiMode);

                    var settings = new StiTxtExportSettings
                    {
                        PageRange = form["PagesRange"] as StiPagesRange,
                        Encoding = form["Encoding"] as Encoding,
                        DrawBorder = (bool)form["DrawBorder"],
                        BorderType = (StiTxtBorderType)form["BorderType"],
                        KillSpaceLines = (bool)form["KillSpaceLines"],
                        PutFeedPageCode = (bool)form["PutFeedPageCode"],
                        CutLongLines = (bool)form["CutLongLines"],
                        ZoomX = (float)form["ZoomX"],
                        ZoomY = (float)form["ZoomY"]
                    };

                    base.StartExport(report, stream, settings, sendEMail, (bool)form["OpenAfterExport"], fileName, guiMode);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Sets the border symbol according to its code depending on the selected border type.
        /// </summary>
        /// <param name="code">Symbol code</param>
        /// <param name="type">Border type</param>
        /// <returns>Border symbol</returns>
        private char GetBorderChar(int code, StiTxtBorderType type)
        {
            if (useFullVerticalBorder)
            {
                //new version
                switch (type)
                {
                    case StiTxtBorderType.UnicodeSingle:
                        return "\x2500\x2502\x250c\x2510\x2514\x2518\x251c\x2524\x252c\x2534\x253c\x20\x2500\x20\x20"[code];

                    case StiTxtBorderType.UnicodeDouble:
                        return "\x2550\x2551\x2554\x2557\x255a\x255d\x2560\x2563\x2566\x2569\x256c\x20\x2550\x20\x20"[code];

                    default:
                        return "-|+++++++++\x20-\x20\x20"[code];
                }
            }
            else
            {
                //old version
                switch (type)
                {
                    case StiTxtBorderType.UnicodeSingle:
                        return "\x2500\x2502\x250c\x2510\x2514\x2518\x251c\x2524\x252c\x2534\x253c\x20\x20\x20\x20"[code];

                    case StiTxtBorderType.UnicodeDouble:
                        return "\x2550\x2551\x2554\x2557\x255a\x255d\x2560\x2563\x2566\x2569\x256c\x20\x20\x20\x20"[code];

                    default:
                        return "-|+++++++++\x20\x20\x20\x20"[code];
                }
            }
        }
        
        /// <summary>
        /// Fills the specified part of a string with the border code.
        /// </summary>
        /// <param name="sb">String pointer</param>
        /// <param name="code">The number of the border code</param>
        /// <param name="startPosition">Start position</param>
        /// <param name="count">Section length</param>
        private void LineFill(StringBuilder sb, int code, int startPosition, int count)
        {
            var ch = borderCodes[code - 1];
            for (var index = 0; index < count; index++)
            {
                if (borderCodes.IndexOf(sb[startPosition + index]) == -1)
                {
                    if (sb[startPosition + index] == ' ')
                        sb[startPosition + index] = ch;
                }
                else
                    sb[startPosition + index] |= ch;
            }
        }

        private void LineFillChar(StringBuilder sb, int code, int startPosition, int count)
        {
            for (var index = 0; index < count; index++)
            {
                sb[startPosition + index] = (char)(code);
            }
        }
        
        private void CheckWordWrap(StiCell cell, ref string[] arraySt, int cellWidth, bool cutLongLines)
        {
            if (useEscapeCodes)
            {
                #region Escape codes
                for (var index = 0; index < arraySt.Length; index++)
                {
                    var st = arraySt[index];
                    var text = new StringBuilder(st);

                    if (StiBidirectionalConvert.StringContainArabicOrHebrew(st))
                    {
                        var pos3 = st.Length;
                        while (char.IsWhiteSpace(st, pos3 - 1)) pos3--;
                        text.Insert(pos3, "<#ltrMark>");
                    }

                    var pos = 0;
                    while (pos < text.Length)
                    {
                        if ((text[pos] == '<') && (pos + 1 < text.Length) && (text[pos + 1] == '#'))
                        {
                            var pos2 = pos;
                            var escapeString = new StringBuilder();
                            while ((text[pos2] == '<') && (pos2 + 1 < text.Length) && (text[pos2 + 1] == '#'))
                            {
                                while ((pos2 < text.Length) && (text[pos2] != '>'))
                                {
                                    escapeString.Append(text[pos2]);
                                    pos2++;
                                }
                                escapeString.Append('>');
                                pos2++;
                                if (pos2 > text.Length - 1) break;
                            }

                            if (pos2 > text.Length - 1)
                            {
                                if (pos > 0)
                                {
                                    pos--;
                                    escapeString.Insert(0, text[pos]);
                                }
                                else
                                {
                                    escapeString.Append((char)0xFFFF);
                                }
                            }
                            else
                            {
                                escapeString.Append(text[pos2]);
                                pos2++;
                            }

                            var escapeIndex = GetEscapeNumber(escapeCodesList, escapeString.ToString());
                            text.Remove(pos, pos2 - pos);
                            text.Insert(pos, (char)(firstEscapeCodeIndex - escapeIndex));
                        }
                        pos++;
                    }

                    if (text.Length != arraySt[index].Length)
                        arraySt[index] = text.ToString();
                }
                #endregion
            }
            else
            {
                #region Check ltrMark
                for (var index = 0; index < arraySt.Length; index++)
                {
                    if (StiBidirectionalConvert.StringContainArabicOrHebrew(arraySt[index]))
                    {
                        var st = arraySt[index];
                        var pos = st.Length - 1;

                        while (char.IsWhiteSpace(st, pos))pos--;

                        var escapeString = st[pos] + ltrMark;
                        var escapeIndex = GetEscapeNumber(escapeCodesList, escapeString);

                        if (pos == st.Length - 1)
                            arraySt[index] = st.Substring(0, pos) + (char) (firstEscapeCodeIndex - escapeIndex);

                        else
                            arraySt[index] = st.Substring(0, pos) + (char) (firstEscapeCodeIndex - escapeIndex) + st.Substring(pos + 1);
                    }
                }
                #endregion
            }

            var widthOfCell = cellWidth;
            if (useFullVerticalBorder)
            {
                if (needVerticalBorders[cell.Left + cell.Width])
                    widthOfCell--;
            }
            else
            {
                if (!useFullTextBoxWidth && widthOfCell > 1)
                    widthOfCell--;
            }

            var wordWrap = false;
            var textOpt = cell.Component as IStiTextOptions;
            if (textOpt != null) wordWrap = textOpt.TextOptions.WordWrap;
            if (wordWrap)
            {
                #region Wordwrap
                var stringList = new ArrayList(arraySt);
                for (var indexLine = 0; indexLine < stringList.Count; indexLine++)
                {
                    #region check line
                    var stt = (string)stringList[indexLine];
                    if (stt.Length > widthOfCell - 1 && widthOfCell > 1)
                    {
                        var wordarr = new int[stt.Length];
                        var wordCounter = 0;
                        var tempIndexSpace = 0;

                        while ((tempIndexSpace < stt.Length) && (stt[tempIndexSpace] == ' '))
                        {
                            wordarr[tempIndexSpace] = wordCounter;
                            tempIndexSpace++;
                        }

                        for (var tempIndex = tempIndexSpace; tempIndex < stt.Length; tempIndex++)
                        {
                            if (stt[tempIndex] == ' ')
                                wordCounter++;

                            wordarr[tempIndex] = wordCounter;
                        }

                        var index = (widthOfCell - 1) - 1;
                        //check words number; if no first - go to begin, else to end of word
                        var index2 = index;
                        if (wordarr[index] > 0) //word is no first
                        {
                            if (wordarr[index] != wordarr[index + 1])
                            {
                                index2 = index++;
                                while ((index < stt.Length) && (stt[index] == ' ')) index++;
                            }
                            else
                            {
                                while (stt[index] != ' ') index--;
                                index2 = index++;
                                while (stt[index2] == ' ') index2--;
                            }
                        }
                        else
                            index++;

                        stringList[indexLine] = stt.Substring(0, index2 + 1);
                        stringList.Insert(indexLine + 1, stt.Substring(index, stt.Length - index));
                    }
                    #endregion
                }

                if (stringList.Count > arraySt.Length)
                {
                    arraySt = new string[stringList.Count];
                    for (var index = 0; index < stringList.Count; index++)
                    {
                        arraySt[index] = (string)stringList[index];
                    }
                }
                #endregion
            }

            if (cutLongLines)
            {
                for (var index = 0; index < arraySt.Length; index++)
                {
                    if (arraySt[index].Length > widthOfCell)
                        arraySt[index] = arraySt[index].Substring(0, widthOfCell);
                }
            }
        }

        private void CheckGrow(List<StringBuilder> lines, List<StringBuilder> styles, List<bool> pageBreaks, int currentY)
        {
            while (lines.Count <= currentY)
            {
                lines.Add(new StringBuilder());
                styles.Add(new StringBuilder());
                pageBreaks.Add(false);
            }
        }

        private int GetStyleNumber(List<StiTxtStyleInfo> tmpStyleList, StiTxtStyleInfo styleInfo)
        {
            if (tmpStyleList.Count > 0)
            {
                for (var index = 0; index < tmpStyleList.Count; index++)
                {
                    var tmpStyle = tmpStyleList[index];
                    if ((tmpStyle.Bold == styleInfo.Bold) &&
                        (tmpStyle.Italic == styleInfo.Italic) &&
                        (tmpStyle.Underline == styleInfo.Underline) &&
                        (tmpStyle.Codes == styleInfo.Codes))
                    {
                        //style is already in table, return number
                        return index;
                    }
                }
            }
            //add style to table, return style number
            tmpStyleList.Add(styleInfo);
            var temp = tmpStyleList.Count - 1;
            return temp;
        }

        private int GetEscapeNumber(List<string> tmpStyleList, string escapeCodes)
        {
            if (tmpStyleList.Count > 0)
            {
                for (var index = 0; index < tmpStyleList.Count; index++)
                {
                    if (tmpStyleList[index] == escapeCodes)
                    {
                        //style is already in table, return number
                        return index + 1;
                    }
                }
            }
            //add style to table, return style number
            tmpStyleList.Add(escapeCodes);
            var temp = tmpStyleList.Count;
            return temp;
        }

        private StringBuilder GetEscapeNames(int oldStyleNumber, int newStyleNumber)
        {
            var oldStyle = styleList[oldStyleNumber];
            var newStyle = styleList[newStyleNumber];
            var sb = new StringBuilder();
            if (!oldStyle.Bold && newStyle.Bold) sb.Append("<#b>");
            if (oldStyle.Bold && !newStyle.Bold) sb.Append("<#/b>");
            if (!oldStyle.Italic && newStyle.Italic) sb.Append("<#i>");
            if (oldStyle.Italic && !newStyle.Italic) sb.Append("<#/i>");
            if (!oldStyle.Underline && newStyle.Underline) sb.Append("<#u>");
            if (oldStyle.Underline && !newStyle.Underline) sb.Append("<#/u>");
            return sb;
        }

        /// <summary>
        /// Exports a rendered report to the text file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="fileName">A name of the file for exporting a rendered report.</param>
        public void ExportTxt(StiReport report, string fileName)
        {
            StiFileUtils.ProcessReadOnly(fileName);
            var stream = new FileStream(fileName, FileMode.Create);
            ExportTxt(report, stream);
            stream.Flush();
            stream.Close();
        }

        /// <summary>
        /// Exports a rendered report to the stream in text file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream for the export.</param>
        public void ExportTxt(StiReport report, Stream stream)
        {
            var settings = new StiTxtExportSettings();
            ExportTxt(report, stream, settings);
        }

        /// <summary>
        /// Exports a rendered report to the stream in the text file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream for the export.</param>
        /// <param name="encoding">A code page of a text.</param>
        /// <param name="pageRange">A Range of a page for exporting.</param>
        public void ExportTxt(StiReport report, Stream stream, Encoding encoding, StiPagesRange pageRange)
        {
            ExportTxt(report, stream, new StiTxtExportSettings
            {
                PageRange = pageRange,
                Encoding = encoding
            });
        }

        /// <summary>
        /// Exports a rendered report to the stream in the text file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">Stream for exporting.</param>
        /// <param name="encoding">A code page of a text.</param>
        /// <param name="drawBorder">If true then borders are exported to text.</param>
        /// <param name="borderType">Type of borders.</param>
        /// <param name="killSpaceLines">If true then empty lines will be removed from the result text.</param>
        /// <param name="killSpaceGraphLines">If true then empty lines with vertical borders will be removed from the result text.</param>
        /// <param name="putFeedPageCode">If true then EOF char will be added to the end of each page.</param>
        /// <param name="cutLongLines">If true then all long lines will be cut.</param>
        /// <param name="zoomX">Horizontal zoom factor by X axis. By default a value is 1.0f what is equal 100% in export settings window.</param>
        /// <param name="zoomY">Vertical zoom factor by Y axis. By default a value is 1.0f what is equal 100% in export settings window.</param>
        /// <param name="pageRange">A Range of the page for exporting.</param>
        public void ExportTxt(StiReport report, Stream stream, Encoding encoding, bool drawBorder,
            StiTxtBorderType borderType, bool killSpaceLines, bool killSpaceGraphLines,
            bool putFeedPageCode, bool cutLongLines, float zoomX, float zoomY, StiPagesRange pageRange)
        {
            ExportTxt(report, stream, new StiTxtExportSettings
            {
                PageRange = pageRange,
                Encoding = encoding,
                DrawBorder = drawBorder,
                BorderType = borderType,
                KillSpaceLines = killSpaceLines,
                KillSpaceGraphLines = killSpaceGraphLines,
                PutFeedPageCode = putFeedPageCode,
                CutLongLines = cutLongLines,
                ZoomX = zoomX,
                ZoomY = zoomY
            });
        }

        /// <summary>
        /// Exports a rendered report to the stream in the text file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">Stream for exporting.</param>
        /// <param name="encoding">A code page of a text.</param>
        /// <param name="drawBorder">If true then borders are exported to text.</param>
        /// <param name="borderType">Type of borders.</param>
        /// <param name="killSpaceLines">If true then empty lines will be removed from the result text.</param>
        /// <param name="killSpaceGraphLines">If true then empty lines with vertical borders will be removed from the result text.</param>
        /// <param name="putFeedPageCode">If true then EOF char will be added to the end of each page.</param>
        /// <param name="cutLongLines">If true then all long lines will be cut.</param>
        /// <param name="zoomX">Horizontal zoom factor by X axis. By default a value is 1.0f what is equal 100% in export settings window.</param>
        /// <param name="zoomY">Vertical zoom factor by Y axis. By default a value is 1.0f what is equal 100% in export settings window.</param>
        /// <param name="pageRange">A Range of the page for exporting.</param>
        /// <param name="useEscapeCodes">Use Escape codes in text.</param>
        /// <param name="escapeCodesCollectionName">Name of the Escape codes collection to use.</param>
        public void ExportTxt(StiReport report, Stream stream, Encoding encoding, bool drawBorder,
            StiTxtBorderType borderType, bool killSpaceLines, bool killSpaceGraphLines,
            bool putFeedPageCode, bool cutLongLines, float zoomX, float zoomY, StiPagesRange pageRange,
            bool useEscapeCodes, string escapeCodesCollectionName)
        {
            ExportTxt(report, stream, new StiTxtExportSettings
            {
                PageRange = pageRange,
                Encoding = encoding,
                DrawBorder = drawBorder,
                BorderType = borderType,
                KillSpaceLines = killSpaceLines,
                KillSpaceGraphLines = killSpaceGraphLines,
                PutFeedPageCode = putFeedPageCode,
                CutLongLines = cutLongLines,
                ZoomX = zoomX,
                ZoomY = zoomY,
                UseEscapeCodes = useEscapeCodes,
                EscapeCodesCollectionName = escapeCodesCollectionName
            });
        }

        /// <summary>
        /// Exports a rendered report to the stream in the text file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream for exporting.</param>
        /// <param name="settings">Export settings.</param>
        public void ExportTxt(StiReport report, Stream stream, StiTxtExportSettings settings)
        {
            StiLogService.Write(this.GetType(), "Export report to Text format");

            this.report = report;

            #region Read settings
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            var pageRange = settings.PageRange;
            var encoding = settings.Encoding;
            var drawBorder = settings.DrawBorder;
            var borderType = settings.BorderType;
            var killSpaceLines = settings.KillSpaceLines;
            var killSpaceGraphLines = settings.KillSpaceGraphLines;
            putFeedPageCode = settings.PutFeedPageCode;
            var cutLongLines = settings.CutLongLines;
            var zoomX = settings.ZoomX;
            var zoomY = settings.ZoomY;
            this.useEscapeCodes = settings.UseEscapeCodes;
            var escapeCodesCollectionName = settings.EscapeCodesCollectionName;
            #endregion

            RichTextBox richtextForConvert = null;
            useFullTextBoxWidth = StiOptions.Export.Text.UseFullTextBoxWidth;
            useFullVerticalBorder = StiOptions.Export.Text.UseFullVerticalBorder;
            useFullHorizontalBorder = StiOptions.Export.Text.UseFullHorizontalBorder;

            //internal constants of a scale by X and Y
            //variant, when the current reports are correct.
            const double insideScaleX = 9.7;
            const double insideScaleY = 18;

            // mandatory verifications
            if (killSpaceLines == false)
                killSpaceGraphLines = false;

            if (drawBorder == false)
                killSpaceGraphLines = false;

            if (useFullVerticalBorder)
                useFullTextBoxWidth = false;

            var ms = new MemoryStream();
            var sw = encoding != null ? new StreamWriter(ms, encoding) : new StreamWriter(ms);

            StatusString = StiLocalization.Get("Export", "ExportingCreatingDocument");
            var pages = pageRange.GetSelectedPages(report.RenderedPages);

            var pageLines = 0;
            styleList = new List<StiTxtStyleInfo>();
            var baseStyle = new StiTxtStyleInfo();
            GetStyleNumber(styleList, baseStyle);
            escapeCodesList = new List<string>();

            CurrentPassNumber = 0;
            MaximumPassNumber = 3;

            #region TextExportMode Table
            var matrix = new StiMatrix(pages, false, this);
            if (IsStopped) return;

            #region calculate needBorders
            needVerticalBorders = new bool[matrix.CoordX.Count];
            for (var columnIndex = 1; columnIndex < matrix.CoordX.Count; columnIndex++)
            {
                var needVertical = false;
                for (var rowIndex = 1; rowIndex < matrix.CoordY.Count; rowIndex++)
                {
                    if (matrix.BordersY[rowIndex - 1, columnIndex] != null)     //right border 
                    {
                        needVertical = true;
                        break;
                    }
                }
                needVerticalBorders[columnIndex - 1] = needVertical;
            }

            needHorizontalBorders = new bool[matrix.CoordY.Count];
            for (var rowIndex = 1; rowIndex < matrix.CoordY.Count; rowIndex++)
            {
                var needHorizontal = false;
                for (var columnIndex = 1; columnIndex < matrix.CoordX.Count; columnIndex++)
                {
                    if (matrix.BordersX[rowIndex, columnIndex - 1] != null)     //bottom border 
                    {
                        needHorizontal = true;
                        break;
                    }
                }
                needHorizontalBorders[rowIndex - 1] = needHorizontal;
            }
            #endregion

            #region calculate default widths
            var defaultWidths = new int[matrix.CoordX.Count];
            for (var columnIndex = 1; columnIndex < matrix.CoordX.Count; columnIndex++)
            {
                var value2 = (double)matrix.CoordX.GetByIndex(columnIndex);
                var value1 = (double)matrix.CoordX.GetByIndex(columnIndex - 1);
                defaultWidths[columnIndex - 1] = (int)Math.Round((value2 - value1) / (insideScaleX / zoomX));
                if (useFullVerticalBorder && needVerticalBorders[columnIndex - 1])
                    defaultWidths[columnIndex - 1]++;
            }
            #endregion

            #region calculate cells height and width
            var readyCells = new bool[matrix.CoordY.Count, matrix.CoordX.Count];
            var widths = new int[matrix.CoordY.Count, matrix.CoordX.Count];
            var heights = new int[matrix.CoordY.Count, matrix.CoordX.Count];

            for (var rowIndex = 1; rowIndex < matrix.CoordY.Count; rowIndex++)
            {
                for (var columnIndex = 1; columnIndex < matrix.CoordX.Count; columnIndex++)
                {
                    if (!readyCells[rowIndex - 1, columnIndex - 1])
                    {
                        var cell = matrix.Cells[rowIndex - 1, columnIndex - 1];
                        widths[rowIndex - 1, columnIndex - 1] = defaultWidths[columnIndex - 1];

                        #region cells exists
                        if (cell != null)
                        {
                            #region calculate width of merged cell
                            if (cell.Width > 0)
                            {
                                for (var index = 0; index < cell.Width; index++)
                                {
                                    widths[rowIndex - 1, columnIndex - 1] += defaultWidths[columnIndex - 1 + 1 + index];
                                }
                            }
                            #endregion

                            if (cell.Component != null)
                            {
                                #region Scan tag for width
                                if (cell.Component != null)
                                {
                                    var sTag = cell.Component.TagValue as string;
                                    if (!string.IsNullOrEmpty(sTag))
                                    {
                                        sTag = sTag.Trim();

                                        var param = 0;
                                        if (int.TryParse(sTag, out param))
                                        {
                                            if (param > 0)
                                            {
                                                widths[rowIndex - 1, columnIndex - 1] = param;
                                                if (useFullVerticalBorder && needVerticalBorders[columnIndex - 1 + cell.Width])
                                                    widths[rowIndex - 1, columnIndex - 1]++;
                                            }
                                        }
                                    }
                                }
                                #endregion

                                #region Text
                                var str = cell.Text;
                                var rtf = cell.Component as StiRichText;
                                if (rtf != null && rtf.RtfText != string.Empty)
                                {
                                    if (richtextForConvert == null)
                                        richtextForConvert = new Controls.StiRichTextBox(false);

                                    rtf.GetPreparedText(richtextForConvert);
                                    str = richtextForConvert.Text;
                                }

                                var mCheckbox = cell.Component as StiCheckBox;
                                if (mCheckbox != null && mCheckbox.CheckedValue != null)
                                {
                                    var isTrue = false;
                                    var isFalse = false;

                                    if (mCheckbox.CheckedValue is bool) //for compiled reports
                                    {
                                        if ((bool)mCheckbox.CheckedValue)
                                            isTrue = true;
                                        else
                                            isFalse = true;
                                    }

                                    if (mCheckbox.CheckedValue is string)   //for reports loaded from mdc-files
                                    {
                                        if ((string)mCheckbox.CheckedValue == "True")
                                            isTrue = true;
                                        else
                                            isFalse = true;
                                    }

                                    if (isTrue)
                                        str = StiOptions.Export.Text.CheckBoxTextForTrue;

                                    if (isFalse)
                                        str = StiOptions.Export.Text.CheckBoxTextForFalse;
                                }

                                if (!cell.Component.IsExportAsImage(StiExportFormat.Text) && !string.IsNullOrEmpty(str))
                                {
                                    if (cell.Component is StiText && (cell.Component as StiText).CheckAllowHtmlTags())
                                    {
                                        var baseState = new StiTextRenderer.StiHtmlState(string.Empty);
                                        var listStates = StiTextRenderer.ParseHtmlToStates(str, baseState);
                                        var sb = new StringBuilder();
                                        foreach (var state in listStates)
                                        {
                                            sb.Append(StiTextRenderer.PrepareStateText(state.Text));
                                        }
                                        str = sb.ToString();
                                    }

                                    var stringList = StiExportUtils.SplitString(str.Replace("\r", ""), false);
                                    var arraySt = new string[stringList.Count];
                                    for (var indexLine = 0; indexLine < stringList.Count; indexLine++)
                                    {
                                        arraySt[indexLine] = stringList[indexLine];
                                    }

                                    CheckWordWrap(cell, ref arraySt, widths[rowIndex - 1, columnIndex - 1], cutLongLines);

                                    var numLines = arraySt.Length;
                                    heights[rowIndex - 1, columnIndex - 1] = numLines;
                                }
                                #endregion
                            }

                            #region Range
                            for (var yy = 0; yy <= cell.Height; yy++)
                            {
                                for (var xx = 0; xx <= cell.Width; xx++)
                                {
                                    readyCells[rowIndex - 1 + yy, columnIndex - 1 + xx] = true;
                                }
                                widths[rowIndex - 1 + yy, columnIndex - 1] = widths[rowIndex - 1, columnIndex - 1];
                            }
                            #endregion
                        }
                        #endregion
                    }
                }
            }
            #endregion

            #region calculate rows heights
            var rowHeights = new int[matrix.CoordY.Count];
            pageLines = 1;

            for (var rowIndex = 1; rowIndex < matrix.CoordY.Count; rowIndex++)
            {
                var value2 = (double)matrix.CoordY.GetByIndex(rowIndex);
                var value1 = (double)matrix.CoordY.GetByIndex(rowIndex - 1);
                var defaultHeight = (int)Math.Round((value2 - value1) / (insideScaleY / zoomY));
                if (defaultHeight < 1) defaultHeight = 1;
                for (var columnIndex = 1; columnIndex < matrix.CoordX.Count; columnIndex++)
                {
                    var cell = matrix.Cells[rowIndex - 1, columnIndex - 1];
                    if (cell != null && cell.Height < 1 && heights[rowIndex - 1, columnIndex - 1] > defaultHeight)
                        defaultHeight = heights[rowIndex - 1, columnIndex - 1];
                }

                if (useFullHorizontalBorder && needHorizontalBorders[rowIndex - 1])
                    defaultHeight++;

                rowHeights[rowIndex - 1] = defaultHeight;
                pageLines += defaultHeight;
            }
            pageLines++;
            #endregion

            var lines = new List<StringBuilder>();
            var styles = new List<StringBuilder>();
            var pageBreaks = new List<bool>();
            for (var indexLine = 0; indexLine < pageLines; indexLine++)
            {
                lines.Add(new StringBuilder());
                styles.Add(new StringBuilder());
                pageBreaks.Add(false);
            }

            #region store data
            readyCells = new bool[matrix.CoordY.Count, matrix.CoordX.Count];
            var cellPositionY = drawBorder ? 1 : 0;
            var checkboxText = new StiText
            {
                VertAlignment = StiVertAlignment.Center,
                HorAlignment = StiTextHorAlignment.Center
            };

            CurrentPassNumber = 2;
            double progressScale = Math.Max(matrix.CoordY.Count / 200f, 1f);
            int progressValue = 0;

            for (var rowIndex = 1; rowIndex < matrix.CoordY.Count; rowIndex++)
            {
                int currentProgress = (int)(rowIndex / progressScale);
                if (currentProgress > progressValue)
                {
                    progressValue = currentProgress;
                    InvokeExporting(rowIndex, matrix.CoordY.Count, CurrentPassNumber, MaximumPassNumber);
                }

                #region check pagebreaks
                for (var indexLine = 0; indexLine < matrix.HorizontalPageBreaks.Count; indexLine++)
                {
                    if ((int) matrix.HorizontalPageBreaks[indexLine] == rowIndex - 1)
                        pageBreaks[cellPositionY - (drawBorder && (rowIndex > 1) && needHorizontalBorders[rowIndex - 1 - 1] ? 1 : 0)] = true;
                }
                #endregion

                var cellPositionX = (drawBorder ? 1 : 0);
                for (var columnIndex = 1; columnIndex < matrix.CoordX.Count; columnIndex++)
                {
                    var cell = matrix.Cells[rowIndex - 1, columnIndex - 1];
                    var cellSizeX = widths[rowIndex - 1, columnIndex - 1];
                    var cellSizeY = rowHeights[rowIndex - 1];
                    if (!readyCells[rowIndex - 1, columnIndex - 1])
                    {
                        var cellWidth = 0;
                        var cellHeight = 0;
                        var cellStyleIndex = 0;

                        if (cell != null)
                        {
                            cellWidth = cell.Width;
                            cellHeight = cell.Height;

                            #region Range
                            for (var xx = 0; xx <= cell.Width; xx++)
                            {
                                for (var yy = 0; yy <= cell.Height; yy++)
                                {
                                    readyCells[rowIndex - 1 + yy, columnIndex - 1 + xx] = true;
                                }
                            }
                            #endregion

                            #region calculate merged cell sizeY
                            if (cell.Height > 0)
                            {
                                for (var index = 0; index < cell.Height; index++)
                                {
                                    cellSizeY += rowHeights[rowIndex - 1 + 1 + index];
                                }
                            }
                            #endregion

                            if (useFullHorizontalBorder && needHorizontalBorders[rowIndex - 1 + cellHeight])
                                cellSizeY--;

                            if (cell.Component != null)
                            {
                                #region Style
                                var textComp = cell.Component as StiText;
                                if (textComp != null)
                                {
                                    var styleInfo = new StiTxtStyleInfo
                                    {
                                        Bold = textComp.Font.Bold,
                                        Italic = textComp.Font.Italic,
                                        Underline = textComp.Font.Underline,
                                        Codes = null
                                    };

                                    cellStyleIndex = GetStyleNumber(styleList, styleInfo);
                                }
                                #endregion

                                #region Text
                                var str = cell.Text;
                                var rtf = cell.Component as StiRichText;
                                if (rtf != null && rtf.RtfText != string.Empty)
                                {
                                    if (richtextForConvert == null)
                                        richtextForConvert = new RichTextBox();

                                    rtf.GetPreparedText(richtextForConvert);
                                    str = richtextForConvert.Text;
                                }
                                var mCheckbox = cell.Component as StiCheckBox;
                                if (mCheckbox != null && mCheckbox.CheckedValue != null)
                                {
                                    var isTrue = false;
                                    var isFalse = false;

                                    if (mCheckbox.CheckedValue is bool) //for compiled reports
                                    {
                                        if ((bool)mCheckbox.CheckedValue)
                                            isTrue = true;
                                        else
                                            isFalse = true;
                                    }

                                    if (mCheckbox.CheckedValue is string)   //for reports loaded from mdc-files
                                    {
                                        if ((string)mCheckbox.CheckedValue == "True")
                                            isTrue = true;
                                        else
                                            isFalse = true;
                                    }

                                    if (isTrue)
                                        str = StiOptions.Export.Text.CheckBoxTextForTrue;

                                    if (isFalse)
                                        str = StiOptions.Export.Text.CheckBoxTextForFalse;
                                }

                                if (!cell.Component.IsExportAsImage(StiExportFormat.Text) && !string.IsNullOrEmpty(str))
                                {
                                    if (cell.Component is StiText && (cell.Component as StiText).CheckAllowHtmlTags())
                                    {
                                        var baseState = new StiTextRenderer.StiHtmlState(string.Empty);
                                        var listStates = StiTextRenderer.ParseHtmlToStates(str, baseState);
                                        var sb = new StringBuilder();
                                        foreach (var state in listStates)
                                        {
                                            sb.Append(StiTextRenderer.PrepareStateText(state.Text));
                                        }
                                        str = sb.ToString();
                                    }
                                    var stringList = StiExportUtils.SplitString(str.Replace("\r", ""), false);
                                    var arraySt = new string[stringList.Count];
                                    for (var indexLine = 0; indexLine < stringList.Count; indexLine++)
                                    {
                                        arraySt[indexLine] = stringList[indexLine];
                                    }

                                    CheckWordWrap(cell, ref arraySt, widths[rowIndex - 1, columnIndex - 1], cutLongLines);

                                    #region IStiVertAlignment
                                    var textOffsetY = 0;
                                    var vertAlignment = cell.Component as IStiVertAlignment;
                                    if (mCheckbox != null) vertAlignment = checkboxText;
                                    if (vertAlignment != null && arraySt.Length < cellSizeY)
                                    {
                                        if ((vertAlignment.VertAlignment & StiVertAlignment.Center) > 0)
                                            textOffsetY = (cellSizeY - arraySt.Length) / 2;

                                        if ((vertAlignment.VertAlignment & StiVertAlignment.Bottom) > 0)
                                            textOffsetY = cellSizeY - arraySt.Length;
                                    }
                                    #endregion

                                    #region store text
                                    for (var indexLine = 0; indexLine < arraySt.Length; indexLine++)
                                    {
                                        #region IStiTextHorAlignment
                                        var textOffsetX = 0;
                                        var widthOfText = cellSizeX;
                                        if (useFullVerticalBorder)
                                        {
                                            if (needVerticalBorders[cell.Left + cell.Width])
                                                widthOfText--;
                                        }
                                        else
                                        {
                                            if (!useFullTextBoxWidth && widthOfText > 1)
                                                widthOfText--;
                                        }

                                        var horAlignment = cell.Component as IStiTextHorAlignment;
                                        if (mCheckbox != null)
                                            horAlignment = checkboxText;

                                        if (horAlignment != null)
                                        {
                                            if (horAlignment.HorAlignment == StiTextHorAlignment.Center)
                                                textOffsetX = (widthOfText - arraySt[indexLine].Length) / 2;

                                            if (horAlignment.HorAlignment == StiTextHorAlignment.Right)
                                                textOffsetX = widthOfText - arraySt[indexLine].Length;
                                        }
                                        #endregion

                                        var textX = cellPositionX + textOffsetX;
                                        var textY = cellPositionY + textOffsetY + indexLine;
                                        if (textX < 0) textX = 0;
                                        var lineEndPos = textX + arraySt[indexLine].Length;

                                        CheckGrow(lines, styles, pageBreaks, textY);

                                        if (lines[textY].Length < lineEndPos)
                                            lines[textY].Append(' ', lineEndPos - lines[textY].Length);

                                        for (var indexChar = 0; indexChar < arraySt[indexLine].Length; indexChar++)
                                        {
                                            var sym = (int)arraySt[indexLine][indexChar];
                                            if (sym < 32) sym += 0xFF00;
                                            lines[textY][textX + indexChar] = (char)sym;
                                        }
                                    }
                                    #endregion

                                }
                                #endregion
                            }
                        }
                        else
                        {
                            if (useFullHorizontalBorder && needHorizontalBorders[rowIndex - 1])
                                cellSizeY--;
                        }

                        if (!useFullHorizontalBorder)
                            cellSizeY--;

                        for (var tempIndexY = 0; tempIndexY < cellSizeY; tempIndexY++)
                        {
                            if (styles[cellPositionY + tempIndexY].Length < cellPositionX + cellSizeX)
                                styles[cellPositionY + tempIndexY].Append('\x00', cellPositionX + cellSizeX - styles[cellPositionY + tempIndexY].Length);

                            var widthOfText = cellSizeX;
                            if (useFullVerticalBorder)
                            {
                                if (cell != null && needVerticalBorders[cell.Left + cell.Width])
                                    widthOfText--;
                            }
                            else
                            {
                                if (!useFullTextBoxWidth && widthOfText > 1)
                                    widthOfText--;
                            }

                            LineFillChar(styles[cellPositionY + tempIndexY], cellStyleIndex, cellPositionX, widthOfText);
                        }

                        #region Render Borders
                        if (drawBorder)
                        {
                            var lineEndPos = cellPositionX + cellSizeX - 1;
                            for (var index = -1; index <= cellSizeY; index++)
                            {
                                CheckGrow(lines, styles, pageBreaks, cellPositionY + index);

                                if (lines[cellPositionY + index].Length < lineEndPos + 1)
                                    lines[cellPositionY + index].Append(' ', lineEndPos + 1 - lines[cellPositionY + index].Length);
                            }

                            #region Check borders
                            var needBorderLeft = true;
                            var needBorderRight = true;
                            for (var index = 0; index < cellHeight + 1; index++)
                            {
                                if (matrix.BordersY[rowIndex - 1 + index, columnIndex - 1] == null)
                                    needBorderLeft = false;

                                if (matrix.BordersY[rowIndex - 1 + index, columnIndex - 1 + cellWidth + 1] == null)
                                    needBorderRight = false;
                            }
                            var needBorderTop = true;
                            var needBorderBottom = true;
                            for (var index = 0; index < cellWidth + 1; index++)
                            {
                                if (matrix.BordersX[rowIndex - 1, columnIndex - 1 + index] == null)
                                    needBorderTop = false;

                                if (matrix.BordersX[rowIndex - 1 + cellHeight + 1, columnIndex - 1 + index] == null)
                                    needBorderBottom = false;
                            }
                            #endregion

                            #region Top
                            if (needBorderTop)
                            {
                                LineFill(lines[cellPositionY - 1], 14, cellPositionX - 1, 1);
                                LineFill(lines[cellPositionY - 1], 1, cellPositionX, cellSizeX - 1);
                                LineFill(lines[cellPositionY - 1], 13, cellPositionX + cellSizeX - 1, 1);
                            }
                            #endregion

                            #region Left
                            if (needBorderLeft)
                            {
                                LineFill(lines[cellPositionY - 1], 15, cellPositionX - 1, 1);
                                for (var index = 0; index < cellSizeY; index++)
                                {
                                    LineFill(lines[cellPositionY + index], 2, cellPositionX - 1, 1);
                                }

                                LineFill(lines[cellPositionY + cellSizeY], 12, cellPositionX - 1, 1);
                            }
                            #endregion

                            #region Right
                            if (needBorderRight)
                            {
                                LineFill(lines[cellPositionY - 1], 15, cellPositionX + cellSizeX - 1, 1);
                                for (var index = 0; index < cellSizeY; index++)
                                {
                                    LineFill(lines[cellPositionY + index], 2, cellPositionX + cellSizeX - 1, 1);
                                }

                                LineFill(lines[cellPositionY + cellSizeY], 12, cellPositionX + cellSizeX - 1, 1);
                            }
                            #endregion

                            #region Bottom
                            if (needBorderBottom)
                            {
                                LineFill(lines[cellPositionY + cellSizeY], 14, cellPositionX - 1, 1);
                                LineFill(lines[cellPositionY + cellSizeY], 1, cellPositionX, cellSizeX - 1);
                                LineFill(lines[cellPositionY + cellSizeY], 13, cellPositionX + cellSizeX - 1, 1);
                            }
                            #endregion
                        }
                        #endregion
                    }
                    cellPositionX += cellSizeX;
                }
                cellPositionY += rowHeights[rowIndex - 1];
            }
            #endregion

            #region Check if first column is empty
            if (drawBorder)
            {
                var isFirstColumnEmpty = true;
                for (var indexLine = 0; indexLine < lines.Count; indexLine++)
                {
                    if (lines[indexLine].Length > 0 && lines[indexLine][0] != ' ')
                    {
                        isFirstColumnEmpty = false;
                        break;
                    }
                }

                if (isFirstColumnEmpty)
                {
                    for (var indexLine = 0; indexLine < lines.Count; indexLine++)
                    {
                        if (lines[indexLine].Length > 0)
                            lines[indexLine].Remove(0, 1);

                        if (styles[indexLine].Length > 0)
                            styles[indexLine].Remove(0, 1);
                    }
                }
            }
            #endregion

            #region Trial
        #if SERVER
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
                double len = ((double)matrix.CoordX.GetByIndex(matrix.CoordX.Count - 1) - (double)matrix.CoordX.GetByIndex(0)) / (insideScaleX / zoomX);
                int offset = Math.Max(1, (int)(len / 2 - 10));
                sw.WriteLine(new string(' ', offset) + "---=== TRIAL ===---\r\n");
            }
            #endregion

            #region Optimize & save page
            var lastLine = lines.Count - 1;
            while (lastLine > 0 && lines[lastLine].Length == 0) lastLine--;

            var lastStyleNumber = 0;
            for (var indexLine = 0; indexLine < lines.Count; indexLine++)
            {
                if (lines[indexLine].Length > 0)
                {
                    #region Replace border codes with border symbols
                    for (var index = 0; index < borderCodes.Length; index++)
                        lines[indexLine].Replace(borderCodes[index], GetBorderChar(index, borderType));
                    #endregion

                    #region Kill space at end of line
                    if (StiOptions.Export.Text.TrimTrailingSpaces)
                    {
                        for (var index = lines[indexLine].Length - 1; index >= 0; index--)
                        {
                            if (lines[indexLine][index] != ' ') break;
                            lines[indexLine].Length--;
                        }
                    }
                    #endregion

                    #region killSpaceGraphLines
                    if (killSpaceGraphLines)
                    {
                        var st = GetBorderChar(1, borderType) + " ";
                        var bb = true;
                        for (var index = 0; index < lines[indexLine].Length; index++)
                        {
                            if (st.IndexOf(lines[indexLine][index]) == -1)
                                bb = false;
                        }
                        if (bb)
                            lines[indexLine].Length = 0;
                    }
                    #endregion

                    if (putFeedPageCode && pageBreaks[indexLine])
                    {
                        if (useEscapeCodes)
                            sw.Write(GetEscapeNames(lastStyleNumber, 0));

                        lastStyleNumber = 0;

                        sw.Write("\x0c");
                    }

                    if (useEscapeCodes)
                    {
                        #region Add escape codes
                        var sbt = new StringBuilder();
                        for (var indexChar = 0; indexChar < lines[indexLine].Length; indexChar++)
                        {
                            var currentStyleNumber = 0;
                            if (indexChar < styles[indexLine].Length) currentStyleNumber = styles[indexLine][indexChar];
                            if (currentStyleNumber != lastStyleNumber)
                            {
                                sbt.Append(GetEscapeNames(lastStyleNumber, currentStyleNumber));
                                lastStyleNumber = currentStyleNumber;
                            }

                            var ch = lines[indexLine][indexChar];
                            if (ch > firstEscapeCodeIndex - escapeCodesList.Count - 2 && ch <= firstEscapeCodeIndex)
                            {
                                var escapeStringIndex = firstEscapeCodeIndex - ch;
                                var escapeString = escapeCodesList[escapeStringIndex - 1];
                                if (escapeString[escapeString.Length - 1] == 0xFFFF)
                                    escapeString = escapeString.Substring(0, escapeString.Length - 1); // + ' ';    //fix 2015.12.15

                                sbt.Append(escapeString);
                            }
                            else
                                sbt.Append(ch);
                        }

                        if (sbt.Length > lines[indexLine].Length)
                            lines[indexLine] = sbt;
                        #endregion
                    }
                    else
                    {
                        #region Check RTL marks
                        var sbt = new StringBuilder();
                        for (var indexChar = 0; indexChar < lines[indexLine].Length; indexChar++)
                        {
                            var ch = lines[indexLine][indexChar];
                            if (ch > firstEscapeCodeIndex - escapeCodesList.Count - 2 && ch <= firstEscapeCodeIndex)
                            {
                                var escapeStringIndex = firstEscapeCodeIndex - ch;
                                var escapeString = escapeCodesList[escapeStringIndex - 1];
                                sbt.Append(escapeString);
                            }
                            else
                                sbt.Append(ch);
                        }

                        if (sbt.Length > lines[indexLine].Length)
                            lines[indexLine] = sbt;
                        #endregion
                    }
                }

                #region killSpaceLines & Save Line
                bool killSpaceLines2 = killSpaceLines || (indexLine + 1 < lines.Count && putFeedPageCode && pageBreaks[indexLine + 1]);
                if ((!killSpaceLines2 && indexLine <= lastLine) || lines[indexLine].Length > 0)
                {
                    for (var indexSym = 0; indexSym < lines[indexLine].Length; indexSym++)
                    {
                        if (lines[indexLine][indexSym] >= 0xFF00)
                            lines[indexLine][indexSym] = (char) (lines[indexLine][indexSym] - 0xFF00);
                    }

                    if (StiOptions.Export.Text.RemoveLastNewLineMarker && (indexLine == lastLine))
                        sw.Write(lines[indexLine]);

                    else
                        sw.WriteLine(lines[indexLine]);
                }
                #endregion
            }
            var sbb = new StringBuilder();
            if (useEscapeCodes)
                sbb.Append(GetEscapeNames(lastStyleNumber, 0));

            if (putFeedPageCode)
                sbb.Append('\x0c');
            
            if (sbb.Length > 0)
                sw.Write(sbb);
            #endregion

            #endregion

            sw.Flush();

            if (useEscapeCodes)
            {
                #region Convert escape codes
                ms.Flush();
                ms.Seek(0, SeekOrigin.Begin);
                var buf = ms.ToArray();
                var sb = new StringBuilder();
                foreach (var bb in buf)
                {
                    sb.Append((char)bb);
                }
                StiEscapeCodesCollection.GetEscapeCodesCollection(escapeCodesCollectionName)["ltrMark"] = ltrMark;
                var result = StiEscapeCodesCollection.ConvertEscapeCodes(sb.ToString(), escapeCodesCollectionName);
                ms = new MemoryStream();
                foreach (var ch in result)
                {
                    ms.WriteByte((byte)ch);
                }
                #endregion
            }
            ms.WriteTo(stream);
            ms.Close();

            #region Clear
            if (richtextForConvert != null)
                richtextForConvert.Dispose();

            needVerticalBorders = null;
            needHorizontalBorders = null;
            styleList = null;
            escapeCodesList = null;
            #endregion
        }

        #endregion
    }
}
