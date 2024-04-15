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

using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Services;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Components.ShapeTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using Stimulsoft.Report.Helpers;

#if STIDRAWING
using ImageEncoder = Stimulsoft.Drawing.Imaging.Encoder;
using Image = Stimulsoft.Drawing.Image;
using Font = Stimulsoft.Drawing.Font;
using ImageCodecInfo = Stimulsoft.Drawing.Imaging.ImageCodecInfo;
using ImageFormat = Stimulsoft.Drawing.Imaging.ImageFormat;
using EncoderParameter = Stimulsoft.Drawing.Imaging.EncoderParameter;
using EncoderParameters = Stimulsoft.Drawing.Imaging.EncoderParameters;
#else
using ImageEncoder = System.Drawing.Imaging.Encoder;
#endif

namespace Stimulsoft.Report.Export
{
    /// <summary>
    /// A class for the RTF export.
    /// </summary>
    [StiServiceBitmap(typeof(StiExportService), "Stimulsoft.Report.Images.Dictionary.ResourceRtf.png")]
    public class StiRtfExportService : StiExportService
    {
        #region StiExportService override
        /// <summary>
		/// Gets or sets a default extension of export. 
		/// </summary>
		public override string DefaultExtension
        {
            get
            {
                return "rtf";
            }
        }

        public override StiExportFormat ExportFormat
        {
            get
            {
                return StiExportFormat.Rtf;
            }
        }

        /// <summary>
        /// Gets a group of the export in the context menu.
        /// </summary>
        public override string GroupCategory
        {
            get
            {
                return "Word";
            }
        }

        /// <summary>
        /// Gets a position of the export in the context menu.
        /// </summary>
        public override int Position
        {
            get
            {
                return (int)StiExportPosition.Rtf;
            }
        }

        /// <summary>
        /// Gets a name of the export in the context menu.
        /// </summary>
        public override string ExportNameInMenu
        {
            get
            {
                return StiLocalization.Get("Export", "ExportTypeRtfFile");
            }
        }

        /// <summary>
        /// Returns a filter for the Rtf files.
        /// </summary>
        /// <returns>Returns a filter for the Rtf files.</returns>
        public override string GetFilter()
        {
            return StiLocalization.Get("FileFilters", "RtfFiles");
        }

        /// <summary>
        /// Exports a document to the stream without dialog of the saving file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream in which report will be exported.</param>
        /// <param name="settings">A settings for the report exporting.</param>
        public override void ExportTo(StiReport report, Stream stream, StiExportSettings settings)
        {
            ExportRtf(report, stream, settings as StiRtfExportSettings);
            InvokeExporting(100, 100, 1, 1);
        }

