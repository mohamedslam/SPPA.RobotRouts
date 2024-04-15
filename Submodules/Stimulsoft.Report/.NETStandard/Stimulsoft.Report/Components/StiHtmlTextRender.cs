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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
using Graphics = Stimulsoft.Drawing.Graphics;
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Image = Stimulsoft.Drawing.Image;
using StringFormat = Stimulsoft.Drawing.StringFormat;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using Region = Stimulsoft.Drawing.Region;
using CharacterRange = Stimulsoft.Drawing.CharacterRange;
#endif

namespace Stimulsoft.Report.Components
{
    public class StiHtmlTextRender
    {
        #region Static properties
        private static Hashtable outlineTextMetricsCache = new Hashtable();
        private static object lockOutlineTextMetricsCache = new object();

        private static Graphics otmStaticGraphics;
        private static Graphics OtmStaticGraphics
        {
            get
            {
                if (otmStaticGraphics == null)
                {
                    Image img = new Bitmap(1, 1);
                    otmStaticGraphics = Graphics.FromImage(img);
                }
                return otmStaticGraphics;
            }
        }
        #endregion

        #region StiRune
        public class StiRune
        {
            public double X, Y;
            public double Height;
            public string Text;
            public double[] Widths;
            public double[] SumWidths;
            public double[] Scales;
            public int IndexBegin;
            public int IndexEnd;
            public int FontIndex;
            public StiTextRenderer.StiHtmlTagsState State;

            public double Width => (IndexEnd > 0 ? SumWidths[IndexEnd - 1] : 0) - (IndexBegin > 0 ? SumWidths[IndexBegin - 1] : 0);

            public StiRune Clone()
            {
                return this.MemberwiseClone() as StiRune;
            }
        }
        #endregion

        #region StiOutlineTextMetric
        public class StiOutlineTextMetric
        {
            //public int otmItalicAngle;
            //public uint otmEMSquare;
            public int otmAscent;
            public int otmDescent;
            public int otmExternalLeading;
            //public uint otmLineGap;
            //public uint otmsCapEmHeight;
            //public uint otmsXHeight;
            //public RECT otmrcFontBox;
            //public POINT otmptSubscriptSize;
            //public POINT otmptSubscriptOffset;
            //public POINT otmptSuperscriptSize;
            //public POINT otmptSuperscriptOffset;
            public int otmsStrikeoutSize;
            public int otmsStrikeoutPosition;
            public int otmsUnderscoreSize;
            public int otmsUnderscorePosition;

            public Font BaseFont;

            public int[] Widths;

            internal Graphics otmGraphics;

            public int GetWidth(int sym)
            {
                if (Widths[sym] == -1)
                {
                    #region Fill part
                    lock (lockOutlineTextMetricsCache)
                    {
                        //int errorStep = 0;
                        using (StringFormat sf = new StringFormat(StringFormat.GenericTypographic))
                        {
                            sf.FormatFlags |= StringFormatFlags.NoWrap;

                            int count = 32;
                            int minSym = sym & 0xFFE0;
                            int maxSym = minSym + count;

                            if (StiOptions.Export.Pdf.AllowInvokeWindowsLibraries)
                            {
                                StringBuilder sb = new StringBuilder();
                                for (int index = minSym; index < maxSym; index++)
                                {
                                    sb.Append((char)index);
                                }
                                string str = sb.ToString();

                                var cr = new CharacterRange[count];
                                for (int index2 = 0; index2 < count; index2++)
                                {
                                    cr[index2].First = index2;
                                    cr[index2].Length = 1;
                                }
                                sf.SetMeasurableCharacterRanges(cr);

                                var ranges = new Region[count];
                                try
                                {
                                    otmGraphics.MeasureCharacterRanges(str, BaseFont, new RectangleF(0, 0, 10000000, 10000000), sf)
                                        .CopyTo(ranges, 0);
                                }
                                catch
                                {
                                    cr = new CharacterRange[1];
                                    cr[0].First = 0;
                                    cr[0].Length = 1;
                                    sf.SetMeasurableCharacterRanges(cr);
                                    for (int index2 = 0; index2 < count; index2++)
                                    {
                                        try
                                        {
                                            otmGraphics.MeasureCharacterRanges(str.Substring(index2, 1), BaseFont, new RectangleF(0, 0, 10000000, 10000000), sf).CopyTo(ranges, index2);
                                        }
                                        catch
                                        {
                                            ranges[index2] = new Region(new Rectangle(0, 0, 1000, 1000));
                                        }
                                    }
                                }

                                for (int index = minSym; index < maxSym; index++)
                                {
                                    double fl = ranges[index - minSym].GetBounds(otmGraphics).Width; // * StiDpiHelper.GraphicsScale;
                                    Widths[index] = (int)(Math.Round(fl));
                                }
                            }
                            else
                            {
                                double wwBase = otmGraphics.MeasureString("DD", BaseFont, 10000000, sf).Width;
                                for (int index = minSym; index < maxSym; index++)
                                {
                                    double fl = otmGraphics.MeasureString("D" + (char)index + "D", BaseFont, 10000000, sf).Width - wwBase; // * StiDpiHelper.GraphicsScale;

                                    if (char.GetUnicodeCategory((char)index) == global::System.Globalization.UnicodeCategory.NonSpacingMark) fl = 0;

                                    Widths[index] = (int)(Math.Round(fl * 0.99));
                                }
                            }
                        }
                    }
                    #endregion
                }
                return Widths[sym];
            }

