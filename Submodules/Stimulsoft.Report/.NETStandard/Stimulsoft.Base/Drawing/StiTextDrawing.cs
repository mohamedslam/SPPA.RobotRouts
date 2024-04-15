#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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

using System.Collections.Generic;
using System.Collections;
using System.Text;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Brush = Stimulsoft.Drawing.Brush;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using Font = Stimulsoft.Drawing.Font;
using StringFormat = Stimulsoft.Drawing.StringFormat;
using Region = Stimulsoft.Drawing.Region;
using Matrix = Stimulsoft.Drawing.Drawing2D.Matrix;
using GraphicsPath = Stimulsoft.Drawing.Drawing2D.GraphicsPath;
using CharacterRange = Stimulsoft.Drawing.CharacterRange;
#endif

namespace Stimulsoft.Base.Drawing
{
    /// <summary>
    /// Class contains methods for text drawing.
    /// </summary>
    public class StiTextDrawing
    {
        #region struct Range
        /// <summary>
        /// Structure describes the range.
        /// Range describes the word or chain symbol in text.
        /// </summary>
        private struct Range
        {
            /// <summary>
            /// Contents of range.
            /// </summary>
            public string Text;

            /// <summary>
            /// Position of range.
            /// </summary>
            public PointD Pos;

            /// <summary>
            /// Size of range.
            /// </summary>
            public SizeD Size;

            /// <summary>
            /// Is start range.
            /// </summary>
            public bool IsStart;

            /// <summary>
            /// Is end range.
            /// </summary>
            public bool IsEnd;

            /// <summary>
            /// After this range, new line is starting.
            /// </summary>
            public bool NewLineForm;

            /// <summary>
            /// Initializes a new instance of the Range class with the specified location and size.
            /// </summary>
            public Range(string text, SizeD size, bool newLineForm)
            {
                Pos = new PointD(0, 0);
                Text = text;
                Size = size;
                IsStart = false;
                IsEnd = false;
                NewLineForm = newLineForm;
            }
        }
        #endregion

        #region struct Word
        private struct Word
        {
            public string Text;

            public bool IsEnter;

            /// <summary>
            /// Returns the collection of the words.
            /// </summary>
            public static Word[] GetTexts(string str)
            {
                var al = new List<Word>();

                var pos = 0;
                var start = true;
                StringBuilder s = new StringBuilder();
                while (pos < str.Length)
                {
                    if (str[pos] == '\n')
                    {
                        al.Add(new Word(s.ToString(), true));
                        s.Clear();
                        start = true;
                    }
                    else if (!start && str[pos] == ' ')
                    {
                        al.Add(new Word(s.ToString(), false));
                        s.Clear();
                        start = true;
                    }
                    else
                    {
                        s.Append(str[pos]);
                        start = false;
                    }
                    pos++;
                }

                if (s.Length > 0)
                    al.Add(new Word(s.ToString(), false));

                return al.ToArray();
            }

            private Word(string text, bool isEnter)
            {
                Text = text;
                IsEnter = isEnter;
            }
        }
        #endregion

        #region struct Line
        /// <summary>
        /// Structure describes the line.
        /// </summary>
        private struct Line
        {
            public int Start;
            public int Count;
            public bool IsEnter;

            public Line(int start, int count, bool isEnter)
            {
                Start = start;
                Count = count;
                IsEnter = isEnter;
            }
        }
        #endregion

        #region Methods.DrawStringWidth
        /// <summary>
        /// Corrects the Range size according to a described rectangle.
        /// </summary>
        private static void CorrectSize(ref Range range, RectangleD rect)
        {
            if (range.Pos.X + range.Size.Width > rect.Right)
                range.Size.Width = rect.Right - range.Pos.X;

            if (range.Pos.Y + range.Size.Height > rect.Bottom)
                range.Size.Height = rect.Bottom - range.Pos.Y;
        }

