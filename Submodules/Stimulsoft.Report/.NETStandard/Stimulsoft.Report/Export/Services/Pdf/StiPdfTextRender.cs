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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using System.Drawing;
using System.Drawing.Drawing2D;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Font = Stimulsoft.Drawing.Font;
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Matrix = Stimulsoft.Drawing.Drawing2D.Matrix;
#endif

namespace Stimulsoft.Report.Export
{
    public partial class StiPdfExportService
    {

        #region RenderText
        internal void RenderText(StiPdfData pp, PointD? basePoint = null)
        {
            StiText textComp = pp.Component as StiText;
            bool useWysiwyg = textComp.TextQuality == StiTextQuality.Wysiwyg;
            bool needAnnots = (annotsCounter > 0 || annots2Counter > 0) && textComp.Editable;

            IStiTextOptions textOpt = pp.Component as IStiTextOptions;
            bool wordWrap = (textOpt != null) && textOpt.TextOptions.WordWrap;

            IStiTextHorAlignment mTextHorAlign = pp.Component as IStiTextHorAlignment;
            bool needWidthAlign = (mTextHorAlign != null) && (mTextHorAlign.HorAlignment == StiTextHorAlignment.Width);

            bool needWinFormsHighAccuracy = StiOptions.Export.Pdf.WinFormsHighAccuracyMode == StiPdfHighAccuracyMode.All || StiOptions.Export.Pdf.WinFormsHighAccuracyMode == StiPdfHighAccuracyMode.WordwrapOnly && wordWrap;
            bool needWpfHighAccuracy = StiOptions.Export.Pdf.WpfHighAccuracyMode == StiPdfHighAccuracyMode.All || StiOptions.Export.Pdf.WpfHighAccuracyMode == StiPdfHighAccuracyMode.WordwrapOnly && wordWrap;
            if (!needAnnots && (!isWpf && needWinFormsHighAccuracy && !useWysiwyg && !needWidthAlign || isWpf && needWpfHighAccuracy))
            {
                RenderTextHQ(pp);
                return;
            }

            IStiTextBrush mTextBrush = pp.Component as IStiTextBrush;
            IStiText text = pp.Component as IStiText;
            IStiVertAlignment mVertAlign = pp.Component as IStiVertAlignment;

            bool useRightToLeft = ((textOpt != null) && (textOpt.TextOptions != null) && (textOpt.TextOptions.RightToLeft));
            string annotText = null;
            int linesCount = 0;
            float textAngle = 0;

            if (needWidthAlign) wordWrap = true;
            
            bool useUnicode = pdfFont.UseUnicode;

            if (text != null)
            {
                #region Calculate text coordinates
                //ConvertTextMargins
                StiMargins margins = textComp.Margins;
                double marginL = hiToTwips * margins.Left;
                double marginR = hiToTwips * margins.Right;
                double marginT = hiToTwips * margins.Top;
                double marginB = hiToTwips * margins.Bottom;
                double textX = pp.X + marginL;
                double textY = pp.Y + marginB;
                double textW = pp.Width - marginL - marginR;
                double textH = pp.Height - marginT - marginB;

                //ConvertTextBorders
                double borderSizeLeft = 0;
                double borderSizeRight = 0;
                double borderSizeTop = 0;
                double borderSizeBottom = 0;

                if (textComp.Border.Style != StiPenStyle.None)
                {
                    var tempSizeOffset = textComp.Border.GetSizeOffset();
                    if (textComp.Border.IsLeftBorderSidePresent) borderSizeLeft = tempSizeOffset;
                    if (textComp.Border.IsRightBorderSidePresent) borderSizeRight = tempSizeOffset;
                    if (textComp.Border.IsTopBorderSidePresent) borderSizeTop = tempSizeOffset;
                    if (textComp.Border.IsBottomBorderSidePresent) borderSizeBottom = tempSizeOffset;
                }

                var advBorder = textComp.Border as StiAdvancedBorder;
                if (advBorder != null)
                {
                    borderSizeLeft = advBorder.LeftSide.GetSizeOffset();
                    borderSizeRight = advBorder.RightSide.GetSizeOffset();
                    borderSizeTop = advBorder.TopSide.GetSizeOffset();
                    borderSizeBottom = advBorder.BottomSide.GetSizeOffset();
                }

                double borderScale = 0.94f;
                textX += hiToTwips * borderScale * borderSizeLeft;
                textY += hiToTwips * borderScale * borderSizeBottom;
                textW -= hiToTwips * borderScale * (borderSizeLeft + borderSizeRight);
                textH -= hiToTwips * borderScale * (borderSizeTop + borderSizeBottom);
                #endregion

                if (clipLongTextLines && !needAnnots)
                {
                    pageStream.WriteLine("q");
                    PushColorToStack();
                    pageStream.WriteLine("{0} {1} {2} {3} re W n",
                        ConvertToString(textX),
                        ConvertToString(textY),
                        ConvertToString(textW),
                        ConvertToString(textH));
                }

                #region Set text color
                Color textColor = Color.Transparent;
                if (mTextBrush != null)
                {
                    //tempColor = StiBrush.ToColor(mTextBrush.TextBrush);
                    textColor = StiBrush.ToColor(mTextBrush.TextBrush);
                    SetNonStrokeColor(textColor);

                    double offsX = basePoint != null ? basePoint.Value.X : 0;
                    double offsY = basePoint != null ? basePoint.Value.Y : 0;
                    StoreShadingData2(offsX + pp.X, offsY + pp.Y, pp.Width, pp.Height, mTextBrush.TextBrush);
                    if (mTextBrush.TextBrush is StiGradientBrush || mTextBrush.TextBrush is StiGlareBrush)
                    {
                        pageStream.WriteLine("/Pattern cs /P{0} scn", 1 + shadingCurrent);
                    }
                    if (mTextBrush.TextBrush is StiHatchBrush)
                    {
                        StiHatchBrush hBrush = mTextBrush.TextBrush as StiHatchBrush;
                        pageStream.WriteLine("/Cs1 cs /PH{0} scn", GetHatchNumber(hBrush) + 1);
                    }

                    if (needAnnots)
                    {
                        StiEditableObject seo = (StiEditableObject)annotsArray[annotsCurrent];
                        seo.FontColor = textColor;
                    }
                }
                #endregion

                #region Calculate font parameters
                double sizeInPt = 1;
                //bool isSymbolic = false;
                IStiFont mFont = pp.Component as IStiFont;
                if (mFont != null)
                {
                    sizeInPt = (mFont.Font.SizeInPoints);
                    //isSymbolic = PdfFonts.IsFontSymbolic(mFont.Font.Name);
                }

                double cf = isWpf || StiDpiHelper.NeedFontScaling ? StiDpiHelper.GraphicsScale : 1;

                //temporarily fix for Linux and Mac, bug in current version of libgdiplus
                double sizeInPt2 = sizeInPt;
                if (StiOptions.Engine.TextLineSpacingScale != 1)
                {
                    if (StiDpiHelper.IsLinux && (sizeInPt >= 4.5 && sizeInPt <= 7.5)) sizeInPt2 = 5.5;
                    if (StiDpiHelper.IsMacOsX && (sizeInPt >= 5.5 && sizeInPt <= 7.5)) sizeInPt2 = 6.5;
                }

                //double fontCH  = sizeInPt2 * fontCorrectValue * pdfFont.CH / 1000 * cf;
                //double fontXH = sizeInPt2 * fontCorrectValue * pdfFont.XH / 1000 * cf;
                double fontASC = sizeInPt2 * fontCorrectValue * pdfFont.ASC / 1000 * cf;
                double fontDESC = sizeInPt2 * fontCorrectValue * pdfFont.DESC / 1000 * cf;
                double fonttmASC = sizeInPt2 * fontCorrectValue * pdfFont.tmASC / 1000 * cf;
                double fonttmDESC = sizeInPt2 * fontCorrectValue * pdfFont.tmDESC / 1000 * (-1) * cf;
                double fonttmExternal = sizeInPt2 * fontCorrectValue * pdfFont.tmExternal / 1000 * cf;
                double fontLineGap = sizeInPt2 * fontCorrectValue * pdfFont.LineGap / 1000 * cf;
                //double fontMacAscent = sizeInPt2 * fontCorrectValue * pdfFont.MacAscend / 1000 * cf;
                //double fontMacDescent = sizeInPt2 * fontCorrectValue * pdfFont.MacDescend / 1000 * cf;
                //double fontMacLineGap = sizeInPt2 * fontCorrectValue * pdfFont.MacLineGap / 1000 * cf;
                double fontUnderscoreSize = sizeInPt2 * fontCorrectValue * pdfFont.UnderscoreSize / 1000 * cf;
                double fontUnderscorePosition = sizeInPt2 * fontCorrectValue * pdfFont.UnderscorePosition / 1000 * cf;
                double fontStrikeoutSize = sizeInPt2 * fontCorrectValue * pdfFont.StrikeoutSize / 1000 * cf;
                double fontStrikeoutPosition = sizeInPt2 * fontCorrectValue * pdfFont.StrikeoutPosition / 1000 * cf;

                double fontLineHeightWithoutLineSpacing = fonttmASC - fonttmDESC + fonttmExternal;
                double fontLineHeight = fontLineHeightWithoutLineSpacing * textComp.LineSpacing;
                double fontAscF = fonttmASC;
                double fontDescF = fonttmDESC;
                if ((pdfFont.fsSelection & 0x80) > 0)    //USE_TYPO_METRICS
                {
                    fontLineHeight = (fontASC - fontDESC + fontLineGap) * textComp.LineSpacing;
                    fontAscF = fontASC;
                    fontDescF = fontDESC;
                }

                if (CompatibleMode160)
                {
                    fontAscF = (fonttmASC - fontASC) / 2;
                    fontDescF = (fontDESC > 0 ? fontDESC : Math.Abs(fontDESC) * 0.4);
                    double fontVV = ((fonttmASC - fonttmDESC) - (fontASC - fontDESC)) / 2;
                    double fontHH2 = fontVV;
                    fontAscF = fontASC + fontVV;
                    fontDescF = fontDESC - fontVV;
                    fontLineHeightWithoutLineSpacing = fontAscF - fontDescF;
                    //fontLineHeight = fontLineHeightWithoutLineSpacing * textComp.LineSpacing;
                    fontLineHeight *= 0.957;    //special fix for get lineHeight from current implementation
                }
                #endregion

                #region IsRotate90
                bool isRotate90 = false;
                if (textOpt != null)
                {
                    textAngle = textOpt.TextOptions.Angle;
                    isRotate90 = ((textAngle > 45) && (textAngle < 135)) || ((textAngle > 225) && (textAngle < 315));
                }
                #endregion

                #region Correction of text coordinates
                if (needAnnots)
                {
                    textX -= pp.X;
                    textY -= pp.Y;
                }

                //float correctX = 0.85f;
                float correctY = 0.85f;

                //correction of coordinates for border
                if (CompatibleMode160)
                {
                    textX += 1f;
                    textW -= 2f;
                    textY += 2f;
                    textH -= 4f;
                }

                double textQualityOffset = 0;
                if (textComp.TextQuality == StiTextQuality.Standard)
                {
#if BLAZOR
                    float correctX = 0;
                    textQualityOffset = sizeInPt * 4.5;
#else
                    float correctX = (float)(0.35 - (14 - sizeInPt) * 0.04);
                    textQualityOffset = 137.61; //pdfFont.Widths[0] / 2d * 0.99;
#endif
                    double halfSpaceOffset = textQualityOffset * (sizeInPt * fontCorrectValue) / 1000;
                    if (isRotate90)
                    {
                        textY += correctX + halfSpaceOffset;
                        textH -= (correctX + halfSpaceOffset) * 2;
                    }
                    else
                    {
                        textX += correctX + halfSpaceOffset;
                        textW -= (correctX + halfSpaceOffset) * 2;
                    }
                }
                if (textComp.TextQuality == StiTextQuality.Typographic)
                {
                    float correctX = (float)(0.05 + (8 - sizeInPt) * 0.009);
                    if (isRotate90)
                    {
                        textY += correctX;
                        textH -= correctX * 2;
                    }
                    else
                    {
                        textX += correctX;
                        textW -= correctX * 2;
                    }
                }
                #endregion

                StringBuilder sb = new StringBuilder(text.Text);
                if (!StiDpiHelper.IsWindows)
                {
                    if (textComp.CheckAllowHtmlTags())
                    {
                        sb = new StringBuilder(StiTextRenderer.GetPlainTextFromHtmlTags(text.Text));
                    }
                    useWysiwyg = false;
                }

                if (isWpf && wordWrap)
                {
                    if (textComp.Page == null) textComp.Page = report.RenderedPages[0];
                    RectangleD rect = textComp.GetPaintRectangle(true, false);
                    rect.X += 1;
                    if (rect.Width >= 2) rect.Width -= 2;
                    rect = textComp.ConvertTextMargins(rect, false);
                    rect = textComp.ConvertTextBorders(rect, false);

                    textW = rect.Width * hiToTwips;
                }

                if (needAnnots)
                {
                    annotText = sb.ToString().Replace("\n", "");
                }
                sb.Replace("\r", "");
                sb.Replace("\x1f", "");

                if ((textW > 0) && (sb.Length > 0))
                {
                    #region calculate rotating angle
                    double normTextX = textX;
                    double normTextY = textY;
                    double normTextW = textW;
                    double normTextH = textH;
                    if (textAngle != 0)
                    {
                        if (isRotate90)
                        {
                            double tempValue = textW;
                            textW = textH;
                            textH = tempValue;
                        }
                        textX = -textW / 2f;
                        textY = -textH / 2f;
                    }
                    #endregion

                    List<string> stringList = null;
                    List<StiTextRenderer.LineInfo> arrLinesInfo = null;

                    if (useWysiwyg)
                    {
                        #region Use wysiwyg textrender for split strings
                        CheckGraphicsForTextRenderer();
                        var arrTextLines = new List<string>();
                        arrLinesInfo = new List<StiTextRenderer.LineInfo>();
                        string textForOutput = sb.ToString();

                        RectangleD rect = textComp.Page.Unit.ConvertToHInches(pp.Component.ComponentToPage(pp.Component.ClientRectangle));
                        rect = textComp.ConvertTextMargins(rect, false);
                        rect = textComp.ConvertTextBorders(rect, false);

                        StiTextRenderer.GetTextLinesAndWidths(
                            graphicsForTextRenderer,
                            ref textForOutput,
                            textComp.Font,
                            rect,
                            1,
                            wordWrap,
                            textOpt.TextOptions.RightToLeft,
                            1,
                            textOpt.TextOptions.Angle,
                            textOpt.TextOptions.Trimming,
                            textComp.CheckAllowHtmlTags(),
                            ref arrTextLines,
                            ref arrLinesInfo,
                            textOpt.TextOptions,
                            1);

                        if (needWidthAlign)
                        {
                            for (int index = 0; index < arrLinesInfo.Count; index++)
                            {
                                if (((StiTextRenderer.LineInfo)arrLinesInfo[index]).NeedWidthAlign)
                                {
                                    arrTextLines[index] = arrTextLines[index] + '\a';
                                }
                            }
                        }
                        stringList = arrTextLines;
                        #endregion
                    }
                    else
                    {
                        sb.Replace("\xAD", "");

                        if (needWidthAlign)
                        {
                            RectangleD rect = textComp.Page.Unit.ConvertToHInches(pp.Component.ComponentToPage(pp.Component.ClientRectangle));
                            rect = textComp.ConvertTextMargins(rect, false);
                            rect = textComp.ConvertTextBorders(rect, false);
                            rect.Width /= StiDpiHelper.DeviceCapsScale;
                            rect.Height /= StiDpiHelper.DeviceCapsScale;
                            CheckGraphicsForTextRenderer();
                            stringList = StiTextDrawing.SplitTextWordwrapWidth(sb.ToString(), graphicsForTextRenderer, textComp.Font, rect);
                            wordWrap = false;
                        }
                        else
                        {
                            stringList = StiExportUtils.SplitString(sb.ToString(), false);
                        }

                        if (wordWrap)
                        {
                            #region Check wordwrap
                            for (int indexLine = 0; indexLine < stringList.Count; indexLine++)
                            {
                                string stt = stringList[indexLine];
                                //remove all whitespaces from end of string
                                //stt = stt.TrimEnd(' ');

                                //remake string with bidiconvert
                                StringBuilder sbt = new StringBuilder();
                                int indexChar = 0;
                                while (indexChar < stt.Length)
                                {
                                    if (char.IsWhiteSpace(stt[indexChar]))
                                    {
                                        sbt.Append(stt[indexChar]);
                                        indexChar++;
                                    }
                                    else
                                    {
                                        StringBuilder sbtWord = new StringBuilder();
                                        while ((indexChar < stt.Length) && (!char.IsWhiteSpace(stt[indexChar])))
                                        {
                                            sbtWord.Append(stt[indexChar]);
                                            indexChar++;
                                        }
                                        //StringBuilder sbtWordBidi = bidi.Convert(sbtWord, false);	//need add parameter - bool reverseString
                                        var sbtWordBidi = StiBidirectionalConvert2.ConvertStringBuilder(sbtWord, false);
                                        sbt.Append(sbtWordBidi);
                                        int lenWord = sbtWord.Length - sbtWordBidi.Length;
                                        if (lenWord > 0)
                                        {
                                            sbt.Append((char)0x00, lenWord);
                                        }
                                    }
                                }

                                int[] charr = new int[stt.Length];
                                for (int tempIndex = 0; tempIndex < stt.Length; tempIndex++)
                                {
                                    //charr[tempIndex] = pdfFont.UnicodeMap[(int)stt[tempIndex]];
                                    charr[tempIndex] = pdfFont.UnicodeMap[(int)sbt[tempIndex]];
                                }

                                int[] summarr = new int[stt.Length];
                                int[] wordarr = new int[stt.Length];
                                int wordCounter = 0;
                                float summ = 0;
                                for (int index = 0; index < charr.Length; index++)
                                {
                                    if (charr[index] >= 32)
                                        summ += pdfFont.Widths[charr[index] - 32];
                                    if (charr[index] == '\t')
                                        summ += GetTabsSize(textOpt, sizeInPt, summ + textQualityOffset) + (float)textQualityOffset;
                                    summarr[index] = (int)summ;
                                    //if ((IsWordWrapSymbol((char)charr[index])) && (index > 0))
                                    if ((IsWordWrapSymbol(sbt, index)) && (index > 0))
                                    {
                                        wordCounter++;
                                    }
                                    wordarr[index] = wordCounter;
                                }
                                double summf = summ * (sizeInPt * fontCorrectValue) / 1000;
                                //line is too long and have more than one word
                                //if ((summf > textW) && (wordarr[stt.Length-1]>0))
                                if (summf > textW)
                                {
                                    int index = stt.Length - 1;
                                    int textWint = (int)(textW * 1000 / (sizeInPt * fontCorrectValue));
                                    while ((summarr[index] > textWint) && (index > 0)) index--;
                                    //check in which word; if not first - find the beginnning of the word, otherwise - the end of the word
                                    int index2 = index;
                                    if (wordarr[index] > 0)
                                    {
                                        if (wordarr[index] != wordarr[index + 1])
                                        {
                                            index2 = index++;
                                            while ((index < sbt.Length) && char.IsWhiteSpace(sbt[index]) && (sbt[index] != '\t')) index++;
                                        }
                                        else
                                        {
                                            while (!IsWordWrapSymbol(sbt, index)) index--;
                                            index2 = index - 1;
                                            while ((char.IsWhiteSpace(sbt[index2])) && (index2 > 0)) index2--;
                                            while (char.IsWhiteSpace(sbt[index]) && sbt[index] != '\t') index++;
                                        }
                                    }
                                    else
                                    {
                                        index++;
                                    }

                                    //this block must be optimized - on long text may be very slow (many pass on long line)
                                    if (needWidthAlign)
                                    {
                                        stringList[indexLine] = stt.Substring(0, index2 + 1) + '\a';
                                    }
                                    else
                                    {
                                        stringList[indexLine] = stt.Substring(0, index2 + 1);
                                    }
                                    if (index < stt.Length)
                                    {
                                        stringList.Insert(indexLine + 1, stt.Substring(index, stt.Length - index).TrimStart(' '));
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                    linesCount = stringList.Count;

                    int trimCountLines = -1;
                    if (textOpt != null && (textOpt.TextOptions.Trimming != StringTrimming.None || textOpt.TextOptions.LineLimit))
                    {
                        double trimCountLinesDouble = textH / fontLineHeight;
                        trimCountLines = (int)trimCountLinesDouble;
                        //if (trimCountLinesDouble > trimCountLines) trimCountLines++;
                        if (trimCountLines < 1) trimCountLines = 1;
                        if (trimCountLines < linesCount) linesCount = trimCountLines;
                        if (textOpt.TextOptions.LineLimit) trimCountLines = -1;
                    }

                    #region mapping Unicode symbols and BidiConvert
                    for (int indexLine = 0; indexLine < stringList.Count; indexLine++)
                    {
                        string stt = stringList[indexLine];
                        //var sbTemp = bidi.Convert(new StringBuilder(stt), useRightToLeft);
                        var sbTemp = StiBidirectionalConvert2.ConvertStringBuilder(new StringBuilder(stt), useRightToLeft);
                        for (int index = 0; index < sbTemp.Length; index++)
                        {
                            sbTemp[index] = (char)pdfFont.UnicodeMap[(int)sbTemp[index]];
                        }
                        stringList[indexLine] = sbTemp.ToString();
                    }
                    #endregion

                    #region VertAlign
                    if (mVertAlign != null)
                    {
                        double textHeight = (linesCount - 1) * fontLineHeight + fontLineHeightWithoutLineSpacing;
                        StiVertAlignment vertAlignment = mVertAlign.VertAlignment;
                        if ((textAngle != 0) && (textAngle != 90) && (textAngle != 180) && (textAngle != 270))
                        {
                            vertAlignment = StiVertAlignment.Center;
                        }
                        if (vertAlignment == StiVertAlignment.Top)
                        {
                            if (CompatibleMode160)
                            {
                                textY += textH - (textHeight - (-fontDescF));
                            }
                            else
                            {
                                textY += (textH - textHeight) + (fontLineHeightWithoutLineSpacing - fontAscF);
                            }
                        }
                        if (vertAlignment == StiVertAlignment.Center)
                        {
                            if (CompatibleMode160)
                            {
                                textY += (textH - textHeight) / 2 + (-fontDescF);
                            }
                            else
                            {
                                if (textComp.TextQuality == StiTextQuality.Typographic)
                                {
                                    textY += (textH - textHeight) / 2 - fontDescF;
                                }
                                else
                                {
                                    textY += (textH - textHeight) / 2 + (fontLineHeightWithoutLineSpacing - fontAscF) * 1.2;
                                }
                            }
                        }
                        if (vertAlignment == StiVertAlignment.Bottom)
                        {
                            if (CompatibleMode160)
                            {
                                textY += (-fontDescF);
                            }
                            else
                            {
                                if (textComp.TextQuality == StiTextQuality.Typographic)
                                {
                                    textY += -fontDescF;
                                }
                                else
                                {
                                    textY += (fontLineHeightWithoutLineSpacing - fontAscF) * 1.4;
                                }
                            }
                        }
                    }
                    #endregion

                    pageStream.WriteLine("BT");

                    #region write text
                    double lastTextLineX = 0;
                    double lastTextLineY = 0;
                    double[,] underlineArray = new double[stringList.Count, 3];
                    Matrix matrix = null;
                    if (textAngle != 0)
                    {
                        double AngleInRadians = textAngle * Math.PI / 180f;

                        matrix = new Matrix(
                            (float)Math.Cos(AngleInRadians),
                            (float)Math.Sin(AngleInRadians),
                            (float)-Math.Sin(AngleInRadians),
                            (float)Math.Cos(AngleInRadians),
                            (float)(normTextX + normTextW / 2),
                            (float)(normTextY + normTextH / 2));

                        if (!(pdfFont.NeedSyntItalic && (mFont != null) && mFont.Font.Italic))
                        {
                            pageStream.WriteLine("{0} {1} {2} {3} {4} {5} Tm",
                                ConvertToString(matrix.Elements[0]),
                                ConvertToString(matrix.Elements[1]),
                                ConvertToString(matrix.Elements[2]),
                                ConvertToString(matrix.Elements[3]),
                                ConvertToString(matrix.Elements[4]),
                                ConvertToString(matrix.Elements[5]));
                        }
                    }

                    if ((pdfFont.NeedSyntBold) && (mFont != null) && (mFont.Font.Bold))
                    {
                        pageStream.WriteLine("{0} w 2 Tr", ConvertToString(boldFontStrokeWidthValue * sizeInPt, 3));
                        SetStrokeColor(textColor);
                    }

                    for (int indexLine = 0; indexLine < linesCount; indexLine++)
                    {
                        if ((textOpt != null) && textOpt.TextOptions.LineLimit && (fontLineHeight * (indexLine + 1) > textH)) break;

                        #region calculate text line length in pt
                        string stt = stringList[indexLine];
                        //						byte[] charr = new byte[stt.Length];
                        float[] tabsArr = new float[stt.Length];
                        int numTabs = 0;
                        double summ = 0;

                        double trimSumm = 0;
                        double trimWidth = textW * 1000 / (sizeInPt * fontCorrectValue);
                        int trimCharWidth = pdfFont.Widths[pdfFont.UnicodeMap[(int)'…']];
                        int trimIndex = 0;

                        //						for (int tempIndex = 0; tempIndex < stt.Length; tempIndex++)
                        //						{
                        //							charr[tempIndex] = (byte)stt[tempIndex];
                        //						}
                        for (int index = 0; index < stt.Length; index++)
                        {
                            int charrSym = (int)stt[index];
                            if (charrSym >= 32)
                            {
                                summ += pdfFont.Widths[charrSym - 32];
                            }
                            if (charrSym == '\t')
                            {
                                float tabSize = GetTabsSize(textOpt, sizeInPt, summ + textQualityOffset) + (float)textQualityOffset;
                                summ += tabSize;
                                tabsArr[numTabs++] = tabSize;
                            }
                            if (summ + trimCharWidth < trimWidth)
                            {
                                trimSumm = summ;
                                trimIndex = index;
                            }
                        }
                        summ = summ * (sizeInPt * fontCorrectValue) / 1000;
                        #endregion

                        if ((trimCountLines != -1) && (((indexLine == linesCount - 1) && (trimCountLines < stringList.Count)) || (summ > textW)))
                        {
                            // "…" trimming надо доделать
                            stt = stt.Substring(0, trimIndex) + (char)pdfFont.UnicodeMap[(int)'…'];
                            summ = (trimSumm + trimCharWidth) * (sizeInPt * fontCorrectValue) / 1000;
                        }

                        if (useUnicode)
                        {
                            #region convert to hex
                            StringBuilder sbt = new StringBuilder();
                            for (int index = 0; index < stt.Length; index++)
                            {
                                if (index % 121 == 120)
                                {
                                    sbt.Append('\r');
                                }
                                int code = (int)stt[index];
                                if ((code == 32) || (code == '\t') || (code == '\a'))
                                {
                                    sbt.Append((char)code);
                                }
                                else
                                {
                                    int glyph = pdfFont.GlyphList[code];
                                    if (glyph == 0xFFFF) glyph = 0x0000;
                                    sbt.Append(glyph.ToString("X4"));
                                }
                            }
                            stt = sbt.ToString();
                            #endregion
                        }
                        else
                        {
                            stt = ConvertToEscapeSequence(stt);
                        }

                        double textLineX = textX;
                        double textLineY = textY + (double)(fontLineHeight) * (linesCount - indexLine - 1);
                        bool isTextHorAlignmentWidth = false;

                        #region HorAlign
                        bool useSpaceWidth = false;
                        if (mTextHorAlign != null)
                        {
                            StiTextHorAlignment horAlign = mTextHorAlign.HorAlignment;

                            if (needWidthAlign)
                            {
                                if ((stt.Length > 0) && (stt[stt.Length - 1] == '\a'))
                                {
                                    stt = stt.Substring(0, stt.Length - 1);
                                }
                                else
                                {
                                    horAlign = StiTextHorAlignment.Left;
                                }
                            }

                            if (textOpt != null && textOpt.TextOptions != null &&
                                textOpt.TextOptions.RightToLeft)
                            {
                                if (horAlign == StiTextHorAlignment.Left) horAlign = StiTextHorAlignment.Right;
                                else if (horAlign == StiTextHorAlignment.Right) horAlign = StiTextHorAlignment.Left;
                            }

                            if (useWysiwyg && summ > textW)
                            {
                                horAlign = StiTextHorAlignment.Width;
                            }

                            // left justify - not need any offset
                            if (horAlign == StiTextHorAlignment.Center)
                            {
                                textLineX += (textW - summ) / 2;
                            }
                            if (horAlign == StiTextHorAlignment.Right)
                            {
                                textLineX += textW - summ;
                            }
                            if (horAlign == StiTextHorAlignment.Width)
                            {
                                #region calculate width align
                                int numSpaces = 0;
                                for (int spaceIndex = 0; spaceIndex < stt.Length; spaceIndex++)
                                {
                                    if (stt[spaceIndex] == ' ') numSpaces++;
                                }
                                if ((numSpaces > 0) && ((indexLine != stringList.Count - 1) || (useWysiwyg && summ > textW) || needWidthAlign))
                                {
                                    useSpaceWidth = true;
                                    double spaceOffset = ((textW - summ) / numSpaces) * 1000 / (sizeInPt * fontCorrectValue);
                                    if (spaceOffset > 0 || useWysiwyg)
                                    {
                                        isTextHorAlignmentWidth = true;
                                        spaceOffset += pdfFont.Widths[0];
                                        if (spaceOffset < pdfFont.Widths[0] * 0.5) spaceOffset = pdfFont.Widths[0] * 0.5;
                                        StringBuilder sbSpace = new StringBuilder((useUnicode ? "<" : "("));
                                        for (int charIndex = 0; charIndex < stt.Length; charIndex++)
                                        {
                                            if (stt[charIndex] == ' ')
                                            {
                                                if (useUnicode)
                                                {
                                                    sbSpace.Append(">-" + ConvertToString(spaceOffset) + "<");
                                                }
                                                else
                                                {
                                                    sbSpace.Append(")-" + ConvertToString(spaceOffset) + "(");
                                                }
                                            }
                                            else
                                            {
                                                sbSpace.Append(stt[charIndex]);
                                            }
                                        }
                                        sbSpace.Append((useUnicode ? ">" : ")"));
                                        stt = sbSpace.ToString();
                                    }
                                }
                                #endregion
                            }
                        }
                        #endregion

                        #region write tabs offset
                        if (numTabs > 0)
                        {
                            int currentTab = 0;
                            StringBuilder sbTabs = new StringBuilder();
                            if (useSpaceWidth == false)
                            {
                                sbTabs.Append((useUnicode ? "<" : "("));
                            }
                            for (int charIndex = 0; charIndex < stt.Length; charIndex++)
                            {
                                if (stt[charIndex] == '\t')
                                {
                                    //								double tabOffset = tabsArr[currentTab++] * sizeInPt / 1000 * fontCorrectValue;
                                    double tabOffset = tabsArr[currentTab++];
                                    if (useUnicode)
                                    {
                                        sbTabs.Append(">-" + ConvertToString(tabOffset) + "<");
                                    }
                                    else
                                    {
                                        sbTabs.Append(")-" + ConvertToString(tabOffset) + "(");
                                    }
                                }
                                else
                                {
                                    sbTabs.Append(stt[charIndex]);
                                }
                            }
                            if (useSpaceWidth == false)
                            {
                                sbTabs.Append((useUnicode ? ">" : ")"));
                            }
                            stt = sbTabs.ToString();
                        }
                        #endregion

                        if (useUnicode)
                        {
                            stt = stt.Replace(" ", pdfFont.GlyphList[32].ToString("X4")).Replace("\a", "0000");
                        }

                        #region prepare text line to write
                        bool needChildFonts = pdfFont.ChildFontsMap != null;
                        byte currentChildFont = 0;
                        //StringBuilder sbbbTemp = new StringBuilder();   // !!!
                        byte[] charr = new byte[stt.Length];
                        ArrayList warpPoints = new ArrayList();
                        for (int tempIndex = 0; tempIndex < stt.Length; tempIndex++)
                        {
                            char ch = stt[tempIndex];
                            charr[tempIndex] = (byte)ch;
                            //if (isSymbolic && !useUnicode)
                            //{
                            //    uint symb = pdfFont.UnicodeMapBack[ch];
                            //    if (symb >= 0xF000 && symb <= 0xF0FF)
                            //    {
                            //        charr[tempIndex] = (byte)(symb - 0xF000);
                            //    }
                            //}
                            bool isWarp = false;
                            if (ch == '\r')
                            {
                                warpPoints.Add(tempIndex);
                                isWarp = true;
                            }
                            if (needChildFonts)
                            {
                                uint symb = pdfFont.UnicodeMapBack[ch];
                                if (ch == '\\')
                                {
                                    symb = pdfFont.UnicodeMapBack[stt[tempIndex + 1]];
                                }

                                byte tempChildFont = pdfFont.ChildFontsMap[symb];
                                if (symb < 64)
                                {
                                    if (tempIndex > 0)
                                    {
                                        tempChildFont = currentChildFont;
                                    }
                                    else
                                    {
                                        if (stt.Length > 1) tempChildFont = pdfFont.ChildFontsMap[pdfFont.UnicodeMapBack[stt[1]]];
                                    }
                                }

                                if (ch != '\\')
                                {
                                    charr[tempIndex] = (byte)pdfFont.fonts[tempChildFont].UnicodeMap[symb];
                                }
                                //sbbbTemp.Append((char)charr[tempIndex]);  // !!!
                                if (tempChildFont != currentChildFont && !isWarp && tempIndex != 0)
                                {
                                    warpPoints.Add(tempIndex);
                                }
                                currentChildFont = tempChildFont;
                            }
                        }
                        warpPoints.Add(stt.Length);
                        #endregion

                        underlineArray[indexLine, 0] = textLineX;
                        underlineArray[indexLine, 1] = textLineY;
                        underlineArray[indexLine, 2] = summ;

                        if (pdfFont.NeedSyntItalic && (mFont != null) && mFont.Font.Italic)
                        {
                            Matrix matrix2 = new Matrix(1, 0, italicAngleTanValue, 1, (float)textLineX, (float)textLineY);
                            if (matrix != null)
                            {
                                matrix2.Multiply(matrix, MatrixOrder.Append);
                            }
                            pageStream.WriteLine("{0} {1} {2} {3} {4} {5} Tm",
                                ConvertToString(matrix2.Elements[0]),
                                ConvertToString(matrix2.Elements[1]),
                                ConvertToString(matrix2.Elements[2]),
                                ConvertToString(matrix2.Elements[3]),
                                ConvertToString(matrix2.Elements[4]),
                                ConvertToString(matrix2.Elements[5]));
                        }
                        else
                        {
                            pageStream.WriteLine("{0} {1} Td", ConvertToString(textLineX - lastTextLineX), ConvertToString(textLineY - lastTextLineY));
                            lastTextLineX = textLineX;
                            lastTextLineY = textLineY;
                        }

                        int startPoint = 0;
                        //foreach (int warpPoint in warpPoints)
                        for (int indexWarpPoint = 0; indexWarpPoint < warpPoints.Count; indexWarpPoint++)
                        {
                            if (needChildFonts && stt.Length > 0)
                            {
                                int posCharToGetFont = (stt[startPoint] < 64) && (stt.Length > startPoint + 1) ? startPoint + 1 : startPoint;
                                byte tempChildFont = pdfFont.ChildFontsMap[pdfFont.UnicodeMapBack[stt[posCharToGetFont]]];
                                pageStream.WriteLine("/F{0} {1} Tf", tempChildFont, ConvertToString(sizeInPt * fontCorrectValue, precision_digits_font));
                            }
                            int warpPoint = (int)warpPoints[indexWarpPoint];
                            int warpLength = warpPoint - startPoint;
                            if (warpLength > 0)
                            {
                                if ((isTextHorAlignmentWidth == true) || (numTabs > 0))
                                {
                                    pageStream.Write("[");
                                    if (indexWarpPoint > 0) pageStream.Write(useUnicode ? "<" : "(");
                                    pageStream.Flush();
                                    memoryPageStream.Write(charr, startPoint, warpLength);
                                    if (indexWarpPoint < warpPoints.Count - 1) pageStream.Write(useUnicode ? ">" : ")");
                                    pageStream.WriteLine("] TJ");
                                }
                                else
                                {
                                    pageStream.Write((useUnicode ? "<" : "("));
                                    pageStream.Flush();
                                    memoryPageStream.Write(charr, startPoint, warpLength);
                                    pageStream.WriteLine("{0} Tj", (useUnicode ? ">" : ")"));
                                }
                            }
                            startPoint = warpPoint;
                            if (warpPoint < charr.Length - 1 && charr[warpPoint] == '\r') startPoint++;
                        }
                    }

                    if ((pdfFont.NeedSyntBold) && (mFont != null) && (mFont.Font.Bold))
                    {
                        pageStream.WriteLine("0 Tr");
                    }
                    #endregion

                    pageStream.WriteLine("ET");

                    #region underline text
                    if ((mFont != null) && (mFont.Font.Underline))
                    {
                        pageStream.WriteLine("q");
                        PushColorToStack();
                        double underscoreSize = sizeInPt * 0.09;
                        double underscorePosition = -sizeInPt * 0.115;
                        if (pdfFont.UnderscoreSize != 0)
                        {
                            underscoreSize = fontUnderscoreSize;
                            underscorePosition = fontUnderscorePosition;
                        }
                        if (underscoreSize < 0.1f) underscoreSize = 0.1f;
                        pageStream.WriteLine("{0} w", ConvertToString(underscoreSize));
                        if (textAngle != 0)
                        {
                            double AngleInRadians = textAngle * Math.PI / 180f;
                            pageStream.WriteLine("{0} {1} {2} {3} {4} {5} cm",
                                ConvertToString(Math.Cos(AngleInRadians)),
                                ConvertToString(Math.Sin(AngleInRadians)),
                                ConvertToString(-Math.Sin(AngleInRadians)),
                                ConvertToString(Math.Cos(AngleInRadians)),
                                ConvertToString(normTextX + normTextW / 2),
                                ConvertToString(normTextY + normTextH / 2));
                        }
                        if (mTextBrush != null)
                        {
                            //tempColor = StiBrush.ToColor(mTextBrush.TextBrush);
                            SetStrokeColor(textColor);
                        }
                        for (int tempIndex = 0; tempIndex < stringList.Count; tempIndex++)
                        {
                            if (underlineArray[tempIndex, 2] != 0)
                            {
                                pageStream.WriteLine("{0} {1} m",
                                    ConvertToString(underlineArray[tempIndex, 0]),
                                    ConvertToString(underlineArray[tempIndex, 1] + underscorePosition));
                                pageStream.WriteLine("{0} {1} l S",
                                    ConvertToString(underlineArray[tempIndex, 0] + underlineArray[tempIndex, 2]),
                                    ConvertToString(underlineArray[tempIndex, 1] + underscorePosition));
                            }
                        }
                        pageStream.WriteLine("Q");
                        PopColorFromStack();
                    }
                    #endregion

                    #region strikeout text
                    if ((mFont != null) && (mFont.Font.Strikeout))
                    {
                        pageStream.WriteLine("q");
                        PushColorToStack();
                        double strikeoutSize = sizeInPt * 0.09;
                        double strikeoutPosition = sizeInPt * 0.4;
                        if (pdfFont.StrikeoutSize != 0)
                        {
                            strikeoutSize = fontStrikeoutSize;
                            strikeoutPosition = fontStrikeoutPosition;
                        }
                        if (strikeoutSize < 0.1f) strikeoutSize = 0.1f;
                        pageStream.WriteLine("{0} w", ConvertToString(strikeoutSize));
                        if (textAngle != 0)
                        {
                            double AngleInRadians = textAngle * Math.PI / 180f;
                            pageStream.WriteLine("{0} {1} {2} {3} {4} {5} cm",
                                ConvertToString(Math.Cos(AngleInRadians)),
                                ConvertToString(Math.Sin(AngleInRadians)),
                                ConvertToString(-Math.Sin(AngleInRadians)),
                                ConvertToString(Math.Cos(AngleInRadians)),
                                ConvertToString(normTextX + normTextW / 2),
                                ConvertToString(normTextY + normTextH / 2));
                        }
                        if (mTextBrush != null)
                        {
                            //tempColor = StiBrush.ToColor(mTextBrush.TextBrush);
                            SetStrokeColor(textColor);
                        }
                        for (int tempIndex = 0; tempIndex < stringList.Count; tempIndex++)
                        {
                            if (underlineArray[tempIndex, 2] != 0)
                            {
                                pageStream.WriteLine("{0} {1} m",
                                    ConvertToString(underlineArray[tempIndex, 0]),
                                    ConvertToString(underlineArray[tempIndex, 1] + strikeoutPosition));
                                pageStream.WriteLine("{0} {1} l S",
                                    ConvertToString(underlineArray[tempIndex, 0] + underlineArray[tempIndex, 2]),
                                    ConvertToString(underlineArray[tempIndex, 1] + strikeoutPosition));
                            }
                        }
                        pageStream.WriteLine("Q");
                        PopColorFromStack();
                    }
                    #endregion

                }

                #region store annotation data
                if (needAnnots)
                {
                    StiEditableObject seo = (StiEditableObject)annotsArray[annotsCurrent];
                    seo.Multiline = wordWrap;
                    seo.X = pp.X;
                    seo.Y = pp.Y;
                    seo.Width = pp.Width;
                    seo.Height = pp.Height;
                    seo.Text = annotText;
                    seo.Component = pp.Component;

                    seo.Alignment = StiTextHorAlignment.Left;
                    if (mTextHorAlign != null)
                    {
                        StiTextHorAlignment horAlign = mTextHorAlign.HorAlignment;
                        if (textOpt != null && textOpt.TextOptions != null && textOpt.TextOptions.RightToLeft)
                        {
                            if (horAlign == StiTextHorAlignment.Left) horAlign = StiTextHorAlignment.Right;
                            else if (horAlign == StiTextHorAlignment.Right) horAlign = StiTextHorAlignment.Left;
                        }
                        seo.Alignment = horAlign;
                    }
                }
                #endregion

                #region Lines of underline
                if (text.LinesOfUnderline != StiPenStyle.None)
                {
                    #region calculate coordinate
                    double coordTextY = pp.Y + marginB + correctY;
                    bool needLineUp = true;
                    bool needLineDown = true;
                    if (mVertAlign != null)
                    {
                        if (linesCount == 0) linesCount = 1;
                        double textHeight = fontLineHeight * linesCount;
                        StiVertAlignment vertAlignment = mVertAlign.VertAlignment;
                        if ((textAngle != 0) && (textAngle != 90) && (textAngle != 180) && (textAngle != 270))
                        {
                            vertAlignment = StiVertAlignment.Center;
                        }
                        if (vertAlignment == StiVertAlignment.Top)
                        {
                            coordTextY += textH - fontAscF;
                            needLineUp = false;
                        }
                        if (vertAlignment == StiVertAlignment.Center)
                        {
                            coordTextY += (textH - textHeight) / 2 + (fontLineHeight - fontAscF);
                        }
                        if (vertAlignment == StiVertAlignment.Bottom)
                        {
                            coordTextY += (fontLineHeight - fontAscF);
                            needLineDown = false;
                        }
                    }
                    coordTextY += fontDescF;
                    #endregion

                    #region style
                    bool needDouble = false;
                    double doubleLineOffset = 0;
                    IStiBorder mBorder = pp.Component as IStiBorder;
                    if (mBorder != null)
                    {
                        Color tempColor2 = mBorder.Border.Color;
                        SetStrokeColor(tempColor2);

                        double borderSizeHi = mBorder.Border.Size;
                        if (text.LinesOfUnderline == StiPenStyle.Double) borderSizeHi = 1;
                        double borderSize = borderSizeHi * hiToTwips * 0.9;
                        pageStream.WriteLine("{0} w", ConvertToString(borderSize));

                        needDouble = text.LinesOfUnderline == StiPenStyle.Double;
                        if (needDouble)
                        {
                            doubleLineOffset = borderSize;
                        }

                        #region set border style
                        double step = borderSize * 0.04;
                        switch (text.LinesOfUnderline)
                        {
                            case StiPenStyle.Dot:
                                pageStream.WriteLine("[{0} {1}] 0 d", ConvertToString(step), ConvertToString(step * 55));
                                break;

                            case StiPenStyle.Dash:
                                pageStream.WriteLine("[{0} {1}] 0 d", ConvertToString(step * 50), ConvertToString(step * 55));
                                break;

                            case StiPenStyle.DashDot:
                                pageStream.WriteLine("[{0} {1} {2} {1}] 0 d", ConvertToString(step * 50), ConvertToString(step * 55), ConvertToString(step));
                                break;

                            case StiPenStyle.DashDotDot:
                                pageStream.WriteLine("[{0} {1} {2} {1} {2} {1}] 0 d", ConvertToString(step * 50), ConvertToString(step * 55), ConvertToString(step));
                                break;
                        }
                        #endregion
                    }
                    #endregion

                    #region draw lines
                    double lineY = coordTextY;
                    if (needLineUp)
                    {
                        while (lineY + fontLineHeight < pp.Y + pp.Height - marginT)
                        {
                            lineY += fontLineHeight;
                        }
                    }
                    while (lineY > pp.Y + marginB + (needLineDown ? 0 : fontLineHeight))
                    {
                        pageStream.WriteLine("{0} {2} m {1} {2} l S",
                            ConvertToString(pp.X),
                            ConvertToString(pp.X + pp.Width),
                            ConvertToString(lineY + doubleLineOffset));
                        if (needDouble)
                        {
                            pageStream.WriteLine("{0} {2} m {1} {2} l S",
                                ConvertToString(pp.X),
                                ConvertToString(pp.X + pp.Width),
                                ConvertToString(lineY - doubleLineOffset));
                        }
                        lineY -= fontLineHeight;
                    }
                    #endregion
                }
                #endregion

                if (clipLongTextLines && !needAnnots)
                {
                    pageStream.WriteLine("Q");
                    PopColorFromStack();
                }
            }

        }
        #endregion

        #region RenderText2
        internal void RenderText2(StiPdfData pp, int pageNumber = -1)
        {
            IStiText text = pp.Component as IStiText;
            IStiTextOptions textOpt = pp.Component as IStiTextOptions;
            StiText textComp = pp.Component as StiText;

            if (text == null || pp.Width <= 0) return;

            StringBuilder sb = new StringBuilder(textComp.Text);
            if (sb.Length == 0) return;

            IStiTextHorAlignment mTextHorAlign = pp.Component as IStiTextHorAlignment;
            bool needWidthAlign = (mTextHorAlign != null) && (mTextHorAlign.HorAlignment == StiTextHorAlignment.Width);
            bool wordWrap = ((textOpt != null) && textOpt.TextOptions.WordWrap) || needWidthAlign;
            bool needAnnots = (annotsCounter > 0 || annots2Counter > 0) && textComp.Editable;
                        
            #region Calculate text coordinates
            StiMargins margins = textComp.Margins;
            double marginScale = 0; // hiToTwips;  calculated later in ConvertTextMargins
            double marginL = marginScale * margins.Left;
            double marginR = marginScale * margins.Right;
            double marginT = marginScale * margins.Top;
            double marginB = marginScale * margins.Bottom;
            double textX = pp.X + marginL;
            double textY = pp.Y + marginB;
            double textW = pp.Width - marginL - marginR;
            double textH = pp.Height - marginT - marginB;
            #endregion

            if (clipLongTextLines && !needAnnots)
            {
                pageStream.WriteLine("q");
                PushColorToStack();
                pageStream.WriteLine("{0} {1} {2} {3} re W n",
                        ConvertToString(textX),
                        ConvertToString(textY),
                        ConvertToString(textW),
                        ConvertToString(textH));
            }

            #region Calculate rotating angle
            if (needAnnots)
            {
                textX = 0;
                textY = 0;
            }
            double normTextX = textX;
            double normTextY = textY;
            double normTextW = textW;
            double normTextH = textH;
            float textAngle = 0;
            if (textOpt != null)
            {
                textAngle = textOpt.TextOptions.Angle;
                if (textAngle != 0)
                {
                    if (((textAngle > 45) && (textAngle < 135)) || ((textAngle > 225) && (textAngle < 315)))
                    {
                        double tempValue = textW;
                        textW = textH;
                        textH = tempValue;
                    }
                    textX = -textW / 2f;
                    textY = -textH / 2f;
                }
            }
            #endregion

            #region get runs info
            CheckGraphicsForTextRenderer();
            var outRunsList = new List<StiTextRenderer.RunInfo>();
            var outFontsList = new List<StiTextRenderer.StiFontState>();
            string textForOutput = sb.ToString();

            Color baseBackColor = StiBrush.ToColor(textComp.Brush);
            bool isBackColorChanged = false;

            RectangleD rectComp = textComp.Page.Unit.ConvertToHInches(pp.Component.ComponentToPage(pp.Component.ClientRectangle));
            RectangleD rectText = textComp.ConvertTextMargins(rectComp, false);
            rectText = textComp.ConvertTextBorders(rectText, false);

            bool useNewHtmlEngine = StiOptions.Engine.UseNewHtmlEngine && textComp.AllowHtmlTags;
            if (useNewHtmlEngine)
            {
                StiHtmlTextRender.DrawTextForOutput(textComp, out outRunsList, out outFontsList);

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
                    textComp.Font,
                    rectText,
                    StiBrush.ToColor(textComp.TextBrush),
                    baseBackColor,
                    textComp.LineSpacing * StiOptions.Engine.TextLineSpacingScale,
                    textComp.HorAlignment,
                    textComp.VertAlignment,
                    wordWrap,
                    textOpt.TextOptions.RightToLeft,
                    1,
                    textOpt.TextOptions.Angle,
                    textOpt.TextOptions.Trimming,
                    textOpt.TextOptions.LineLimit,
                    textComp.CheckAllowHtmlTags(),
                    outRunsList,
                    outFontsList,
                    textOpt.TextOptions,
                    1);

                if (!useUnicodeMode)
                {
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
            }
            #endregion

            foreach (StiTextRenderer.RunInfo runInfo in outRunsList)
            {
                double posX = textX + hiToTwips * (rectText.Left + runInfo.XPos - rectComp.Left);
                double posY = textY + hiToTwips * (rectComp.Bottom - (rectText.Top + runInfo.YPos));
                if (((textAngle > 45) && (textAngle < 135)) || ((textAngle > 225) && (textAngle < 315)))
                {
                    posX = textX + hiToTwips * (rectComp.Bottom - rectText.Bottom + runInfo.XPos);
                    posY = textY + hiToTwips * (rectComp.Right - (rectText.Left + runInfo.YPos));
                }

                pageStream.WriteLine("q");
                PushColorToStack();

                Font tempFont = (outFontsList[runInfo.FontIndex]).FontBase;
                int fnt = pdfFont.GetFontNumber(tempFont);
                double fntSize = tempFont.SizeInPoints * fontCorrectValue;
                if (StiDpiHelper.NeedGraphicsScale && StiDpiHelper.NeedFontScaling && !useNewHtmlEngine)
                    fntSize /= StiDpiHelper.GraphicsScale;
                pageStream.WriteLine("/F{0} {1} Tf", fnt, ConvertToString(fntSize, precision_digits_font));
                pdfFont.CurrentFont = fnt;

                bool useUnicode = pdfFont.UseUnicode;

                #region calculate font parameters
                double sizeInPt = tempFont.SizeInPoints;
                if (StiDpiHelper.NeedGraphicsScale && StiDpiHelper.NeedFontScaling && !useNewHtmlEngine)
                    sizeInPt /= StiDpiHelper.GraphicsScale;

                double cf = StiDpiHelper.NeedFontScaling ? StiDpiHelper.GraphicsScale : 1;

                double fontASC = sizeInPt * fontCorrectValue * pdfFont.ASC / 1000 * cf;
                double fontDESC = sizeInPt * fontCorrectValue * pdfFont.DESC / 1000 * cf;
                double fontLineGap = sizeInPt * fontCorrectValue * pdfFont.LineGap / 1000 * cf;
                double fonttmASC = sizeInPt * fontCorrectValue * pdfFont.tmASC / 1000 * cf;
                double fonttmDESC = sizeInPt * fontCorrectValue * pdfFont.tmDESC / 1000 * (-1) * cf;
                double fonttmExternal = sizeInPt * fontCorrectValue * pdfFont.tmExternal / 1000 * cf;
                double fontUnderscoreSize = sizeInPt * fontCorrectValue * pdfFont.UnderscoreSize / 1000 * cf;
                double fontUnderscorePosition = sizeInPt * fontCorrectValue * pdfFont.UnderscorePosition / 1000 * cf;
                double fontStrikeoutSize = sizeInPt * fontCorrectValue * pdfFont.StrikeoutSize / 1000 * cf;
                double fontStrikeoutPosition = sizeInPt * fontCorrectValue * pdfFont.StrikeoutPosition / 1000 * cf;

                double fontLineHeight = fonttmASC - fonttmDESC + fonttmExternal;
                double fontAscF = fonttmASC;
                double fontDescF = fonttmDESC;
                if ((pdfFont.fsSelection & 0x80) > 0)    //USE_TYPO_METRICS
                {
                    fontLineHeight = (fontASC - fontDESC + fontLineGap) * textComp.LineSpacing;
                    fontAscF = fontASC;
                    fontDescF = fontDESC;
                }
                #endregion

                bool hasGlyphs = true;
                if (useNewHtmlEngine && useUnicode)
                {
                    if ((runInfo.GlyphIndexList != null) && (runInfo.GlyphIndexList.Length > 0) && (runInfo.GlyphIndexList[0] == -1))
                    {
                        hasGlyphs = false;
                        for (int index = 0; index < runInfo.Text.Length; index++)
                        {
                            runInfo.GlyphIndexList[index] = pdfFont.GlyphList[pdfFont.UnicodeMap[runInfo.Text[index]]];
                        }
                    }
                }

                posY -= fontAscF; // +fonttmExternal;

                #region calculate line width
                int summGdi = 0;
                int summPdf = 0;
                for (int indexGlyph = 0; indexGlyph < runInfo.GlyphIndexList.Length; indexGlyph++)
                {
                    int currentChar = 0;
                    if (indexGlyph < runInfo.Text.Length) currentChar = runInfo.Text[indexGlyph];
                    if (useUnicode && hasGlyphs)
                    {
                        currentChar = runInfo.GlyphIndexList[indexGlyph];
                        currentChar = pdfFont.GlyphBackList[currentChar];
                    }
                    int code = pdfFont.UnicodeMap[currentChar];
                    int currentCharWidth = 1000;	//default width of symbols
                    if (code >= 32)
                    {
                        currentCharWidth = pdfFont.Widths[code - 32];
                    }
                    if (currentChar == 0 && pdfFont.GlyphWidths != null)
                    {
                        currentCharWidth = pdfFont.GlyphWidths[runInfo.GlyphIndexList[indexGlyph]];
                    }
                    currentCharWidth = (int)(currentCharWidth * runInfo.ScaleList[indexGlyph]);
                    if (useUnicode)
                    {
                        if (runInfo.GlyphWidths[indexGlyph] > 0)
                        {
                            summGdi += runInfo.GlyphWidths[indexGlyph];
                            summPdf += currentCharWidth;
                        }
                    }
                    else
                    {
                        summGdi += runInfo.Widths[indexGlyph];
                        summPdf += currentCharWidth;
                    }
                }
                double lineWidth = summGdi * hiToTwips;
                double lineScale = (lineWidth / ((sizeInPt * fontCorrectValue) / 1000)) / summPdf;
                #endregion

                #region set colors
                if (!isBackColorChanged)
                {
                    if (!runInfo.BackColor.Equals(baseBackColor)) isBackColorChanged = true;
                }
                if (isBackColorChanged && runInfo.BackColor.A != 0)
                {
                    SetNonStrokeColor(runInfo.BackColor);	//fill color
                    pageStream.WriteLine("{0} {1} {2} {3} re f",
                        ConvertToString(posX),
                        ConvertToString(posY - (-fonttmDESC)),
                        ConvertToString(lineWidth),
                        ConvertToString(fontLineHeight));
                }

                SetNonStrokeColor(runInfo.TextColor);
                #endregion

                bool useOldMode = false;

                #region correct text line length
                StringBuilder stt = new StringBuilder();
                summGdi = 0;
                summPdf = 0;
                int summPdf2 = 0;
                stt.Append((useUnicode ? "<" : "("));
                for (int index = 0; index < runInfo.GlyphIndexList.Length; index++)
                {
                    int currentChar = 0;
                    if (index < runInfo.Text.Length) currentChar = runInfo.Text[index];
                    if (useUnicode && hasGlyphs)
                    {
                        currentChar = runInfo.GlyphIndexList[index];
                        currentChar = pdfFont.GlyphBackList[currentChar];
                    }
                    int code = pdfFont.UnicodeMap[currentChar];
                    int currentCharWidth = 1000;	//default width of symbols
                    if (code >= 32)
                    {
                        currentCharWidth = pdfFont.Widths[code - 32];
                    }
                    //if (currentChar == 0 && pdfFont.GlyphWidths != null)
                    //{
                    //    currentCharWidth = pdfFont.GlyphWidths[runInfo.GlyphIndexList[index]];
                    //}

                    double correctionValue = 0;
                    if (useOldMode)
                    {
                        summGdi += runInfo.GlyphWidths[index];
                        summPdf += currentCharWidth;
                        correctionValue = -(summGdi * hiToTwips / ((sizeInPt * fontCorrectValue) / 1000) - summPdf);
                        summPdf += -(int)correctionValue;
                    }
                    else
                    {
                        if ((useUnicode ? runInfo.GlyphWidths[index] : runInfo.Widths[index]) > 0)
                        {
                            summPdf += (int)(currentCharWidth * runInfo.ScaleList[index]);
                            summPdf2 += currentCharWidth;
                            correctionValue = -(summPdf * lineScale - summPdf2);
                            summPdf2 += -(int)correctionValue;
                        }
                        else
                        {
                            correctionValue = currentCharWidth;
                        }
                    }

                    if (useUnicode)
                    {
                        //int glyph = pdfFont.GlyphList[code];
                        //if (useGlyphIndex) glyph = runInfo.GlyphIndexList[index];
                        int glyph = runInfo.GlyphIndexList[index];
                        if (glyph == 0xFFFF) glyph = 0x0000;
                        stt.Append(glyph.ToString("X4"));
                        if (index < runInfo.GlyphIndexList.Length - 1)
                        {
                            stt.Append(">" + ConvertToString(correctionValue) + "<");
                        }
                    }
                    else
                    {
                        stt.Append(ConvertToEscapeSequencePlusTabs(((char)code).ToString()));
                        if (index < runInfo.GlyphIndexList.Length - 1)
                        {
                            stt.Append(")" + ConvertToString(correctionValue) + "(");
                        }
                    }
                }
                stt.Append((useUnicode ? ">" : ")"));
                #endregion

                #region output text
                byte[] charr = new byte[stt.Length];
                for (int tempIndex = 0; tempIndex < stt.Length; tempIndex++)
                {
                    charr[tempIndex] = (byte)stt[tempIndex];
                }
                pageStream.WriteLine("BT");

                Matrix matrix = null;
                if (textAngle != 0)
                {
                    double AngleInRadians = textAngle * Math.PI / 180f;
                    matrix = new Matrix(
                        (float)Math.Cos(AngleInRadians),
                        (float)Math.Sin(AngleInRadians),
                        (float)-Math.Sin(AngleInRadians),
                        (float)Math.Cos(AngleInRadians),
                        (float)(normTextX + normTextW / 2),
                        (float)(normTextY + normTextH / 2));
                    Matrix matrix2 = new Matrix(1, 0, 0, 1, (float)posX, (float)posY);
                    matrix.Multiply(matrix2, MatrixOrder.Prepend);
                }

                if (pdfFont.NeedSyntItalic && tempFont.Italic)
                {
                    if (matrix != null)
                    {
                        Matrix matrix2 = new Matrix(1, 0, italicAngleTanValue, 1, 0, 0);
                        matrix.Multiply(matrix2, MatrixOrder.Prepend);
                    }
                    else
                    {
                        Matrix matrix2 = new Matrix(1, 0, italicAngleTanValue, 1, (float)posX, (float)posY);
                        matrix = matrix2;
                    }
                }

                if (matrix != null)
                {
                    pageStream.WriteLine("{0} {1} {2} {3} {4} {5} Tm",
                        ConvertToString(matrix.Elements[0], 7),
                        ConvertToString(matrix.Elements[1], 7),
                        ConvertToString(matrix.Elements[2], 7),
                        ConvertToString(matrix.Elements[3], 7),
                        ConvertToString(matrix.Elements[4], 6),
                        ConvertToString(matrix.Elements[5], 6));
                }
                else
                {
                    pageStream.WriteLine("{0} {1} Td", ConvertToString(posX), ConvertToString(posY));
                }

                if (pdfFont.NeedSyntBold && tempFont.Bold)
                {
                    pageStream.WriteLine("{0} w 2 Tr", ConvertToString(boldFontStrokeWidthValue * sizeInPt * fontCorrectValue, precision_digits_font));
                    SetStrokeColor(runInfo.TextColor);
                }

                pageStream.Write("[");
                pageStream.Flush();
                memoryPageStream.Write(charr, 0, charr.Length);
                pageStream.WriteLine("] TJ");
                pageStream.WriteLine("ET");

                if (pdfFont.NeedSyntBold && tempFont.Bold)
                {
                    pageStream.WriteLine("0 Tr");
                }
                #endregion

                #region underline text
                if (tempFont.Underline)
                {
                    pageStream.WriteLine("q");
                    PushColorToStack();
                    double underscoreSize = sizeInPt * 0.09;
                    double underscorePosition = -sizeInPt * 0.115;
                    if (pdfFont.UnderscoreSize != 0)
                    {
                        underscoreSize = fontUnderscoreSize;
                        underscorePosition = fontUnderscorePosition;
                    }
                    if (underscoreSize < 0.1f) underscoreSize = 0.1f;
                    pageStream.WriteLine("{0} w", ConvertToString(underscoreSize));
                    if (textAngle != 0)
                    {
                        double AngleInRadians = textAngle * Math.PI / 180f;
                        pageStream.WriteLine("{0} {1} {2} {3} {4} {5} cm",
                            ConvertToString(Math.Cos(AngleInRadians), 7),
                            ConvertToString(Math.Sin(AngleInRadians), 7),
                            ConvertToString(-Math.Sin(AngleInRadians), 7),
                            ConvertToString(Math.Cos(AngleInRadians), 7),
                            ConvertToString(normTextX + normTextW / 2, 6),
                            ConvertToString(normTextY + normTextH / 2, 6));
                    }
                    SetStrokeColor(runInfo.TextColor);
                    pageStream.WriteLine("{0} {1} m",
                        ConvertToString(posX),
                        ConvertToString(posY + underscorePosition));
                    pageStream.WriteLine("{0} {1} l S",
                        ConvertToString(posX + lineWidth),
                        ConvertToString(posY + underscorePosition));
                    pageStream.WriteLine("Q");
                    PopColorFromStack();
                }
                #endregion

                #region strikeout text
                if (tempFont.Strikeout)
                {
                    pageStream.WriteLine("q");
                    PushColorToStack();
                    double strikeoutSize = sizeInPt * 0.09;
                    double strikeoutPosition = sizeInPt * 0.4;
                    if (pdfFont.StrikeoutSize != 0)
                    {
                        strikeoutSize = fontStrikeoutSize;
                        strikeoutPosition = fontStrikeoutPosition;
                    }
                    if (strikeoutSize < 0.1f) strikeoutSize = 0.1f;
                    pageStream.WriteLine("{0} w", ConvertToString(strikeoutSize));
                    if (textAngle != 0)
                    {
                        double AngleInRadians = textAngle * Math.PI / 180f;
                        pageStream.WriteLine("{0} {1} {2} {3} {4} {5} cm",
                            ConvertToString(Math.Cos(AngleInRadians), 7),
                            ConvertToString(Math.Sin(AngleInRadians), 7),
                            ConvertToString(-Math.Sin(AngleInRadians), 7),
                            ConvertToString(Math.Cos(AngleInRadians), 7),
                            ConvertToString(normTextX + normTextW / 2, 6),
                            ConvertToString(normTextY + normTextH / 2, 6));
                    }
                    SetStrokeColor(runInfo.TextColor);
                    pageStream.WriteLine("{0} {1} m",
                        ConvertToString(posX),
                        ConvertToString(posY + strikeoutPosition));
                    pageStream.WriteLine("{0} {1} l S",
                        ConvertToString(posX + lineWidth),
                        ConvertToString(posY + strikeoutPosition));
                    pageStream.WriteLine("Q");
                    PopColorFromStack();
                }
                #endregion

                pageStream.WriteLine("Q");
                PopColorFromStack();

                #region Check hyperlink
                if ((pageNumber != -1) && !string.IsNullOrWhiteSpace(runInfo.Href) && !runInfo.Href.Trim().StartsWith("javascript:") && !usePdfA)
                {
                    StiLinkObject stl = new StiLinkObject();
                    stl.Link = runInfo.Href;
                    stl.X = posX;
                    stl.Y = posY + fontDescF;
                    stl.Width = lineWidth;
                    stl.Height = fontLineHeight;
                    stl.Page = pageNumber;
                    stl.DestPage = -1;
                    stl.DestY = -1;
                    linksArray.Add(stl);
                }
                #endregion
            }

            #region Lines of underline
            if (text.LinesOfUnderline != StiPenStyle.None)
            {
                //#region calculate coordinate
                //double coordTextY = pp.Y + marginB + correctY;
                //bool needLineUp = true;
                //bool needLineDown = true;
                //IStiVertAlignment mVertAlign = pp.Component as IStiVertAlignment;
                //if (mVertAlign != null)
                //{
                //    if (linesCount == 0) linesCount = 1;
                //    double textHeight = fontLineHeight * linesCount;
                //    StiVertAlignment vertAlignment = mVertAlign.VertAlignment;
                //    if ((textAngle != 0) && (textAngle != 90) && (textAngle != 180) && (textAngle != 270))
                //    {
                //        vertAlignment = StiVertAlignment.Center;
                //    }
                //    if (vertAlignment == StiVertAlignment.Top)
                //    {
                //        coordTextY += textH - fontAscF;
                //        needLineUp = false;
                //    }
                //    if (vertAlignment == StiVertAlignment.Center)
                //    {
                //        coordTextY += (textH - textHeight) / 2 + (fontLineHeight - fontAscF);
                //    }
                //    if (vertAlignment == StiVertAlignment.Bottom)
                //    {
                //        coordTextY += (fontLineHeight - fontAscF);
                //        needLineDown = false;
                //    }
                //}
                //coordTextY += fontDescF;
                //#endregion

                //#region style
                //bool needDouble = false;
                //double doubleLineOffset = 0;
                //IStiBorder mBorder = pp.Component as IStiBorder;
                //if (mBorder != null)
                //{
                //    Color tempColor2 = mBorder.Border.Color;
                //    SetStrokeColor(tempColor2);

                //    double borderSizeHi = mBorder.Border.Size;
                //    if (text.LinesOfUnderline == StiPenStyle.Double) borderSizeHi = 1;
                //    double borderSize = borderSizeHi * hiToTwips * 0.9;
                //    pageStream.WriteLine("{0} w", ConvertToString(borderSize));

                //    needDouble = text.LinesOfUnderline == StiPenStyle.Double;
                //    if (needDouble)
                //    {
                //        doubleLineOffset = borderSize;
                //    }

                //    #region set border style
                //    double step = borderSize * 0.04;
                //    switch (text.LinesOfUnderline)
                //    {
                //        case StiPenStyle.Dot:
                //            pageStream.WriteLine("[{0} {1}] 0 d", ConvertToString(step), ConvertToString(step * 55));
                //            break;

                //        case StiPenStyle.Dash:
                //            pageStream.WriteLine("[{0} {1}] 0 d", ConvertToString(step * 50), ConvertToString(step * 55));
                //            break;

                //        case StiPenStyle.DashDot:
                //            pageStream.WriteLine("[{0} {1} {2} {1}] 0 d", ConvertToString(step * 50), ConvertToString(step * 55), ConvertToString(step));
                //            break;

                //        case StiPenStyle.DashDotDot:
                //            pageStream.WriteLine("[{0} {1} {2} {1} {2} {1}] 0 d", ConvertToString(step * 50), ConvertToString(step * 55), ConvertToString(step));
                //            break;
                //    }
                //    #endregion
                //}
                //#endregion

                //#region draw lines
                //double lineY = coordTextY;
                //if (needLineUp)
                //{
                //    while (lineY + fontLineHeight < pp.Y + pp.Height - marginT)
                //    {
                //        lineY += fontLineHeight;
                //    }
                //}
                //while (lineY > pp.Y + marginB + (needLineDown ? 0 : fontLineHeight))
                //{
                //    pageStream.WriteLine("{0} {2} m {1} {2} l S",
                //        ConvertToString(pp.X),
                //        ConvertToString(pp.X + pp.Width),
                //        ConvertToString(lineY + doubleLineOffset));
                //    if (needDouble)
                //    {
                //        pageStream.WriteLine("{0} {2} m {1} {2} l S",
                //            ConvertToString(pp.X),
                //            ConvertToString(pp.X + pp.Width),
                //            ConvertToString(lineY - doubleLineOffset));
                //    }
                //    lineY -= fontLineHeight;
                //}
                //#endregion
            }
            #endregion

            if (clipLongTextLines && !needAnnots)
            {
                pageStream.WriteLine("Q");
                PopColorFromStack();
            }

        }
        #endregion

        #region RenderTextHQ
        internal void RenderTextHQ(StiPdfData pp)
        {
            IStiTextBrush mTextBrush = pp.Component as IStiTextBrush;
            IStiText text = pp.Component as IStiText;
            IStiTextOptions textOpt = pp.Component as IStiTextOptions;
            IStiTextHorAlignment mTextHorAlign = pp.Component as IStiTextHorAlignment;
            IStiVertAlignment mVertAlign = pp.Component as IStiVertAlignment;
            StiText textComp = pp.Component as StiText;

            bool wordWrap = (textOpt != null) && textOpt.TextOptions.WordWrap;
            bool needWidthAlign = (mTextHorAlign != null) && (mTextHorAlign.HorAlignment == StiTextHorAlignment.Width);
            bool useRightToLeft = ((textOpt != null) && (textOpt.TextOptions != null) && (textOpt.TextOptions.RightToLeft));
            int linesCount = 0;
            float textAngle = 0;

            if (needWidthAlign) wordWrap = true;

            bool useUnicode = pdfFont.UseUnicode;

            if (text != null)
            {
                #region Calculate text coordinates
                StiMargins margins = textComp.Margins;
                double marginL = hiToTwips * margins.Left;
                double marginR = hiToTwips * margins.Right;
                double marginT = hiToTwips * margins.Top;
                double marginB = hiToTwips * margins.Bottom;
                double textX = pp.X + marginL;
                double textY = pp.Y + marginB;
                double textW = pp.Width - marginL - marginR;
                double textH = pp.Height - marginT - marginB;
                #endregion

                if (clipLongTextLines)
                {
                    pageStream.WriteLine("q");
                    PushColorToStack();
                    pageStream.WriteLine("{0} {1} {2} {3} re W n",
                        ConvertToString(textX),
                        ConvertToString(textY),
                        ConvertToString(textW),
                        ConvertToString(textH));
                }

                #region Set text color
                Color textColor = Color.Transparent;
                if (mTextBrush != null)
                {
                    //tempColor = StiBrush.ToColor(mTextBrush.TextBrush);
                    textColor = StiBrush.ToColor(mTextBrush.TextBrush);
                    SetNonStrokeColor(textColor);

                    StoreShadingData2(pp.X, pp.Y, pp.Width, pp.Height, mTextBrush.TextBrush);
                    if (mTextBrush.TextBrush is StiGradientBrush || mTextBrush.TextBrush is StiGlareBrush)
                    {
                        pageStream.WriteLine("/Pattern cs /P{0} scn", 1 + shadingCurrent);
                    }
                    if (mTextBrush.TextBrush is StiHatchBrush)
                    {
                        StiHatchBrush hBrush = mTextBrush.TextBrush as StiHatchBrush;
                        pageStream.WriteLine("/Cs1 cs /PH{0} scn", GetHatchNumber(hBrush) + 1);
                    }
                }
                #endregion

                #region Calculate font parameters
                double sizeInPt = 1;
                IStiFont mFont = pp.Component as IStiFont;
                if (mFont != null) sizeInPt = (mFont.Font.SizeInPoints);

                double cf = StiDpiHelper.NeedFontScaling ? StiDpiHelper.GraphicsScale : 1;

                double fontASC = sizeInPt * fontCorrectValue * pdfFont.ASC / 1000 * cf;
                double fontDESC = sizeInPt * fontCorrectValue * pdfFont.DESC / 1000 * cf;
                double fontLineGap = sizeInPt * fontCorrectValue * pdfFont.LineGap / 1000 * cf;
                double fonttmASC = sizeInPt * fontCorrectValue * pdfFont.tmASC / 1000 * cf;
                double fonttmDESC = sizeInPt * fontCorrectValue * pdfFont.tmDESC / 1000 * (-1) * cf;
                double fonttmExternal = sizeInPt * fontCorrectValue * pdfFont.tmExternal / 1000 * cf;
                double fontUnderscoreSize = sizeInPt * fontCorrectValue * pdfFont.UnderscoreSize / 1000 * cf;
                double fontUnderscorePosition = sizeInPt * fontCorrectValue * pdfFont.UnderscorePosition / 1000 * cf;
                double fontStrikeoutSize = sizeInPt * fontCorrectValue * pdfFont.StrikeoutSize / 1000 * cf;
                double fontStrikeoutPosition = sizeInPt * fontCorrectValue * pdfFont.StrikeoutPosition / 1000 * cf;

                double fontLineHeight = (fonttmASC - fonttmDESC + fonttmExternal) * textComp.LineSpacing * StiOptions.Engine.TextLineSpacingScale;
                double fontAscF = fonttmASC;
                double fontDescF = fonttmDESC;
                if ((pdfFont.fsSelection & 0x80) > 0)    //USE_TYPO_METRICS
                {
                    fontLineHeight = (fontASC - fontDESC + fontLineGap) * textComp.LineSpacing;
                    fontAscF = fontASC;
                    fontDescF = fontDESC;
                }
                #endregion

                #region Correction of text coordinates
                float correctX = 0.85f;
                float correctY = 0.85f;

                //correction of coordinates for border
                textX += correctX;
                textW -= correctX * 2f;
                textY += correctY;
                textH -= correctY * 2f;

                double textQualityOffset = 0;
                if (!isWpf && textComp.TextQuality == StiTextQuality.Standard)
                {
                    textQualityOffset = pdfFont.Widths[0] / 2d * 0.99;
                    double halfSpaceOffset = textQualityOffset * (sizeInPt * fontCorrectValue) / 1000;
                    textX += halfSpaceOffset;
                    textW -= halfSpaceOffset * 2;
                }
                #endregion

                StringBuilder sb = new StringBuilder(text.Text);
                sb.Replace("\r", "");
                sb.Replace("\x1f", "");
                List<StiTextRenderer.LineInfo> arrLinesInfo;

                if (isWpf)
                {
                    arrLinesInfo = new List<StiTextRenderer.LineInfo>();
                    List<string> stringList2 = StiExportUtils.SplitString(sb.ToString(), false);

                    if (textComp.Page == null) textComp.Page = report.RenderedPages[0];
                    RectangleD rect = textComp.GetPaintRectangle(true, false);
                    rect.X += 1;
                    if (rect.Width >= 2) rect.Width -= 2;
                    rect = textComp.ConvertTextMargins(rect, false);
                    rect = textComp.ConvertTextBorders(rect, false);

                    Events.StiSplitTextIntoLinesEventArgs e = new Events.StiSplitTextIntoLinesEventArgs();
                    e.TextComp = textComp;
                    e.RectD = rect;
                    e.Text = stringList2;
                    e.LinesInfo = arrLinesInfo;
                    e.WordWrap = wordWrap;
                    string wpfText = OnSplitTextIntoLines(e);
                }
                else
                {
                    RectangleD rect = textComp.GetPaintRectangle(true, false);
                    rect = textComp.ConvertTextMargins(rect, false);
                    rect = textComp.ConvertTextBorders(rect, false);

                    var img = new Bitmap(1, 1);
                    var g = Graphics.FromImage(img);

                    arrLinesInfo = StiTextDrawing.SplitTextWordwrap(
                        sb.ToString(),
                        g,
                        StiFontUtils.ChangeFontSize(textComp.Font, textComp.Font.Size * (float)StiDpiHelper.GraphicsScale), //textComp.Font,
                        rect,
                        textOpt?.TextOptions,
                        mTextHorAlign == null ? StiTextHorAlignment.Left : mTextHorAlign.HorAlignment,
                        textComp.TextQuality == StiTextQuality.Typographic);
                }

                for (int indexLine = 0; indexLine < arrLinesInfo.Count; indexLine++)
                {
                    //arrLinesInfo[indexLine].Text = bidi.Convert(new StringBuilder(arrLinesInfo[indexLine].Text), useRightToLeft).ToString();
                    arrLinesInfo[indexLine].Text = StiBidirectionalConvert2.ConvertString(arrLinesInfo[indexLine].Text, useRightToLeft);
                }

                if ((textW > 0) && (sb.Length > 0))
                {
                    #region calculate rotating angle
                    double normTextX = textX;
                    double normTextY = textY;
                    double normTextW = textW;
                    double normTextH = textH;
                    if (textOpt != null)
                    {
                        textAngle = textOpt.TextOptions.Angle;
                        if (textAngle != 0)
                        {
                            if (((textAngle > 45) && (textAngle < 135)) || ((textAngle > 225) && (textAngle < 315)))
                            {
                                double tempValue = textW;
                                textW = textH;
                                textH = tempValue;
                            }
                            textX = -textW / 2f;
                            textY = -textH / 2f;
                        }
                    }
                    #endregion

                    linesCount = arrLinesInfo.Count;

                    int trimCountLines = -1;
                    if (textOpt != null && textOpt.TextOptions.Trimming != StringTrimming.None)
                    {
                        double trimCountLinesDouble = textH / fontLineHeight;
                        trimCountLines = (int)trimCountLinesDouble;
                        if (trimCountLinesDouble > trimCountLines) trimCountLines++;
                        if (trimCountLines < 1) trimCountLines = 1;
                        if (trimCountLines < linesCount) linesCount = trimCountLines;
                    }

                    #region VertAlign
                    if (mVertAlign != null)
                    {
                        double lineHeight = fontLineHeight;
                        double textHeight = linesCount * lineHeight;
                        StiVertAlignment vertAlignment = mVertAlign.VertAlignment;
                        if ((textAngle != 0) && (textAngle != 90) && (textAngle != 180) && (textAngle != 270))
                        {
                            vertAlignment = StiVertAlignment.Center;
                        }
                        if (vertAlignment == StiVertAlignment.Top)
                        {
                            if (CompatibleMode160)
                            {
                                textY += textH - (textHeight - (-fontDescF));
                            }
                            else
                            {
                                textY += (textH - textHeight) + (lineHeight - fontAscF);
                            }
                        }
                        if (vertAlignment == StiVertAlignment.Center)
                        {
                            if (CompatibleMode160)
                            {
                                textY += (textH - textHeight) / 2 + (-fontDescF);
                            }
                            else
                            {
                                textY += (textH - textHeight) / 2 + (lineHeight - fontAscF);
                            }
                        }
                        if (vertAlignment == StiVertAlignment.Bottom)
                        {
                            if (CompatibleMode160)
                            {
                                textY += (-fontDescF);
                            }
                            else
                            {
                                textY += (lineHeight - fontAscF);
                            }
                        }
                    }
                    #endregion

                    pageStream.WriteLine("BT");

                    #region write text
                    double lastTextLineX = 0;
                    double lastTextLineY = 0;
                    double[,] underlineArray = new double[arrLinesInfo.Count, 3];
                    Matrix matrix = null;
                    if (textAngle != 0)
                    {
                        double AngleInRadians = textAngle * Math.PI / 180f;

                        matrix = new Matrix(
                            (float)Math.Cos(AngleInRadians),
                            (float)Math.Sin(AngleInRadians),
                            (float)-Math.Sin(AngleInRadians),
                            (float)Math.Cos(AngleInRadians),
                            (float)(normTextX + normTextW / 2),
                            (float)(normTextY + normTextH / 2));

                        if (!(pdfFont.NeedSyntItalic && (mFont != null) && mFont.Font.Italic))
                        {
                            pageStream.WriteLine("{0} {1} {2} {3} {4} {5} Tm",
                                ConvertToString(matrix.Elements[0]),
                                ConvertToString(matrix.Elements[1]),
                                ConvertToString(matrix.Elements[2]),
                                ConvertToString(matrix.Elements[3]),
                                ConvertToString(matrix.Elements[4]),
                                ConvertToString(matrix.Elements[5]));
                        }
                    }

                    if ((pdfFont.NeedSyntBold) && (mFont != null) && (mFont.Font.Bold))
                    {
                        pageStream.WriteLine("{0} w 2 Tr", ConvertToString(boldFontStrokeWidthValue * sizeInPt, 3));
                        SetStrokeColor(textColor);
                    }

                    for (int indexLine = 0; indexLine < linesCount; indexLine++)
                    {
                        #region correct text line length
                        StringBuilder stb = new StringBuilder();
                        double summGdi = 0;
                        int summPdf = 0;
                        stb.Append((useUnicode ? "<" : "("));
                        var lineInfo = arrLinesInfo[indexLine];
                        for (int index = 0; index < lineInfo.Text.Length; index++)
                        {
                            int currentChar = lineInfo.Text[index];
                            if (currentChar == '\t') currentChar = 32;
                            int code = pdfFont.UnicodeMap[currentChar];
                            int currentCharWidth = 1000;	//default width of symbols
                            if (code >= 32)
                            {
                                currentCharWidth = pdfFont.Widths[code - 32];
                            }
                            double correctionValue = 0;

                            summGdi += lineInfo.Widths[index] * 10;
                            summPdf += currentCharWidth;
                            correctionValue = -(summGdi * hiToTwips / ((sizeInPt * fontCorrectValue) / 100) - summPdf);
                            summPdf += -(int)correctionValue;

                            if (useUnicode)
                            {
                                int glyph = pdfFont.GlyphList[code];
                                if (glyph == 0xFFFF) glyph = 0x0000;
                                stb.Append(glyph.ToString("X4"));
                                if (index < lineInfo.Text.Length - 1)
                                {
                                    stb.Append(">" + ConvertToString(correctionValue) + "<");
                                }
                            }
                            else
                            {
                                stb.Append(ConvertToEscapeSequencePlusTabs(((char)code).ToString()));
                                if (index < lineInfo.Text.Length - 1)
                                {
                                    stb.Append(")" + ConvertToString(correctionValue) + "(");
                                }
                            }
                        }
                        stb.Append((useUnicode ? ">" : ")"));
                        string stt = stb.ToString();
                        double summ = summGdi * hiToTwips / 10;
                        #endregion

                        double textLineX = textX;
                        double textLineY = textY + (double)(fontLineHeight) * (linesCount - indexLine - 1);
                        //bool isTextHorAlignmentWidth = false;

                        #region HorAlign
                        //bool useSpaceWidth = false;
                        if (mTextHorAlign != null)
                        {
                            StiTextHorAlignment horAlign = mTextHorAlign.HorAlignment;

                            if (needWidthAlign && !lineInfo.NeedWidthAlign)
                            {
                                horAlign = StiTextHorAlignment.Left;
                            }

                            if (textOpt != null && textOpt.TextOptions != null &&
                                textOpt.TextOptions.RightToLeft)
                            {
                                if (horAlign == StiTextHorAlignment.Left) horAlign = StiTextHorAlignment.Right;
                                else if (horAlign == StiTextHorAlignment.Right) horAlign = StiTextHorAlignment.Left;
                            }

                            // left justify - not need any offset
                            if (horAlign == StiTextHorAlignment.Center)
                            {
                                textLineX += (textW - summ) / 2;
                            }
                            if (horAlign == StiTextHorAlignment.Right)
                            {
                                textLineX += textW - summ;
                            }
                            if (horAlign == StiTextHorAlignment.Width)
                            {
                                #region calculate width align
                                //int numSpaces = 0;
                                //for (int spaceIndex = 0; spaceIndex < stt.Length; spaceIndex++)
                                //{
                                //    if (stt[spaceIndex] == ' ') numSpaces++;
                                //}
                                //if ((numSpaces > 0) && (indexLine != stringList.Count - 1))
                                //{
                                //    useSpaceWidth = true;
                                //    double spaceOffset = ((textW - summ) / numSpaces) * 1000 / (sizeInPt * fontCorrectValue);
                                //    if (spaceOffset > 0)
                                //    {
                                //        isTextHorAlignmentWidth = true;
                                //        spaceOffset += pdfFont.Widths[0];
                                //        if (spaceOffset < pdfFont.Widths[0] * 0.5) spaceOffset = pdfFont.Widths[0] * 0.5;
                                //        StringBuilder sbSpace = new StringBuilder((useUnicode ? "<" : "("));
                                //        for (int charIndex = 0; charIndex < stt.Length; charIndex++)
                                //        {
                                //            if (stt[charIndex] == ' ')
                                //            {
                                //                if (useUnicode)
                                //                {
                                //                    sbSpace.Append(">-" + ConvertToString(spaceOffset) + "<");
                                //                }
                                //                else
                                //                {
                                //                    sbSpace.Append(")-" + ConvertToString(spaceOffset) + "(");
                                //                }
                                //            }
                                //            else
                                //            {
                                //                sbSpace.Append(stt[charIndex]);
                                //            }
                                //        }
                                //        sbSpace.Append((useUnicode ? ">" : ")"));
                                //        stt = sbSpace.ToString();
                                //    }
                                //}
                                #endregion
                            }
                        }
                        #endregion

                        if (useUnicode)
                        {
                            stt = stt.Replace(" ", pdfFont.GlyphList[32].ToString("X4")).Replace("\a", "0000");
                        }
                        
                        underlineArray[indexLine, 0] = textLineX;
                        underlineArray[indexLine, 1] = textLineY;
                        underlineArray[indexLine, 2] = summ;

                        if (pdfFont.NeedSyntItalic && (mFont != null) && mFont.Font.Italic)
                        {
                            Matrix matrix2 = new Matrix(1, 0, italicAngleTanValue, 1, (float)textLineX, (float)textLineY);
                            if (matrix != null)
                            {
                                matrix2.Multiply(matrix, MatrixOrder.Append);
                            }
                            pageStream.WriteLine("{0} {1} {2} {3} {4} {5} Tm",
                                ConvertToString(matrix2.Elements[0]),
                                ConvertToString(matrix2.Elements[1]),
                                ConvertToString(matrix2.Elements[2]),
                                ConvertToString(matrix2.Elements[3]),
                                ConvertToString(matrix2.Elements[4]),
                                ConvertToString(matrix2.Elements[5]));
                        }
                        else
                        {
                            pageStream.WriteLine("{0} {1} Td", ConvertToString(textLineX - lastTextLineX), ConvertToString(textLineY - lastTextLineY));
                            lastTextLineX = textLineX;
                            lastTextLineY = textLineY;
                        }

                        #region output text
                        byte[] charr = new byte[stt.Length];
                        for (int tempIndex = 0; tempIndex < stt.Length; tempIndex++)
                        {
                            charr[tempIndex] = (byte)stt[tempIndex];
                        }

                        pageStream.Write("[");
                        pageStream.Flush();
                        memoryPageStream.Write(charr, 0, charr.Length);
                        pageStream.WriteLine("] TJ");
                        #endregion
                    }

                    if ((pdfFont.NeedSyntBold) && (mFont != null) && (mFont.Font.Bold))
                    {
                        pageStream.WriteLine("0 Tr");
                    }
                    #endregion

                    pageStream.WriteLine("ET");

                    #region underline text
                    if ((mFont != null) && (mFont.Font.Underline))
                    {
                        pageStream.WriteLine("q");
                        PushColorToStack();
                        double underscoreSize = sizeInPt * 0.09;
                        double underscorePosition = -sizeInPt * 0.115;
                        if (pdfFont.UnderscoreSize != 0)
                        {
                            underscoreSize = fontUnderscoreSize;
                            underscorePosition = fontUnderscorePosition;
                        }
                        if (underscoreSize < 0.1f) underscoreSize = 0.1f;
                        pageStream.WriteLine("{0} w", ConvertToString(underscoreSize));
                        if (textAngle != 0)
                        {
                            double AngleInRadians = textAngle * Math.PI / 180f;
                            pageStream.WriteLine("{0} {1} {2} {3} {4} {5} cm",
                                ConvertToString(Math.Cos(AngleInRadians)),
                                ConvertToString(Math.Sin(AngleInRadians)),
                                ConvertToString(-Math.Sin(AngleInRadians)),
                                ConvertToString(Math.Cos(AngleInRadians)),
                                ConvertToString(normTextX + normTextW / 2),
                                ConvertToString(normTextY + normTextH / 2));
                        }
                        if (mTextBrush != null)
                        {
                            //tempColor = StiBrush.ToColor(mTextBrush.TextBrush);
                            SetStrokeColor(textColor);
                        }
                        for (int tempIndex = 0; tempIndex < arrLinesInfo.Count; tempIndex++)
                        {
                            if (underlineArray[tempIndex, 2] != 0)
                            {
                                pageStream.WriteLine("{0} {1} m",
                                    ConvertToString(underlineArray[tempIndex, 0]),
                                    ConvertToString(underlineArray[tempIndex, 1] + underscorePosition));
                                pageStream.WriteLine("{0} {1} l S",
                                    ConvertToString(underlineArray[tempIndex, 0] + underlineArray[tempIndex, 2]),
                                    ConvertToString(underlineArray[tempIndex, 1] + underscorePosition));
                            }
                        }
                        pageStream.WriteLine("Q");
                        PopColorFromStack();
                    }
                    #endregion

                    #region strikeout text
                    if ((mFont != null) && (mFont.Font.Strikeout))
                    {
                        pageStream.WriteLine("q");
                        PushColorToStack();
                        double strikeoutSize = sizeInPt * 0.09;
                        double strikeoutPosition = sizeInPt * 0.4;
                        if (pdfFont.StrikeoutSize != 0)
                        {
                            strikeoutSize = fontStrikeoutSize;
                            strikeoutPosition = fontStrikeoutPosition;
                        }
                        if (strikeoutSize < 0.1f) strikeoutSize = 0.1f;
                        pageStream.WriteLine("{0} w", ConvertToString(strikeoutSize));
                        if (textAngle != 0)
                        {
                            double AngleInRadians = textAngle * Math.PI / 180f;
                            pageStream.WriteLine("{0} {1} {2} {3} {4} {5} cm",
                                ConvertToString(Math.Cos(AngleInRadians)),
                                ConvertToString(Math.Sin(AngleInRadians)),
                                ConvertToString(-Math.Sin(AngleInRadians)),
                                ConvertToString(Math.Cos(AngleInRadians)),
                                ConvertToString(normTextX + normTextW / 2),
                                ConvertToString(normTextY + normTextH / 2));
                        }
                        if (mTextBrush != null)
                        {
                            //tempColor = StiBrush.ToColor(mTextBrush.TextBrush);
                            SetStrokeColor(textColor);
                        }
                        for (int tempIndex = 0; tempIndex < arrLinesInfo.Count; tempIndex++)
                        {
                            if (underlineArray[tempIndex, 2] != 0)
                            {
                                pageStream.WriteLine("{0} {1} m",
                                    ConvertToString(underlineArray[tempIndex, 0]),
                                    ConvertToString(underlineArray[tempIndex, 1] + strikeoutPosition));
                                pageStream.WriteLine("{0} {1} l S",
                                    ConvertToString(underlineArray[tempIndex, 0] + underlineArray[tempIndex, 2]),
                                    ConvertToString(underlineArray[tempIndex, 1] + strikeoutPosition));
                            }
                        }
                        pageStream.WriteLine("Q");
                        PopColorFromStack();
                    }
                    #endregion

                }

                #region Lines of underline
                if (text.LinesOfUnderline != StiPenStyle.None)
                {
                    #region calculate coordinate
                    double coordTextY = pp.Y + marginB + correctY;
                    bool needLineUp = true;
                    bool needLineDown = true;
                    if (mVertAlign != null)
                    {
                        if (linesCount == 0) linesCount = 1;
                        double textHeight = fontLineHeight * linesCount;
                        StiVertAlignment vertAlignment = mVertAlign.VertAlignment;
                        if ((textAngle != 0) && (textAngle != 90) && (textAngle != 180) && (textAngle != 270))
                        {
                            vertAlignment = StiVertAlignment.Center;
                        }
                        if (vertAlignment == StiVertAlignment.Top)
                        {
                            coordTextY += textH - fontAscF;
                            needLineUp = false;
                        }
                        if (vertAlignment == StiVertAlignment.Center)
                        {
                            coordTextY += (textH - textHeight) / 2 + (fontLineHeight - fontAscF);
                        }
                        if (vertAlignment == StiVertAlignment.Bottom)
                        {
                            coordTextY += (fontLineHeight - fontAscF);
                            needLineDown = false;
                        }
                    }
                    coordTextY += fontDescF;
                    #endregion

                    #region style
                    bool needDouble = false;
                    double doubleLineOffset = 0;
                    IStiBorder mBorder = pp.Component as IStiBorder;
                    if (mBorder != null)
                    {
                        Color tempColor2 = mBorder.Border.Color;
                        SetStrokeColor(tempColor2);

                        double borderSizeHi = mBorder.Border.Size;
                        if (text.LinesOfUnderline == StiPenStyle.Double) borderSizeHi = 1;
                        double borderSize = borderSizeHi * hiToTwips * 0.9;
                        pageStream.WriteLine("{0} w", ConvertToString(borderSize));

                        needDouble = text.LinesOfUnderline == StiPenStyle.Double;
                        if (needDouble)
                        {
                            doubleLineOffset = borderSize;
                        }

                        #region set border style
                        double step = borderSize * 0.04;
                        switch (text.LinesOfUnderline)
                        {
                            case StiPenStyle.Dot:
                                pageStream.WriteLine("[{0} {1}] 0 d", ConvertToString(step), ConvertToString(step * 55));
                                break;

                            case StiPenStyle.Dash:
                                pageStream.WriteLine("[{0} {1}] 0 d", ConvertToString(step * 50), ConvertToString(step * 55));
                                break;

                            case StiPenStyle.DashDot:
                                pageStream.WriteLine("[{0} {1} {2} {1}] 0 d", ConvertToString(step * 50), ConvertToString(step * 55), ConvertToString(step));
                                break;

                            case StiPenStyle.DashDotDot:
                                pageStream.WriteLine("[{0} {1} {2} {1} {2} {1}] 0 d", ConvertToString(step * 50), ConvertToString(step * 55), ConvertToString(step));
                                break;
                        }
                        #endregion
                    }
                    #endregion

                    #region draw lines
                    double lineY = coordTextY;
                    if (needLineUp)
                    {
                        while (lineY + fontLineHeight < pp.Y + pp.Height - marginT)
                        {
                            lineY += fontLineHeight;
                        }
                    }
                    while (lineY > pp.Y + marginB + (needLineDown ? 0 : fontLineHeight))
                    {
                        pageStream.WriteLine("{0} {2} m {1} {2} l S",
                            ConvertToString(pp.X),
                            ConvertToString(pp.X + pp.Width),
                            ConvertToString(lineY + doubleLineOffset));
                        if (needDouble)
                        {
                            pageStream.WriteLine("{0} {2} m {1} {2} l S",
                                ConvertToString(pp.X),
                                ConvertToString(pp.X + pp.Width),
                                ConvertToString(lineY - doubleLineOffset));
                        }
                        lineY -= fontLineHeight;
                    }
                    #endregion
                }
                #endregion

                if (clipLongTextLines)
                {
                    pageStream.WriteLine("Q");
                    PopColorFromStack();
                }
            }

        }
        #endregion

        #region RenderTextFont
        internal void RenderTextFont(StiPdfData pp)
        {
            IStiFont mFont = pp.Component as IStiFont;
            if (mFont != null)
            {
                int fnt = pdfFont.GetFontNumber(mFont.Font);
                double fntSize = mFont.Font.SizeInPoints * fontCorrectValue;
                pageStream.WriteLine("/F{0} {1} Tf", fnt, ConvertToString(fntSize, precision_digits_font));
                pdfFont.CurrentFont = fnt;

                if ((annotsCounter > 0 || annots2Counter > 0) && (pp.Component as StiText).Editable)
                {
                    StiEditableObject seo = (StiEditableObject)annotsArray[annotsCurrent];
                    seo.FontNumber = fnt;
                    seo.FontSize = fntSize;
                }

            }
        }
        #endregion

    }
}