            public StiOutlineTextMetric(Graphics gr)
            {
                otmGraphics = gr;
                Widths = new int[65536];
                for (int index = 0; index < 65536; index++) Widths[index] = -1;
            }
        }
        #endregion

        #region GetOutlineTextMetricsCached
        private static StiOutlineTextMetric GetOutlineTextMetricsCached(string fontName, FontStyle fontStyle, Graphics otmGraphics)
        {
            var st = fontName + "*" + (char)(48 + (int)fontStyle);
            var obj = outlineTextMetricsCache[st];
            if (obj == null)
            {
                var otm = new StiOutlineTextMetric(otmGraphics);

                lock (lockOutlineTextMetricsCache)
                {
                    //int errorStep = 0;
                    try
                    {
                        otm.BaseFont = StiFontCollection.CreateFont(fontName, (float)(StiTextRenderer.MaxFontSize * StiDpiHelper.GraphicsScale), fontStyle);
                        using (StringFormat sf = new StringFormat(StringFormat.GenericTypographic))
                        {
                            sf.FormatFlags |= StringFormatFlags.NoWrap;

                            //get text metrics
                            string str = "0\n1";
                            CharacterRange[] cr = new CharacterRange[1];
                            cr[0] = new CharacterRange(2, 1);
                            sf.SetMeasurableCharacterRanges(cr);

                            //errorStep = 10;

                            Region[] ranges = otmGraphics.MeasureCharacterRanges(str, otm.BaseFont, new RectangleF(0, 0, 10000000, 10000000), sf);
                            RectangleF rect = ranges[0].GetBounds(otmGraphics);

                            //errorStep = 20;

                            double symHeight = rect.Height;
                            double lineHeight = rect.Y;
                            double wwBase = 0;

                            if (!StiOptions.Export.Pdf.AllowInvokeWindowsLibraries)
                            {
                                symHeight = otmGraphics.MeasureString("D\r\nD\r\nD\r\nD\r\nD\r\nD\r\nD\r\nD\r\nD\r\nD", otm.BaseFont, 10000000, sf).Height / 10; // * 0.987;
                                lineHeight = symHeight;
                                wwBase = otmGraphics.MeasureString("DD", otm.BaseFont, 10000000, sf).Width;
                            }

                            if (StiOptions.Engine.TextLineSpacingScale != 1)
                            {
                                symHeight *= StiOptions.Engine.TextLineSpacingScale;
                                lineHeight *= StiOptions.Engine.TextLineSpacingScale;
                            }

                            otm.otmAscent = (int)(symHeight * 0.8);
                            otm.otmDescent = (int)(symHeight * 0.2);
                            otm.otmExternalLeading = (int)(lineHeight - symHeight);
                            otm.otmsStrikeoutPosition = (int)(symHeight * 0.25);
                            otm.otmsStrikeoutSize = (int)(symHeight * 0.05);
                            otm.otmsUnderscorePosition = -(int)(symHeight * 0.1);
                            otm.otmsUnderscoreSize = (int)(symHeight * 0.07);
                        }
                    }
                    catch
                    {
                    }

                    outlineTextMetricsCache[st] = otm;
                }
                return otm;
            }
            var otm2 = (StiOutlineTextMetric)obj;
            otm2.otmGraphics = otmGraphics;
            return otm2;
        }
        #endregion

        #region StiFontState2
        public class StiFontState2 : StiTextRenderer.StiFontState
        {
            //public int[] Widths;
            public StiOutlineTextMetric Otm;

            public static StiFontState2 CreateFrom(StiTextRenderer.StiFontState state)
            {
                var state2 = new StiFontState2() { Ascend = state.Ascend, Descend = state.Descend, EmValue = state.EmValue, FontBase = state.FontBase, LineHeight = state.LineHeight, FontName = state.FontName };
                return state2;
            }
        }
        #endregion