        /// <summary>
        /// Returns ranges array.
        /// </summary>
        /// <param name="g">Graphics to measure sizes.</param>
        /// <param name="rect">Describes a rectangle.</param>
        /// <param name="text">Text for burst into arrays.</param>
        /// <param name="font">Font of the text.</param>
        /// <param name="sf">Text format.</param>
        /// <returns>Ranges.</returns>
        private static Range[] GetRange(Graphics g, RectangleD rect, string text, Font font, StringFormat sf, double lineHeight = 1)
        {
            var forceWidthAlign = text.EndsWith(StiTextRenderer.StiForceWidthAlignTag, StringComparison.InvariantCulture);
            if (forceWidthAlign)
            {
                text = text.Substring(0, text.Length - StiTextRenderer.StiForceWidthAlignTag.Length);
            }
            text = text.TrimEnd(new char[] { ' ' });

            var rn = new List<Range>();
            var txt = Word.GetTexts(text);
            var ln = new List<Line>();

            double heightFont = 0;
            using (var tempFont = StiFontUtils.ChangeFontSize(font, 1024))
            {
                heightFont = tempFont.Height * font.Size / tempFont.Size * lineHeight;
            }

            var spaceSize = GetAdditionalSpaceSize(g, font);

            //Form words
            for (var k = 0; k < txt.Length; k++)
            {
                var size = g.MeasureString(txt[k].Text, font);
                rn.Add(new Range(txt[k].Text, new SizeD(size.Width + spaceSize, size.Height), txt[k].IsEnter));
            }

            #region Divide into lines
            var posX = rect.Left;
            var posY = rect.Top;
            var line = 1;
            var wordCount = 0;
            var start = 0;
            var forceNewLine = false;

            var ranges = rn.ToArray();

            for (var k = 0; k < ranges.Length; k++)
            {
                posX += ranges[k].Size.Width;
                if (forceNewLine || (posX >= rect.Right && wordCount >= 1))
                {
                    posX = rect.Left + ranges[k].Size.Width;
                    line++;
                    posY += heightFont;
                    ln.Add(new Line(start, wordCount, forceNewLine));
                    start = k;
                    wordCount = 0;
                }
                wordCount++;

                ranges[k].Pos.Y = posY;
                forceNewLine = ranges[k].NewLineForm;
            }
            ln.Add(new Line(start, wordCount, !forceWidthAlign));
            #endregion

            var lines = ln.ToArray();

            #region Examine lines and sets horizontal coordinates
            for (var h = 0; h < lines.Length; h++)
            {
                var startPos = lines[h].Start;
                var endPos = lines[h].Start + lines[h].Count - 1;

                ranges[startPos].IsStart = true;
                ranges[endPos].IsEnd = true;

                #region Last line or line finishing Enter
                if (lines[h].IsEnter)
                {
                    posX = rect.Left;
                    for (var f = startPos; f <= endPos; f++)
                    {
                        ranges[f].Pos.X = posX;
                        posX += ranges[f].Size.Width;
                    }
                }
                #endregion
                else
                {
                    #region One word
                    if (lines[h].Count == 1) ranges[startPos].Pos.X = rect.Left;
                    #endregion

                    #region Much words
                    else
                    {
                        ranges[startPos].Pos.X = rect.Left;
                        ranges[endPos].Pos.X = rect.Right - ranges[endPos].Size.Width;

                        double space = 0;
                        if (lines[h].Count > 2)
                        {
                            double wx = 0;
                            for (var a = startPos + 1; a < endPos; a++)
                                wx += ranges[a].Size.Width;

                            space = ((rect.Width -
                                ranges[startPos].Size.Width -
                                ranges[endPos].Size.Width -
                                wx) / (lines[h].Count - 1));
                        }

                        posX = ranges[startPos].Size.Width + rect.Left + space;
                        for (var f = startPos + 1; f < endPos; f++)
                        {
                            ranges[f].Pos.X = posX;
                            posX += space + ranges[f].Size.Width;
                        }
                    }
                    #endregion
                }
            }
            #endregion

            #region Aligning the text
            if (sf.LineAlignment != StringAlignment.Near)
            {
                var allHeight = heightFont * (lines.Length + 0.05);  //additional is correction of the line height calculation
                double dist = 0;

                if (sf.LineAlignment == StringAlignment.Far)
                    dist = rect.Height - allHeight;

                if (sf.LineAlignment == StringAlignment.Center)
                    dist = (rect.Height - allHeight) / 2;

                for (var k = 0; k < ranges.Length; k++)
                    ranges[k].Pos.Y += dist;
            }
            #endregion

            #region Correct of the text (LineLimit & Trimming)
            for (var k = 0; k < ranges.Length; k++) CorrectSize(ref ranges[k], rect);
            #endregion

            return ranges;
        }

        /// <summary>
        /// Draws the text aligned to width.
        /// </summary>
        /// <param name="g">Graphics to draw on.</param>
        /// <param name="text">Text to draw on.</param>
        /// <param name="font">Font to draw on.</param>
        /// <param name="brush">Brush to draw on.</param>
        /// <param name="rect">Rectangle to draw on.</param>
        /// <param name="stringFormat">Text format.</param>
        public static void DrawStringWidth(Graphics g, string text, Font font, Brush brush,
            RectangleD rect, StringFormat stringFormat, double lineHeight = 1)
        {
            var svClip = g.Clip;
            g.SetClip(rect.ToRectangleF(), CombineMode.Intersect);
            if (!string.IsNullOrEmpty(text))
            {
                if ((stringFormat.FormatFlags & StringFormatFlags.NoWrap) > 0)
                    stringFormat.FormatFlags ^= StringFormatFlags.NoWrap;

                var ranges = GetRange(g, rect, text, font, stringFormat, lineHeight);

                stringFormat.LineAlignment = StringAlignment.Near;
                stringFormat.FormatFlags |= StringFormatFlags.LineLimit;

                for (var k = 0; k < ranges.Length; k++)
                {
                    if (ranges[k].IsStart)
                        stringFormat.Alignment = StringAlignment.Near;

                    else if (ranges[k].IsEnd)
                        stringFormat.Alignment = StringAlignment.Far;

                    else
                        stringFormat.Alignment = StringAlignment.Center;

                    g.DrawString(ranges[k].Text, font, brush,
                        new RectangleD(ranges[k].Pos, ranges[k].Size).ToRectangleF(), stringFormat);
                }
            }
            g.SetClip(svClip, CombineMode.Replace);
        }