        /// <summary>
        /// Exports  rendered report to the RTF file.
        /// Also file may be sent via e-mail.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="fileName">A name of the file for exporting a rendered report.</param>
        /// <param name="sendEMail">A parameter indicating whether the exported report will be sent via e-mail.</param>
        public override void Export(StiReport report, string fileName, bool sendEMail, StiGuiMode guiMode)
        {
            using (var form = StiGuiOptions.GetExportFormRunner("StiRtfExportSetupForm", guiMode, this.OwnerWindow))
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

        private StiReport report;
        private string fileName;
        private bool sendEMail;
        private StiGuiMode guiMode;
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

                            var settings = new StiRtfExportSettings
                            {
                                PageRange = form["PagesRange"] as StiPagesRange,
                                ExportMode = (StiRtfExportMode)form["ExportMode"],
                                UsePageHeadersAndFooters = (bool)form["UsePageHeadersAndFooters"],
                                ImageQuality = (float)form["ImageQuality"],
                                ImageResolution = (float)form["Resolution"],
                                RemoveEmptySpaceAtBottom = (bool)form["RemoveEmptySpaceAtBottom"]
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
        /// Gets a value indicating a number of files in the exported document as a result of export
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

        #region this
        #region struct StiRtfData
        /// <summary>
        /// Internal representation of printing objects
        /// </summary>
        private struct StiRtfData
        {
            /// <summary>
            /// A coordinate of the X frame.
            /// </summary>
            public int X;

            /// <summary>
            /// A coordinate of the Y frame.
            /// </summary>
            public int Y;

            /// <summary>
            /// A Border width.
            /// </summary>
            public int Width;

            /// <summary>
            /// A Border height.
            /// </summary>
            public int Height;

            /// <summary>
            /// A Component.
            /// </summary>
            public StiComponent Component;
        }
        #endregion

        #region struct StiRtfStyleInfo
        private struct StiRtfStyleInfo
        {
            public string Name;
            public StiTextHorAlignment Alignment;
            public bool RightToLeft;
            public int FontNumber;
            public int FontSize;
            public bool Bold;
            public bool Italic;
            public bool Underline;
            public int TextColor;
        }
        #endregion

        #region GetColorNumber
        /// <summary>
        /// Returns the color number in the color palette.
        /// </summary>
        /// <param name="tmpColorList">A color table</param>
        /// <param name="incomingColor">Processed color</param>
        /// <returns>Color number</returns>
        private int GetColorNumberInt(ArrayList tmpColorList, Color incomingColor)
        {
            if (tmpColorList.Count > 0)
            {
                for (int index = 0; index < tmpColorList.Count; index++)
                {
                    if ((Color)tmpColorList[index] == incomingColor)
                    {
                        //color is already in table, return color number
                        //						return index.ToString();
                        return index;
                    }
                }
            }
            //add color to table, return color number
            tmpColorList.Add(incomingColor);
            int temp = tmpColorList.Count - 1;
            //			return temp.ToString();
            return temp;
        }

        private string GetColorNumber(ArrayList tmpColorList, Color incomingColor)
        {
            return GetColorNumberInt(tmpColorList, incomingColor).ToString();
        }
        #endregion

        #region GetFontNumber
        /// <summary>
        /// Returns font number in font table.
        /// </summary>
        /// <param name="tmpFontList">A table of fonts</param>
        /// <param name="incomingFont">Processed font</param>
        /// <returns>Font number</returns>
        //		private string GetFontNumber(ArrayList tmpFontList, Font incomingFont)
        private int GetFontNumber(ArrayList tmpFontList, Font incomingFont)
        {
            if (tmpFontList.Count > 0)
            {
                for (int index = 0; index < tmpFontList.Count; index++)
                {
                    Font tmpFont = (Font)tmpFontList[index];
                    if (tmpFont.Name == incomingFont.Name)
                    {
                        //font is already in table, return font number
                        return index * (charsetCount == 0 ? (byte)1 : charsetCount);
                    }
                }
            }
            //add font to table, return font number
            tmpFontList.Add(incomingFont);
            int temp = tmpFontList.Count - 1;
            return temp * (charsetCount == 0 ? (byte)1 : charsetCount);
        }

        private int GetFontNumber(ArrayList tmpFontList, Font incomingFont, int charset)
        {
            if (tmpFontList.Count > 0)
            {
                for (int index = 0; index < tmpFontList.Count; index++)
                {
                    Font tmpFont = (Font)tmpFontList[index];
                    if (tmpFont.Name == incomingFont.Name)
                    {
                        //font is already in table, return font number
                        return index * (charsetCount == 0 ? (byte)1 : charsetCount) + GetCharsetIndex(charset);
                    }
                }
            }
            //add font to table, return font number
            tmpFontList.Add(incomingFont);
            int temp = tmpFontList.Count - 1;
            return temp * (charsetCount == 0 ? (byte)1 : charsetCount) + GetCharsetIndex(charset);
        }

        private int GetCharsetIndex(int charset)
        {
            int charsetIndex = 0;
            for (int index = 0; index < StiEncode.codePagesTableSize; index++)
            {
                if (StiEncode.codePagesTable[index, 2] == charset)
                {
                    charsetIndex = index + 1;
                    break;
                }
            }
            if (charsetCount == 0)
            {
                codePageToFont[charsetIndex] = 1;
                return 0;
            }
            return codePageToFont[charsetIndex];
        }

        private int GetFontNumber(ArrayList tmpFontList, string fontName)
        {
            if (tmpFontList.Count > 0)
            {
                for (int index = 0; index < tmpFontList.Count; index++)
                {
                    Font tmpFont = (Font)tmpFontList[index];
                    if (tmpFont.Name == fontName)
                    {
                        //font is already in table, return font number
                        return index * (charsetCount == 0 ? (byte)1 : charsetCount);
                    }
                }
            }
            //add font to table, return font number
            tmpFontList.Add(new Font(StiFontCollection.GetFontFamily(fontName), 8));
            int temp = tmpFontList.Count - 1;
            return temp * (charsetCount == 0 ? (byte)1 : charsetCount);
        }
        #endregion

        #region GetStyleNumber
        private int GetStyleNumber(ArrayList tmpStyleList, StiRtfStyleInfo styleInfo)
        {
            if (tmpStyleList.Count > 0)
            {
                for (int index = 0; index < tmpStyleList.Count; index++)
                {
                    StiRtfStyleInfo tmpStyle = (StiRtfStyleInfo)tmpStyleList[index];
                    if ((tmpStyle.Alignment == styleInfo.Alignment) &&
                        (tmpStyle.Name == styleInfo.Name) &&
                        (tmpStyle.FontNumber == styleInfo.FontNumber) &&
                        (tmpStyle.FontSize == styleInfo.FontSize) &&
                        (tmpStyle.Bold == styleInfo.Bold) &&
                        (tmpStyle.Italic == styleInfo.Italic) &&
                        (tmpStyle.Underline == styleInfo.Underline) &&
                        (tmpStyle.TextColor == styleInfo.TextColor) &&
                        (tmpStyle.RightToLeft == styleInfo.RightToLeft))
                    {
                        //style is already in table, return number
                        return index + 1;
                    }
                }
            }
            //add style to table, return style number
            tmpStyleList.Add(styleInfo);
            int temp = tmpStyleList.Count - 1;
            return temp + 1;
        }
        #endregion

        #region GetLineStyle
        private string GetLineStyle(StiBorderSide border, ArrayList colorList)
        {
            StringBuilder sb = new StringBuilder();
            if ((border != null) && (border.Style != StiPenStyle.None))
            {
                switch (border.Style)
                {
                    case StiPenStyle.Solid:
                        sb.Append("\\brdrs");
                        break;

                    case StiPenStyle.Dot:
                        sb.Append("\\brdrdot");
                        break;

                    case StiPenStyle.Dash:
                        sb.Append("\\brdrdash");
                        break;

                    case StiPenStyle.DashDot:
                        sb.Append("\\brdrdashd");
                        break;

                    case StiPenStyle.DashDotDot:
                        sb.Append("\\brdrdashdd");
                        break;

                    case StiPenStyle.Double:
                        sb.Append("\\brdrdb");
                        break;
                }
                sb.Append("\\brdrw");
                sb.Append((int)(border.Size * 15));
                sb.Append(string.Format("\\brdrcf{0}", GetColorNumber(colorList, border.Color)));
            }
            return sb.ToString();
        }
        #endregion

        private StreamWriter sw = null;
        private ArrayList colorList = null;
        private ArrayList fontList = null;
        private ArrayList styleList = null;
        private int[] unicodeMapArray = null;
        private byte[] codePageToFont = null;
        private byte charsetCount = 0;
        private int[] fontToCodePages = null;
        private int baseFontNumber = 0;
        private Stream sw2 = null;
        private bool usePageHeadersAndFooters = false;
        private float imageResolution = 0.96f;  //dpi
        private float imageQuality = 0.75f;
        private StiExportFormat imageFormat = StiExportFormat.Rtf;
        private ImageCodecInfo imageCodec = null;
        private bool useStyles = false;
        private Hashtable bookmarkList = null;
        private Hashtable usedBookmarks = null;

        private bool removeEmptySpaceAtBottom = StiOptions.Export.RichText.RemoveEmptySpaceAtBottom;
        internal bool RemoveEmptySpaceAtBottom
        {
            get
            {
                return removeEmptySpaceAtBottom;
            }
        }


        //conversion of hundredths of inch to twips
        const double HiToTwips = 14.4;

        const int frameCorrectValue = 38;

        private int pageHeight;
        private int pageWidth;

        private StiMatrix matrix = null;
        internal StiMatrix Matrix
        {
            get
            {
                return matrix;
            }
        }

        private string ConvertStringToBookmark(string inputString)
        {
            StringBuilder sbOutput = new StringBuilder();
            foreach (char ch in inputString)
            {
                if (char.IsLetterOrDigit(ch)) sbOutput.Append(ch);
            }
            return sbOutput.ToString();
        }

        private bool CompareExcellSheetNames(string name1, string name2)
        {
            string st1 = name1;
            if (string.IsNullOrEmpty(st1)) st1 = string.Empty;
            string st2 = name2;
            if (string.IsNullOrEmpty(st2)) st2 = string.Empty;
            return (st1 == st2);
        }

        #region ConvertTextWithHtmlTagsToRtfText
        private string ConvertTextWithHtmlTagsToRtfText(StiText stiText, string text)
        {
            string inputText = text;
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

            baseFontNumber = GetFontNumber(fontList, stiText.Font);

            //StringBuilder finalText = new StringBuilder();
            var finalText = new StringBuilder(GetLineHeightInTwips(stiText.LineSpacing));
            StiTextRenderer.StiHtmlTagsState prevState = baseTagsState;
            for (int index = 0; index < statesList.Count; index++)
            {
                StiTextRenderer.StiHtmlState htmlState = statesList[index];
                StiTextRenderer.StiHtmlTagsState state = htmlState.TS;

                var outputText = new StringBuilder();
                string storedTextAlign = null;

                if (StiOptions.Engine.Html.AllowListItemSecondLineIndent && (state.Indent > 0) && (index > 0) && (statesList[index - 1].Text.ToString() == "\x0A"))
                {
                    int indent = (int)(20 + 53 * state.FontSize * state.Indent);
                    outputText.AppendFormat("\\fi-{0}\\li{0}", indent);
                }

                if (state.Bold != prevState.Bold)
                {
                    outputText.Append(state.Bold ? "\\b" : "\\b0");
                }
                if (state.Italic != prevState.Italic)
                {
                    outputText.Append(state.Italic ? "\\i" : "\\i0");
                }
                if (state.Underline != prevState.Underline)
                {
                    outputText.Append(state.Underline ? "\\ul" : "\\ul0");
                }
                if (state.Strikeout != prevState.Strikeout)
                {
                    outputText.Append(state.Strikeout ? "\\strike" : "\\strike0");
                }
                if (state.Superscript != prevState.Superscript)
                {
                    outputText.Append(state.Superscript ? "\\super" : "\\nosupersub");
                }
                if (state.Subscript != prevState.Subscript)
                {
                    outputText.Append(state.Subscript ? "\\sub" : "\\nosupersub");
                }
                if (state.FontColor != prevState.FontColor)
                {
                    outputText.Append(string.Format("\\cf{0}", GetColorNumberInt(colorList, state.FontColor)));
                }
                if (state.BackColor != prevState.BackColor)
                {
                    //for old version of MS-Word
                    //outputText.Append(string.Format("\\highlight{0}", GetColorNumber(colorList, state.BackColor)));

                    //for new version of MS-Word
                    outputText.Append(string.Format("\\chshdng0\\chcbpat{0}", GetColorNumber(colorList, state.BackColor)));
                }
                if (state.FontSize != prevState.FontSize)
                {
                    outputText.Append(string.Format("\\fs{0}", (int)(state.FontSize * 2)));
                }
                if (state.LetterSpacing != prevState.LetterSpacing)
                {
                    outputText.Append(string.Format("\\expnd{0}", (int)(state.LetterSpacing * state.FontSize * 3.8 + StiOptions.Export.RichText.SpaceBetweenCharacters)));
                }
                //if (state.WordSpacing != baseTagsState.WordSpacing)
                //{
                //    fontStyle.Append(string.Format("word-spacing:{0}em;", state.WordSpacing).Replace(",", "."));
                //}
                if (state.LineHeight != prevState.LineHeight)
                {
                    outputText.Append(GetLineHeightInTwips(state.LineHeight));
                }
                if (state.TextAlign != prevState.TextAlign)
                {
                    string textAlign = "\\ql";
                    if (state.TextAlign == StiTextHorAlignment.Center) textAlign = "\\qc";
                    if (state.TextAlign == StiTextHorAlignment.Right) textAlign = "\\qr";
                    if (state.TextAlign == StiTextHorAlignment.Width) textAlign = "\\qj";
                    if (htmlState.Text.ToString() == "\x0A")
                    {
                        storedTextAlign = textAlign;
                    }
                    else
                    {
                        outputText.Append(textAlign);
                    }
                }
                if (state.FontName != prevState.FontName)
                {
                    baseFontNumber = GetFontNumber(fontList, state.FontName);
                    //outputText.Append(string.Format("\\f{0}", GetFontNumber(fontList, state.FontName)));
                    outputText.Append(string.Format("{0}{1}", '\x10', (char)(256 + baseFontNumber)));
                }

                if (outputText.Length > 0)
                {
                    finalText.Append(outputText.Replace("\\", "\x17"));
                    finalText.Append(" ");
                }

                if (htmlState.Text.ToString() == "\x0A")
                {
                    finalText.Append("\x0a");
                    if (storedTextAlign != null) finalText.Append(storedTextAlign.Replace("\\", "\x17"));
                }
                else
                {
                    char lastChar = finalText[finalText.Length - 1];
                    if (!(lastChar == ' ' || lastChar == '\x0A' || lastChar == '?' || lastChar == '\xA0')) finalText.Append(" ");
                    finalText.Append(StiTextRenderer.PrepareStateText(htmlState.Text));
                }

                prevState = state;
            }

            return finalText.ToString();
        }

        private static string GetLineHeightInTwips(double lineHeightScale)
        {
            //const double correctLineHeight = 0.98;
            double correctLineHeight = StiOptions.Export.RichText.LineSpacing;
            return string.Format("{0}sl{1}{0}slmult1", "\x17", Math.Round(240 * correctLineHeight * lineHeightScale));
        }

        internal static string GetRtfFileFromHtmlTags(StiText stiText, string text)
        {
            StiText clone = stiText.Clone(true) as StiText;
            clone.Border = new StiBorder();
            clone.Left = 0;
            clone.Top = 0;
            clone.Text = new StiExpression(text);
            clone.TagValue = "rtfparagraph";

            MemoryStream ms = new MemoryStream();
            StiReport tempReport = new StiReport();
            StiPage page = new StiPage(tempReport);
            page.Components.Add(clone);
            tempReport.RenderedPages.Clear();
            tempReport.RenderedPages.AddV2Internal(page);

            StiRtfExportService service = new StiRtfExportService();
            service.ExportRtf(tempReport, ms, StiRtfExportMode.Table);
            ms.Seek(0, SeekOrigin.Begin);
            StreamReader sr = new StreamReader(ms, Encoding.GetEncoding(1252));
            string st = sr.ReadToEnd();

            return st;
        }
        #endregion

        #region GetNearestHighlightColorNumber
        //may be usefull for backward compability,
        //but not necessary for MS-Word XP and later
        //
        //private int GetNearestHighlightColorNumber(Color incomingColor)
        //{
        //    int colorNumber = 0;
        //    int colorDifference = 0xFFFFFF;
        //    for (int index = 0; index < highlightColorsTable.Length; index++)
        //    {
        //        int currentColor = highlightColorsTable[index];
        //        int dr = Math.Abs(incomingColor.R - ((currentColor >> 16) & 0xFF));
        //        int dg = Math.Abs(incomingColor.G - ((currentColor >> 8) & 0xFF));
        //        int db = Math.Abs(incomingColor.B - ((currentColor) & 0xFF));
        //        int diff = dr * dr + dg * dg + db * db;
        //        if (diff < colorDifference)
        //        {
        //            colorNumber = index;
        //            colorDifference = diff;
        //        }
        //    }
        //    if (colorNumber == 7) colorNumber = 0;
        //    return colorNumber + 1;
        //}

        //private static int[] highlightColorsTable =
        //{
        //    0x000000,   //black
        //    0x0000FF,   //blue
        //    0x00FFFF,   //cyan
        //    0x00FF00,   //green
        //    0xFF00FF,   //magenta
        //    0xFF0000,   //red
        //    0xFFFF00,   //yellow
        //    0x000000,   //unused
        //    0x000080,   //dark blue
        //    0x008080,   //dark cyan
        //    0x008000,   //dark green
        //    0x800080,   //dark magenta
        //    0x800000,   //dark red
        //    0x808000,   //dark yellow
        //    0x808080,   //dark grey
        //    0xC0C0C0    //light gray
        //};
        #endregion


        #region ReplacePardInRtf
        //keyword "intbl" should not appear inside "\shp" object, otherwise MS-Word cannot open this RTF-file
        private string ReplacePardInRtf(string st)
        {
            StringBuilder sb = new StringBuilder();
            int counter = 0;
            int limit = 0;
            bool waitLimit = false;
            int pos = 0;
            while (pos < st.Length)
            {
                char ch = st[pos++];
                sb.Append(ch);
                if (ch == '{')
                {
                    counter++;
                    continue;
                }
                if (ch == '}')
                {
                    counter--;
                    if (counter < limit)
                    {
                        waitLimit = false;
                    }
                    continue;
                }
                if (ch == '\\')
                {
                    if ((pos < st.Length) && ((st[pos] == '{') || (st[pos] == '}')))
                    {
                        pos++;
                        continue;
                    }
                    if ((pos + 2 < st.Length) && (st[pos] == 's') && (st[pos + 1] == 'h') && (st[pos + 2] == 'p') && (st[pos - 2] == '{'))
                    {
                        waitLimit = true;
                        limit = counter;
                    }
                    if (!waitLimit && (pos + 3 < st.Length) && (st[pos] == 'p') && (st[pos + 1] == 'a') && (st[pos + 2] == 'r') && (st[pos + 3] == 'd'))
                    {
                        pos += 4;
                        sb.Append("pard\\intbl");
                    }
                }
            }
            return sb.ToString();
        }
        #endregion

        #region GetImageString
        private string GetImageString(Image image, float zoom, int absw, int absh)
        {
            MemoryStream memw = new MemoryStream();
            string blipName = "jpeg";

            if (imageFormat == StiExportFormat.ImagePng)
            {
                blipName = "png";
                image.Save(memw, ImageFormat.Png);
            }
            else
            {
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
            }

            byte[] bytes = memw.ToArray();
            memw.Close();
            StringBuilder sb = new StringBuilder(((bytes.Length * 2) + 200));

            float zoom2 = zoom / imageResolution;

            sb.Append("{" + string.Format("\\pict\\picscalex{0}\\picscaley{1}\\picwgoal{2}\\pichgoal{3}\\{4}blip ",
                //(int)(100 / zoom2),	//corrected 2008.03.03
                //(int)(100 / zoom2),	//corrected 2008.03.03
                100,                    //corrected 2008.03.03
                100,                    //corrected 2008.03.03
                                        //absw * zoom,
                                        //absh * zoom));
                absw,
                absh,
                blipName));

            for (int index = 0; index < bytes.Length; index++)
            {
                sb.Append(bytes[index].ToString("x2"));
            }
            sb.Append("}");
            return sb.ToString();
        }
        #endregion

        #region DrawLine
        private void DrawLine(int tX1, int tY1, int tX2, int tY2, Color tColor, string stBorderWidth)
        {
            if (tColor.A != 0)
            {
                sw.Write("{\\shp{\\*");
                sw.Write("\\shpinst\\shpleft{0}\\shptop{1}\\shpright{2}\\shpbottom{3}",
                    (tX1 < tX2 ? tX1 : tX2), (tY1 < tY2 ? tY1 : tY2), (tX1 > tX2 ? tX1 : tX2), (tY1 > tY2 ? tY1 : tY2));
                sw.Write("\\shpwr3");
                sw.Write("{\\sp{\\sn shapeType}{\\sv 20}}");
                sw.Write("{\\sp{\\sn fFlipH}{\\sv " + (tX1 < tX2 ? "0" : "1") + "}}");
                sw.Write("{\\sp{\\sn fFlipV}{\\sv " + (tY1 < tY2 ? "0" : "1") + "}}");
                sw.Write("{\\sp{\\sn fFilled}{\\sv 0}}");
                sw.Write("{\\sp{\\sn lineColor}{\\sv " + string.Format("{0}", tColor.B * 65536 + tColor.G * 256 + tColor.R) + "}}");
                sw.Write("{\\sp{\\sn lineWidth}{\\sv " + stBorderWidth + "}}");
                sw.Write("{\\sp{\\sn fLine}{\\sv 1}}");
                sw.WriteLine("}}");
            }
        }
        #endregion

        #region FillRect
        private void FillRect(StiRtfData pp, Color tColor)
        {
            if (tColor.A != 0)
            {
                sw.Write("{\\shp{\\*");
                sw.Write("\\shpinst\\shpleft{0}\\shptop{1}\\shpright{2}\\shpbottom{3}",
                    pp.X, pp.Y, pp.X + pp.Width, pp.Y + pp.Height);
                sw.Write("\\shpwr3");
                sw.Write("{\\sp{\\sn shapeType}{\\sv 1}}");
                sw.Write("{\\sp{\\sn fFlipH}{\\sv 0}}");
                sw.Write("{\\sp{\\sn fFlipV}{\\sv 0}}");
                sw.Write("{\\sp{\\sn fillColor}{\\sv " + string.Format("{0}", tColor.B * 65536 + tColor.G * 256 + tColor.R) + "}}");
                sw.Write("{\\sp{\\sn fFilled}{\\sv 1}}");
                sw.Write("{\\sp{\\sn fLine}{\\sv 0}}");
                sw.WriteLine("}}");
            }
        }
        #endregion

        #region CheckShape1
        private bool CheckShape1(StiShape shape)
        {
            if (
                (shape.ShapeType is StiVerticalLineShapeType) ||
                (shape.ShapeType is StiHorizontalLineShapeType) ||
                (shape.ShapeType is StiTopAndBottomLineShapeType) ||
                (shape.ShapeType is StiLeftAndRightLineShapeType) ||
                (shape.ShapeType is StiRectangleShapeType)
                //				(shape.ShapeType is StiRoundedRectangleShapeType) ||
                //				(shape.ShapeType is StiDiagonalDownLineShapeType) ||
                //				(shape.ShapeType is StiDiagonalUpLineShapeType) ||
                //				(shape.ShapeType is StiTriangleShapeType) ||
                //				(shape.ShapeType is StiOvalShapeType) ||
                //				(shape.ShapeType is StiArrowShapeType)
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region RenderShape1
        private void RenderShape1(StiRtfData pp, int correctX, int correctW)
        {
            StiComponent component = pp.Component;
            StiShape shape = component as StiShape;
            if (shape != null)
            {
                if (CheckShape1(shape) == true)
                {
                    #region Render primitive
                    IStiBrush mBrush = component as IStiBrush;

                    #region Fillcolor
                    string stBrush = string.Empty;
                    Color tempColor = Color.Transparent;
                    if (mBrush != null) tempColor = StiBrush.ToColor(mBrush.Brush);
                    if (tempColor.A != 0)
                    {
                        string tempColorSt = GetColorNumber(colorList, tempColor);
                        stBrush = String.Format("\\cbpat{0}", tempColorSt);
                    }
                    #endregion

                    #region stroke color
                    string stBorder = string.Empty;
                    Color tempColor2 = shape.BorderColor;
                    if (tempColor2.A != 0)
                    {
                        string tempColor2St = GetColorNumber(colorList, tempColor2);
                        stBorder = String.Format("\\brdrcf{0}", tempColor2St);
                    }
                    #endregion

                    int borderWidth = (int)(shape.Size * 14);
                    string stBorderWidth = string.Format("\\brdrs\\brdrw{0}", borderWidth);

                    sw.Write("\\fs1");

                    #region VerticalLine
                    if (shape.ShapeType is StiVerticalLineShapeType)
                    {
                        if (tempColor.A != 0)
                        {
                            sw.Write(stBrush);
                        }
                        if (tempColor2.A != 0)
                        {
                            sw.WriteLine(" \\par}");
                            sw.Write("{");
                            sw.Write("\\nowrap\\posx{0}\\posy{1}\\absw{2}\\absh{3}",
                                pp.X + correctX, pp.Y, (int)((pp.Width - frameCorrectValue * 2 - borderWidth) / 2), pp.Height);
                            sw.Write("\\brdrr" + stBorderWidth + stBorder + "\\fs1");
                        }
                    }
                    #endregion

                    #region HorizontalLine
                    if (shape.ShapeType is StiHorizontalLineShapeType)
                    {
                        if (tempColor.A != 0)
                        {
                            sw.Write(stBrush);
                        }
                        if (tempColor2.A != 0)
                        {
                            sw.WriteLine(" \\par}");
                            sw.Write("{");
                            sw.Write("\\nowrap\\posx{0}\\posy{1}\\absw{2}\\absh{3}",
                                pp.X + correctX, pp.Y, pp.Width - correctW, (int)((pp.Height + borderWidth) / 2));
                            sw.Write("\\brdrb" + stBorderWidth + stBorder + "\\fs1");
                        }
                    }
                    #endregion

                    #region TopAndBottomLine
                    if (shape.ShapeType is StiTopAndBottomLineShapeType)
                    {
                        if (tempColor.A != 0)
                        {
                            sw.Write(stBrush);
                        }
                        sw.Write("\\brdrt" + stBorderWidth + stBorder);
                        sw.Write("\\brdrb" + stBorderWidth + stBorder);
                        sw.WriteLine("\\fs1");
                    }
                    #endregion

                    #region LeftAndRightLine
                    if (shape.ShapeType is StiLeftAndRightLineShapeType)
                    {
                        if (tempColor.A != 0)
                        {
                            sw.Write(stBrush);
                        }
                        sw.Write("\\brdrl" + stBorderWidth + stBorder);
                        sw.Write("\\brdrr" + stBorderWidth + stBorder);
                        sw.WriteLine("\\fs1");
                    }
                    #endregion

                    #region Rectangle
                    if (shape.ShapeType is StiRectangleShapeType)
                    {
                        if (tempColor.A != 0)
                        {
                            sw.Write(stBrush);
                        }
                        sw.Write("\\box" + stBorderWidth + stBorder);
                        sw.WriteLine("\\fs1");
                    }
                    #endregion

                    //					sw.WriteLine(" \\par}");
                    #endregion
                }
                else
                {
                    RenderImage12(component, pp.Width - correctW, pp.Height);
                }
            }
        }
        #endregion

        #region CheckShape2
        private bool CheckShape2(StiShape shape)
        {
            if (
                (shape.ShapeType is StiVerticalLineShapeType) ||
                (shape.ShapeType is StiHorizontalLineShapeType) ||
                (shape.ShapeType is StiTopAndBottomLineShapeType) ||
                (shape.ShapeType is StiLeftAndRightLineShapeType) ||
                (shape.ShapeType is StiRectangleShapeType) ||
                //				(shape.ShapeType is StiRoundedRectangleShapeType) ||
                (shape.ShapeType is StiDiagonalDownLineShapeType) ||
                (shape.ShapeType is StiDiagonalUpLineShapeType) ||
                //				(shape.ShapeType is StiTriangleShapeType) ||
                (shape.ShapeType is StiOvalShapeType)
                //				(shape.ShapeType is StiArrowShapeType)
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region RenderShape2
        private void RenderShape2(StiRtfData pp)
        {
            StiComponent component = pp.Component;
            StiShape shape = component as StiShape;
            if (shape != null)
            {
                if (CheckShape2(shape) == true)
                {
                    #region Render primitive
                    IStiBrush mBrush = component as IStiBrush;

                    // Fillcolor
                    Color tempColor = Color.Transparent;
                    if (mBrush != null) tempColor = StiBrush.ToColor(mBrush.Brush);
                    // stroke color
                    Color tempColor2 = shape.BorderColor;

                    int borderWidth = (int)(shape.Size * 8400);
                    string stBorderWidth = string.Format("{0}", borderWidth);

                    #region VerticalLine
                    if (shape.ShapeType is StiVerticalLineShapeType)
                    {
                        FillRect(pp, tempColor);
                        DrawLine((int)(pp.X + pp.Width / 2), pp.Y, (int)(pp.X + pp.Width / 2), pp.Y + pp.Height, tempColor2, stBorderWidth);
                    }
                    #endregion

                    #region HorizontalLine
                    if (shape.ShapeType is StiHorizontalLineShapeType)
                    {
                        FillRect(pp, tempColor);
                        DrawLine(pp.X, (int)(pp.Y + pp.Height / 2), pp.X + pp.Width, (int)(pp.Y + pp.Height / 2), tempColor2, stBorderWidth);
                    }
                    #endregion

                    #region TopAndBottomLine
                    if (shape.ShapeType is StiTopAndBottomLineShapeType)
                    {
                        FillRect(pp, tempColor);
                        DrawLine(pp.X, pp.Y, pp.X + pp.Width, pp.Y, tempColor2, stBorderWidth);
                        DrawLine(pp.X, pp.Y + pp.Height, pp.X + pp.Width, pp.Y + pp.Height, tempColor2, stBorderWidth);
                    }
                    #endregion

                    #region LeftAndRightLine
                    if (shape.ShapeType is StiLeftAndRightLineShapeType)
                    {
                        FillRect(pp, tempColor);
                        DrawLine(pp.X, pp.Y, pp.X, pp.Y + pp.Height, tempColor2, stBorderWidth);
                        DrawLine(pp.X + pp.Width, pp.Y, pp.X + pp.Width, pp.Y + pp.Height, tempColor2, stBorderWidth);
                    }
                    #endregion

                    #region Rectangle
                    if (shape.ShapeType is StiRectangleShapeType)
                    {
                        sw.Write("{\\shp{\\*");
                        sw.Write("\\shpinst\\shpleft{0}\\shptop{1}\\shpright{2}\\shpbottom{3}",
                            pp.X, pp.Y, pp.X + pp.Width, pp.Y + pp.Height);
                        sw.Write("\\shpwr3");
                        sw.Write("{\\sp{\\sn shapeType}{\\sv 1}}");
                        sw.Write("{\\sp{\\sn fFlipH}{\\sv 0}}");
                        sw.Write("{\\sp{\\sn fFlipV}{\\sv 0}}");
                        if (tempColor.A != 0)
                        {
                            sw.Write("{\\sp{\\sn fillColor}{\\sv " + string.Format("{0}", tempColor.B * 65536 + tempColor.G * 256 + tempColor.R) + "}}");
                            sw.Write("{\\sp{\\sn fFilled}{\\sv 1}}");
                        }
                        else
                        {
                            sw.Write("{\\sp{\\sn fFilled}{\\sv 0}}");
                        }
                        if (tempColor2.A != 0)
                        {
                            sw.Write("{\\sp{\\sn lineColor}{\\sv " + string.Format("{0}", tempColor2.B * 65536 + tempColor2.G * 256 + tempColor2.R) + "}}");
                            sw.Write("{\\sp{\\sn lineWidth}{\\sv " + stBorderWidth + "}}");
                            sw.Write("{\\sp{\\sn fLine}{\\sv 1}}");
                        }
                        else
                        {
                            sw.Write("{\\sp{\\sn fLine}{\\sv 0}}");
                        }
                        sw.WriteLine("}}");
                    }
                    #endregion

                    #region Oval
                    if (shape.ShapeType is StiOvalShapeType)
                    {
                        sw.Write("{\\shp{\\*");
                        sw.Write("\\shpinst\\shpleft{0}\\shptop{1}\\shpright{2}\\shpbottom{3}",
                            pp.X, pp.Y, pp.X + pp.Width, pp.Y + pp.Height);
                        sw.Write("\\shpwr3");
                        sw.Write("{\\sp{\\sn shapeType}{\\sv 3}}");
                        sw.Write("{\\sp{\\sn fFlipH}{\\sv 0}}");
                        sw.Write("{\\sp{\\sn fFlipV}{\\sv 0}}");
                        if (tempColor.A != 0)
                        {
                            sw.Write("{\\sp{\\sn fillColor}{\\sv " + string.Format("{0}", tempColor.B * 65536 + tempColor.G * 256 + tempColor.R) + "}}");
                            sw.Write("{\\sp{\\sn fFilled}{\\sv 1}}");
                        }
                        else
                        {
                            sw.Write("{\\sp{\\sn fFilled}{\\sv 0}}");
                        }
                        if (tempColor2.A != 0)
                        {
                            sw.Write("{\\sp{\\sn lineColor}{\\sv " + string.Format("{0}", tempColor2.B * 65536 + tempColor2.G * 256 + tempColor2.R) + "}}");
                            sw.Write("{\\sp{\\sn lineWidth}{\\sv " + stBorderWidth + "}}");
                            sw.Write("{\\sp{\\sn fLine}{\\sv 1}}");
                        }
                        else
                        {
                            sw.Write("{\\sp{\\sn fLine}{\\sv 0}}");
                        }
                        sw.WriteLine("}}");
                    }
                    #endregion

                    #region DiagonalDownLine
                    if (shape.ShapeType is StiDiagonalDownLineShapeType)
                    {
                        FillRect(pp, tempColor);
                        DrawLine(pp.X, pp.Y, pp.X + pp.Width, pp.Y + pp.Height, tempColor2, stBorderWidth);
                    }
                    #endregion

                    #region DiagonalUpLine
                    if (shape.ShapeType is StiDiagonalUpLineShapeType)
                    {
                        FillRect(pp, tempColor);
                        DrawLine(pp.X, pp.Y + pp.Height, pp.X + pp.Width, pp.Y, tempColor2, stBorderWidth);
                    }
                    #endregion

                    #endregion
                }
                else
                {
                    RenderImage12(component, pp.Width, pp.Height);
                }
            }
        }
        #endregion

        #region Render procedures

        private StringBuilder CheckArabic(StringBuilder outputString)
        {
            if (StiOptions.Export.RichText.ConvertDigitsToArabic)
            {
                return StiExportUtils.ConvertDigitsToArabic(outputString, StiOptions.Export.RichText.ArabicDigitsType);
            }
            return outputString;
        }

        private StringBuilder UnicodeToRtfString(StringBuilder stInput, bool useRightToLeft)
        {
            // convert unicode string to rtf charset-based string
            if (useRightToLeft)
            {
                stInput = CheckArabic(stInput);
            }
            stInput.Replace("\\", "\\\\");
            stInput.Replace("{", "\\{");
            stInput.Replace("}", "\\}");

            if (stInput.Length > 1 && stInput[0] == '\t') stInput.Insert(0, "\t\t");
            stInput.Replace("\x0a\t", "\x0a\t\t\t");

            stInput.Replace("\x0a", "\\par ");

            stInput.Replace("\x17", "\\");
            stInput.Replace("\x18", "{\\field{\\*\\fldinst {PAGE}}}");
            stInput.Replace("\x19", "{\\field{\\*\\fldinst {NUMPAGES}}}");

            //stInput.Replace("\x1a", "{\\field{\\*\\fldinst {PAGEREF ");
            stInput.Replace("\x1a", "{\\field{\\*\\fldinst {");
            stInput.Replace("\x1b", " }}{\\fldrslt {");
            stInput.Replace("\x1c", "{\\*\\bkmkstart ");
            stInput.Replace("\x1d", "}{\\*\\bkmkend ");
            stInput.Replace("\x1e", "}");

            //{\\field{\\*\\fldinst {PAGEREF memo1 }}{\\fldrslt {1}}
            //{\\*\\bkmkstart memo1}{\\*\\bkmkend memo1}

            //{\field{\*\fldinst {HYPERLINK \\l "memo1" }}{\fldrslt { memo1}}}
            //{\field{\*\fldinst {HYPERLINK "https://www.stimulsoft.com/" }}{\fldrslt { memo1}}}

            string stTemp = stInput.ToString();
            string[] stArray = null;
            if (stTemp.IndexOf('\x10') == -1)
            {
                stArray = new string[1];
                stArray[0] = stTemp;
            }
            else
            {
                stArray = stTemp.Split(new char[] { '\x10' });
            }

            StringBuilder sbOutput = new StringBuilder();
            for (int indexString = 0; indexString < stArray.Length; indexString++)
            {
                string stCurrent = stArray[indexString];
                if (indexString != 0)
                {
                    baseFontNumber = (int)(stCurrent[0]) - 256;
                    if (baseFontNumber < 0) baseFontNumber = 0;
                    stCurrent = stCurrent.Substring(2); //fontNumber and space after fontNumber
                }

                int stIndex = 0;
                while (stIndex < stCurrent.Length)
                {
                    int stIndexBegin = stIndex;
                    StringBuilder tempsb = new StringBuilder();
                    int cpValue = 1;
                    while ((stIndex < stCurrent.Length) && (StiEncode.unicodeToCodePageArray[(int)stCurrent[stIndex]] == 0))
                    {
                        tempsb.Append(stCurrent[stIndex]);
                        stIndex++;
                    }
                    if (stIndex < stCurrent.Length)
                    {
                        cpValue = StiEncode.unicodeToCodePageArray[(int)stCurrent[stIndex]];
                        tempsb.Append(stCurrent[stIndex]);
                        stIndex++;
                        while ((stIndex < stCurrent.Length) &&
                            ((StiEncode.unicodeToCodePageArray[(int)stCurrent[stIndex]] == 0) ||
                             (StiEncode.unicodeToCodePageArray[(int)stCurrent[stIndex]] == cpValue)))
                        {
                            tempsb.Append(stCurrent[stIndex]);
                            stIndex++;
                        }
                    }
                    StringBuilder tempbb = StiEncode.Encode(tempsb, cpValue);

                    sbOutput.Append("\\f" + (baseFontNumber + codePageToFont[cpValue]).ToString() + " ");
                    for (int tempIndex = 0; tempIndex < tempbb.Length; tempIndex++)
                    {
                        int curSym = (int)stCurrent[stIndexBegin + tempIndex];
                        if ((tempbb[tempIndex] == (char)0x003f) &&
                            (StiEncode.unicodeToCodePageArray[curSym] < 2))
                        {
                            sbOutput.Append("\\u" + curSym.ToString() + "?");
                        }
                        else
                        {
                            sbOutput.Append(tempbb[tempIndex]);
                        }
                    }
                }
            }
            return sbOutput;
        }

        #region GetRtfString
        private string deleteToken(string inputString, string token)
        {
            int offset;
            do
            {
                offset = inputString.IndexOf(token, StringComparison.InvariantCulture);
                if (offset > -1)
                {
                    int offset2 = offset + token.Length;
                    while ((offset2 < inputString.Length) && char.IsDigit(inputString[offset2]))
                    {
                        offset2++;
                    }
                    inputString = inputString.Remove(offset, offset2 - offset);
                }
            }
            while (offset != -1);

            return inputString;
        }

        private string GetRtfString(StiComponent component)
        {
            string st = string.Empty;

            StiRichText rtf = component as StiRichText;
            if ((rtf != null) && (rtf.RtfText != string.Empty))
            {
                //		if (rtf.Image == null)rtf.RenderMetafile();
                //		sw.Write(GetImageString(rtf.Image));

                ArrayList arrFont = new ArrayList();
                ArrayList arrFont2 = new ArrayList();
                Font tmpFont;
                ArrayList arrColor = new ArrayList();
                Color tmpColor;

                st = rtf.RtfText;

                if (st.TrimStart().ToLowerInvariant().IndexOf("rtf") == -1) return st;

                #region check for codepage
                int index = st.IndexOf("\\ansicpg");
                int tempIndex2 = st.IndexOf("\\\'");
                if ((index != -1) && (tempIndex2 != -1) && char.IsDigit(st, index + 8))
                {
                    index += 8;
                    int count = 0;
                    while ((index + count < st.Length) && (char.IsDigit(st, index + count)))
                    {
                        count++;
                    }
                    int codePage = int.Parse(st.Substring(index, count));

                    Encoding enc = null;
                    try
                    {
                        enc = Encoding.GetEncoding(codePage);
                    }
                    catch
                    {
                    }

                    if (enc != null)
                    {
                        StringBuilder sbb = new StringBuilder();
                        index = 0;
                        while (index < st.Length)
                        {
                            if ((index < st.Length - 4) && (st[index] == '\\') && (st[index + 1] == '\''))
                            {
                                List<byte> list = new List<byte>();
                                while ((index < st.Length - 4) && (st[index] == '\\') && (st[index + 1] == '\''))
                                {
                                    int hex = ParseHexTwoCharToInt(st[index + 2], st[index + 3]);
                                    if (hex < 256)
                                    {
                                        list.Add((byte)hex);
                                    }
                                    else
                                    {
                                        list.Add((byte)'?');
                                    }
                                    index += 4;
                                }
                                if (list.Count > 0)
                                {
                                    sbb.Append(enc.GetString(list.ToArray()));
                                }
                            }
                            else
                            {
                                sbb.Append(st[index]);
                                if ((st[index] == '\\') && (index < st.Length - 2) && (st[index + 1] == '\\'))
                                {
                                    sbb.Append('\\');
                                    index++;
                                }
                                index++;
                            }
                        }
                        st = sbb.ToString();
                    }
                }
                #endregion

                #region scan font table
                index = st.IndexOf("{\\fonttbl", StringComparison.InvariantCulture);
                if (index != -1)
                {
                    index += 9;
                    while ((index < st.Length) && (st[index] != '}'))
                    {
                        while ((index < st.Length) && (st[index] != '{')) index++;
                        int kolSkob = 0;
                        int index2 = index;
                        do
                        {
                            if (st[index2] == '{') kolSkob++;
                            if (st[index2] == '}') kolSkob--;
                            index2++;
                        } while (kolSkob > 0);
                        arrFont.Add(st.Substring(index, index2 - index));
                        index = index2;
                        while ((index < st.Length) && ((st[index] != '{') && (st[index] != '}'))) index++;
                    }
                    st = st.Remove(0, index + 1);
                }
                #endregion

                #region prepare font convertation table
                for (int tempIndex = 0; tempIndex < arrFont.Count; tempIndex++)
                {
                    string stt = (string)arrFont[tempIndex];

                    #region find and remove optional parameters and notices {\* }
                    int tempi;
                    do
                    {
                        tempi = stt.IndexOf("{\\*", StringComparison.InvariantCulture);
                        if (tempi != -1)
                        {
                            int kolSkob = 0;
                            int index2 = tempi;
                            do
                            {
                                if (stt[index2] == '{') kolSkob++;
                                if (stt[index2] == '}') kolSkob--;
                                index2++;
                            } while (kolSkob > 0);
                            stt = stt.Remove(tempi, index2 - tempi);
                            if ((stt[tempi - 1] != ' ') && (stt[tempi] != ' '))
                            {
                                stt = stt.Insert(tempi, " ");
                            }
                        }
                    } while (tempi != -1);
                    #endregion

                    tempi = stt.IndexOf(" ", StringComparison.InvariantCulture);
                    //		string stt2 = stt.Substring(tempi+1,stt.Length-tempi-3);
                    int tempLen = stt.Length - tempi - 2;
                    if (stt[tempi + 1 + tempLen - 1] == ';') tempLen--;
                    string stt2 = stt.Substring(tempi + 1, tempLen);
                    tmpFont = new Font(StiFontCollection.GetFontFamily(stt2), 10);

                    int charset = 0;
                    tempi = stt.IndexOf("\\fcharset", StringComparison.InvariantCulture);
                    if (tempi > 0)
                    {
                        tempi += 9;
                        var stbCharset = new StringBuilder();
                        while (Char.IsDigit(stt[tempi]))
                        {
                            stbCharset.Append(stt[tempi]);
                            tempi++;
                        }

                        if (!int.TryParse(stbCharset.ToString(), out charset))
                        {
                            charset = 0;
                        }
                    }

                    string stt3 = GetFontNumber(fontList, tmpFont, charset).ToString();

                    arrFont2.Add(stt3);
                    StringBuilder stb = new StringBuilder();
                    tempi = stt.IndexOf("\\f", StringComparison.InvariantCulture);
                    tempi += 2;
                    while (Char.IsDigit(stt[tempi]))
                    {
                        stb.Append(stt[tempi]);
                        tempi++;
                    }
                    arrFont[tempIndex] = stb.ToString();
                }
                #endregion

                #region scan color table
                index = st.IndexOf("{\\colortbl", StringComparison.InvariantCulture);
                if (index != -1)
                {
                    index += 10;
                    while ((index < st.Length) && (st[index] != '}'))
                    {
                        while ((index < st.Length) && ((st[index] != ';') && (st[index] != '\\'))) index++;
                        int index2 = index;
                        while ((index2 < st.Length) && (st[index2] != ';') && (st[index2] != '}')) index2++;
                        //	index2++;
                        arrColor.Add(st.Substring(index, index2 + 1 - index));
                        if (st[index2] == ';') index2++;
                        index = index2;
                        while ((index < st.Length) && ((st[index] != ';') &&
                            (st[index] != '\\') && (st[index] != '}'))) index++;
                    }
                    st = st.Remove(0, index + 1);
                }
                #endregion

                #region prepare color conversation table
                for (int tempIndex = 0; tempIndex < arrColor.Count; tempIndex++)
                {
                    string stt = (string)arrColor[tempIndex];

                    if (stt != ";")
                    {
                        int tempi;
                        string stt1;
                        string stt2;
                        string stt3;
                        StringBuilder stb;

                        tempi = stt.IndexOf("\\red", StringComparison.InvariantCulture) + 4;
                        stb = new StringBuilder();
                        while (Char.IsDigit(stt[tempi]))
                        {
                            stb.Append(stt[tempi]);
                            tempi++;
                        }
                        stt1 = stb.ToString();
                        tempi = stt.IndexOf("\\green", StringComparison.InvariantCulture) + 6;
                        stb = new StringBuilder();
                        while (Char.IsDigit(stt[tempi]))
                        {
                            stb.Append(stt[tempi]);
                            tempi++;
                        }
                        stt2 = stb.ToString();
                        tempi = stt.IndexOf("\\blue", StringComparison.InvariantCulture) + 5;
                        stb = new StringBuilder();
                        while (Char.IsDigit(stt[tempi]))
                        {
                            stb.Append(stt[tempi]);
                            tempi++;
                        }
                        stt3 = stb.ToString();
                        tmpColor = Color.FromArgb(int.Parse(stt1), int.Parse(stt2), int.Parse(stt3));
                        arrColor[tempIndex] = GetColorNumber(colorList, tmpColor);
                    }
                    else
                    {
                        //arrColor[tempIndex] = "0";
                        arrColor[tempIndex] = GetColorNumber(colorList, Color.Transparent);
                    }
                }
                #endregion

                #region scan stylesheet table
                index = st.IndexOf("{\\stylesheet", StringComparison.InvariantCulture);
                if (index != -1)
                {
                    int kolSkob = 0;
                    int index2 = index;
                    do
                    {
                        if (st[index2] == '{') kolSkob++;
                        if (st[index2] == '}') kolSkob--;
                        index2++;
                    } while (kolSkob > 0);
                    st = st.Remove(0, index2);
                }
                #endregion

                #region scan generator info
                index = st.IndexOf("{\\*\\generator", StringComparison.InvariantCulture);
                if (index != -1)
                {
                    int kolSkob = 0;
                    int index2 = index;
                    do
                    {
                        if (st[index2] == '{') kolSkob++;
                        if (st[index2] == '}') kolSkob--;
                        index2++;
                    } while (kolSkob > 0);
                    st = st.Remove(0, index2);
                }
                #endregion

                //	st = st.Replace("\\pard", "");

                #region convert font
                int tempix = 0;
                StringBuilder stbb;
                do
                {
                    tempix = st.IndexOf("\\f", tempix, StringComparison.InvariantCulture);
                    if (tempix != -1)
                    {
                        if (Char.IsDigit(st[tempix + 2]) == true)
                        {
                            tempix += 2;
                            int tempi2 = tempix;
                            stbb = new StringBuilder();
                            while (Char.IsDigit(st[tempi2]))
                            {
                                stbb.Append(st[tempi2]);
                                tempi2++;
                            }
                            string stt = stbb.ToString();
                            for (int tempIndex = 0; tempIndex < arrFont.Count; tempIndex++)
                            {
                                if ((string)arrFont[tempIndex] == stt)
                                {
                                    //st = st.Remove(tempix, tempi2 - tempix);
                                    //st = st.Insert(tempix, (string)arrFont2[tempIndex]);
                                    st = st.Substring(0, tempix) + (string)arrFont2[tempIndex] + st.Substring(tempi2);
                                }
                            }
                            tempix = tempi2;
                        }
                        else
                        {
                            tempix += 2;
                        }
                    }
                } while (tempix != -1);
                #endregion

                if (arrColor.Count > 0)
                {
                    #region convert color 1
                    //		int tempix;
                    //		StringBuilder stbb;
                    tempix = 0;
                    int tokenLength = 3;
                    do
                    {
                        tempix = st.IndexOf("\\cf", tempix, StringComparison.InvariantCulture);
                        if (tempix != -1)
                        {
                            if (Char.IsDigit(st[tempix + tokenLength]) == true)
                            {
                                tempix += tokenLength;
                                int tempi2 = tempix;
                                stbb = new StringBuilder();
                                while (Char.IsDigit(st[tempi2]))
                                {
                                    stbb.Append(st[tempi2]);
                                    tempi2++;
                                }
                                int tempv = int.Parse(stbb.ToString());
                                //st = st.Remove(tempix, tempi2 - tempix);
                                //st = st.Insert(tempix, (string)arrColor[tempv]);
                                if (tempv < arrColor.Count)
                                {
                                    st = st.Substring(0, tempix) + (string)arrColor[tempv] + st.Substring(tempi2);
                                }
                                tempix = tempi2;
                            }
                            else
                            {
                                tempix += tokenLength;
                            }
                        }
                    } while (tempix != -1);
                    #endregion

                    #region convert color 2
                    tempix = 0;
                    tokenLength = 6;
                    do
                    {
                        tempix = st.IndexOf("\\cbpat", tempix, StringComparison.InvariantCulture);
                        if (tempix != -1)
                        {
                            if (Char.IsDigit(st[tempix + tokenLength]) == true)
                            {
                                tempix += tokenLength;
                                int tempi2 = tempix;
                                stbb = new StringBuilder();
                                while (Char.IsDigit(st[tempi2]))
                                {
                                    stbb.Append(st[tempi2]);
                                    tempi2++;
                                }
                                int tempv = int.Parse(stbb.ToString());
                                //st = st.Remove(tempix, tempi2 - tempix);
                                //st = st.Insert(tempix, (string)arrColor[tempv]);
                                if (tempv < arrColor.Count)
                                {
                                    st = st.Substring(0, tempix) + (string)arrColor[tempv] + st.Substring(tempi2);
                                }
                                tempix = tempi2;
                            }
                            else
                            {
                                tempix += tokenLength;
                            }
                        }
                    } while (tempix != -1);
                    #endregion

                    #region convert color 3
                    tempix = 0;
                    tokenLength = 10;
                    do
                    {
                        tempix = st.IndexOf("\\highlight", tempix, StringComparison.InvariantCulture);
                        if (tempix != -1)
                        {
                            if (Char.IsDigit(st[tempix + tokenLength]) == true)
                            {
                                tempix += tokenLength;
                                int tempi2 = tempix;
                                stbb = new StringBuilder();
                                while (Char.IsDigit(st[tempi2]))
                                {
                                    stbb.Append(st[tempi2]);
                                    tempi2++;
                                }
                                int tempv = int.Parse(stbb.ToString());
                                //st = st.Remove(tempix, tempi2 - tempix);
                                //st = st.Insert(tempix, (string)arrColor[tempv]);
                                if (tempv < arrColor.Count)
                                {
                                    st = st.Substring(0, tempix) + (string)arrColor[tempv] + st.Substring(tempi2);
                                }
                                tempix = tempi2;
                            }
                            else
                            {
                                tempix += tokenLength;
                            }
                        }
                    } while (tempix != -1);
                    #endregion
                }

                index = st.Length - 1;
                while (st[index] != '}') index--;
                index--;
                //while ((st[index] == '\r') || (st[index] == '\n')) index--;   //fix 25.11.2008
                while ((st[index] == '\r') || (st[index] == '\n') || (st[index] == ' ')) index--;
                st = st.Substring(0, index + 1);
                if (st.EndsWith("\\par", StringComparison.InvariantCulture))
                {
                    st = st.Substring(0, st.Length - "\\par".Length);
                }
                if (st.EndsWith("\\par }", StringComparison.InvariantCulture))     //add 25.11.2008
                {
                    st = st.Substring(0, st.Length - "\\par }".Length) + '}';
                }

                st = deleteToken(st, "\\viewkind");
                st = deleteToken(st, "\\uc");

                if (string.IsNullOrWhiteSpace(st)) return "";

                while ((st[0] == '\r') || (st[0] == '\n'))
                {
                    st = st.Remove(0, 1);
                }
                if (st.Substring(0, 5) == "\\pard")
                {
                    st = st.Remove(0, 5);
                }

                st = deleteToken(st, "\\formprot");
                st = deleteToken(st, "\\pagebb");   //fix 25.11.2008

                //added 10.01.2008
                StringBuilder sb = new StringBuilder();
                for (int indexChar = 0; indexChar < st.Length; indexChar++)
                {
                    int bt = (int)st[indexChar];
                    if (bt > 255)
                    {
                        sb.AppendFormat("\\u{0}{1}", bt, "?");
                    }
                    else
                    {
                        sb.Append(st[indexChar]);
                    }
                }
                st = sb.ToString();

            }

            #region check {} parity
            int counter = 0;
            int pos = 0;
            while (pos < st.Length)
            {
                char ch = st[pos++];
                if (ch == '{')
                {
                    counter++;
                    continue;
                }
                if (ch == '}')
                {
                    counter--;
                    continue;
                }
                if (ch == '\\')
                {
                    if ((pos < st.Length) && ((st[pos] == '{') || (st[pos] == '}')))
                    {
                        pos++;
                        continue;
                    }
                }
            }
            if (counter < 0) st = new string('{', -counter) + st;
            if (counter > 0) st += new string('}', counter);
            #endregion

            return st;
        }

        private static int ParseHexTwoCharToInt(char c1, char c2)
        {
            return ParseHexCharToInt(c1) * 16 + ParseHexCharToInt(c2);
        }
        private static int ParseHexCharToInt(char ch)
        {
            if (ch >= '0' && ch <= '9') return (int)ch - (int)'0';
            if (ch == 'a' || ch == 'A') return 10;
            if (ch == 'b' || ch == 'B') return 11;
            if (ch == 'c' || ch == 'C') return 12;
            if (ch == 'd' || ch == 'D') return 13;
            if (ch == 'e' || ch == 'E') return 14;
            if (ch == 'f' || ch == 'F') return 15;
            return 256;
        }
        #endregion

        #region MakeHorAlignString
        private string MakeHorAlignString(StiTextHorAlignment alignment, bool rightToLeft)
        {
            int indent = 0; //StiOptions.Export.Rtf.ParagraphRightIndentInTwips;
            string stAlignment = "\\ql" + (indent != 0 ? string.Format("\\ri{0}", indent) : "");
            if (((alignment == StiTextHorAlignment.Left) &&
                (rightToLeft == true)) ||
                ((alignment == StiTextHorAlignment.Right) &&
                (rightToLeft == false)))
            {
                stAlignment = "\\qr" + (indent != 0 ? string.Format("\\li{0}", indent) : "");
            }
            if (alignment == StiTextHorAlignment.Center) stAlignment = "\\qc" + (indent != 0 ? string.Format("\\ri{0}", indent) : "");
            if (alignment == StiTextHorAlignment.Width) stAlignment = "\\qj";
            return stAlignment;
        }
        #endregion

        #region GetRtfStyleFromComponent
        private int GetRtfStyleFromComponent(StiComponent component)
        {
            if (component != null)
            {
                IStiFont mFont = component as IStiFont;
                IStiTextBrush mTextBrush = component as IStiTextBrush;
                IStiTextHorAlignment mTextHorAlign = component as IStiTextHorAlignment;
                IStiTextOptions textOpt = component as IStiTextOptions;

                StiRtfStyleInfo style = new StiRtfStyleInfo();
                style.Name = component.Name;
                if ((component.ComponentStyle != null) && (component.ComponentStyle.Length > 0))
                {
                    style.Name = component.ComponentStyle;
                }
                if (mFont != null)
                {
                    style.FontNumber = GetFontNumber(fontList, mFont.Font);
                    style.FontSize = (int)Math.Round(mFont.Font.SizeInPoints * 2, 0);
                    style.Bold = mFont.Font.Bold;
                    style.Italic = mFont.Font.Italic;
                    style.Underline = mFont.Font.Underline;
                }
                if (mTextBrush != null)
                {
                    style.TextColor = GetColorNumberInt(colorList, StiBrush.ToColor(mTextBrush.TextBrush));
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
            return 0;
        }
        #endregion

        private void RenderStartDoc()
        {
            sw.Write("{\\rtf1");
            sw.WriteLine("\\ansi\\ansicpg{0}", 1252);
            //sw.WriteLine("");
            sw.WriteLine("{\\fonttbl");
            for (int index = 0; index < fontList.Count; index++)
            {
                Font tmpFont = (Font)fontList[index];
                for (int index2 = 0; index2 < charsetCount; index2++)
                {
                    int charset = StiEncode.codePagesTable[fontToCodePages[index2], 2];
                    sw.Write("{");
                    sw.Write("\\f{0}\\fcharset{1} {2};",
                        index * charsetCount + index2,
                        (charset == 1 ? 0 : charset),
                        tmpFont.Name);
                    sw.WriteLine("}");
                }
            }
            sw.WriteLine("}");
            //sw.WriteLine("");

            sw.WriteLine("{\\colortbl");
            for (int index = 0; index < colorList.Count; index++)
            {
                Color tmpColor = (Color)colorList[index];
                if (tmpColor == Color.Transparent)
                {
                    sw.WriteLine(";");
                }
                else
                {
                    sw.WriteLine("\\red{0}\\green{1}\\blue{2};", tmpColor.R, tmpColor.G, tmpColor.B);
                }
            }
            sw.WriteLine("}");
            //sw.WriteLine("");

            if (useStyles)
            {
                sw.WriteLine("{\\stylesheet");
                for (int index = 0; index < styleList.Count; index++)
                {
                    sw.Write("{");
                    StiRtfStyleInfo tmpStyle = (StiRtfStyleInfo)styleList[index];
                    sw.Write("\\s{0}\\sbasedon0\\snext{0} ", index + 1);

                    if (tmpStyle.FontNumber != -1)
                    {
                        sw.Write("\\f{0}", tmpStyle.FontNumber);
                        sw.Write("\\fs{0}", tmpStyle.FontSize);
                        if (tmpStyle.Bold) sw.Write("\\b");
                        if (tmpStyle.Italic) sw.Write("\\i");
                        if (tmpStyle.Underline) sw.Write("\\ul");
                    }
                    if (tmpStyle.TextColor != -1)
                    {
                        sw.Write("\\cf{0}", tmpStyle.TextColor);
                    }
                    sw.Write(MakeHorAlignString(tmpStyle.Alignment, tmpStyle.RightToLeft));
                    if (tmpStyle.RightToLeft)
                    {
                        sw.Write("\\rtlpar");
                    }
                    else
                    {
                        sw.Write("\\ltrpar");
                    }

                    sw.Write(" {0};", tmpStyle.Name);
                    sw.WriteLine("}");
                }
                sw.WriteLine("}");
                //sw.WriteLine("");
            }

            sw.WriteLine("{\\info{\\doccomm " + Stimulsoft.Report.Export.StiExportUtils.GetReportVersion(report) + "}}");

            sw.Write("\\viewkind1");
            sw.Write("\\deftab{0}", (int)Math.Round(HiToTwips * 20));

            sw.Write("\\nouicompat");   //fix 2012.07.23
        }

        private void RenderEndDoc()
        {
            sw.WriteLine("");
            sw.WriteLine("}");
        }

        private void RenderPageHeader(StiPage page)
        {
            bool useTemplatePageSize = StiOptions.Export.RichText.UseTemplatePageSize;
            pageHeight = (int)Math.Round(HiToTwips * page.Unit.ConvertToHInches(page.PageHeight * (useTemplatePageSize ? 1 : page.SegmentPerHeight)));
            pageWidth = (int)Math.Round(HiToTwips * page.Unit.ConvertToHInches(page.PageWidth * (useTemplatePageSize ? 1 : page.SegmentPerWidth)));

            int mgLeft = (int)Math.Round(HiToTwips * page.Unit.ConvertToHInches(page.Margins.Left));
            int mgRight = (int)Math.Round(HiToTwips * page.Unit.ConvertToHInches(page.Margins.Right));
            int mgTop = (int)Math.Round(HiToTwips * page.Unit.ConvertToHInches(page.Margins.Top));
            int mgBottom = (int)Math.Round(HiToTwips * page.Unit.ConvertToHInches(page.Margins.Bottom)) - StiOptions.Export.RichText.BottomMarginCorrection;
            if (mgBottom < 0) mgBottom = 0;
            if (pageWidth > 31500) pageWidth = 31500;
            if (pageHeight > 31500) pageHeight = 31500;

            sw.Write("\\sectd");
            if (page.Orientation == StiPageOrientation.Landscape)
            {
                sw.Write("\\lndscpsxn");
            }
            sw.Write("\\paperw{0}\\paperh{1}", pageWidth, pageHeight);
            sw.WriteLine("\\margl{0}\\margr{1}\\margt{2}\\margb{3}\\headery{2}\\footery{3}", mgLeft, mgRight, mgTop, mgBottom);
            //sw.WriteLine("{");
        }

        private void RenderPageFooter()
        {
            if (StiOptions.Export.RichText.UseNewPageCommandInsteadOfNewSection)
            {
                sw.WriteLine("\\page\\par");
            }
            else
            {
                sw.WriteLine("\\sect");
            }
        }

        private void RenderTextAngle1(StiComponent component)
        {
            IStiTextOptions textOpt = component as IStiTextOptions;
            if (textOpt != null)
            {
                float textAngle = textOpt.TextOptions.Angle;
                int textAngleNum = 0;
                if ((textAngle > 45) && (textAngle < 135)) textAngleNum = 1;
                if ((textAngle > 225) && (textAngle < 315)) textAngleNum = 2;
                switch (textAngleNum)
                {
                    case 1:
                        sw.Write("\\frmtxbtlr");
                        break;
                    case 2:
                        sw.Write("\\frmtxtbrl");
                        break;
                    default:
                        sw.Write("\\frmtxlrtb");
                        break;
                }
            }
        }

        private void RenderTextAngle2(StiComponent component)
        {
            IStiTextOptions textOpt = component as IStiTextOptions;
            if (textOpt != null)
            {
                float textAngle = textOpt.TextOptions.Angle;
                int textAngleNum = 0;
                if ((textAngle > 45) && (textAngle < 135)) textAngleNum = 1;
                if ((textAngle > 225) && (textAngle < 315)) textAngleNum = 2;
                switch (textAngleNum)
                {
                    case 1:
                        sw.Write("\\cltxbtlr");
                        break;
                    case 2:
                        sw.Write("\\cltxtbrl");
                        break;
                    default:
                        sw.Write("\\cltxlrtb");
                        break;
                }
            }
        }

        private void RenderHorAlign12(StiComponent component)
        {
            IStiTextHorAlignment mTextHorAlign = component as IStiTextHorAlignment;
            IStiTextOptions textOpt = component as IStiTextOptions;
            bool rtl = (textOpt != null) ? textOpt.TextOptions.RightToLeft : false;
            if (mTextHorAlign != null)
            {
                string alignment = MakeHorAlignString(mTextHorAlign.HorAlignment, rtl);
                sw.Write("{0}", alignment);
            }
        }

        private void RenderVerAlign2(StiComponent component)
        {
            IStiVertAlignment mVertAlign = component as IStiVertAlignment;
            if (mVertAlign != null)
            {
                if (mVertAlign.VertAlignment == StiVertAlignment.Top) sw.Write("\\clvertalt");
                if (mVertAlign.VertAlignment == StiVertAlignment.Center) sw.Write("\\clvertalc");
                if (mVertAlign.VertAlignment == StiVertAlignment.Bottom) sw.Write("\\clvertalb");
            }
        }

        private void RenderBorder1(StiComponent component)
        {
            IStiBorder mBorder = component as IStiBorder;
            if (mBorder != null && (!(component is IStiIgnoreBorderWhenExport)))
            {
                if (mBorder.Border is StiAdvancedBorder)
                {
                    StiAdvancedBorder advBorder = mBorder.Border as StiAdvancedBorder;
                    if (advBorder.IsLeftBorderSidePresent && (advBorder.LeftSide.Color.A != 0))
                    {
                        sw.Write("\\brdrl" + GetLineStyle(advBorder.LeftSide, colorList));
                    }
                    if (advBorder.IsRightBorderSidePresent && (advBorder.RightSide.Color.A != 0))
                    {
                        sw.Write("\\brdrr" + GetLineStyle(advBorder.RightSide, colorList));
                    }
                    if (advBorder.IsTopBorderSidePresent && (advBorder.TopSide.Color.A != 0))
                    {
                        sw.Write("\\brdrt" + GetLineStyle(advBorder.TopSide, colorList));
                    }
                    if (advBorder.IsBottomBorderSidePresent && (advBorder.BottomSide.Color.A != 0))
                    {
                        sw.Write("\\brdrb" + GetLineStyle(advBorder.BottomSide, colorList));
                    }
                }
                else
                {
                    Color tempColor = mBorder.Border.Color;
                    if (tempColor.A != 0)
                    {
                        //StringBuilder sb = new StringBuilder();
                        //sb.Append("\\brdrs\\brdrw");
                        //sb.Append((int)(mBorder.Border.Size * 15));
                        //sb.Append(string.Format("\\brdrcf{0}", GetColorNumber(colorList, tempColor)));
                        string sb = GetLineStyle(new StiBorderSide(mBorder.Border.Color, mBorder.Border.Size, mBorder.Border.Style), colorList);
                        if ((mBorder.Border.Side & StiBorderSides.Left) > 0) sw.Write("\\brdrl" + sb);
                        if ((mBorder.Border.Side & StiBorderSides.Right) > 0) sw.Write("\\brdrr" + sb);
                        if ((mBorder.Border.Side & StiBorderSides.Top) > 0) sw.Write("\\brdrt" + sb);
                        if ((mBorder.Border.Side & StiBorderSides.Bottom) > 0) sw.Write("\\brdrb" + sb);
                    }
                }
            }
        }

        private void RenderBorder2(StiComponent component)
        {
            IStiBorder mBorder = component as IStiBorder;
            if (mBorder != null && (!(component is IStiIgnoreBorderWhenExport)))
            {
                if (mBorder.Border is StiAdvancedBorder)
                {
                    StiAdvancedBorder advBorder = mBorder.Border as StiAdvancedBorder;
                    if (advBorder.IsLeftBorderSidePresent && (advBorder.LeftSide.Color.A != 0))
                    {
                        sw.Write("\\clbrdrl" + GetLineStyle(advBorder.LeftSide, colorList));
                    }
                    if (advBorder.IsRightBorderSidePresent && (advBorder.RightSide.Color.A != 0))
                    {
                        sw.Write("\\clbrdrr" + GetLineStyle(advBorder.RightSide, colorList));
                    }
                    if (advBorder.IsTopBorderSidePresent && (advBorder.TopSide.Color.A != 0))
                    {
                        sw.Write("\\clbrdrt" + GetLineStyle(advBorder.TopSide, colorList));
                    }
                    if (advBorder.IsBottomBorderSidePresent && (advBorder.BottomSide.Color.A != 0))
                    {
                        sw.Write("\\clbrdrb" + GetLineStyle(advBorder.BottomSide, colorList));
                    }
                }
                else
                {
                    Color tempColor = mBorder.Border.Color;
                    if (tempColor.A != 0)
                    {
                        //StringBuilder sb = new StringBuilder();
                        //sb.Append("\\brdrs\\brdrw");
                        //sb.Append((int)(mBorder.Border.Size * 15));
                        //sb.Append(string.Format("\\brdrcf{0}", GetColorNumber(colorList, tempColor)));
                        string sb = GetLineStyle(new StiBorderSide(mBorder.Border.Color, mBorder.Border.Size, mBorder.Border.Style), colorList);
                        if ((mBorder.Border.Side & StiBorderSides.Left) > 0) sw.Write("\\clbrdrl" + sb);
                        if ((mBorder.Border.Side & StiBorderSides.Right) > 0) sw.Write("\\clbrdrr" + sb);
                        if ((mBorder.Border.Side & StiBorderSides.Top) > 0) sw.Write("\\clbrdrt" + sb);
                        if ((mBorder.Border.Side & StiBorderSides.Bottom) > 0) sw.Write("\\clbrdrb" + sb);
                    }
                }
            }
        }

        private string RenderBorder2Table(int rowIndex, int columnIndex, int height, int width, bool returnValue, bool checkHeaderAP = false)
        {
            bool needBorderLeft = true;
            bool needBorderRight = true;
            for (int index = 0; index < height + 1; index++)
            {
                if (matrix.BordersY[rowIndex - 1 + index, columnIndex - 1] == null) needBorderLeft = false;
                if (matrix.BordersY[rowIndex - 1 + index, columnIndex - 1 + width + 1] == null) needBorderRight = false;
            }
            bool needBorderTop = true;
            bool needBorderBottom = true;
            for (int index = 0; index < width + 1; index++)
            {
                if (matrix.BordersX[rowIndex - 1, columnIndex - 1 + index] == null) needBorderTop = false;
                if (matrix.BordersX[rowIndex - 1 + height + 1, columnIndex - 1 + index] == null) needBorderBottom = false;
            }

            if (checkHeaderAP)
            {
                if ((rowIndex < Matrix.CoordY.Count) && (matrix.LinePlacement[rowIndex] == StiMatrix.StiTableLineInfo.HeaderAP))
                {
                    needBorderBottom = false;
                }
            }

            StringBuilder sb = new StringBuilder();
            if (needBorderLeft)
            {
                //StiBorder border = matrix.BordersY[rowIndex - 1, columnIndex - 1];
                //StringBuilder sb = new StringBuilder();
                //sb.Append("\\brdrs\\brdrw");
                //sb.Append((int)(border.Size * 15));
                //sb.Append(string.Format("\\brdrcf{0}", GetColorNumber(colorList, border.Color)));
                //sw.Write("\\clbrdrl" + sb);
                sb.Append("\\clbrdrl" + GetLineStyle(matrix.BordersY[rowIndex - 1, columnIndex - 1], colorList));
            }
            if (needBorderRight)
            {
                //StiBorder border = matrix.BordersY[rowIndex - 1, columnIndex - 1 + width + 1];
                //StringBuilder sb = new StringBuilder();
                //sb.Append("\\brdrs\\brdrw");
                //sb.Append((int)(border.Size * 15));
                //sb.Append(string.Format("\\brdrcf{0}", GetColorNumber(colorList, border.Color)));
                //sw.Write("\\clbrdrr" + sb);
                sb.Append("\\clbrdrr" + GetLineStyle(matrix.BordersY[rowIndex - 1, columnIndex - 1 + width + 1], colorList));
            }
            if (needBorderTop)
            {
                //StiBorder border = matrix.BordersX[rowIndex - 1, columnIndex - 1];
                //StringBuilder sb = new StringBuilder();
                //sb.Append("\\brdrs\\brdrw");
                //sb.Append((int)(border.Size * 15));
                //sb.Append(string.Format("\\brdrcf{0}", GetColorNumber(colorList,border.Color)));
                //sw.Write("\\clbrdrt" + sb);
                sb.Append("\\clbrdrt" + GetLineStyle(matrix.BordersX[rowIndex - 1, columnIndex - 1], colorList));
            }
            if (needBorderBottom)
            {
                //StiBorder border = matrix.BordersX[rowIndex - 1 + height + 1, columnIndex - 1];
                //StringBuilder sb = new StringBuilder();
                //sb.Append("\\brdrs\\brdrw");
                //sb.Append((int)(border.Size * 15));
                //sb.Append(string.Format("\\brdrcf{0}", GetColorNumber(colorList, border.Color)));
                //sw.Write("\\clbrdrb" + sb);
                sb.Append("\\clbrdrb" + GetLineStyle(matrix.BordersX[rowIndex - 1 + height + 1, columnIndex - 1], colorList));
            }
            if (returnValue)
            {
                return sb.ToString();
            }
            if (sb.Length > 0) sw.Write(sb.ToString());
            return null;
        }

        private StiBorderSides RenderBorder2TableGetValues(int rowIndex, int columnIndex, ref string styles)
        {
            StiBorderSides sides = new StiBorderSides();
            StringBuilder sb = new StringBuilder();
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
                sb.Append("\\clbrdrt" + GetLineStyle(matrix.BordersX[rowIndex - 1, columnIndex - 1], colorList));
                sides |= StiBorderSides.Top;
            }
            if (matrix.BordersX[rowIndex - 1 + 1, columnIndex - 1] != null)
            {
                //needBorderBottom = true;
                sb.Append("\\clbrdrb" + GetLineStyle(matrix.BordersX[rowIndex - 1 + 1, columnIndex - 1], colorList));
                sides |= StiBorderSides.Bottom;
            }
            styles = sb.ToString();
            return sides;
        }

        private void RenderBrush1(StiComponent component)
        {
            IStiBrush mBrush = component as IStiBrush;
            if ((mBrush != null) && (!component.IsExportAsImage(StiExportFormat.Rtf)))
            {
                Color tempColor = StiBrush.ToColor(mBrush.Brush);
                if (tempColor.A != 0)
                {
                    string tempColorSt = GetColorNumber(colorList, tempColor);
                    sw.Write("\\cbpat{0}", tempColorSt);
                }
            }
        }

        private void RenderBrush2(StiComponent component)
        {
            IStiBrush mBrush = component as IStiBrush;
            if ((mBrush != null) && (!component.IsExportAsImage(StiExportFormat.Rtf)))
            {
                Color tempColor = StiBrush.ToColor(mBrush.Brush);
                sw.Write("{\\sp{\\sn fillColor}{\\sv ");
                sw.Write("{0}", tempColor.B * 65536 + tempColor.G * 256 + tempColor.R);
                sw.Write("}}");
                if (tempColor.A != 0)
                {
                    sw.Write("{\\sp{\\sn fFilled}{\\sv 1}} ");
                }
                else
                {
                    sw.Write("{\\sp{\\sn fFilled}{\\sv 0}} ");
                }
            }
        }

        private void RenderTextBrush12(StiComponent component)
        {
            IStiTextBrush mTextBrush = component as IStiTextBrush;
            if (mTextBrush != null)
            {
                string tempColor = GetColorNumber(colorList, StiBrush.ToColor(mTextBrush.TextBrush));
                sw.Write("\\cf{0}", tempColor);
            }
        }

        private void RenderTextFont12(StiComponent component)
        {
            IStiFont mFont = component as IStiFont;
            if (mFont != null)
            {
                baseFontNumber = GetFontNumber(fontList, mFont.Font);
                sw.Write("\\f{0}", baseFontNumber);
                sw.Write("\\fs{0}", Math.Round(mFont.Font.SizeInPoints * 2));
                if (mFont.Font.Bold) sw.Write("\\b");
                if (mFont.Font.Italic) sw.Write("\\i");
                if (mFont.Font.Underline) sw.Write("\\ul");
                sw.Write(string.Format("\\sl{0}\\slmult1", Math.Round(240 * StiOptions.Export.RichText.LineSpacing * (component is StiText ? (component as StiText).LineSpacing : 1))));
                if (StiOptions.Export.RichText.SpaceBetweenCharacters != 0)
                {
                    sw.Write(string.Format("\\expndtw{0}", StiOptions.Export.RichText.SpaceBetweenCharacters));
                }
            }
        }

        private void RenderText12(StiComponent component)
        {
            IStiText text = component as IStiText;
            IStiTextOptions textOpt = component as IStiTextOptions;
            if (text != null && (!component.IsExportAsImage(StiExportFormat.Rtf)))
            {
                string st = text.Text;

                StiText stiText = component as StiText;
                if ((stiText != null) && (stiText.CheckAllowHtmlTags()))
                {
                    st = ConvertTextWithHtmlTagsToRtfText(stiText, st);
                }

                if ((component as StiText) != null && (component as StiText).TextQuality == StiTextQuality.Wysiwyg && !string.IsNullOrEmpty(st) && st.EndsWith(StiTextRenderer.StiForceWidthAlignTag))
                {
                    st = st.Substring(0, st.Length - StiTextRenderer.StiForceWidthAlignTag.Length);
                }
                StringBuilder stInput = new StringBuilder(st);
                bool useRightToLeft =
                    ((textOpt != null) && (textOpt.TextOptions != null) && (textOpt.TextOptions.RightToLeft));
                StringBuilder sbOutput = UnicodeToRtfString(stInput, useRightToLeft);
                byte[] tbb = new byte[sbOutput.Length];
                for (int tempI = 0; tempI < sbOutput.Length; tempI++)
                {
                    tbb[tempI] = (byte)sbOutput[tempI];
                }
                sw.Flush();
                sw2.Write(tbb, 0, tbb.Length);
            }
        }

        private void RenderImage12(StiComponent component, int absw, int absh)
        {
            IStiExportImage exportImage = component as IStiExportImage;
            if (exportImage != null)
            {
                float zoom = imageResolution;

                IStiExportImageExtended exportImageExtended = exportImage as IStiExportImageExtended;

                Image image = null;

                if (component.IsExportAsImage(StiExportFormat.Rtf))
                {
                    if (exportImageExtended != null && exportImageExtended.IsExportAsImage(StiExportFormat.Rtf))
                        image = exportImageExtended.GetImage(ref zoom, imageFormat);
                    else image = exportImage.GetImage(ref zoom);
                }

                if (image != null)
                {
                    sw.Write(GetImageString(image, zoom, absw, absh));
                    image.Dispose();
                }
            }
        }

        private void RenderRtf12(StiComponent component, long swPos = 0)
        {
            StiRichText rtf = component as StiRichText;
            if (rtf != null)
            {
                if (swPos > 0)
                {
                    sw.Flush();
                    long swPos2 = sw.BaseStream.Position;
                    byte[] buf = new byte[swPos2 - (swPos + 1)];
                    sw.BaseStream.Position = swPos + 1;
                    sw.BaseStream.Read(buf, 0, buf.Length);
                    string stPos = Encoding.ASCII.GetString(buf);

                    string st = GetRtfString(component);

                    st = st.Replace(@"\pard\par", "\x16");
                    st = st.Replace("\\pard", "");
                    st = st.Replace("\x16", "\\pard" + stPos + "\\par");

                    sw.Write(st);
                }
                else
                {
                    //string st = GetRtfString(component).Replace("\\pard", "\\par"); //corrected 10.01.2008
                    string st = GetRtfString(component).Replace("\\pard", ""); //corrected 2.04.2012
                    sw.Write(st);
                }
            }
        }

        private void RenderImage3(StiComponent component, StringBuilder dataStr, int absw, int absh)
        {
            IStiExportImageExtended exportImage = component as IStiExportImageExtended;
            if (exportImage != null)
            {
                float zoom = imageResolution;
                IStiExportImageExtended exportImageExtended = exportImage as IStiExportImageExtended;

                Image image = null;

                if (component.IsExportAsImage(StiExportFormat.Rtf))
                {
                    if (exportImageExtended != null && exportImageExtended.IsExportAsImage(StiExportFormat.Rtf))
                        image = exportImage.GetImage(ref zoom, imageFormat);
                    else image = exportImage.GetImage(ref zoom);
                }

                if (image != null)
                {
                    dataStr.Append(GetImageString(image, zoom, absw, absh));
                    image.Dispose();
                }
            }
        }

        private void RenderRtf3(StiComponent component, StringBuilder dataStr)
        {
            StiRichText rtf = component as StiRichText;
            if (rtf != null)
            {
                //corrected 10.01.2008
                string st = GetRtfString(component).Replace("\\pard", "\\par");
                dataStr.Append(st);
            }
        }

        private void RenderText3(StiComponent component, StringBuilder dataStr)
        {
            IStiText text = component as IStiText;
            IStiTextOptions textOpt = component as IStiTextOptions;
            if (text != null && (!component.IsExportAsImage(StiExportFormat.Rtf)))
            {
                string st = text.Text;
                if ((component as StiText) != null && (component as StiText).TextQuality == StiTextQuality.Wysiwyg && !string.IsNullOrEmpty(st) && st.EndsWith(StiTextRenderer.StiForceWidthAlignTag))
                {
                    st = st.Substring(0, st.Length - StiTextRenderer.StiForceWidthAlignTag.Length);
                }
                StringBuilder stInput = new StringBuilder(st);
                bool useRightToLeft =
                    ((textOpt != null) && (textOpt.TextOptions != null) && (textOpt.TextOptions.RightToLeft));
                StringBuilder sbOutput = UnicodeToRtfString(stInput, useRightToLeft);
                dataStr.Append(sbOutput);
            }
        }

        private void RenderTextFont3(StiComponent component, StringBuilder dataStr)
        {
            IStiFont mFont = component as IStiFont;
            if (mFont != null)
            {
                dataStr.Append("\\f");
                baseFontNumber = GetFontNumber(fontList, mFont.Font);
                dataStr.Append(baseFontNumber.ToString());
                dataStr.Append("\\fs");
                dataStr.Append(Math.Round(mFont.Font.SizeInPoints * 2));
                if (mFont.Font.Bold) dataStr.Append("\\b");
                if (mFont.Font.Italic) dataStr.Append("\\i");
                if (mFont.Font.Underline) dataStr.Append("\\ul");
                dataStr.Append(string.Format("\\sl{0}\\slmult1", Math.Round(240 * StiOptions.Export.RichText.LineSpacing * (component is StiText ? (component as StiText).LineSpacing : 1))));
                if (StiOptions.Export.RichText.SpaceBetweenCharacters != 0)
                {
                    dataStr.Append(string.Format("\\expndtw{0}", StiOptions.Export.RichText.SpaceBetweenCharacters));
                }
            }
        }

        private void RenderTextBrush3(StiComponent component, StringBuilder dataStr)
        {
            IStiTextBrush mTextBrush = component as IStiTextBrush;
            if (mTextBrush != null)
            {
                string tempColor = GetColorNumber(colorList, StiBrush.ToColor(mTextBrush.TextBrush));
                dataStr.Append("\\cf");
                dataStr.Append(tempColor);
            }
        }

        private void RenderBrush3(StiComponent component, StringBuilder dataStr)
        {
            IStiBrush mBrush = component as IStiBrush;
            if ((mBrush != null) && (!component.IsExportAsImage(StiExportFormat.Rtf)))
            {
                string tempColor = GetColorNumber(colorList, StiBrush.ToColor(mBrush.Brush));
                dataStr.Append("\\cbpat");
                dataStr.Append(tempColor);
            }
        }

        private void RenderBorder3(StiComponent component, StringBuilder dataStr)
        {
            IStiBorder mBorder = component as IStiBorder;
            if (mBorder != null && (!(component is IStiIgnoreBorderWhenExport)))
            {
                if (mBorder.Border is StiAdvancedBorder)
                {
                    StiAdvancedBorder advBorder = mBorder.Border as StiAdvancedBorder;
                    if (advBorder.IsLeftBorderSidePresent && (advBorder.LeftSide.Color.A != 0))
                    {
                        dataStr.Append("\\brdrl" + GetLineStyle(advBorder.LeftSide, colorList));
                    }
                    if (advBorder.IsRightBorderSidePresent && (advBorder.RightSide.Color.A != 0))
                    {
                        dataStr.Append("\\brdrr" + GetLineStyle(advBorder.RightSide, colorList));
                    }
                    if (advBorder.IsTopBorderSidePresent && (advBorder.TopSide.Color.A != 0))
                    {
                        dataStr.Append("\\brdrt" + GetLineStyle(advBorder.TopSide, colorList));
                    }
                    if (advBorder.IsBottomBorderSidePresent && (advBorder.BottomSide.Color.A != 0))
                    {
                        dataStr.Append("\\brdrb" + GetLineStyle(advBorder.BottomSide, colorList));
                    }
                }
                else
                {
                    Color tempColor = mBorder.Border.Color;
                    if (tempColor.A != 0)
                    {
                        string sb = GetLineStyle(new StiBorderSide(mBorder.Border.Color, mBorder.Border.Size, mBorder.Border.Style), colorList);
                        if ((mBorder.Border.Side & StiBorderSides.Left) > 0) dataStr.Append("\\brdrl" + sb);
                        if ((mBorder.Border.Side & StiBorderSides.Right) > 0) dataStr.Append("\\brdrr" + sb);
                        if ((mBorder.Border.Side & StiBorderSides.Top) > 0) dataStr.Append("\\brdrt" + sb);
                        if ((mBorder.Border.Side & StiBorderSides.Bottom) > 0) dataStr.Append("\\brdrb" + sb);
                    }
                }
            }

        }

        private void RenderStyle12(StiComponent component)
        {
            StiText text = component as StiText;
            if (useStyles && text != null)
            {
                int styleNumber = GetRtfStyleFromComponent(component);
                sw.Write("\\s{0}", styleNumber);
            }
        }
        #endregion

        #region write from matrix
        private void writeFromMatrix(int beginLine, int endLine, bool outHeadersAndFooters)
        {
            int maxCoordX = Matrix.CoordX.Count;
            //if (maxCoordX > 64) maxCoordX = 64;

            #region prepare columns width
            int[] RtfCoordX = new int[Matrix.CoordX.Count];
            for (int columnIndex = 1; columnIndex < Matrix.CoordX.Count; columnIndex++)
            {
                double value = (double)Matrix.CoordX.GetByIndex(columnIndex);
                RtfCoordX[columnIndex] = (int)Math.Round(value * HiToTwips);
            }
            #endregion

            #region write rows
            int[,] readyCells = new int[Matrix.CoordY.Count, Matrix.CoordX.Count];
            int[,] readyCellsVert = new int[Matrix.CoordY.Count, Matrix.CoordX.Count];

            string lastHeaderParentBandName = string.Empty;
            int counterParagraphLines = 0;

            StatusString = StiLocalization.Get("Export", "ExportingCreatingDocument");
            double progressScale = Math.Max(Matrix.CoordY.Count / 200f, 1f);
            int progressValue = 0;

            for (int rowIndex = 1; rowIndex < Matrix.CoordY.Count; rowIndex++)
            {
                int currentProgress = (int)(rowIndex / progressScale);
                if (currentProgress > progressValue)
                {
                    progressValue = currentProgress;
                    InvokeExporting(rowIndex, matrix.CoordY.Count, CurrentPassNumber, MaximumPassNumber);
                }

                if (matrix.isPaginationMode && matrix.HorizontalPageBreaksHash.ContainsKey(rowIndex)) continue; //skip pagination space

                int rowHeight = (int)Math.Round(((double)Matrix.CoordY.GetByIndex(rowIndex)
                    - (double)Matrix.CoordY.GetByIndex(rowIndex - 1)) * HiToTwips);

                bool needOutLine = ((rowIndex - 1) >= beginLine) && ((rowIndex - 1) <= endLine) && (counterParagraphLines == 0);
                if (counterParagraphLines > 0) counterParagraphLines--;

                if (outHeadersAndFooters == false)
                {
                    if ((matrix.LinePlacement[rowIndex - 1] == StiMatrix.StiTableLineInfo.PageHeader) ||
                        (matrix.LinePlacement[rowIndex - 1] == StiMatrix.StiTableLineInfo.PageFooter) ||
                        (matrix.LinePlacement[rowIndex - 1] == StiMatrix.StiTableLineInfo.Trash))
                    {
                        needOutLine = false;
                    }
                }

                double maxTopMargin = 100005;

                #region check for rtfparagraph, rtfnewpage and CanBreak
                //StiCell paragraphCell = null;
                ArrayList paragraphList = new ArrayList();
                bool needNewPage = false;
                bool needBreak = StiOptions.Export.RichText.UseCanBreakProperty;
                if (needOutLine)
                {
                    for (int columnIndex = 1; columnIndex < maxCoordX; columnIndex++)
                    {
                        StiCell cell = Matrix.Cells[rowIndex - 1, columnIndex - 1];
                        if ((readyCells[rowIndex, columnIndex] >= 0) && (cell != null) && (cell.Component != null))
                        {
                            if (cell.Component.TagValue != null)
                            {
                                string cellTag = cell.Component.TagValue.ToString().ToLowerInvariant();
                                if (cellTag.IndexOf("rtfparagraph", StringComparison.InvariantCulture) != -1)
                                {
                                    //paragraphCell = cell;
                                    paragraphList.Add(cell);
                                    needOutLine = false;
                                }
                                if (cellTag.IndexOf("rtfnewpage", StringComparison.InvariantCulture) != -1) needNewPage = true;
                            }
                            IStiBreakable breakable = cell.Component as IStiBreakable;
                            if ((breakable != null) && (!breakable.CanBreak))
                            {
                                needBreak = false;
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
                                }
                            }
                        }
                    }
                }
                if (needNewPage)
                {
                    sw.WriteLine("}\\fs2\\page\\par{");
                }
                if (!usePageHeadersAndFooters) needBreak = false;
                #endregion

                //if (maxTopMargin > 100000) maxTopMargin = 0;
                maxTopMargin *= HiToTwips;
                int maxTopMarginInt = (int)Math.Round(maxTopMargin);

                if (needOutLine)
                {
                    if ((usePageHeadersAndFooters) && (matrix.LinePlacement[rowIndex - 1] == StiMatrix.StiTableLineInfo.HeaderAP))
                    {
                        string headerName = matrix.ParentBandName[rowIndex - 1];
                        int symPos = headerName.IndexOf('\x1f');
                        if (symPos != -1)
                        {
                            headerName = headerName.Substring(0, symPos);
                        }
                        if (headerName != lastHeaderParentBandName)
                        {
                            lastHeaderParentBandName = headerName;
                            sw.WriteLine("}\\fs2\\par{");
                        }
                    }
                    bool notNeedLineHeightExactly = (usePageHeadersAndFooters && (matrix.LinePlacement[rowIndex - 1] != StiMatrix.StiTableLineInfo.Empty))
                        || !StiOptions.Export.RichText.LineHeightExactly;
                    sw.Write("\\trowd");
                    if (!usePageHeadersAndFooters || StiOptions.Export.RichText.ForceLineHeight)
                    {
                        sw.Write("\\trrh{0}{1}", (notNeedLineHeightExactly ? "" : "-"), rowHeight);
                    }
                    if (!needBreak)
                    {
                        //sw.Write("\\trrh{0}{1}", (notNeedLineHeightExactly ? "" : "-"), rowHeight);
                        sw.Write("\\trkeep");
                    }
                    sw.Write("\\ltrrow");
                    if ((usePageHeadersAndFooters) && (matrix.LinePlacement[rowIndex - 1] == StiMatrix.StiTableLineInfo.HeaderAP))
                    {
                        sw.Write("\\trhdr");
                    }
                    //commented 16.01.2008
                    //sw.Write("\\trgaph24");		//default cell margins
                }

                StringBuilder sbt = new StringBuilder();

                int curCoordX = 0;
                for (int columnIndex = 1; columnIndex < maxCoordX; columnIndex++)
                {
                    StiCell cell = Matrix.Cells[rowIndex - 1, columnIndex - 1];

                    if (readyCells[rowIndex, columnIndex] >= 0)
                    {
                        curCoordX++;
                        sbt.Append("{");

                        #region cells exists
                        if (cell != null)
                        {
                            StiCellStyle style = cell.CellStyle;
                            bool mRightToLeft = false;
                            if (style.TextOptions != null) mRightToLeft = style.TextOptions.RightToLeft;

                            #region Style
                            Font mFont = style.Font;
                            baseFontNumber = GetFontNumber(fontList, mFont);

                            if (useStyles)
                            {
                                int styleNumber = GetRtfStyleFromComponent(cell.Component);
                                sbt.Append(string.Format("\\s{0}", styleNumber));
                            }

                            if (mRightToLeft)
                            {
                                sbt.Append("\\rtlpar");
                            }
                            else
                            {
                                sbt.Append("\\ltrpar");
                            }

                            sbt.Append(MakeHorAlignString(style.HorAlignment, mRightToLeft));

                            string tempColor = GetColorNumber(colorList, style.TextColor);
                            sbt.Append(string.Format("\\cf{0}", tempColor));

                            sbt.Append(string.Format("\\f{0}", baseFontNumber));
                            sbt.Append(string.Format("\\fs{0}", Math.Round(mFont.SizeInPoints * 2)));
                            if (mFont.Bold) sbt.Append("\\b");
                            if (mFont.Italic) sbt.Append("\\i");
                            if (mFont.Underline) sbt.Append("\\ul");
                            sbt.Append(string.Format("\\sl{0}\\slmult1", Math.Round(240 * StiOptions.Export.RichText.LineSpacing * (cell.Component is StiText ? (cell.Component as StiText).LineSpacing : 1))));
                            if (StiOptions.Export.RichText.SpaceBetweenCharacters != 0)
                            {
                                sbt.Append(string.Format("\\expndtw{0}", StiOptions.Export.RichText.SpaceBetweenCharacters));
                            }
                            #endregion

                            //add bookmark
                            string bkm = Matrix.Bookmarks[rowIndex - 1, columnIndex - 1];
                            if ((bkm != null) && (needOutLine) && (!usedBookmarks.ContainsKey(bkm)))
                            {
                                usedBookmarks[bkm] = bkm;
                                string stRef = ConvertStringToBookmark(bkm);
                                sbt.Append(UnicodeToRtfString(new StringBuilder('\x1c' + stRef + '\x1d' + stRef + '\x1e'), false));
                            }

                            if (!(cell.Component is StiRichText))
                            {
                                #region Text
                                //								IStiText text = cell.Component as IStiText;
                                string text = cell.Text;
                                //								if (text != null && (!cell.Component.IsExportAsImage))
                                if ((text != null) && (text.Length > 0) && (!cell.Component.IsExportAsImage(StiExportFormat.Rtf)))
                                {
                                    StiText stiText = cell.Component as StiText;

                                    //StringBuilder stInput = new StringBuilder(text.Text);
                                    StringBuilder stInput = new StringBuilder(text);

                                    if ((stiText != null) && (stiText.CheckAllowHtmlTags()))
                                    {
                                        stInput = new StringBuilder(ConvertTextWithHtmlTagsToRtfText(stiText, text));
                                    }

                                    if (usePageHeadersAndFooters)
                                    {
                                        string expr = cell.Component.TagValue as string;
                                        if ((expr != null) && (expr.Length > 0))
                                        {
                                            if (StiOptions.Export.RichText.UsePageRefField && bookmarkList.ContainsKey(expr))
                                            {
                                                stInput = new StringBuilder('\x1a' + "PAGEREF " + ConvertStringToBookmark(expr) + '\x1b' + cell.Text + "\x1e\x1e\x1e");
                                            }
                                            else
                                            {
                                                if (expr.ToLowerInvariant().IndexOf("rtfnewpage", StringComparison.InvariantCulture) == -1 &&
                                                    (expr.IndexOf("#PageNumber#", StringComparison.InvariantCulture) != -1 ||
                                                     expr.IndexOf("#TotalPageCount#", StringComparison.InvariantCulture) != -1 ||
                                                     expr.IndexOf("#PageRef", StringComparison.InvariantCulture) != -1))
                                                {
                                                    stInput = new StringBuilder(expr);
                                                    stInput.Replace("#PageNumber#", "\x18");
                                                    stInput.Replace("#TotalPageCount#", "\x19");
                                                    //stInput.Replace("#PageNofM#", StiLocalization.SystemVariables.PageNofM);

                                                    int pos = 0;
                                                    while ((pos = expr.IndexOf("#PageRef", pos, StringComparison.InvariantCulture)) != -1)
                                                    {
                                                        int pos2 = pos + 8;
                                                        while ((pos2 < expr.Length) && (expr[pos2] != '#')) pos2++;
                                                        string stRef = ConvertStringToBookmark(expr.Substring(pos + 8, pos2 - (pos + 8)));
                                                        stInput.Replace(expr.Substring(pos, pos2 + 1 - pos), '\x1a' + "PAGEREF " + expr + '\x1b' + cell.Text + "\x1e\x1e\x1e");
                                                        pos += 8;
                                                    }

                                                }
                                            }
                                        }
                                    }

                                    #region Hyperlink
                                    if (cell.Component.HyperlinkValue != null)
                                    {
                                        string hyperlink = cell.Component.HyperlinkValue.ToString().Trim();
                                        if (hyperlink.Length > 0 && !hyperlink.StartsWith("javascript:"))
                                        {
                                            string description = stInput.ToString();
                                            if (description.Length == 0) description = hyperlink;

                                            if (hyperlink.StartsWith("#", StringComparison.InvariantCulture))
                                            {
                                                hyperlink = hyperlink.Substring(1);
                                                stInput = new StringBuilder('\x1a' + "HYPERLINK \\l \"" + ConvertStringToBookmark(hyperlink) + '\"' + '\x1b' + description + "\x1e\x1e\x1e");
                                            }
                                            else
                                            {
                                                stInput = new StringBuilder('\x1a' + "HYPERLINK \"" + hyperlink + '\"' + '\x1b' + description + "\x1e\x1e\x1e");
                                            }
                                        }
                                    }
                                    #endregion

                                    StringBuilder sbOutput = UnicodeToRtfString(stInput, mRightToLeft);
                                    sbt.Append(string.Format(" {0}", sbOutput));
                                }
                                #endregion
                            }
                            if (cell.Component is StiRichText)
                            {
                                #region richtext
                                StiRichText rtf = cell.Component as StiRichText;
                                if (rtf != null)
                                {
                                    //sbt.Append(GetRtfString(cell.Component));     //fix 25.11.2008
                                    StringBuilder stInput = new StringBuilder(ReplacePardInRtf(GetRtfString(cell.Component)));

                                    if (cell.Component.HyperlinkValue != null)
                                    {
                                        string hyperlink = cell.Component.HyperlinkValue.ToString().Trim();
                                        if (hyperlink.Length > 0 && !hyperlink.StartsWith("javascript:"))
                                        {
                                            string description = stInput.ToString();
                                            if (description.Length == 0) description = hyperlink;

                                            if (hyperlink.StartsWith("#", StringComparison.InvariantCulture))
                                            {
                                                hyperlink = hyperlink.Substring(1);
                                                stInput = new StringBuilder('\x1a' + "HYPERLINK \\\\l \"" + ConvertStringToBookmark(hyperlink) + '\"' + '\x1b' + description + "\x1e\x1e\x1e");
                                            }
                                            else
                                            {
                                                stInput = new StringBuilder('\x1a' + "HYPERLINK \"" + hyperlink + '\"' + '\x1b' + description + "\x1e\x1e\x1e");
                                            }
                                        }
                                    }

                                    //stInput.Replace("\x0a", "\\par ");    // fix 2012.07.23

                                    stInput.Replace("\x17", "\\");
                                    stInput.Replace("\x18", "{\\field{\\*\\fldinst {PAGE}}}");
                                    stInput.Replace("\x19", "{\\field{\\*\\fldinst {NUMPAGES}}}");

                                    //stInput.Replace("\x1a", "{\\field{\\*\\fldinst {PAGEREF ");
                                    stInput.Replace("\x1a", "{\\field{\\*\\fldinst {");
                                    stInput.Replace("\x1b", " }}{\\fldrslt {");
                                    stInput.Replace("\x1c", "{\\*\\bkmkstart ");
                                    stInput.Replace("\x1d", "}{\\*\\bkmkend ");
                                    stInput.Replace("\x1e", "}");

                                    sbt.Append(stInput);
                                }
                                #endregion
                            }
                            else
                            {
                                #region export image
                                IStiExportImage exportImage = cell.Component as IStiExportImage;
                                if (exportImage != null)
                                {
                                    float zoom = imageResolution;
                                    IStiExportImageExtended exportImageExtended = exportImage as IStiExportImageExtended;

                                    Image image = null;
                                    if (cell.Component.IsExportAsImage(StiExportFormat.Rtf))
                                    {
                                        if (exportImageExtended != null && exportImageExtended.IsExportAsImage(StiExportFormat.Rtf))
                                            image = exportImageExtended.GetImage(ref zoom, imageFormat);
                                        else image = exportImage.GetImage(ref zoom);
                                    }

                                    if (image != null)
                                    {
                                        Image img = Matrix.GetRealImageData(cell, image);
                                        if (img != null) image = img;

                                        string stImage = GetImageString(image, zoom,
                                            (int)Math.Round(((double)Matrix.CoordX.GetByIndex(columnIndex + cell.Width)
                                            - (double)Matrix.CoordX.GetByIndex(columnIndex - 1)) * HiToTwips),
                                            (int)Math.Round(((double)Matrix.CoordY.GetByIndex(rowIndex + cell.Height)
                                            - (double)Matrix.CoordY.GetByIndex(rowIndex - 1)) * HiToTwips));
                                        image.Dispose();

                                        #region Hyperlink
                                        if (cell.Component.HyperlinkValue != null)
                                        {
                                            string hyperlink = cell.Component.HyperlinkValue.ToString().Trim();
                                            if (hyperlink.Length > 0 && !hyperlink.StartsWith("javascript:"))
                                            {
                                                string description = stImage;
                                                if (hyperlink.StartsWith("#", StringComparison.InvariantCulture))
                                                {
                                                    hyperlink = hyperlink.Substring(1);
                                                    stImage = "{\\field{\\*\\fldinst {" + "HYPERLINK \\\\l \"" + ConvertStringToBookmark(hyperlink) + '\"' + " }}{\\fldrslt {" + description + "}}}";
                                                }
                                                else
                                                {
                                                    stImage = "{\\field{\\*\\fldinst {" + "HYPERLINK \"" + hyperlink + '\"' + " }}{\\fldrslt {" + description + "}}}";
                                                }
                                            }
                                        }
                                        #endregion

                                        sbt.Append(stImage);

                                        sbt.Append("\\f0\\fs2");
                                    }
                                }
                                #endregion
                            }

                            #region Range
                            if (readyCellsVert[rowIndex, columnIndex] != 2)
                            {
                                for (int yy = 0; yy <= cell.Height; yy++)
                                {
                                    for (int xx = 0; xx <= cell.Width; xx++)
                                    {
                                        readyCells[rowIndex + yy, columnIndex + xx] = -1;
                                    }
                                    readyCells[rowIndex + yy, columnIndex] = cell.Width;
                                }
                                if (cell.Height > 0)
                                {
                                    readyCellsVert[rowIndex, columnIndex] = 1;
                                    for (int yy = 1; yy <= cell.Height; yy++)
                                    {
                                        readyCellsVert[rowIndex + yy, columnIndex] = 2;
                                    }
                                }
                            }
                            #endregion

                        }
                        #endregion

                        #region cell not exist
                        else
                        {
                            if (readyCellsVert[rowIndex, columnIndex] == 0 && StiOptions.Export.RichText.ForceEmptyCellsOptimization)
                            {
                                string baseSt = null;
                                StiBorderSides baseSides = RenderBorder2TableGetValues(rowIndex, columnIndex, ref baseSt);
                                if ((baseSides & StiBorderSides.Right) == 0)
                                {
                                    int newWidth = 0;
                                    while ((columnIndex + newWidth + 1 < maxCoordX) && (Matrix.Cells[rowIndex - 1, columnIndex + newWidth + 1 - 1] == null) &&
                                        (readyCells[rowIndex, columnIndex + newWidth + 1] == 0) && (readyCellsVert[rowIndex, columnIndex + newWidth + 1] == 0))
                                    {
                                        string newSt = null;
                                        StiBorderSides newSides = RenderBorder2TableGetValues(rowIndex, columnIndex + newWidth + 1, ref newSt);
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
                                            readyCells[rowIndex, columnIndex + xx] = -1;
                                        }
                                        readyCells[rowIndex, columnIndex] = newWidth;
                                    }
                                }
                            }
                        }
                        #endregion

                        sbt.Append("\\cell}");
                    }
                    if (curCoordX >= 64) break;
                }

                if (needOutLine)
                {
                    #region columns data
                    curCoordX = 0;
                    for (int columnIndex = 1; columnIndex < maxCoordX; columnIndex++)
                    {
                        if (readyCellsVert[rowIndex, columnIndex] == 1) sw.Write("\\clvmgf");
                        if (readyCellsVert[rowIndex, columnIndex] == 2) sw.Write("\\clvmrg");
                        if (readyCells[rowIndex, columnIndex] >= 0)
                        {
                            curCoordX++;
                            if (readyCellsVert[rowIndex, columnIndex] == 2)
                            {
                                #region repeat merged cells
                                int tempRowIndex = rowIndex;
                                while (readyCellsVert[tempRowIndex, columnIndex] == 2) tempRowIndex--;
                                StiCell cell = Matrix.Cells[tempRowIndex - 1, columnIndex - 1];
                                //								RenderBorder2Table(cell.Component);
                                RenderBorder2Table(tempRowIndex, columnIndex, cell.Height, cell.Width, false);
                                #endregion
                            }
                            else
                            {
                                #region render cell
                                StiCell cell = Matrix.Cells[rowIndex - 1, columnIndex - 1];
                                if (cell != null)
                                {
                                    StiCellStyle style = cell.CellStyle;

                                    RenderVerAlign2(cell.Component);
                                    RenderBorder2Table(rowIndex, columnIndex, cell.Height, cell.Width, false);
                                    RenderTextAngle2(cell.Component);

                                    #region Brush and Margins
                                    if (!cell.Component.IsExportAsImage(StiExportFormat.Rtf))
                                    {
                                        Color tempColor = style.Color;
                                        if (tempColor.A != 0)
                                        {
                                            string tempColorSt = GetColorNumber(colorList, tempColor);
                                            sw.Write("\\clcbpat{0}", tempColorSt);
                                        }

                                        StiText mText = cell.Component as StiText;
                                        StiRichText mRichText = cell.Component as StiRichText;
                                        if ((mText != null) || (mRichText != null))
                                        {
                                            int corr = StiOptions.Export.RichText.RightMarginCorrection;
                                            int mLeft = 0;
                                            int mRight = 0;
                                            int mTop = 0;
                                            //int mBottom = 0;
                                            if (mText != null)
                                            {
                                                mLeft = (int)Math.Round(mText.Margins.Left * HiToTwips);
                                                mRight = (int)Math.Round((mText.Margins.Right - corr) * HiToTwips);
                                                mTop = (int)Math.Round(mText.Margins.Top * HiToTwips);
                                                //mBottom = (int)Math.Round(mText.Margins.Bottom * HiToTwips);
                                            }
                                            if (mRichText != null)
                                            {
                                                mLeft = (int)Math.Round(mRichText.Margins.Left * HiToTwips);
                                                mRight = (int)Math.Round((mRichText.Margins.Right - corr) * HiToTwips);
                                                mTop = (int)Math.Round(mRichText.Margins.Top * HiToTwips);
                                                //mBottom = (int)Math.Round(mRichText.Margins.Bottom * HiToTwips);
                                            }
                                            if (mTop > maxTopMarginInt) mTop = maxTopMarginInt;
                                            if (mLeft > 0)
                                            {
                                                sw.Write("\\clpadt{0}\\clpadft3", mLeft);   //left and top are reversed - it's bug of MS Word
                                            }
                                            if (mRight > 0)
                                            {
                                                sw.Write("\\clpadr{0}\\clpadfr3", mRight);
                                            }
                                            if (mTop > 0)
                                            {
                                                sw.Write("\\clpadl{0}\\clpadfl3", mTop);
                                            }
                                            //if (mBottom > 0)
                                            //{
                                            //    sw.Write("\\clpadb{0}\\clpadfb3", mBottom);
                                            //}
                                        }
                                    }
                                    #endregion
                                }
                                else
                                {
                                    RenderBorder2Table(rowIndex, columnIndex, 0, readyCells[rowIndex, columnIndex], false, usePageHeadersAndFooters);
                                }
                                #endregion
                            }
                        }
                        if (readyCells[rowIndex, columnIndex] >= 0)
                        {
                            sw.Write("\\cellx{0}", RtfCoordX[columnIndex + readyCells[rowIndex, columnIndex]]);
                        }
                        if (curCoordX >= 64) break;
                    }
                    #endregion

                    sw.Write("\\pard\\intbl ");

                    #region write table row data to stream
                    byte[] tbb = new byte[sbt.Length];
                    for (int tempI = 0; tempI < sbt.Length; tempI++)
                    {
                        tbb[tempI] = (byte)sbt[tempI];
                    }
                    sw.Flush();
                    sw2.Write(tbb, 0, tbb.Length);
                    #endregion

                    sw.WriteLine("\\row");
                }

                //if (paragraphCell != null)
                foreach (StiCell paragraphCell in paragraphList)
                {
                    sw.Write("\\pard{");
                    sbt = new StringBuilder();

                    counterParagraphLines = paragraphCell.Height;

                    #region Text
                    string text = paragraphCell.Text;
                    if (!(paragraphCell.Component is StiRichText) && (text != null) && (text.Length > 0) && (!paragraphCell.Component.IsExportAsImage(StiExportFormat.Rtf)))
                    {
                        #region Style
                        StiCellStyle style = paragraphCell.CellStyle;
                        bool mRightToLeft = false;
                        if (style.TextOptions != null) mRightToLeft = style.TextOptions.RightToLeft;
                        Font mFont = style.Font;
                        baseFontNumber = GetFontNumber(fontList, mFont);
                        if (useStyles)
                        {
                            int styleNumber = GetRtfStyleFromComponent(paragraphCell.Component);
                            sbt.Append(string.Format("\\s{0}", styleNumber));
                        }
                        if (mRightToLeft)
                        {
                            sbt.Append("\\rtlpar");
                        }
                        else
                        {
                            sbt.Append("\\ltrpar");
                        }
                        sbt.Append(MakeHorAlignString(style.HorAlignment, mRightToLeft));
                        string tempColor = GetColorNumber(colorList, style.TextColor);
                        sbt.Append(string.Format("\\cf{0}", tempColor));
                        sbt.Append(string.Format("\\f{0}", baseFontNumber));
                        sbt.Append(string.Format("\\fs{0}", Math.Round(mFont.SizeInPoints * 2)));
                        if (mFont.Bold) sbt.Append("\\b");
                        if (mFont.Italic) sbt.Append("\\i");
                        if (mFont.Underline) sbt.Append("\\ul");
                        sbt.Append(string.Format("\\sl{0}\\slmult1", Math.Round(240 * StiOptions.Export.RichText.LineSpacing * (paragraphCell.Component is StiText ? (paragraphCell.Component as StiText).LineSpacing : 1))));
                        if (StiOptions.Export.RichText.SpaceBetweenCharacters != 0)
                        {
                            sbt.Append(string.Format("\\expndtw{0}", StiOptions.Export.RichText.SpaceBetweenCharacters));
                        }
                        #endregion

                        StringBuilder stInput = new StringBuilder(text);

                        StiText stiText = paragraphCell.Component as StiText;
                        if ((stiText != null) && (stiText.CheckAllowHtmlTags()))
                        {
                            stInput = new StringBuilder(ConvertTextWithHtmlTagsToRtfText(stiText, text));
                        }

                        #region Hyperlink
                        if (paragraphCell.Component.HyperlinkValue != null)
                        {
                            string hyperlink = paragraphCell.Component.HyperlinkValue.ToString().Trim();
                            if (hyperlink.Length > 0 && !hyperlink.StartsWith("javascript:"))
                            {
                                string description = stInput.ToString();
                                if (description.Length == 0) description = hyperlink;

                                if (hyperlink.StartsWith("#", StringComparison.InvariantCulture))
                                {
                                    hyperlink = hyperlink.Substring(1);
                                    stInput = new StringBuilder('\x1a' + "HYPERLINK \\\\l \"" + ConvertStringToBookmark(hyperlink) + '\"' + '\x1b' + description + "\x1e\x1e\x1e");
                                }
                                else
                                {
                                    stInput = new StringBuilder('\x1a' + "HYPERLINK \"" + hyperlink + '\"' + '\x1b' + description + "\x1e\x1e\x1e");
                                }
                            }
                        }
                        #endregion

                        StringBuilder sbOutput = UnicodeToRtfString(stInput, mRightToLeft);
                        sbt.Append(string.Format(" {0}", sbOutput));
                    }
                    #endregion

                    if (paragraphCell.Component is StiRichText)
                    {
                        StiRichText rtf = paragraphCell.Component as StiRichText;
                        if (rtf != null)
                        {
                            //sbt.Append(GetRtfString(paragraphCell.Component));  //fix 25.11.2008
                            //sbt.Append(ReplacePardInRtf(GetRtfString(paragraphCell.Component)));
                            //sbt.Append(GetRtfString(paragraphCell.Component));
                            StringBuilder stInput = new StringBuilder(GetRtfString(paragraphCell.Component));

                            if (paragraphCell.Component.HyperlinkValue != null)
                            {
                                string hyperlink = paragraphCell.Component.HyperlinkValue.ToString().Trim();
                                if (hyperlink.Length > 0 && !hyperlink.StartsWith("javascript:"))
                                {
                                    string description = stInput.ToString();
                                    if (description.Length == 0) description = hyperlink;

                                    if (hyperlink.StartsWith("#", StringComparison.InvariantCulture))
                                    {
                                        hyperlink = hyperlink.Substring(1);
                                        stInput = new StringBuilder('\x1a' + "HYPERLINK \\\\l \"" + ConvertStringToBookmark(hyperlink) + '\"' + '\x1b' + description + "\x1e\x1e\x1e");
                                    }
                                    else
                                    {
                                        stInput = new StringBuilder('\x1a' + "HYPERLINK \"" + hyperlink + '\"' + '\x1b' + description + "\x1e\x1e\x1e");
                                    }
                                }
                            }

                            //stInput.Replace("\x0a", "\\par ");    //fix 2012.07.23

                            stInput.Replace("\x17", "\\");
                            stInput.Replace("\x18", "{\\field{\\*\\fldinst {PAGE}}}");
                            stInput.Replace("\x19", "{\\field{\\*\\fldinst {NUMPAGES}}}");

                            //stInput.Replace("\x1a", "{\\field{\\*\\fldinst {PAGEREF ");
                            stInput.Replace("\x1a", "{\\field{\\*\\fldinst {");
                            stInput.Replace("\x1b", " }}{\\fldrslt {");
                            stInput.Replace("\x1c", "{\\*\\bkmkstart ");
                            stInput.Replace("\x1d", "}{\\*\\bkmkend ");
                            stInput.Replace("\x1e", "}");

                            sbt.Append(stInput);
                        }
                    }
                    else
                    {
                        #region export image
                        IStiExportImage exportImage = paragraphCell.Component as IStiExportImage;
                        if (exportImage != null)
                        {
                            float zoom = imageResolution;
                            IStiExportImageExtended exportImageExtended = exportImage as IStiExportImageExtended;

                            Image image = null;
                            if (paragraphCell.Component.IsExportAsImage(StiExportFormat.Rtf))
                            {
                                if (exportImageExtended != null && exportImageExtended.IsExportAsImage(StiExportFormat.Rtf))
                                    image = exportImageExtended.GetImage(ref zoom, imageFormat);
                                else image = exportImage.GetImage(ref zoom);
                            }

                            if (image != null)
                            {
                                Image img = Matrix.GetRealImageData(paragraphCell, image);
                                if (img != null) image = img;

                                string stImage = GetImageString(image, zoom,
                                    (int)Math.Round(((double)Matrix.CoordX.GetByIndex(paragraphCell.Left + 1 + paragraphCell.Width)
                                    - (double)Matrix.CoordX.GetByIndex(paragraphCell.Left)) * HiToTwips),
                                    (int)Math.Round(((double)Matrix.CoordY.GetByIndex(paragraphCell.Top + 1 + paragraphCell.Height)
                                    - (double)Matrix.CoordY.GetByIndex(paragraphCell.Top)) * HiToTwips));
                                image.Dispose();

                                #region Hyperlink
                                if (paragraphCell.Component.HyperlinkValue != null)
                                {
                                    string hyperlink = paragraphCell.Component.HyperlinkValue.ToString().Trim();
                                    if (hyperlink.Length > 0 && !hyperlink.StartsWith("javascript:"))
                                    {
                                        string description = stImage;
                                        if (hyperlink.StartsWith("#", StringComparison.InvariantCulture))
                                        {
                                            hyperlink = hyperlink.Substring(1);
                                            stImage = "{\\field{\\*\\fldinst {" + "HYPERLINK \\\\l \"" + ConvertStringToBookmark(hyperlink) + '\"' + " }}{\\fldrslt {" + description + "}}}";
                                        }
                                        else
                                        {
                                            stImage = "{\\field{\\*\\fldinst {" + "HYPERLINK \"" + hyperlink + '\"' + " }}{\\fldrslt {" + description + "}}}";
                                        }
                                    }
                                }
                                #endregion

                                StiTextHorAlignment horAlignment = paragraphCell.CellStyle.HorAlignment;
                                if ((paragraphCell.Component as IStiTextHorAlignment) == null)
                                {
                                    IStiHorAlignment horAlignComp = paragraphCell.Component as IStiHorAlignment;
                                    if (horAlignComp != null)
                                    {
                                        if (horAlignComp.HorAlignment == StiHorAlignment.Left) horAlignment = StiTextHorAlignment.Left;
                                        if (horAlignComp.HorAlignment == StiHorAlignment.Center) horAlignment = StiTextHorAlignment.Center;
                                        if (horAlignComp.HorAlignment == StiHorAlignment.Right) horAlignment = StiTextHorAlignment.Right;
                                    }
                                }
                                bool mRightToLeft = false;
                                if (paragraphCell.CellStyle.TextOptions != null) mRightToLeft = paragraphCell.CellStyle.TextOptions.RightToLeft;
                                sbt.Append(MakeHorAlignString(horAlignment, mRightToLeft));

                                sbt.Append(stImage);
                            }
                        }
                        sbt.Append("\\f0\\fs2");
                        #endregion
                    }

                    #region write table row data to stream
                    byte[] tbb = new byte[sbt.Length];
                    for (int tempI = 0; tempI < sbt.Length; tempI++)
                    {
                        tbb[tempI] = (byte)sbt[tempI];
                    }
                    sw.Flush();
                    sw2.Write(tbb, 0, tbb.Length);
                    #endregion

                    if (!sbt.ToString().Trim().EndsWith("\\par"))
                    {
                        sw.Write("\\par");
                    }
                    sw.WriteLine("}");
                }
            }
            #endregion

        }
        #endregion

        /// <summary>
        /// Exports a rendered report to the RTF file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="fileName">A name of the file for exporting of a rendered report.</param>
        public void ExportRtf(StiReport report, string fileName)
        {
            StiFileUtils.ProcessReadOnly(fileName);
            FileStream stream = new FileStream(fileName, FileMode.Create);
            ExportRtf(report, stream);
            stream.Flush();
            stream.Close();
        }


        /// <summary>
        /// Exports a rendered report to the RTF file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream for the export of a document.</param>
        public void ExportRtf(StiReport report, Stream stream)
        {
            StiRtfExportSettings settings = new StiRtfExportSettings();
            ExportRtf(report, stream, settings);
        }


        /// <summary>
        /// Exports a rendered report to the RTF file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream for export of a document.</param>
        /// <param name="exportMode">A parameter for setting exporting modes of a document.</param>
        public void ExportRtf(StiReport report, Stream stream, StiRtfExportMode exportMode)
        {
            StiRtfExportSettings settings = new StiRtfExportSettings();

            settings.ExportMode = exportMode;

            ExportRtf(report, stream, settings);
        }


        /// <summary>
        /// Exports a rendered report to the RTF file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream for export of a document.</param>
        /// <param name="exportMode">A parameter for setting exporting modes of a document.</param>
        /// <param name="pageRange">Describes range of pages of the document for the export.</param>
        public void ExportRtf(StiReport report, Stream stream, StiRtfExportMode exportMode, StiPagesRange pageRange)
        {
            StiRtfExportSettings settings = new StiRtfExportSettings();

            settings.PageRange = pageRange;
            settings.ExportMode = exportMode;

            ExportRtf(report, stream, settings);
        }


        /// <summary>
        /// Exports a rendered report to the RTF file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream for export of a document.</param>
        /// <param name="exportMode">A parameter for setting exporting modes of a document.</param>
        /// <param name="pageRange">Describes range of pages of the document for the export.</param>
        /// <param name="usePageHeadersAndFooters"></param>
        public void ExportRtf(StiReport report, Stream stream, StiRtfExportMode exportMode, StiPagesRange pageRange, bool usePageHeadersAndFooters)
        {
            StiRtfExportSettings settings = new StiRtfExportSettings();

            settings.PageRange = pageRange;
            settings.ExportMode = exportMode;
            settings.UsePageHeadersAndFooters = usePageHeadersAndFooters;

            ExportRtf(report, stream, settings);
        }


        /// <summary>
        /// Exports a rendered report to the RTF file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream for export of a document.</param>
        /// <param name="codePage">A code page of the exported document.</param>
        /// <param name="exportMode">A parameter for setting exporting modes of a document.</param>
        /// <param name="pageRange">Describes range of pages of the document for the export.</param>
        public void ExportRtf(StiReport report, Stream stream, int codePage, StiRtfExportMode exportMode, StiPagesRange pageRange)
        {
            StiRtfExportSettings settings = new StiRtfExportSettings();

            settings.PageRange = pageRange;
            settings.CodePage = codePage;
            settings.ExportMode = exportMode;

            ExportRtf(report, stream, settings);
        }


        /// <summary>
        /// Exports a rendered report to the RTF file.
        /// </summary>
        /// <param name="report">A report which is to be exported.</param>
        /// <param name="stream">A stream for export of a document.</param>
        /// <param name="codePage">A code page of the exported document.</param>
        /// <param name="exportMode">A parameter for setting exporting modes of a document.</param>
        /// <param name="pageRange">Describes range of pages of the document for the export.</param>
        /// <param name="usePageHeadersAndFooters"></param>
        public void ExportRtf(StiReport report, Stream stream, int codePage, StiRtfExportMode exportMode, StiPagesRange pageRange,
            bool usePageHeadersAndFooters)
        {
            StiRtfExportSettings settings = new StiRtfExportSettings();

            settings.PageRange = pageRange;
            settings.CodePage = codePage;
            settings.ExportMode = exportMode;
            settings.UsePageHeadersAndFooters = usePageHeadersAndFooters;

            ExportRtf(report, stream, settings);
        }


        public void ExportRtf(StiReport report, Stream stream, StiRtfExportSettings settings)
        {
            try
            {
                //StiExportUtils.DisableFontSmoothing();
                ExportRtf1(report, stream, settings);
            }
            finally
            {
                StiExportUtils.EnableFontSmoothing(report);
            }
        }

        /// <summary>
        /// Exports a rendered report to the RTF file.
        /// </summary>
        private void ExportRtf1(StiReport report, Stream stream, StiRtfExportSettings settings)
        {
            StiLogService.Write(this.GetType(), "Export report to Rtf format");

#if NETSTANDARD || NETCOREAPP
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif

            #region Read settings
            if (settings == null)
                throw new ArgumentNullException("The 'settings' argument cannot be equal in null.");

            int codePage = settings.CodePage;
            StiRtfExportMode exportMode = settings.ExportMode;
            StiPagesRange pageRange = settings.PageRange;
            usePageHeadersAndFooters = settings.UsePageHeadersAndFooters;
            removeEmptySpaceAtBottom = settings.RemoveEmptySpaceAtBottom;
            imageResolution = settings.ImageResolution;
            imageQuality = settings.ImageQuality;
            bool storeImagesAsPng = settings.StoreImagesAsPng;
            #endregion

            unicodeMapArray = new int[65536];
            codePageToFont = new byte[StiEncode.codePagesTableSize + 1];
            fontToCodePages = new int[StiEncode.codePagesTableSize + 1];
            charsetCount = 0;

            useStyles = true;

            if (imageQuality < 0) imageQuality = 0;
            if (imageQuality > 1) imageQuality = 1;
            if (imageResolution < 10) imageResolution = 10;
            imageResolution = imageResolution / 100f;
            imageFormat = storeImagesAsPng ? StiExportFormat.ImagePng : StiExportFormat.Rtf;

            #region initialization
            // A suitable variant for the Designer tweak - grid may be exactly set.
            const double insideScaleY = 12;
            double scaleX = 1.0;
            double scaleY = 1.0;

            colorList = new ArrayList();
            fontList = new ArrayList();
            styleList = new ArrayList();
            sw = new StreamWriter(stream, Encoding.GetEncoding(1252));  //Latin-1
            sw2 = stream;

            //clear UnicodeMapArray
            for (int index = 0; index < 65536; index++) unicodeMapArray[index] = 0;

            imageCodec = StiImageCodecInfo.GetImageCodec("image/jpeg");

            bookmarkList = new Hashtable();
            usedBookmarks = new Hashtable();

            #endregion

            CurrentPassNumber = 0;
            MaximumPassNumber = (exportMode == StiRtfExportMode.Table ? 4 : 2) + (StiOptions.Export.RichText.DivideSegmentPages ? 1 : 0);

            //			StiEncode sttt = new StiEncode();
            StiPagesCollection pages = pageRange.GetSelectedPages(report.RenderedPages);
            if (StiOptions.Export.RichText.DivideSegmentPages)
            {
                pages = StiSegmentPagesDivider.Divide(pages, this);
                CurrentPassNumber++;
            }

            StatusString = StiLocalization.Get("Report", "PreparingReport");

            #region Scan for assemble Color and Font table
            foreach (StiPage page in pages)
            {
                pages.GetPage(page);
                InvokeExporting(page, pages, CurrentPassNumber, MaximumPassNumber);
                foreach (StiComponent component in page.Components)
                {
                    if (component.Enabled)
                    {
                        IStiTextBrush mTextBrush = component as IStiTextBrush;
                        if (mTextBrush != null) GetColorNumberInt(colorList, StiBrush.ToColor(mTextBrush.TextBrush));

                        IStiBrush mBrush = component as IStiBrush;
                        if (mBrush != null) GetColorNumber(colorList, StiBrush.ToColor(mBrush.Brush));

                        IStiBorder mBorder = component as IStiBorder;
                        if (mBorder != null && (!(component is IStiIgnoreBorderWhenExport)))
                        {
                            StiAdvancedBorder advBorder = mBorder.Border as StiAdvancedBorder;
                            if (advBorder != null)
                            {
                                GetColorNumber(colorList, advBorder.LeftSide.Color);
                                GetColorNumber(colorList, advBorder.RightSide.Color);
                                GetColorNumber(colorList, advBorder.TopSide.Color);
                                GetColorNumber(colorList, advBorder.BottomSide.Color);
                            }
                            else
                            {
                                GetColorNumber(colorList, mBorder.Border.Color);
                            }
                        }

                        StiShape shape = component as StiShape;
                        if (shape != null) GetColorNumber(colorList, shape.BorderColor);

                        IStiFont mFont = component as IStiFont;
                        if (mFont != null) GetFontNumber(fontList, mFont.Font);

                        StiRichText rtf = component as StiRichText;
                        if (rtf != null)
                        {
                            GetRtfString(component);
                            GetColorNumber(colorList, rtf.BackColor);
                        }

                        //make map of the unicode symbols
                        StiText text = component as StiText;
                        if (text != null)
                        {
                            StringBuilder sbbb = new StringBuilder(text.Text);
                            if (text.CheckAllowHtmlTags())
                            {
                                sbbb = new StringBuilder(ConvertTextWithHtmlTagsToRtfText(text, text.Text));
                            }

                            StringBuilder sb = CheckArabic(sbbb);
                            for (int indexChar = 0; indexChar < sb.Length; indexChar++)
                            {
                                unicodeMapArray[(int)sb[indexChar]] = 1;
                            }

                            //prepare style table
                            GetRtfStyleFromComponent(component);
                        }

                        if ((StiOptions.Export.RichText.UsePageRefField) && (component.BookmarkValue != null))
                        {
                            string bkm = component.BookmarkValue.ToString();
                            if ((bkm != null) && (bkm.Length > 0))
                            {
                                if (!bookmarkList.ContainsKey(bkm)) bookmarkList.Add(bkm, bkm);
                            }
                        }
                    }

                }
            }
            if (exportMode == StiRtfExportMode.Table) GetFontNumber(fontList, "Arial"); //fix
            #endregion

            #region make codepage to font table
            for (int index = 0; index < 65536; index++)
            {
                if (unicodeMapArray[index] != 0)
                {
                    codePageToFont[StiEncode.unicodeToCodePageArray[index]] = 1;
                }
            }
            codePageToFont[1] = 1;  //by default codepage 1252 - English

            for (int index = 1; index < StiEncode.codePagesTableSize + 1; index++)
            {
                if (codePageToFont[index] != 0)
                {
                    codePageToFont[index] = charsetCount;
                    fontToCodePages[charsetCount] = index - 1;
                    charsetCount++;
                }
            }
            codePageToFont[0] = codePageToFont[1];
            #endregion

            #region Correct fonts numbers in table of styles
            for (int index = 0; index < styleList.Count; index++)
            {
                StiRtfStyleInfo style = (StiRtfStyleInfo)styleList[index];
                style.FontNumber = style.FontNumber * charsetCount;
                styleList[index] = style;
            }
            #endregion

            RenderStartDoc();

            CurrentPassNumber++;

            #region Write data
            if (exportMode == StiRtfExportMode.Table)
            {
                sw.WriteLine("\\nolead");		//compability flags

                sw.WriteLine("\\afs16");        //fix - set the default font size to 8, in Word2007 this value is set 12

                StiPagesCollection allPages = pages;
                pages = null;
                bool headersAndFootersFounded = false;

                int indexPage = 0;
                while (indexPage < allPages.Count)
                {
                    //if (usePageHeadersAndFooters)
                    //{
                    pages = new StiPagesCollection(report, report.RenderedPages);
                    pages.CacheMode = report.RenderedPages.CacheMode;
                    pages.AddV2Internal(allPages.GetPageWithoutCache(indexPage));

                    string sheetName = allPages.GetPageWithoutCache(indexPage).ExcelSheetValue;
                    while ((indexPage < allPages.Count - 1) && CompareExcellSheetNames(allPages.GetPageWithoutCache(indexPage + 1).ExcelSheetValue, sheetName) &&
                        (allPages.GetPageWithoutCache(indexPage + 1).Orientation == allPages.GetPageWithoutCache(indexPage).Orientation))
                    {
                        indexPage++;
                        pages.AddV2Internal(allPages.GetPageWithoutCache(indexPage));
                    }
                    //}
                    //else
                    //{
                    //    //pages = allPages;
                    //    //indexPage = allPages.Count - 1;

                    //    pages = new StiPagesCollection(report);
                    //    pages.CacheMode = report.RenderedPages.CacheMode;
                    //    pages.Add(allPages[indexPage]);

                    //    while ((indexPage < allPages.Count - 1) && (allPages[indexPage + 1].Orientation == allPages[indexPage].Orientation))
                    //    {
                    //        indexPage++;
                    //        pages.Add(allPages[indexPage]);
                    //    }
                    //}

                    StiPage firstPage = pages[0];
                    if (firstPage != null)
                    {
                        pages.GetPage(firstPage);
                        if (!StiOptions.Export.RichText.UseNewPageCommandInsteadOfNewSection || (indexPage == 0))
                        {
                            RenderPageHeader(firstPage);
                        }
                        bool divideBigCells = StiOptions.Export.RichText.DivideBigCells && !StiOptions.Export.RichText.UseCanBreakProperty;
                        if (usePageHeadersAndFooters)
                        {
                            #region write header and footer
                            StiPagesCollection pages2 = new StiPagesCollection(report, report.RenderedPages);
                            pages2.CacheMode = report.RenderedPages.CacheMode;
                            pages2.AddV2Internal(pages[0]);
                            matrix = new StiMatrix(pages2, divideBigCells, this);
                            if (IsStopped) return;
                            matrix.ScanComponentsPlacement(false, true);

                            bool foundedPageHeaderExceptFirstPage = false;

                            int startLine;
                            int endLine;

                            endLine = Math.Max((matrix.CoordY.Count - 1) - 1, 0);
                            while ((matrix.LinePlacement[endLine] != StiMatrix.StiTableLineInfo.PageHeader) && (endLine > 0)) endLine--;
                            if (matrix.LinePlacement[endLine] == StiMatrix.StiTableLineInfo.PageHeader)
                            {
                                startLine = 0;
                                while (matrix.LinePlacement[startLine] != StiMatrix.StiTableLineInfo.PageHeader) startLine++;
                                sw.WriteLine("{");
                                sw.WriteLine("{\\header ");
                                writeFromMatrix(startLine, endLine, true);
                                sw.WriteLine("}");
                                headersAndFootersFounded = true;
                            }
                            else
                            {
                                bool isHeaderFounded = false;
                                if ((pages.Count > 1) && (pages[1] != null))
                                {
                                    //try to find header on second page
                                    StiMatrix tempMatrix = matrix;
                                    pages2 = new StiPagesCollection(report, report.RenderedPages);
                                    pages2.CacheMode = report.RenderedPages.CacheMode;
                                    pages2.AddV2Internal(pages[1]);
                                    matrix = new StiMatrix(pages2, divideBigCells, this);
                                    if (IsStopped) return;
                                    matrix.ScanComponentsPlacement(false, true);
                                    endLine = Math.Max((matrix.CoordY.Count - 1) - 1, 0);
                                    while ((matrix.LinePlacement[endLine] != StiMatrix.StiTableLineInfo.PageHeader) && (endLine > 0)) endLine--;
                                    if (matrix.LinePlacement[endLine] == StiMatrix.StiTableLineInfo.PageHeader)
                                    {
                                        startLine = 0;
                                        while (matrix.LinePlacement[startLine] != StiMatrix.StiTableLineInfo.PageHeader) startLine++;

                                        #region find ExceptFirstPage property
                                        StiPrintOnType printOn = StiPrintOnType.AllPages;
                                        bool founded = false;
                                        for (int rowIndex = startLine; rowIndex < endLine; rowIndex++)
                                        {
                                            for (int columnIndex = 0; columnIndex < Matrix.CoordX.Count - 1; columnIndex++)
                                            {
                                                StiCell cell = Matrix.Cells[rowIndex, columnIndex];
                                                if ((cell != null) && (cell.Component != null))
                                                {
                                                    printOn = cell.Component.PrintOn;
                                                    founded = true;
                                                    break;
                                                }
                                            }
                                            if (founded) break;
                                        }
                                        founded &= (printOn == StiPrintOnType.ExceptFirstAndLastPage) || (printOn == StiPrintOnType.ExceptFirstPage);
                                        #endregion

                                        if (founded)
                                        {
                                            sw.WriteLine("\\titlepg ");
                                            foundedPageHeaderExceptFirstPage = true;
                                        }
                                        sw.WriteLine("{");
                                        sw.WriteLine("{\\header ");
                                        writeFromMatrix(startLine, endLine, true);
                                        sw.WriteLine("}");
                                        headersAndFootersFounded = true;
                                        isHeaderFounded = true;
                                    }
                                    else
                                    {
                                        sw.WriteLine("{");
                                    }
                                    matrix = tempMatrix;
                                }
                                else
                                {
                                    sw.WriteLine("{");
                                }
                                if (!isHeaderFounded && headersAndFootersFounded) sw.WriteLine("{\\header \\fs2 }");
                            }

                            startLine = 0;
                            while ((matrix.LinePlacement[startLine] != StiMatrix.StiTableLineInfo.PageFooter) && (startLine < (matrix.CoordY.Count - 1) - 1)) startLine++;
                            if (matrix.LinePlacement[startLine] == StiMatrix.StiTableLineInfo.PageFooter)
                            {
                                endLine = (matrix.CoordY.Count - 1) - 1;
                                while (matrix.LinePlacement[endLine] != StiMatrix.StiTableLineInfo.PageFooter) endLine--;
                                if (foundedPageHeaderExceptFirstPage)
                                {
                                    sw.WriteLine("{\\footerf ");
                                    writeFromMatrix(startLine, endLine, true);
                                    sw.WriteLine("}");
                                }
                                sw.WriteLine("{\\footer ");
                                writeFromMatrix(startLine, endLine, true);
                                sw.WriteLine("}");
                                headersAndFootersFounded = true;
                            }
                            else
                            {
                                if (headersAndFootersFounded) sw.WriteLine("{\\footer \\fs2 }");
                            }
                            #endregion

                            matrix = new StiMatrix(pages, divideBigCells, this);
                            matrix.ScanComponentsPlacement(true, true);
                            endLine = (matrix.CoordY.Count - 1) - 1;
                            //while (matrix.LinePlacement[endLine] != StiMatrix.StiTableLineInfo.Unknown) endLine --;
                            CurrentPassNumber = StiOptions.Export.RichText.DivideSegmentPages ? 4 : 3;
                            writeFromMatrix(0, endLine, false);
                        }
                        else
                        {
                            matrix = new StiMatrix(pages, divideBigCells, this);
                            if (IsStopped) return;
                            sw.WriteLine("{");
                            CurrentPassNumber = StiOptions.Export.RichText.DivideSegmentPages ? 4 : 3;
                            writeFromMatrix(0, (matrix.CoordY.Count - 1) - 1, true);
                        }
                        sw.WriteLine("\\pard\\fs2 \\par");  //for fix empty page after end of text, if PageFooter used
                        sw.WriteLine("}");
                    }

                    if (indexPage < allPages.Count - 1) RenderPageFooter();
                    indexPage++;
                }
            }
            else
            {
                StatusString = StiLocalization.Get("Export", "ExportingCreatingDocument");

                //sw.WriteLine("\\nolead\\sprslnsp");		//compability flags
                sw.WriteLine("\\nolead");

                foreach (StiPage page in pages)
                {
                    pages.GetPage(page);

                    InvokeExporting(page, pages, CurrentPassNumber, MaximumPassNumber);
                    if (IsStopped) return;

                    RenderPageHeader(page);
                    sw.WriteLine("{");

                    //initialize arrays
                    int pageLines = (int)Math.Round(pageHeight / (insideScaleY / scaleY)) + 2;
                    SortedList[] list = new SortedList[pageLines];
                    for (int indexLine = 0; indexLine < pageLines; indexLine++)
                    {
                        list[indexLine] = new SortedList();
                    }

                    Hashtable hashCoordinates = new Hashtable();

                    #region Prepare page for render
                    foreach (StiComponent component in page.Components)
                    {
                        if (component.Enabled)
                        {
                            double x1 = page.Unit.ConvertToHInches(component.Left);
                            double y1 = page.Unit.ConvertToHInches(component.Top);
                            double x2 = page.Unit.ConvertToHInches(component.Right);
                            double y2 = page.Unit.ConvertToHInches(component.Bottom);

                            StiRtfData pp = new StiRtfData();

                            pp.X = (int)Math.Round(x1 * HiToTwips * scaleX);
                            pp.Y = (int)Math.Round(y1 * HiToTwips * scaleY);
                            pp.Width = (int)Math.Round(x2 * HiToTwips * scaleX) - pp.X;
                            pp.Height = (int)Math.Round(y2 * HiToTwips * scaleY) - pp.Y;
                            pp.Component = component;
                            pp.X++;
                            pp.Y++;

                            while (true)
                            {
                                string coordsSt = string.Format("{0}:{1}:{2}:{3}", pp.X, pp.Y, pp.Width, pp.Height);
                                if (!hashCoordinates.Contains(coordsSt))
                                {
                                    hashCoordinates[coordsSt] = coordsSt;
                                    break;
                                }
                                pp.Width++;
                            }

                            int tempX = pp.X;
                            int tempY = (int)Math.Round(y1 / (insideScaleY / scaleY));
                            if (tempY < 0) tempY = 0;

                            while (list[tempY].IndexOfKey(tempX) != -1) tempX++;
                            list[tempY].Add(tempX, pp);
                        }
                    }
                    #endregion

                    switch (exportMode)
                    {
                        case StiRtfExportMode.Frame:
                            #region Render page - RTF frames
                            sw.WriteLine("\\nowidctlpar\\phmrg\\pvmrg\\nowrap");

                            #region First pass
                            for (int indexLine = 0; indexLine < list.GetLength(0); indexLine++)
                            {
                                if (list[indexLine].Count > 0)
                                {
                                    sw.Write("\\pard");
                                    for (int indexObj = 0; indexObj < list[indexLine].Count; indexObj++)
                                    {
                                        StiRtfData pp = (StiRtfData)list[indexLine].GetByIndex(indexObj);
                                        StiComponent component = pp.Component;
                                        if (component.Width == 0 || component.Height == 0) continue;
                                        if (component is StiLinePrimitive) continue;

                                        #region correct frame X and W
                                        int correctX = 0;
                                        int correctW = 0;
                                        IStiBorder mBorder = component as IStiBorder;
                                        if (mBorder != null && (!(component is IStiIgnoreBorderWhenExport)))
                                        {
                                            if ((mBorder.Border.Side & StiBorderSides.Left) > 0)
                                            {
                                                correctX += frameCorrectValue;
                                                correctW += frameCorrectValue;
                                            }
                                            if ((mBorder.Border.Side & StiBorderSides.Right) > 0)
                                            {
                                                correctW += frameCorrectValue;
                                            }
                                        }
                                        #endregion

                                        sw.Flush();
                                        long swPos = sw.BaseStream.Position;

                                        sw.Write("{");
                                        sw.Write("\\nowrap\\posx{0}\\posy{1}\\absw{2}\\absh{3}",
                                            pp.X + correctX, pp.Y, pp.Width - correctW, pp.Height);

                                        RenderTextAngle1(component);
                                        RenderBorder1(component);
                                        RenderBrush1(component);
                                        RenderStyle12(component);
                                        RenderHorAlign12(component);
                                        RenderTextBrush12(component);
                                        RenderTextFont12(component);
                                        if (component is StiText) RenderText12(component);
                                        if (component is StiRichText) RenderRtf12(component, swPos);
                                        else
                                        {
                                            if (component is StiShape)
                                            {
                                                RenderShape1(pp, correctX, correctW);
                                            }
                                            else
                                            {
                                                RenderImage12(component, pp.Width - correctW, pp.Height);
                                            }
                                        }
                                        sw.WriteLine(" \\par}");
                                    }
                                }
                            }
                            #endregion

                            #region Second pass - primitives
                            for (int indexLine = 0; indexLine < list.GetLength(0); indexLine++)
                            {
                                if (list[indexLine].Count > 0)
                                {
                                    sw.Write("\\pard");
                                    for (int indexObj = 0; indexObj < list[indexLine].Count; indexObj++)
                                    {
                                        StiRtfData pp = (StiRtfData)list[indexLine].GetByIndex(indexObj);
                                        StiComponent component = pp.Component;
                                        if (component.Width == 0 || component.Height == 0) continue;
                                        if (!(component is StiLinePrimitive)) continue;

                                        #region correct frame X
                                        int correctX = 0;
                                        IStiBorder mBorder = component as IStiBorder;
                                        if (mBorder != null && (!(component is IStiIgnoreBorderWhenExport)))
                                        {
                                            if ((mBorder.Border.Side & StiBorderSides.Left) > 0)
                                            {
                                                correctX += frameCorrectValue;
                                            }
                                        }
                                        #endregion

                                        sw.Write("{");
                                        sw.Write("\\nowrap\\posx{0}\\posy{1}\\absw{2}\\absh{3}",
                                            pp.X + correctX, pp.Y, pp.Width, pp.Height);

                                        RenderBorder1(component);

                                        sw.WriteLine("\\fs0\\par}");
                                    }
                                }
                            }
                            #endregion

                            #endregion
                            break;

                        case StiRtfExportMode.WinWord:
                            #region Render page - RTF WinWord mode
                            for (int indexLine = 0; indexLine < list.GetLength(0); indexLine++)
                            {
                                if (list[indexLine].Count > 0)
                                {
                                    for (int indexObj = 0; indexObj < list[indexLine].Count; indexObj++)
                                    {
                                        StiRtfData pp = (StiRtfData)list[indexLine].GetByIndex(indexObj);
                                        StiComponent component = pp.Component;

                                        StiShape shape = component as StiShape;
                                        if ((component is StiShape) && (shape != null) && (CheckShape2(shape) == true))
                                        {
                                            RenderShape2(pp);
                                        }
                                        else
                                        {
                                            sw.Write("{\\shp{\\*");
                                            sw.Write("\\shpinst\\shpleft{0}\\shptop{1}\\shpright{2}\\shpbottom{3}",
                                                pp.X, pp.Y, pp.X + pp.Width, pp.Y + pp.Height);
                                            sw.Write("\\shpwr3");
                                            sw.Write("{\\sp{\\sn dxTextLeft}{\\sv 0}}");
                                            sw.Write("{\\sp{\\sn dyTextTop}{\\sv 0}}");
                                            sw.Write("{\\sp{\\sn dxTextRight}{\\sv 0}}");
                                            sw.Write("{\\sp{\\sn dyTextBottom}{\\sv 0}}");

                                            RenderBrush2(component);

                                            sw.Write("{\\sp{\\sn fLine}{\\sv 0}}");
                                            sw.Write("{\\shptxt\\trowd\\trqc");
                                            sw.Write("\\trrh-{0}", pp.Height);

                                            RenderVerAlign2(component);
                                            RenderBorder2(component);
                                            RenderTextAngle2(component);

                                            sw.Write("\\cellx{0}\\intbl", pp.Width);

                                            RenderHorAlign12(component);
                                            RenderTextBrush12(component);
                                            RenderTextFont12(component);
                                            if (!(component is StiRichText)) RenderText12(component);
                                            if (component is StiRichText)
                                            {
                                                RenderRtf12(component);
                                            }
                                            else
                                            {
                                                RenderImage12(component, pp.Width, pp.Height);
                                            }
                                            sw.WriteLine("\\cell\\row\\pard\\par}}}");
                                        }  //else
                                    }  //for
                                }  //if
                            }  //for
                            #endregion
                            break;

                        case StiRtfExportMode.TabbedText:
                            #region Render page - RTF simple text
                            for (int index1 = 0; index1 < list.GetLength(0); index1++)
                            {
                                if (list[index1].Count > 0) sw.Write("\\pard{");

                                if (list[index1].Count > 0)
                                {
                                    StringBuilder codeStr = new StringBuilder();
                                    StringBuilder dataStr = new StringBuilder();

                                    for (int index2 = 0; index2 < list[index1].Count; index2++)
                                    {
                                        StiRtfData pp = (StiRtfData)list[index1].GetByIndex(index2);
                                        StiComponent component = pp.Component;

                                        int coordTX = pp.X;

                                        #region Hor align
                                        IStiTextHorAlignment mTextHorAlign = component as IStiTextHorAlignment;
                                        if (mTextHorAlign != null)
                                        {
                                            if ((mTextHorAlign.HorAlignment & StiTextHorAlignment.Center) > 0)
                                            {
                                                coordTX = pp.X + (int)Math.Round((double)pp.Width / 2) - 1;
                                                codeStr.Append("\\tqc");
                                            }
                                            if ((mTextHorAlign.HorAlignment & StiTextHorAlignment.Right) > 0)
                                            {
                                                // 28 - correction for Word, so as tabs will not be merged
                                                coordTX = pp.X + pp.Width - 28;
                                                codeStr.Append("\\tqr");
                                            }
                                        }
                                        #endregion

                                        codeStr.Append("\\tx");
                                        codeStr.Append(coordTX);
                                        dataStr.Append("\\tab");

                                        RenderBorder3(component, dataStr);
                                        RenderBrush3(component, dataStr);
                                        RenderTextFont3(component, dataStr);
                                        RenderTextBrush3(component, dataStr);
                                        if (!(component is StiRichText)) RenderText3(component, dataStr);

                                        if (component is StiRichText) RenderRtf3(component, dataStr);
                                        else
                                            RenderImage3(component, dataStr, pp.Width, pp.Height);

                                    }
                                    sw.Write(codeStr);

                                    byte[] tbb = new byte[dataStr.Length];
                                    for (int tempI = 0; tempI < dataStr.Length; tempI++)
                                    {
                                        tbb[tempI] = (byte)dataStr[tempI];
                                    }
                                    sw.Flush();
                                    sw2.Write(tbb, 0, tbb.Length);
                                    //								sw.Write(dataStr);

                                }
                                if (list[index1].Count > 0) sw.WriteLine("\\par}");
                            }
                            #endregion
                            break;
                    }

                    sw.WriteLine("}");
                    if (page != pages[pages.Count - 1])
                        RenderPageFooter();
                }
            }
            #endregion

            RenderEndDoc();
            sw.Flush();

            #region Clear
            if (matrix != null)
            {
                matrix.Clear();
                matrix = null;
            }
            colorList = null;
            fontList = null;
            styleList = null;
            unicodeMapArray = null;
            codePageToFont = null;
            fontToCodePages = null;
            imageCodec = null;
            bookmarkList = null;
            usedBookmarks = null;
            #endregion

            if (report.RenderedPages.CacheMode) StiMatrix.GCCollect();
        }
        #endregion
    }
}