        #region StiHtmlParser
        public class StiHtmlParser
        {
            #region Fields
            private List<StiTextRenderer.StiFontState> fontList = null;
            private StiTextRenderer.StiHtmlState[] stateList = null;

            private List<StiRune> outputRunes = null;
            private List<StiRune> currentLineRunes = null;

            private double positionX = 0;
            private double positionY = 0;
            private double correctX = 0;
            private double indentCalcSize = 0;

            private RectangleD bounds;
            private double breakHeight = 0;
            private bool needBreakText = false;
            private bool isRightToLeft = false;

            private int lastLineBeginStateIndex = 0;
            private int lastLineBeginRuneIndex = -1;

            private Graphics otmGraphics = null;
            #endregion

            public SizeD Parse(StiText textbox, string text, out List<StiTextRenderer.RunInfo> outRunsList, out List<StiTextRenderer.StiFontState> outFontsList, double breakHeight = 0, List<string> breakParts = null)
            {
                otmGraphics = (textbox.Report != null) ? textbox.GetMeasureGraphics() : OtmStaticGraphics;

                outRunsList = new List<StiTextRenderer.RunInfo>();
                outFontsList = new List<StiTextRenderer.StiFontState>();

                if (textbox.Page != null)
                {
                    bounds = textbox.Page.Unit.ConvertToHInches(textbox.ClientRectangle);
                }
                else
                {
                    bounds = textbox.GetPaintRectangle(true, false);
                }
                bounds = textbox.ConvertTextMargins(bounds, false);
                bounds = textbox.ConvertTextBorders(bounds, false);

                correctX = otmGraphics.MeasureString(" ", textbox.Font).Width / 5;
                bounds.X += correctX;
                bounds.Width -= correctX * 2;

                float angle = textbox.Angle;
                if ((angle > 45 && angle < 135) || (angle > 215 && angle < 315))
                {
                    double tempW = bounds.Width;
                    bounds.Width = bounds.Height;
                    bounds.Height = tempW;
                }

                this.breakHeight = breakHeight;
                this.needBreakText = breakParts != null;
                var forceWidthAlign = text != null && text.EndsWith(StiTextRenderer.StiForceWidthAlignTag, StringComparison.InvariantCulture);

                isRightToLeft = textbox.RightToLeft;

                SizeD size = new SizeD(0, 0);
                double minX = bounds.Right;
                double maxX = 0;

                try
                {
                    #region Prepare states
                    var baseTagsState = new StiTextRenderer.StiHtmlTagsState(
                        textbox.Font.Bold,
                        textbox.Font.Italic,
                        textbox.Font.Underline,
                        textbox.Font.Strikeout,
                        textbox.Font.SizeInPoints,
                        textbox.Font.Name,
                        StiBrush.ToColor(textbox.TextBrush),
                        StiBrush.ToColor(textbox.Brush),
                        false,
                        false,
                        0,
                        0,
                        textbox.LineSpacing,
                        textbox.HorAlignment);
                    var baseState = new StiTextRenderer.StiHtmlState(baseTagsState, 0);

                    stateList = StiTextRenderer.ParseHtmlToStates(text, baseState, true).ToArray();
                    #endregion

                    #region Make fonts list
                    fontList = new List<StiTextRenderer.StiFontState>();
                    for (var indexState = 0; indexState < stateList.Length; indexState++)
                    {
                        var ffontSize = stateList[indexState].TS.FontSize;
                        //if (StiDpiHelper.GraphicsScale != null)
                        //{
                        //    ffontSize = (float)(ffontSize * StiDpiHelper.GraphicsScale);
                        //}

                        stateList[indexState].FontIndex = StiTextRenderer.GetFontIndex(
                            stateList[indexState].TS.FontName,
                            ffontSize,
                            stateList[indexState].TS.Bold,
                            stateList[indexState].TS.Italic,
                            stateList[indexState].TS.Underline,
                            stateList[indexState].TS.Strikeout,
                            stateList[indexState].TS.Superscript || stateList[indexState].TS.Subscript,
                            fontList);
                    }

                    for (var indexFont = 0; indexFont < fontList.Count; indexFont++)
                    {
                        StiFontState2 fontState = StiFontState2.CreateFrom(fontList[indexFont]);
                        fontState.EmValue = fontState.FontBase.Size;

                        var otm = GetOutlineTextMetricsCached(fontState.FontName, fontState.FontBase.Style, otmGraphics);

                        var lineHeightTextMetric = (otm.otmAscent + otm.otmDescent + otm.otmExternalLeading) / StiTextRenderer.MaxFontSize * fontState.EmValue;
                        var ascend = otm.otmAscent / StiTextRenderer.MaxFontSize * fontState.EmValue;
                        var descend = otm.otmDescent / StiTextRenderer.MaxFontSize * fontState.EmValue;
                        var typoOffset = 0d;
                        //if ((otm.otmfsSelection & 0x80) > 0)    //USE_TYPO_METRICS
                        //{
                        //    typoOffset = ascend;
                        //    lineHeightTextMetric = (otm.otmAscent - otm.otmDescent + otm.otmLineGap) / MaxFontSize * fontState.EmValue;
                        //    ascend = otm.otmAscent / MaxFontSize * fontState.EmValue * scale;
                        //    descend = -otm.otmDescent / MaxFontSize * fontState.EmValue * scale;
                        //    typoOffset = ascend - typoOffset;
                        //}
                        var lineHeight = lineHeightTextMetric;
                        //var lineHeightWithSpacing = lineHeight * lineSpacing;
                        fontState.LineHeight = lineHeight;
                        fontState.Ascend = ascend;
                        fontState.Descend = descend;
                        fontState.TypoOffset = typoOffset;

                        fontState.Otm = otm;

                        fontList[indexFont] = fontState;
                    }
                    #endregion

                    #region Process states
                    outputRunes = new List<StiRune>();
                    currentLineRunes = new List<StiRune>();

                    bool isNewLine = true;
                    indentCalcSize = 0;

                    //check if text was breaked on list item, and calculate indent
                    if ((stateList.Length > 0) && (stateList[0].TS.Indent > 0) && (stateList[0].TS.Tag.Tag == StiTextRenderer.StiHtmlTag.None))
                    {
                        var newState = new StiTextRenderer.StiHtmlState(stateList[0]);
                        newState.Text = new StringBuilder(new string('\xA0', newState.TS.Indent * 10));
                        var indentRune = GetRune(newState);
                        positionX = indentRune.Width;
                        indentCalcSize = positionX;
                        isNewLine = false;
                    }

                    for (int index = 0; index < stateList.Length; index++)
                    {
                        var state = stateList[index];
                        if (state.Text.Length == 0) continue;

                        if (needBreakText && (positionY > breakHeight)) break;

                        if (state.Text.Length == 1 && state.Text[0] == '\n')
                        {
                            CompleteCurrentLine(index, true);
                            isNewLine = true;
                            indentCalcSize = 0;
                            continue;
                        }

                        var rune = GetRune(state);

                        //set indent if it's indent string
                        if (isNewLine && (state.TS.Indent > 0) && StiOptions.Engine.Html.AllowListItemSecondLineIndent)
                        {
                            indentCalcSize = rune.Width;
                        }
                        isNewLine = false;

                        while ((positionX + rune.Width > bounds.Width) && textbox.WordWrap)
                        {
                            var newRune = BreakRune(rune, bounds.Width - positionX);
                            if (newRune != null)
                            {
                                AddRuneToCurrentLine(rune);
                                rune = newRune;
                                CompleteCurrentLine(index);
                            }
                            else
                            {
                                CompleteCurrentLine(index > 0 ? index - 1 : 0);
                            }
                            if (needBreakText && (positionY > breakHeight)) break;
                            positionX = indentCalcSize;
                        }
                        AddRuneToCurrentLine(rune);
                    }
                    if (currentLineRunes.Count > 0)
                    {
                        CompleteCurrentLine(stateList.Length - 1, true, forceWidthAlign);
                    }
                    #endregion

                    if (needBreakText)
                    {
                        #region Break text
                        if (positionY < breakHeight)
                        {
                            //everything fits
                            breakParts.Add(text);
                            breakParts.Add(string.Empty);
                        }
                        else if(lastLineBeginStateIndex == 0 && lastLineBeginRuneIndex < 0)
                        {
                            //nothing fits, all to next part
                            breakParts.Add(string.Empty);
                            breakParts.Add(text);
                        }
                        else
                        {
                            var lastRune = outputRunes[lastLineBeginRuneIndex];
                            int statePos = lastLineBeginStateIndex;
                            var state = stateList[statePos];
                            int posBreakText = lastRune.IndexEnd;
                            bool forceBreakWidthAlign = false;

                            string writtenText = null;
                            string breakText = null;

                            if (lastRune.IndexEnd != lastRune.Text.Length)
                            {
                                //not entirely rune
                                string writtenTextEnd = lastRune.Text.Substring(0, lastRune.IndexEnd);
                                if (lastLineBeginRuneIndex + 1 < outputRunes.Count)
                                {
                                    posBreakText = outputRunes[lastLineBeginRuneIndex + 1].IndexBegin;
                                }
                                if (state.TS.TextAlign == StiTextHorAlignment.Width) forceBreakWidthAlign = true;

                                if ((statePos > 0) && (stateList[statePos - 1].TS.Tag.Tag == StiTextRenderer.StiHtmlTag.ListItem) && (state.TS.Tag.Tag == StiTextRenderer.StiHtmlTag.ListItem))
                                {
                                    if ((stateList[statePos - 1].ListLevels == null) || (state.ListLevels == null) || (stateList[statePos - 1].ListLevels.Count == state.ListLevels.Count))
                                    {
                                        writtenTextEnd = "<li>" + writtenTextEnd;
                                    }
                                    else
                                    {
                                        writtenTextEnd = (state.ListLevels[state.ListLevels.Count - 1] > 0 ? "<ol>" : "<ul>") + writtenTextEnd;
                                    }
                                }
                                else
                                {
                                    writtenTextEnd = StiTextRenderer.StateToHtml(state, state, writtenTextEnd, 0);  //lineInfo.Indent=0
                                }

                                writtenText = text.Substring(0, state.PosBegin) + writtenTextEnd + (forceBreakWidthAlign ? StiTextRenderer.StiForceWidthAlignTag : "");
                            }
                            else
                            { 
                                var nextState = (statePos + 1 < stateList.Length) ? stateList[statePos + 1] : state;
                                writtenText = text.Substring(0, nextState.PosBegin) + (forceBreakWidthAlign ? StiTextRenderer.StiForceWidthAlignTag : "");
                            }

                            var stateIndex = statePos + 1;
                            breakText = StiTextRenderer.StateToHtml(
                                state, 
                                (state.TS.Tag.Tag == StiTextRenderer.StiHtmlTag.ListItem || state.TS.Tag.Tag == StiTextRenderer.StiHtmlTag.P) && (stateIndex < stateList.Length) ? stateList[stateIndex] : state, 
                                lastRune.Text.Substring(posBreakText),
                                0); //lineInfo.Indent=0
                            //if (state.TS.Tag.Tag == StiTextRenderer.StiHtmlTag.ListItem && stateIndex < stateList.Length)
                            //{
                            //    breakText += stateList[stateIndex].Text;
                            //    stateIndex++;
                            //}
                            if (stateIndex < stateList.Length)
                            {
                                breakText += text.Substring(stateList[stateIndex].PosBegin);
                            }

                            breakParts.Add(writtenText);
                            breakParts.Add(breakText);
                        }
                        #endregion
                    }
                    else
                    {
                        #region Make output arrays
                        double vertOffset = 0;
                        if (textbox.VertAlignment == StiVertAlignment.Center)
                        {
                            vertOffset = (bounds.Height - positionY) / 2;
                        }
                        if (textbox.VertAlignment == StiVertAlignment.Bottom)
                        {
                            vertOffset = bounds.Height - positionY;
                        }

                        foreach (var rune in outputRunes)
                        {
                            if (rune.Text.Length == 0) continue;

                            var runInfo = new StiTextRenderer.RunInfo();
                            runInfo.Text = rune.Text.Substring(rune.IndexBegin, rune.IndexEnd - rune.IndexBegin);
                            runInfo.XPos = rune.X;
                            runInfo.YPos = rune.Y + vertOffset;
                            runInfo.TextColor = rune.State.FontColor;
                            runInfo.BackColor = rune.State.BackColor;
                            runInfo.FontIndex = rune.FontIndex;
                            runInfo.Href = rune.State.Href;

                            runInfo.Widths = new int[runInfo.Text.Length];
                            runInfo.GlyphWidths = new int[runInfo.Text.Length];
                            runInfo.ScaleList = new double[runInfo.Text.Length];
                            runInfo.GlyphIndexList = new int[runInfo.Text.Length];
                            int sumI = 0;
                            double sumD = 0;
                            for (int index = 0; index < runInfo.Widths.Length; index++)
                            {
                                sumD += rune.Widths[rune.IndexBegin + index];
                                int width = (int)Math.Round(sumD - sumI);
                                runInfo.Widths[index] = width;
                                runInfo.GlyphWidths[index] = width;
                                runInfo.ScaleList[index] = rune.Scales[rune.IndexBegin + index];
                                runInfo.GlyphIndexList[index] = -1;
                                sumI += width;
                            }

                            minX = Math.Min(minX, rune.X);
                            maxX = Math.Max(maxX, rune.X + sumD);

                            outRunsList.Add(runInfo);
                        }

                        outFontsList.AddRange(fontList);
                        #endregion
                    }
                }
                catch { }
                finally
                {
                    size.Width = maxX - minX;
                    size.Height = positionY;
                }

                return size;
            }