        /// <summary>
        /// Draws aligned to width text on the angle.
        /// </summary>
        /// <param name="g">Graphics to draw on.</param>
        /// <param name="text">Text to draw on.</param>
        /// <param name="font">Font to draw on.</param>
        /// <param name="brush">Brush to draw.</param>
        /// <param name="rect">Rectangle to draw.</param>
        /// <param name="stringFormat">Text format.</param>
        /// <param name="angle">Show text at an angle.</param>
        public static void DrawStringWidth(Graphics g, string text, Font font, Brush brush,
            RectangleD rect, StringFormat stringFormat, float angle, double lineHeight = 1)
        {
            angle %= 360;
            if (angle < 0) angle = 360 + angle;

            if (angle != 0)
            {
                var svClip = g.Clip;
                g.SetClip(rect.ToRectangleF(), CombineMode.Intersect);
                var gs = g.Save();
                g.TranslateTransform((float)(rect.Left + rect.Width / 2), (float)(rect.Top + rect.Height / 2));
                g.RotateTransform(-angle);
                rect.X = -rect.Width / 2;
                rect.Y = -rect.Height / 2;
                var drawRect = new RectangleD(rect.X, rect.Y, rect.Width, rect.Height);

                if (angle > 45 && angle < 135 || angle > 225 && angle < 315)
                    drawRect = new RectangleD(rect.Y, rect.X, rect.Height, rect.Width);

                DrawStringWidth(g, text, font, brush, drawRect, stringFormat, lineHeight);
                g.Restore(gs);
                g.SetClip(svClip, CombineMode.Replace);
            }
            else
                DrawStringWidth(g, text, font, brush, rect, stringFormat, lineHeight);
        }
        #endregion

        #region Methods.DrawString
        public static void DrawString(Graphics g, string text, Font font, Brush brush,
            RectangleF rect, StringFormat stringFormat, float angle, double lineHeight = 1)
        {
            DrawString(g, text, font, brush, RectangleD.CreateFromRectangle(rect), stringFormat, angle);
        }