            private void AddRuneToCurrentLine(StiRune rune)
            {
                rune.X = positionX;
                rune.Y = positionY;
                currentLineRunes.Add(rune);
                positionX += rune.Width;
            }

            #region CompleteCurrentLine
            private void CompleteCurrentLine(int stateIndex, bool endLine = false, bool forceWidthAlign = false)
            {
                var state = stateList[stateIndex];

                //определяем максимальный размер шрифта
                int fontIndexMax = state.FontIndex;
                double sumWidth = 0;
                int spacesCount = 0;
                foreach (var rune in currentLineRunes)
                {
                    if (fontList[fontIndexMax].EmValue < fontList[rune.FontIndex].EmValue)
                    {
                        fontIndexMax = rune.FontIndex;
                    }
                    else if (fontList[fontIndexMax].LineHeight < fontList[rune.FontIndex].LineHeight)
                    {
                        fontIndexMax = rune.FontIndex;
                    }
                    sumWidth += rune.Width;
                    for(int index = rune.IndexBegin; index < rune.IndexEnd; index++)
                    {
                        char ch = rune.Text[index];
                        if (char.IsWhiteSpace(ch) && (ch != 0xA0)) spacesCount++;
                    }
                }
                double maxLineHeight = fontList[fontIndexMax].LineHeight;

                //параметры для выравнивания по ширине
                var textAlign = state.TS.TextAlign;
                if (currentLineRunes.Count > 0) textAlign = currentLineRunes[currentLineRunes.Count - 1].State.TextAlign;

                double lineJustifyOffset = 0;
                bool needWidthAlign = forceWidthAlign;
                if (textAlign == StiTextHorAlignment.Width)
                {
                    if (!endLine) needWidthAlign = true;
                }
                if (needWidthAlign && spacesCount > 0) lineJustifyOffset = (bounds.Width - sumWidth) / spacesCount;

                //определяем смещение для выравнивания по горизонтали
                double offsetLeft = 0;
                if (textAlign == StiTextHorAlignment.Center)
                {
                    offsetLeft = (bounds.Width - sumWidth) / 2;
                }
                if (!isRightToLeft && textAlign == StiTextHorAlignment.Right ||
                    isRightToLeft && textAlign == StiTextHorAlignment.Left)
                {
                    offsetLeft = bounds.Width - sumWidth;
                }

                //выровнять текущие руны по базовой линии и применить выравнивание по горизонтали 
                double baseLine = maxLineHeight - fontList[fontIndexMax].Descend;
                double nextRuneAdditionalOffset = 0;
                foreach (var rune in currentLineRunes)
                {
                    rune.X += offsetLeft + correctX + nextRuneAdditionalOffset;
                    if (rune.State.Superscript || rune.State.Subscript)
                    {
                        if (rune.State.Superscript)
                        {
                            rune.Y += maxLineHeight - fontList[fontList[rune.FontIndex].ParentFontIndex].LineHeight;
                        }
                        else
                        {
                            rune.Y += baseLine + fontList[fontList[rune.FontIndex].ParentFontIndex].Descend - rune.Height;
                        }
                    }
                    else
                    {
                        rune.Y += baseLine + fontList[rune.FontIndex].Descend - rune.Height;
                    }
                    if (needWidthAlign)
                    {
                        for (int index = rune.IndexBegin; index < rune.IndexEnd; index++)
                        {
                            char ch = rune.Text[index];
                            if (char.IsWhiteSpace(ch) && (ch != 0xA0))
                            {
                                double newWidth = rune.Widths[index] + lineJustifyOffset;
                                rune.Scales[index] = newWidth / rune.Widths[index];
                                rune.Widths[index] = newWidth;
                                nextRuneAdditionalOffset += lineJustifyOffset;
                            }
                        }
                    }
                }

                outputRunes.AddRange(currentLineRunes);

                positionX = 0;
                positionY += maxLineHeight * state.TS.LineHeight;

                currentLineRunes.Clear();

                if (needBreakText)
                {
                    if (positionY > breakHeight) return;

                    lastLineBeginStateIndex = stateIndex;
                    lastLineBeginRuneIndex = outputRunes.Count - 1;
                }
            }
            #endregion

            #region GetRune
            private StiRune GetRune(StiTextRenderer.StiHtmlState state)
            {
                string text = StiTextRenderer.PrepareStateText(state.Text).ToString();

                var rune = new StiRune();
                rune.Text = text;
                rune.Widths = new double[text.Length];
                rune.SumWidths = new double[text.Length];
                rune.Scales = new double[text.Length];
                rune.IndexBegin = 0;
                rune.IndexEnd = text.Length;
                rune.State = state.TS;
                rune.FontIndex = state.FontIndex;

                var font = fontList[state.FontIndex] as StiFontState2;
                var addedLetterSpacing = font.EmValue * state.TS.LetterSpacing;
                var addedWordSpacing = font.EmValue * state.TS.WordSpacing;

                double sum = 0;
                for (int index = 0; index < text.Length; index++)
                {
                    char ch = text[index];
                    double baseWidth = font.Otm.GetWidth(ch) / StiTextRenderer.MaxFontSize * font.EmValue;
                    double width = baseWidth + addedLetterSpacing;
                    if (char.IsWhiteSpace(ch) && (ch != 0xA0)) width += addedWordSpacing;
                    sum += width;
                    rune.Widths[index] = width;
                    rune.SumWidths[index] = sum;
                    rune.Scales[index] = (baseWidth == 0) ? 1 : width / baseWidth;
                }

                //rune.Width = sum;
                rune.Height = fontList[state.FontIndex].LineHeight;

                return rune;
            }
            #endregion