        /// <summary>
        /// Draws text at an angle.
        /// </summary>
        /// <param name="g">Graphics to draw on.</param>
        /// <param name="text">Text to draw on.</param>
        /// <param name="font">Font to draw on.</param>
        /// <param name="brush">Brush to draw.</param>
        /// <param name="rect">Rectangle to draw.</param>
        /// <param name="stringFormat">Text format.</param>
        /// <param name="angle">Show text at an angle.</param>
        public static void DrawString(Graphics g, string text, Font font, Brush brush,
            RectangleD rect, StringFormat stringFormat, float angle, double lineHeight = 1)
        {
            if (string.IsNullOrWhiteSpace(text)) return;

            angle %= 360;
            if (angle < 0) angle = 360 + angle;

            try
            {
                if (angle != 0)
                {
                    //after RotateTransform the CharacterRange.GetBounds() method work very slow, so do this before
                    List<StiTextRenderer.LineInfo> listLines = null;
                    if (lineHeight != 1)
                    {
                        listLines = SplitTextWordwrap(text, g, font,
                            (angle > 45 && angle < 135) || (angle > 225 && angle < 315) ? new RectangleD(0, 0, rect.Height, rect.Width) : new RectangleD(0, 0, rect.Width, rect.Height),
                            stringFormat);
                    }

                    var svClip = g.Clip;
                    g.SetClip(rect.ToRectangleF(), CombineMode.Intersect);
                    var gs = g.Save();
                    g.TranslateTransform((float)(rect.Left + rect.Width / 2), (float)(rect.Top + rect.Height / 2));
                    g.RotateTransform(-(float)angle);
                    rect.X = -rect.Width / 2;
                    rect.Y = -rect.Height / 2;

                    var drawRect = new RectangleD(rect.X, rect.Y, rect.Width, rect.Height);

                    if ((angle > 45 && angle < 135) || (angle > 225 && angle < 315))
                        drawRect = new RectangleD(rect.Y, rect.X, rect.Height, rect.Width);

                    if (angle == 0 || angle == 90 || angle == 180 || angle == 270)
                    {
                        if (lineHeight == 1)
                            g.DrawString(text, font, brush, drawRect.ToRectangleF(), stringFormat);
                        else
                            DrawStringLineHeight(g, text, font, brush, drawRect, stringFormat, lineHeight, listLines);
                    }
                    else
                    {
                        stringFormat.SetTabStops(20f, new[] { 30f, 30f, 30f });
                        stringFormat.Alignment = StringAlignment.Center;
                        stringFormat.LineAlignment = StringAlignment.Center;

                        if (lineHeight == 1)
                            g.DrawString(text, font, brush, (float)(drawRect.X + drawRect.Width / 2), (float)(drawRect.Y + drawRect.Height / 2), stringFormat);
                        else
                            DrawStringLineHeight(g, text, font, brush, drawRect, stringFormat, lineHeight, listLines);

                    }
                    g.Restore(gs);
                    g.SetClip(svClip, CombineMode.Replace);
                }
                else
                {
                    if (lineHeight == 1)
                        g.DrawString(text, font, brush, rect.ToRectangleF(), stringFormat);
                    else
                    {
                        var svClip = g.Clip;
                        g.SetClip(rect.ToRectangleF(), CombineMode.Intersect);

                        DrawStringLineHeight(g, text, font, brush, rect, stringFormat, lineHeight);

                        g.SetClip(svClip, CombineMode.Replace);
                    }
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Draws text at an angle.
        /// </summary>
        /// <param name="g">Graphics to draw on.</param>
        /// <param name="text">Text to draw on.</param>
        /// <param name="font">Font to draw on.</param>
        /// <param name="color">Brush to draw.</param>
        /// <param name="rect">Rectangle to draw.</param>
        /// <param name="stringFormat">Text format.</param>
        public static void DrawString(Graphics g, string text, Font font, Color color,
            RectangleD rect, StringFormat stringFormat)
        {
            using (var brush = new SolidBrush(color))
            {
                DrawString(g, text, font, brush, rect, stringFormat, 0);
            }
        }

        /// <summary>
        /// Draws text at an angle.
        /// </summary>
        /// <param name="g">Graphics to draw on.</param>
        /// <param name="text">Text to draw on.</param>
        /// <param name="font">Font to draw on.</param>
        /// <param name="brush">Brush to draw.</param>
        /// <param name="rect">Rectangle to draw.</param>
        /// <param name="stringFormat">Text format.</param>
        public static void DrawString(Graphics g, string text, Font font, Brush brush,
            RectangleD rect, StringFormat stringFormat)
        {
            DrawString(g, text, font, brush, rect, stringFormat, 0);
        }

        /// <summary>
        /// Draws text.
        /// </summary>
        /// <param name="g">Graphics to draw on.</param>
        /// <param name="text">Text to draw on.</param>
        /// <param name="font">Font to draw on.</param>
        /// <param name="brush">Brush to draw.</param>
        /// <param name="rect">Rectangle to draw.</param>
        /// <param name="textOptions">Options to show text.</param>
        /// <param name="ha">Horizontal alignment.</param>
        /// <param name="va">Vertical alignment.</param>
        public static void DrawString(Graphics g, string text, Font font, Brush brush,
            RectangleD rect, StiTextOptions textOptions, StiTextHorAlignment ha, StiVertAlignment va)
        {
            DrawString(g, text, font, brush, rect, textOptions, ha, va, false, 1);
        }

        /// <summary>
        /// Draws text.
        /// </summary>
        /// <param name="g">Graphics to draw on.</param>
        /// <param name="text">Text to draw on.</param>
        /// <param name="font">Font to draw on.</param>
        /// <param name="brush">Brush to draw.</param>
        /// <param name="rect">Rectangle to draw.</param>
        /// <param name="textOptions">Options to show text.</param>
        /// <param name="ha">Horizontal alignment.</param>
        /// <param name="va">Vertical alignment.</param>
        public static void DrawString(Graphics g, string text, Font font, Brush brush,
            RectangleD rect, StiTextOptions textOptions,
            StiTextHorAlignment ha, StiVertAlignment va, bool antialiasing,
            float zoom, double lineHeight = 1)
        {
            using (var stringFormat = GetStringFormat(textOptions, ha, va, antialiasing, zoom))
            {
                if (ha == StiTextHorAlignment.Width)
                    DrawStringWidth(g, text, font, brush, rect, stringFormat, textOptions.Angle, lineHeight);

                else
                    DrawString(g, text, font, brush, rect, stringFormat, textOptions.Angle, lineHeight);
            }
        }
        #endregion

        #region Methods.DrawStringLineHeight
        private static void DrawStringLineHeight(Graphics g, string text, Font font, Brush brush, RectangleD rect, StringFormat sf, double lineHeight = 1, List<StiTextRenderer.LineInfo> list = null)
        {
            if (list == null)
            {
                list = SplitTextWordwrap(text, g, font, rect, sf);
            }

            float maxWidth = sf.FormatFlags.HasFlag(StringFormatFlags.NoWrap) ? 0 : (float)rect.Width;
            var size = g.MeasureString(text, font, new SizeF(maxWidth, 999999f), sf);

            bool needCut = false;
            double fullLineHeight = size.Height / list.Count * lineHeight;
            if (size.Height * lineHeight > rect.Height * 1.05)
            {
                fullLineHeight = list[0].LineHeight * lineHeight;
                needCut = true;
            }

            StringFormat sf2 = new StringFormat(sf);
            sf2.FormatFlags |= StringFormatFlags.NoWrap;

            double startPos = 0;
            if (!needCut)
            {
                if (sf2.LineAlignment == StringAlignment.Center) startPos = (rect.Height - size.Height * lineHeight) / 2;
                if (sf2.LineAlignment == StringAlignment.Far) startPos = rect.Height - size.Height * lineHeight;
            }

            var rectf = rect.ToRectangleF();
            rectf.Height = (float)fullLineHeight;
            rectf.Y += (float)startPos;

            foreach (var line in list)
            {
                g.DrawString(line.Text, font, brush, rectf, sf2);
                rectf.Y += (float)fullLineHeight;
                if (needCut && rectf.Y > rect.Bottom) break;
            }
        }

        public static List<StiTextRenderer.LineInfo> SplitTextWordwrap(string text, Graphics g, Font font, RectangleD rect, StiTextOptions textOptions, StiTextHorAlignment ha, bool typographic)
        {
            if (textOptions == null) textOptions = new StiTextOptions();
            float zoom = 10f;
            StringFormat sf = GetStringFormat(textOptions, StiTextHorAlignment.Left, StiVertAlignment.Top, typographic, zoom);
            return SplitTextWordwrap(text, g, font, rect, sf, ha == StiTextHorAlignment.Width);
        }

        public static List<StiTextRenderer.LineInfo> SplitTextWordwrap(string text, Graphics g, Font font, RectangleD rect, StringFormat sf, bool horAlignWidth = false)
        {
            var arrLinesInfo = new List<StiTextRenderer.LineInfo>();

            float zoom = 10f;
            double rectWidth = (sf.FormatFlags.HasFlag(StringFormatFlags.NoWrap)) ? 999999f : rect.Width;
            double rectHeight = rect.Height * zoom;
            RectangleF rectF2 = new RectangleF(0, 0, (float)(rectWidth * zoom), 999999f);
            Font font2 = StiFontUtils.ChangeFontSize(font, font.Size * zoom);
            RectangleF baseRect = new RectangleF(0, 0, (float)(rect.Width * zoom), (float)(rect.Height * zoom));
            MeasurableInfo info = new MeasurableInfo(g, font2, rectF2, sf, baseRect);
            bool needCut = false;

            var lines = SplitString(text, true);

            for (int indexLine = 0; indexLine < lines.Count; indexLine++)
            {
                string stt = lines[indexLine];

                if (stt.Length == 0)
                {
                    arrLinesInfo.Add(new StiTextRenderer.LineInfo() { Text = string.Empty, LineHeight = info.LineHeight / 10 });
                    continue;
                }

                info.SetText(stt);

                if (!sf.FormatFlags.HasFlag(StringFormatFlags.NoWrap) && !needCut)
                {
                    #region Calculate wordwrap points
                    RectangleF rectf = info.GetRect(0);
                    double lastCenter = rectf.Top + rectf.Height / 2d;
                    int pos = 0;
                    int skip = 0;
                    for (int index = 1; index < stt.Length; index++)
                    {
                        if (skip == 0)
                        {
                            int count = 15;
                            if (index + count > stt.Length - 1) count = stt.Length - index - 1;
                            if (count > 0)
                            {
                                rectf = info.GetRect(index + count);
                                if ((rectf.Top < lastCenter) && !rectf.IsEmpty)
                                {
                                    index += count;
                                    continue;
                                }
                                else
                                {
                                    skip = count;
                                }
                            }
                        }
                        else
                        {
                            skip--;
                        }
                        rectf = info.GetRect(index);
                        if (rectf.Top > lastCenter)
                        {
                            //new line
                            arrLinesInfo.Add(MakeLineInfo(stt, pos, index - pos, info, horAlignWidth, needCut));
                            pos = index;
                            lastCenter = rectf.Top + rectf.Height / 2d;
                            skip = 0;

                            if ((arrLinesInfo.Count - 1) * info.LineHeight > rectHeight)
                            {
                                needCut = true;
                                break;
                            }
                        }
                    }
                    if (pos < stt.Length)
                    {
                        arrLinesInfo.Add(MakeLineInfo(stt, pos, stt.Length - pos, info, false, needCut));
                    }
                    #endregion
                }
                else
                {
                    arrLinesInfo.Add(MakeLineInfo(stt, 0, stt.Length, info, false, needCut));
                }
            }

            //replace "soft hyphen"
            for (int index = 0; index < arrLinesInfo.Count; index++)
            {
                string stt = arrLinesInfo[index].Text;
                if (!string.IsNullOrWhiteSpace(stt) && stt.IndexOf('\xAD') != -1)
                {
                    string stt2 = stt.Replace("\xAD", "");
                    if (stt[stt.Length - 1] == '\xAD')
                    {
                        stt2 += '\xAD';
                    }
                    arrLinesInfo[index].Text = stt2;
                }
            }

            return arrLinesInfo;
        }

        private static StiTextRenderer.LineInfo MakeLineInfo(string st, int begin, int length, MeasurableInfo info, bool needWidthAligh, bool needCut)
        {
            if (!needCut)
                while ((length > 0) && info.GetRect(begin + length - 1).IsEmpty) length--;
            var lineInfo = new StiTextRenderer.LineInfo();
            lineInfo.Text = st.Substring(begin, length);
            lineInfo.NeedWidthAlign = needWidthAligh;
            lineInfo.Widths = new double[length];
            if (!needCut)
            {
                for (int index = 0; index < length; index++)
                {
                    var rect = info.GetRect(begin + index);
                    if (rect.Left > info.baseRect.Right) break; //big speed optimize
                    lineInfo.Widths[index] = rect.Width / 10;
                }
            }
            lineInfo.LineHeight = info.GetRect(begin).Height / 10;
            return lineInfo;
        }

        public static List<string> SplitString(string inputString, bool removeControl)
        {
            var stringList = new List<string>();
            if (inputString == null) inputString = string.Empty;

            var st = new StringBuilder();
            foreach (char ch in inputString)
            {
                if (ch == '\n')
                {
                    stringList.Add(st.ToString().TrimEnd());
                    st.Length = 0;
                }
                else
                {
                    if (!(removeControl && (char.IsControl(ch)) && (ch != '\t')))
                    {
                        st.Append(ch);
                    }
                }
            }
            if (st.Length > 0) stringList.Add(st.ToString().TrimEnd());
            if (stringList.Count == 0) stringList.Add(string.Empty);

            return stringList;
        }

        private class MeasurableInfo
        {
            private string text;
            private Graphics g;
            private Font font;
            private StringFormat sf;
            private RectangleF rect;
            private RectangleF?[] rects;
            internal RectangleF baseRect;

            public void SetText(string st)
            {
                //speed optimize
                if (!sf.FormatFlags.HasFlag(StringFormatFlags.NoWrap) && (st.Length > 100))
                {
                    var size = g.MeasureString(st, font, new SizeF(rect.Width, rect.Height), sf);
                    float cf = size.Height / baseRect.Height;
                    if (cf > 2)
                    {
                        int newLen = (int)(st.Length / cf * 2);
                        if (newLen < st.Length)
                            st = st.Substring(0, newLen);
                    }
                }

                this.text = st;
                rects = new RectangleF?[st.Length];
            }

            double defaultLineHeight = 0;
            public double LineHeight
            {
                get
                {
                    if (defaultLineHeight == 0)
                    {
                        CharacterRange[] cr = new CharacterRange[1];
                        cr[0].First = 0;
                        cr[0].Length = 1;
                        sf.SetMeasurableCharacterRanges(cr);

                        defaultLineHeight = g.MeasureCharacterRanges("\xA0", font, rect, sf)[0].GetBounds(g).Height;
                    }
                    return defaultLineHeight;
                }
            }

            public RectangleF GetRect(int index)
            {
                if (index >= rects.Length) index = rects.Length - 1;
                if (rects[index] == null)
                {
                    int pos = index & 0xFFFFE0;
                    int count = text.Length - pos;
                    if (count > 32) count = 32;

                    CharacterRange[] cr = new CharacterRange[count];
                    for (int index2 = 0; index2 < count; index2++)
                    {
                        cr[index2].First = pos + index2;
                        cr[index2].Length = 1;
                    }
                    sf.SetMeasurableCharacterRanges(cr);

                    Region[] ranges;
                    try
                    {
                        ranges = g.MeasureCharacterRanges(text, font, rect, sf);
                    }
                    catch
                    {
                        for (int index2 = 0; index2 < count; index2++)
                        {
                            cr[index2].First = index2;
                            cr[index2].Length = 1;
                        }
                        sf.SetMeasurableCharacterRanges(cr);
                        ranges = g.MeasureCharacterRanges(text.Substring(pos, count), font, rect, sf);
                    }

                    for (int index2 = 0; index2 < count; index2++)
                    {
                        rects[pos + index2] = ranges[index2].GetBounds(g);
                    }
                }
                return rects[index].Value;
            }

            public MeasurableInfo(Graphics g, Font font, RectangleF rect, StringFormat sf, RectangleF baseRect)
            {
                this.g = g;
                this.font = font;
                this.rect = rect;
                this.sf = sf;
                this.baseRect = baseRect;
            }
        }

        public static string CutLineLimit(ref string text, Graphics g, Font font, RectangleD rect, StiTextOptions textOptions, bool typographic, bool isWindows = true)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;

            if (textOptions == null) textOptions = new StiTextOptions();
            float zoom = isWindows ? 10f : 1f;
            StringFormat sf = GetStringFormat(textOptions, StiTextHorAlignment.Left, StiVertAlignment.Top, typographic, zoom);

            double rectWidth = (sf.FormatFlags.HasFlag(StringFormatFlags.NoWrap)) ? 999999f : rect.Width;
            double rectHeight = rect.Height * zoom;
            RectangleF rectF2 = new RectangleF(0, 0, (float)(rectWidth * zoom), 999999f);
            Font font2 = StiFontUtils.ChangeFontSize(font, font.Size * zoom);
            RectangleF baseRect = new RectangleF(0, 0, (float)(rect.Width * zoom), (float)(rect.Height * zoom));
            MeasurableInfo info = new MeasurableInfo(g, font2, rectF2, sf, baseRect);

            string norm = text;
            if (!isWindows)
            {
                text = text.Replace("\r", "");
                norm = text.Replace("\n", "\n\u200b");
            }
            info.SetText(norm);

            var rectf = info.GetRect(text.Length - 1);
            if (!rectf.IsEmpty && rectf.Bottom < rectHeight) return text;
            rectf = info.GetRect(0);
            if (rectf.Bottom > rectHeight) return string.Empty;

            int pos1 = 0;
            int pos2 = text.Length - 1;
            while (pos2 - pos1 > 1)
            {
                int pos = (pos1 + pos2) / 2;
                rectf = info.GetRect(pos);

                if (!isWindows && rectf.IsEmpty)
                {
                    if (!char.IsWhiteSpace(text, pos))  //bug of libgdi - empty rects after some coordinates
                    {
                        rectf.Height = (float)(rectHeight + 1);
                    }
                    else
                    {
                        int posT = pos;
                        while (rectf.IsEmpty && char.IsWhiteSpace(text, posT) && (posT >= pos1))
                        {
                            posT--;
                            rectf = info.GetRect(posT);
                        };
                    }
                }

                if (rectf.Bottom > rectHeight)
                {
                    pos2 = pos;
                }
                else
                {
                    pos1 = pos;
                }
            }

            if (pos2 - pos1 == 1)
            {
                rectf = info.GetRect(pos1);
                if (rectf.Bottom > rectHeight) return text.Substring(0, pos1);
            }

            string result = text.Substring(0, pos2);
            text = text.Substring(pos2);
            return result;
        }
        #endregion

        #region Methods.HorAlignWidthUtils
        public static string BreakTextWidth(Graphics g, ref string text, Font font, RectangleD rect, StringFormat stringFormat, double lineHeight = 1)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            double heightFont = 0;
            using (var tempFont = StiFontUtils.ChangeFontSize(font, 1024))
            {
                heightFont = tempFont.Height * font.Size / tempFont.Size * lineHeight;
            }

            var posX = rect.Left;
            var posY = rect.Top;
            var wordCount = 0;
            var forceNewLine = false;

            var txt = Word.GetTexts(text.TrimEnd(new char[] { ' ' }));

            var spaceSize = GetAdditionalSpaceSize(g, font);

            StringBuilder rendered = new StringBuilder();
            int k = 0;
            for (k = 0; k < txt.Length; k++)
            {
                var size = g.MeasureString(txt[k].Text, font);

                posX += size.Width + spaceSize;
                if (forceNewLine || (posX >= rect.Right && wordCount >= 1))
                {
                    posX = rect.Left + size.Width + spaceSize;
                    posY += heightFont;
                    wordCount = 0;
                }
                wordCount++;

                if (posY + size.Height > rect.Bottom) break;

                //ranges[k].Pos.Y = posY;
                forceNewLine = txt[k].IsEnter;

                rendered.Append(txt[k].Text + (txt[k].IsEnter ? "\n" : " "));
            }

            if (k == 0) return null;
            if (k == txt.Length)
            {
                string st = text;
                text = string.Empty;
                return st;
            }

            StringBuilder sb = new StringBuilder();
            for (int m = k; m < txt.Length; m++)
            {
                sb.Append(txt[m].Text + (txt[m].IsEnter ? "\n" : " "));
            }
            text = sb.ToString();
            return rendered.ToString() + (forceNewLine ? "" : StiTextRenderer.StiForceWidthAlignTag);
        }

        public static List<string> SplitTextWordwrapWidth(string text, Graphics g, Font font, RectangleD rect)
        {
            var forceWidthAlign = text.EndsWith(StiTextRenderer.StiForceWidthAlignTag, StringComparison.InvariantCulture);
            if (forceWidthAlign)
            {
                text = text.Substring(0, text.Length - StiTextRenderer.StiForceWidthAlignTag.Length);
            }
            text = text.TrimEnd(new char[] { ' ' });

            var rn = new List<Range>();
            var txt = Word.GetTexts(text);
            var ln = new List<Line>();
            double heightFont = font.Height;

            var spaceSize = GetAdditionalSpaceSize(g, font);

            //Form words
            for (var k = 0; k < txt.Length; k++)
            {
                var size = g.MeasureString(txt[k].Text, font);
                rn.Add(new Range(txt[k].Text, new SizeD(size.Width + spaceSize, size.Height), txt[k].IsEnter));
            }

            #region Divide into lines
            var posX = rect.Left;
            var posY = rect.Top;
            var line = 1;
            var wordCount = 0;
            var start = 0;
            var forceNewLine = false;

            var ranges = rn.ToArray();

            for (var k = 0; k < ranges.Length; k++)
            {
                posX += ranges[k].Size.Width;
                if (forceNewLine || (posX >= rect.Right && wordCount >= 1))
                {
                    posX = rect.Left + ranges[k].Size.Width;
                    line++;
                    posY += heightFont;
                    ln.Add(new Line(start, wordCount, forceNewLine));
                    start = k;
                    wordCount = 0;
                }
                wordCount++;

                ranges[k].Pos.Y = posY;
                forceNewLine = ranges[k].NewLineForm;
            }
            ln.Add(new Line(start, wordCount, !forceWidthAlign));
            #endregion

            var arrLines = new List<string>();

            foreach (Line line2 in ln)
            {
                StringBuilder sb = new StringBuilder();
                int lineEnd = line2.Start + line2.Count;
                for (int index = line2.Start; index < lineEnd; index++)
                {
                    sb.Append(ranges[index].Text);
                    if (index == lineEnd - 1)
                    {
                        sb.Append(line2.IsEnter ? "" : "\a");
                    }
                    else
                    {
                        sb.Append(" ");
                    }
                }
                arrLines.Add(sb.ToString());
            }

            return arrLines;
        }

        private static float GetAdditionalSpaceSize(Graphics g, Font font)
        {
            //check for TrailingSpaces
            var spaceSize = 0f;
            var sp1 = g.MeasureString("H", font).Width;
            var sp2 = g.MeasureString("HHHHHHHHHHHHHHHHHHHH", font).Width / 20;
            if (sp2 / sp1 > 0.9) //need additional space, maybe it's Linux
            {
                spaceSize = g.MeasureString(" ", font, 100000).Width / 1.5f;
            }
            return spaceSize;
        }

        private static bool? needAdditionalSpace = null;
        #endregion

        #region Methods.MeasureString
        /// <summary>
        /// Draws the text aligned to width.
        /// </summary>
        /// <param name="g">Graphics to draw on.</param>
        /// <param name="text">Text to draw on.</param>
        /// <param name="font">Font to draw on.</param>
        /// <param name="size">Size of rectangle to draw on.</param>
        /// <param name="stringFormat">Text format.</param>
        public static SizeF MeasureStringWidth(Graphics g, string text, Font font, SizeD size, StringFormat stringFormat, double lineHeight = 1)
        {
            if (!string.IsNullOrEmpty(text))
            {
                if ((stringFormat.FormatFlags & StringFormatFlags.NoWrap) > 0)
                    stringFormat.FormatFlags ^= StringFormatFlags.NoWrap;

                var ranges = GetRange(g, new RectangleD(0, 0, size.Width, size.Height), text, font, stringFormat, lineHeight);

                if (ranges.Length > 0)
                {
                    var last = ranges[ranges.Length - 1];
                    var height = (float)(last.Pos.Y + last.Size.Height - ranges[0].Pos.Y);
                    return new SizeF((float)size.Width, height);
                }
            }
            return new SizeF();
        }

        public static SizeF MeasureString(Graphics g, string text, Font font)
        {
            return MeasureString(g, text, font, StringAlignment.Center, StringAlignment.Center, 0f);
        }

        public static SizeF MeasureString(Graphics g, string text, Font font,
            StringAlignment horizontalAligment, StringAlignment vertiacalAligment, float angle)
        {

            StiTextHorAlignment ha;

            switch (horizontalAligment)
            {
                case StringAlignment.Center:
                    ha = StiTextHorAlignment.Center;
                    break;

                case StringAlignment.Far:
                    ha = StiTextHorAlignment.Right;
                    break;

                default:
                    ha = StiTextHorAlignment.Left;
                    break;
            }

            StiVertAlignment va;

            switch (vertiacalAligment)
            {
                case StringAlignment.Center:
                    va = StiVertAlignment.Center;
                    break;

                case StringAlignment.Far:
                    va = StiVertAlignment.Bottom;
                    break;

                default:
                    va = StiVertAlignment.Top;
                    break;
            }

            return MeasureString(g, text, font, ha, va, angle).ToSizeF();
        }

        public static SizeD MeasureString(Graphics g, string text, Font font,
            StiTextHorAlignment ha, StiVertAlignment va, float angle)
        {
            return MeasureString(g, text, font, ha, va, false, angle);
        }

        public static SizeD MeasureString(Graphics g, string text, Font font,
            StiTextHorAlignment ha, StiVertAlignment va, bool antialiasing, float angle)
        {
            var options = new StiTextOptions();
            options.Angle = angle;
            return MeasureString(g, text, font, 0, options, ha, va, antialiasing);
        }

        public static SizeD MeasureString(Graphics g, string text, Font font,
            double width, StiTextOptions textOptions, StiTextHorAlignment ha, StiVertAlignment va,
            bool antialiasing, double lineHeight = 1)
        {
            #region Cache for stringFormat - about 4% speed optimization
            int hashCode;
            unchecked
            {
                hashCode = textOptions.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)ha;
                hashCode = (hashCode * 397) ^ (int)va;
                hashCode = (hashCode * 397) ^ (antialiasing ? 1231 : 1237);
            }
            var stringFormat = (StringFormat)hashStringFormats[hashCode];
            if (stringFormat == null)
            {
                stringFormat = GetStringFormat(textOptions, ha, va, antialiasing, 1);
                lock (hashStringFormats)
                {
                    hashStringFormats[hashCode] = stringFormat;
                }
            }
            #endregion

            double maxWidth = width;
            if (!textOptions.WordWrap) width = 0;

            SizeF size;

            try
            {
                if (ha == StiTextHorAlignment.Width)
                {
                    size = MeasureStringWidth(g, text, font, new SizeD((float)maxWidth, 999999f), stringFormat, lineHeight);
                }
                else
                {
                    size = g.MeasureString(text, font, new SizeF((float)width, 999999f), stringFormat);
                    size.Height *= (float)lineHeight;
                }
            }
            catch
            {
                if (text.Length > 16384)    //approximate value
                {
                    try
                    {
                        size = g.MeasureString(text.Substring(0, 16384), font, new SizeF((float)width, 999999f), stringFormat);
                        size.Height *= (float)lineHeight;
                    }
                    catch
                    {
                        size = new SizeF((float)width, 0);
                    }
                }
                else
                {
                    size = new SizeF((float)width, 0);
                }
            }

            if (!antialiasing && (ha != StiTextHorAlignment.Width))
            {
                if (needAdditionalSpace == null)
                {
                    needAdditionalSpace = GetAdditionalSpaceSize(g, font) > 0;
                }
                if (needAdditionalSpace.Value)  //possible Linux
                {
                    var spaceWidth = g.MeasureString(" ", font).Width;
                    var newWidth = size.Width + spaceWidth;
                    if (textOptions.WordWrap) newWidth = Math.Min(newWidth, (float)maxWidth);
                    size.Width = newWidth;
                }
            }

            float textOptionsAngle = textOptions.Angle % 360;
            if (textOptionsAngle < 0) textOptionsAngle = 360 + textOptionsAngle;

            if (textOptionsAngle == 90 || textOptionsAngle == 270)
            {
                var rw = size.Width;
                size.Width = size.Height;
                size.Height = rw;
            }
            else if (textOptionsAngle != 0f)
            {
                var sx = size.Width / 2;
                var sy = size.Height / 2;

                var points = new[]
                {
                    new PointF(-sx, -sy),
                    new PointF(sx, -sy),
                    new PointF(sx, sy),
                    new PointF(-sx, sy)
                };

                var m = new Matrix();
                m.Rotate(-(float)textOptionsAngle);
                m.TransformPoints(points);

                var index = 0;
                foreach (var point in points)
                {
                    double px = point.X;
                    double py = point.Y;

                    points[index++] = new PointF((float)(px + size.Width / 2), (float)(py + size.Height / 2));
                }

                using (var path = new GraphicsPath())
                {
                    path.AddPolygon(points);
                    var rect2 = path.GetBounds();
                    return new SizeD(rect2.Width, rect2.Height);
                }
            }
            return new SizeD(size.Width, size.Height);
        }
        #endregion