            #region BreakRune
            private StiRune BreakRune(StiRune baseRune, double size)
            {
                //find last fit symbol
                int pos = baseRune.IndexBegin;
                double sumStart = pos > 0 ? baseRune.SumWidths[pos - 1] : 0;
                while ((baseRune.SumWidths[pos] - sumStart) < size) pos++;

                //find wrap point
                int pos2 = pos;
                while (!IsWrapPoint(baseRune.Text, pos2) && (pos2 > baseRune.IndexBegin)) pos2--;

                StiRune newRune = null;

                if (pos2 == baseRune.IndexBegin)
                {
                    //no break points in range
                    if (currentLineRunes.Count > 0)
                    {
                        //no break, full to next line
                        newRune = null;
                    }
                    else
                    {
                        if (pos == baseRune.IndexBegin)
                        {
                            //at least one symbol on line, to avoid looping
                            pos++;
                        }
                        //break direct in position pos
                        newRune = baseRune.Clone();
                        newRune.IndexBegin = pos;
                        baseRune.IndexEnd = pos;
                    }
                }
                else
                {
                    //check for trim
                    while ((pos2 > baseRune.IndexBegin) && char.IsWhiteSpace(baseRune.Text, pos2 - 1)) pos2--;
                    int pos3 = pos2;
                    while ((pos3 < baseRune.IndexEnd) && char.IsWhiteSpace(baseRune.Text, pos3)) pos3++;

                    if (pos2 == 0)
                    {
                        //there were only spaces at the beginning
                        baseRune.IndexBegin = pos3;
                    }
                    else
                    {
                        //break in position pos2..pos3
                        newRune = baseRune.Clone();
                        newRune.IndexBegin = pos3;
                        baseRune.IndexEnd = pos2;
                    }
                }

                return newRune;
            }

            private bool IsWrapPoint(string st, int pos)
            {
                var sym2 = st[pos];
                if (sym2 == ' ') return true;
                if ((char.GetUnicodeCategory(sym2) == global::System.Globalization.UnicodeCategory.SpaceSeparator) && (sym2 != 0xA0)) return true;
                if (pos > 0)
                {
                    var sym1 = st[pos - 1];
                    if (sym1 == ' ') return true;
                    if (isCJKSymbol(sym1) && isCJKSymbol(sym2)) return true;
                    if (sym1 == '!' || sym1 == '%' || sym1 == ')' || sym1 == '}' || sym1 == '-' || sym1 == '?' ||
                        sym1 == '）' || sym1 == '：' || sym1 == '、' || sym1 == '，' || sym1 == '。') return true; //CJK punctuation
                }
                return false;
            }
            private static bool isCJKSymbol(int sym)
            {
                return (sym >= 0x4e00 && sym <= 0x9fcc) || (sym >= 0x3400 && sym <= 0x4db5); // || (sym >= 0x20000 && sym <= 0x2a6df); //char is only 16bit
            }
            #endregion
        }
        #endregion

        #region DrawString
        public static void DrawString(Graphics g, RectangleD rect, string text, StiText textBox, double scale = 1)
        {
            var parser = new StiHtmlParser();
            parser.Parse(textBox, text, out List<StiTextRenderer.RunInfo> outRunsList, out List<StiTextRenderer.StiFontState> outFontsList);

            var zoom = textBox.Page.Zoom * scale;
            var scaledZoom = zoom * StiScale.Factor;

            if (StiScale.IsNoScaling && !textBox.IsPrinting && StiDpiHelper.NeedDeviceCapsScale)
                zoom *= StiDpiHelper.DeviceCapsScale;

            var state = g.Save();

            try
            {
                g.SetClip(rect.ToRectangleF());

                using (var sf = new StringFormat(StringFormat.GenericTypographic))
                {
                    sf.FormatFlags |= StringFormatFlags.NoWrap;

                    foreach (var run in outRunsList)
                    {
                        var totalWidth = run.Widths.Sum();

                        var fontState = outFontsList[run.FontIndex];
                        if ((zoom != 1) && (fontState.FontScaled == null))
                            fontState.FontScaled = new Font(fontState.FontBase.FontFamily, (float)(fontState.EmValue * zoom), fontState.FontBase.Style);

                        var usedFont = (zoom == 1) ? fontState.FontBase : fontState.FontScaled;

                        var runRect = new Rectangle(
                            (int)(rect.X + run.XPos * scaledZoom),
                            (int)(rect.Y + run.YPos * scaledZoom),
                            (int)(totalWidth * scaledZoom),
                            (int)(fontState.LineHeight * scaledZoom));

                        using (var fillBrush = new SolidBrush(run.BackColor))
                        using (var textBrush = new SolidBrush(run.TextColor))
                        {
                            g.FillRectangle(fillBrush, runRect);

                            bool hasScale = false;
                            foreach (var d in run.ScaleList)
                            {
                                if (d < 0.99 || d > 1.01)
                                {
                                    hasScale = true;
                                    break;
                                }
                            }

                            if (!hasScale)
                            {
                                g.DrawString(run.Text, usedFont, textBrush, new PointF((float)(rect.X + run.XPos * scaledZoom), (float)(rect.Y + run.YPos * scaledZoom)), sf);
                            }
                            else
                            {
                                double posX = rect.X + run.XPos * scaledZoom;
                                double posY = rect.Y + run.YPos * scaledZoom;

                                for (int index = 0; index < run.Text.Length; index++)
                                {
                                    g.DrawString(run.Text[index].ToString(), usedFont, textBrush, new PointF((float)posX, (float)posY), sf);
                                    posX += run.Widths[index] * scaledZoom;
                                }
                            }
                        }

                        //Red line
                        //g.DrawRectangle(Pens.Red, runRect);
                    }
                }
            }
            finally
            {
                g.Restore(state);
            }
        }
        #endregion