        #region Methods.StringFormat
        public static StringFormat GetStringFormat(
            StiTextOptions textOptions, StiTextHorAlignment ha, StiVertAlignment va, float zoom)
        {
            return GetStringFormat(textOptions, ha, va, false, zoom);
        }

        public static StringAlignment GetAlignment(StiTextHorAlignment alignment)
        {
            switch (alignment)
            {
                case StiTextHorAlignment.Center:
                case StiTextHorAlignment.Width:
                    return StringAlignment.Center;

                case StiTextHorAlignment.Right:
                    return StringAlignment.Far;

                default:
                    return StringAlignment.Near;
            }
        }

        public static StringAlignment GetAlignment(StiVertAlignment alignment)
        {
            switch (alignment)
            {
                case StiVertAlignment.Center:
                    return StringAlignment.Center;

                case StiVertAlignment.Bottom:
                    return StringAlignment.Far;

                default:
                    return StringAlignment.Near;
            }
        }

        public static StringFormat GetStringFormat(
            StiTextOptions textOptions, StiTextHorAlignment ha, StiVertAlignment va, bool antialiasing, float zoom)
        {
            var stringFormat = textOptions.GetStringFormat(antialiasing, zoom);

            stringFormat.Alignment = GetAlignment(ha);
            stringFormat.LineAlignment = GetAlignment(va);

            if (MeasureTrailingSpaces)
                stringFormat.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

            return stringFormat;
        }
        #endregion

        #region Fields.Static
        private static Hashtable hashStringFormats = new Hashtable();
        #endregion

        #region Properties.Static
        /// <summary>
        /// Gets or sets value which indicates that text drawing engine will be measure text string including trailing spaces.
        /// </summary>
        public static bool MeasureTrailingSpaces { get; set; }
        #endregion
    }
}