        #region MeasureString
        public static SizeD MeasureString(StiText textBox)
        {
            StiHtmlParser ps = new StiHtmlParser();
            SizeD actualSize = ps.Parse(textBox, textBox.Text, out List<StiTextRenderer.RunInfo> outRunsList, out List<StiTextRenderer.StiFontState> outFontsList);

            //correction of the width to prevent rounding error
            if (textBox.Report != null && textBox.Report.ReportUnit != StiReportUnitType.HundredthsOfInch)
            {
                var unit = textBox.Report.Unit;
                var fullWidth = actualSize.Width + textBox.Margins.Left + textBox.Margins.Right + textBox.Border.Size;

                if (unit.ConvertToHInches(Math.Round(unit.ConvertFromHInches(fullWidth), 2)) < fullWidth)
                    actualSize.Width += unit.ConvertToHInches(0.01);
            }

            if (textBox.Angle == 90 || textBox.Angle == 180)
                actualSize.Width *= StiOptions.Engine.TextDrawingMeasurement.MeasurementFactorTypographic;

            else
                actualSize.Height *= StiOptions.Engine.TextDrawingMeasurement.MeasurementFactorTypographic;

            return actualSize;
        }
        #endregion

        #region BreakText
        public static string BreakText(RectangleD rect, ref string text, StiText textBox)
        {
            List<string> parts = new List<string>();

            StiHtmlParser ps = new StiHtmlParser();
            ps.Parse(textBox, textBox.Text, out List<StiTextRenderer.RunInfo> outRunsList, out List<StiTextRenderer.StiFontState> outFontsList, rect.Height, parts);

            text = parts[1];
            return parts[0];
        }
        #endregion

        #region DrawTextForOutput
        public static void DrawTextForOutput(StiText textBox, out List<StiTextRenderer.RunInfo> outRunsList, out List<StiTextRenderer.StiFontState> outFontsList)
        {
            StiHtmlParser ps = new StiHtmlParser();
            ps.Parse(textBox, textBox.Text, out outRunsList, out outFontsList);
        }
        #endregion

    }
}
