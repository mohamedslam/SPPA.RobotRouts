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

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
using Graphics = Stimulsoft.Drawing.Graphics;
using StringFormat = Stimulsoft.Drawing.StringFormat;
using SolidBrush = Stimulsoft.Drawing.SolidBrush;
using Metafile = Stimulsoft.Drawing.Imaging.Metafile;
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Base.Drawing
{
    [SecuritySafeCritical]
    public partial class StiTextRenderer
    {
        #region class StiFontState
        public class StiFontState
        {
            public string FontName; //original font name
            public Font FontBase;
            public Font FontScaled;
            public int SuperOrSubscriptIndex;
            public int ParentFontIndex;
            public IntPtr hFont;
            public IntPtr hFontScaled;
            public IntPtr hScriptCache;
            public IntPtr hScriptCacheScaled;
            public double LineHeight;
            public double Ascend;
            public double Descend;
            public double ElipsisWidth = 0;
            public double EmValue;
            public double TypoOffset;
            public bool HasNbspGlyf = true;

            private string fontNameReal = null;
            public string FontNameReal //cached name of FontBase
            {
                get
                {
                    return fontNameReal ?? (fontNameReal = FontBase.Name);
                }
            }
        }
        #endregion

        #region class LineInfo
        public class LineInfo
        {
            public int Begin;
            public int Length;
            public bool NeedWidthAlign;

            public int End
            {
                get
                {
                    return Begin + Length;
                }
                set
                {
                    Length = value - Begin;
                }
            }

            public int Width;
            public double[] Widths;
            public double JustifyOffset;
            public string Text;
            public int IndexOfMaxFont;
            public double LineHeight;
            public StiTextHorAlignment TextAlignment;
            public int Indent;
        }
        #endregion

        #region struct RunInfo
        public struct RunInfo
        {
            public string Text;
            public double XPos;
            public double YPos;
            public int[] Widths;
            public int[] GlyphWidths;
            public Color TextColor;
            public Color BackColor;
            public int FontIndex;
            public int[] GlyphIndexList;
            public double[] ScaleList;
            public string Href;
            public StiTextHorAlignment TextAlign;
        }
        #endregion

        #region Properties.Static
        /// <summary>
        /// Obsolete
        /// </summary>
        public static double PrecisionModeFactor { get; set; } = 4;

        public static bool PrecisionModeEnabled { get; set; } = false;

        public static bool PrecisionMode2Enabled { get; set; } = false;
        public static bool PrecisionMode3Enabled { get; set; } = true;

        /// <summary>
        /// Enable the inter-character distance correction
        /// </summary>
        public static bool CorrectionEnabled { get; set; } = true;

        public static float MaxFontSize { get; set; } = 1024;

        /// <summary>
        /// Enable text scaling mode as in the 2009.1 version.
        /// For backward compatibility 
        /// </summary>
        public static bool Compatibility2009 { get; set; }

        /// <summary>
        /// Optimize the bottom margin of the text
        /// </summary>
        public static bool OptimizeBottomMargin { get; set; } = true;

        /// <summary>
        /// Interprete the FontSize attribute in the Html-tags as in Html
        /// </summary>
        public static bool InterpreteFontSizeInHtmlTagsAsInHtml { get; set; }

        public static double StiDpiHelperGraphicsScale { get; set; } = 1;

        public static string StiForceWidthAlignTag { get; set; } = "<forcewidth>";

        public static bool AllowCheckMissingGlyphs { get; set; } = true;
        #endregion

        #region Fields.Static
        private static Hashtable htmlNameToColor;
        private static object lockHtmlNameToColor = new object();
        private static Hashtable outlineTextMetricsCache = new Hashtable();
        private static object lockOutlineTextMetricsCache = new object();

        private static object lockHashFonts = new object();
        private static Hashtable hashFontWidths = new Hashtable();
        private static Hashtable hashFontGlyphs = new Hashtable();
        #endregion

        #region Methods.GetFontData
        public static object GetFontGlyphs(Font font)
        {
            var fontName = font.Name + (font.Bold ? ",bold" : "") + (font.Italic ? ",italic" : "");
            object data = null;
            lock (lockHashFonts)
            {
                data = hashFontGlyphs[fontName];
                if (data == null)
                {
                    GetGdiFontData(font, fontName);
                    data = hashFontGlyphs[fontName];
                }
            }
            return data;
        }

        private static ushort[] GetFontWidth(StiFontState fontState)
        {
            var fontName = fontState.FontNameReal + (fontState.FontBase.Bold ? ",bold" : "") + (fontState.FontBase.Italic ? ",italic" : "");
            object data = null;
            lock (lockHashFonts)
            {
                data = hashFontWidths[fontName];
                if (data == null)
                {
                    GetGdiFontData(fontState.FontBase, fontName);
                    data = hashFontWidths[fontName];
                }
            }
            return (ushort[])data;
        }

        [SecuritySafeCritical]
        private static void GetGdiFontData(Font font, string fontHashName)
        {
            var widths = new ushort[0];
            var glyphs = new ushort[0];

            bool allow = true;
#if STIDRAWING
            allow = Graphics.GraphicsEngine == Stimulsoft.Drawing.GraphicsEngine.Gdi;
#endif

            if (allow)
            {
                try
                {
#if NETSTANDARD
                var image = new global::System.Drawing.Bitmap(1, 1);
                var gr = Graphics.FromImage(image);
#else
                    var gr = Graphics.FromHwnd(IntPtr.Zero);
#endif
                    try
                    {
                        var hdc = gr.GetHdc();
                        try
                        {
                            using (var tempFont = StiFontCollection.CreateFont(font.Name, MaxFontSize, font.Style))
                            {
                                var hFont = tempFont.ToHfont();
                                try
                                {
                                    var oldObj = SelectObject(hdc, hFont);

                                    //Symbols
                                    var sb = new StringBuilder();
                                    for (var index = 0; index < 0x10000; index++)
                                    {
                                        sb.Append((char)index);
                                    }

                                    var str = sb.ToString();
                                    var count = str.Length;
                                    var rtcode = new ushort[count];
                                    var res = GetGlyphIndices(hdc, str, count, rtcode, 1);
                                    if (res == GDI_ERROR) ThrowError(100);

                                    glyphs = rtcode;

                                    //Glyphs
                                    var count1 = 0;
                                    for (var index = 0; index < count; index++)
                                    {
                                        if (rtcode[index] != 0xFFFF && rtcode[index] > count1) count1 = rtcode[index];
                                    }

                                    var rtcode1 = new ushort[count1];
                                    for (ushort index = 0; index < count1; index++)
                                    {
                                        rtcode1[index] = index;
                                    }

                                    //Widths
                                    var abc = new ABC[count1];
                                    var gdiErr = GetCharABCWidthsI(hdc, 0, (uint)count1, rtcode1, abc);
                                    if (gdiErr == false) ThrowError(101);

                                    var tempWidths = new ushort[count1];
                                    for (var index = 0; index < count1; index++)
                                    {
                                        //double fl = (abc[index].abcA + abc[index].abcB + abc[index].abcC) * FontFactor * GraphicsScale;
                                        //double fl = (abc[index].abcA + abc[index].abcB + abc[index].abcC) * GraphicsScale;
                                        double fl = (abc[index].abcA + abc[index].abcB + abc[index].abcC);
                                        tempWidths[index] = (ushort)(Math.Round(fl));
                                    }

                                    var pos = count1 - 1;
                                    while (pos > 0 && tempWidths[pos - 1] == tempWidths[count1 - 1]) pos--;
                                    pos++;

                                    widths = new ushort[pos];
                                    for (var index = 0; index < pos; index++)
                                    {
                                        widths[index] = tempWidths[index];
                                    }

                                    SelectObject(hdc, oldObj);
                                }
                                finally
                                {
                                    var error = DeleteObject(hFont);
                                    if (!error) ThrowError(102);
                                }
                            }
                        }
                        finally
                        {
                            gr.ReleaseHdc(hdc);
                        }
                    }
                    finally
                    {
                        gr.Dispose();
#if NETSTANDARD
                    image.Dispose();
#endif
                    }
                }
                catch { }
            }

            hashFontGlyphs[fontHashName] = glyphs;
            hashFontWidths[fontHashName] = widths;
        }
#endregion

#region Methods.ThrowError
        private static void ThrowError(int step)
        {
            var myEx = new Win32Exception(Marshal.GetLastWin32Error());
            throw new Exception($"TextRender error at step {step}, code #{myEx.ErrorCode:X8}: {myEx.Message}");
        }

        private static void ThrowError(int step, int error)
        {
            var myEx = new Win32Exception(Marshal.GetLastWin32Error());
            throw new Exception($"TextRender error at step {step}, code #{myEx.ErrorCode:X8}(#{error:X8}): {myEx.Message}");
        }
#endregion

#region Consts
        private const int precisionDigits = 5;
        private const double defaultParagraphLineHeight = 0.7;
#endregion

#region Utils
        /// <summary>
        /// Convert Color to Win32 GDI format
        /// </summary>
        /// <param name="c">Input Color</param>
        /// <returns>Result GDI Color</returns>
        internal static int ColorToWin32(Color c)
        {
            return c.R | (c.G << 8) | (c.B << 0x10);
        }

        private static int GetTabsWidth(StiTextOptions textOptions, double tabSpaceWidth, double currentPosition)
        {
            float tDistanceBetweenTabs = 20;
            float tFirstTabOffset = 40;
            if (textOptions != null)
            {
                tDistanceBetweenTabs = textOptions.DistanceBetweenTabs;
                tFirstTabOffset = textOptions.FirstTabOffset;
            }

            var position = currentPosition;
            var otherTab = tabSpaceWidth * tDistanceBetweenTabs;
            var firstTab = tabSpaceWidth * tFirstTabOffset + otherTab;

            if (currentPosition < firstTab)
            {
                position = firstTab;
            }
            else
            {
                if (tDistanceBetweenTabs > 0)
                {
                    var kolTabs = (int)((currentPosition - firstTab) / otherTab);
                    kolTabs++;
                    position = firstTab + kolTabs * otherTab;
                }
            }

            var result = (int)Math.Round((decimal)(position - currentPosition));
            return result;
        }

        public static int GetFontIndex(string fontName, float fontSize, bool bold, bool italic, bool underlined, bool strikeout,
            bool superOrSubscript, List<StiFontState> tempFontList)
        {
            var fontIndex = GetFontIndex2(fontName, fontSize, bold, italic, underlined, strikeout, tempFontList);
            if (superOrSubscript)
            {
                var fontIndex2 = (tempFontList[fontIndex]).SuperOrSubscriptIndex;
                if (fontIndex2 == -1)
                {
                    fontIndex2 = GetFontIndex2(fontName, fontSize / 1.5f, bold, italic, underlined, strikeout, tempFontList);
                    (tempFontList[fontIndex]).SuperOrSubscriptIndex = fontIndex2;
                    (tempFontList[fontIndex2]).ParentFontIndex = fontIndex;
                }

                fontIndex = fontIndex2;
            }

            return fontIndex;
        }

        private static int GetFontIndex2(string fontName, float fontSize, bool bold, bool italic, bool underlined, bool strikeout,
            List<StiFontState> tempFontList)
        {
            if (tempFontList.Count > 0)
            {
                for (var indexFont = 0; indexFont < tempFontList.Count; indexFont++)
                {
                    var tempFontState = tempFontList[indexFont];
                    if ((tempFontState.FontName == fontName) &&
                        (tempFontState.FontBase.Size == fontSize) &&
                        (tempFontState.FontBase.Bold == bold) &&
                        (tempFontState.FontBase.Italic == italic) &&
                        (tempFontState.FontBase.Underline == underlined) &&
                        (tempFontState.FontBase.Strikeout == strikeout))
                    {
                        return indexFont;
                    }
                }
            }

            var fontStyle = FontStyle.Regular;
            if (bold) fontStyle |= FontStyle.Bold;
            if (italic) fontStyle |= FontStyle.Italic;
            if (underlined) fontStyle |= FontStyle.Underline;
            if (strikeout) fontStyle |= FontStyle.Strikeout;

            Font stateFont = null;
            if (fontName.IndexOf(',') != -1)
            {
                var fontNames = fontName.Split(',');
                foreach (var fontNamePart in fontNames)
                {
                    stateFont = StiFontCollection.CreateFont(fontNamePart, fontSize, fontStyle);
                    if (stateFont.Name.Equals(fontNamePart, StringComparison.InvariantCultureIgnoreCase)) break;
                }
            }
            else
            {
                stateFont = StiFontCollection.CreateFont(fontName, fontSize, fontStyle);
            }

            var fontState = new StiFontState
            {
                FontName = fontName,
                FontBase = stateFont,
                ParentFontIndex = -1,
                SuperOrSubscriptIndex = -1
            };
            tempFontList.Add(fontState);
            return tempFontList.Count - 1;
        }
#endregion

#region Font position correction
        private static List<string> fpc_fontName = new List<string>
        {
            "Arial",
            "Bookman Old Style"
        };

        private static int[,] fpc_fontData =
        {
            {
                0, 0, 0, 0, 0, 0, 2, 4, 2, -2, 0, 0, 2, 2, 0, 0, 2, 2, 2, 1, 0, 1, 0, 2, 2, 0, 0, 2, 2, 1, 0, 0, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2
            }, //bold
            {
                0, 2, 2, 0, 0, 0, 2, 2, 0, -2, -1, 0, 1, 0, -1, 0, 0, 0, 1, 0, -1, 1, 0, 1, 0, -1, 0, 0, 0, 0, -1, -1, -1, 0, 0, -1, -1, 1, 0, 1, 0, 0
            }, //italic
            {
                0, 0, 0, 0, 0, 0, -2, 2, 0, 2, 1, 0, 0, 0, 1, 0, 0, 0, -1, 1, 1, 0, 0, 1, 1, 1, 0, 0, -1, 0, 1, 1, 1, 1, 1, 1, 1, 0, 1, 0, 0, 1
            }, //bold + italic

            {
                -1, -1, -1, 1, 1, 0, 1, 1, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 1, 1, 1, 0, 0, 0, 0, 0, 1, 2, 2, 2, 1, 0, 0, 0, 1, 1, 2, 1, 0, 0, 0
            }, //bold
            {
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
            }, //italic
            {
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
            }, //bold + italic
        };

        private const int fpc_startIndex = 11;

        private static double GetFontPositionCorrection(StiFontState fontState)
        {
            var fontIndex = fpc_fontName.IndexOf(fontState.FontNameReal);
            if (fontIndex == -1) return 0;

            double resultOffset = 0;

            var styleIndex = 0;
            if ((fontState.FontBase.Style & FontStyle.Bold) > 0) styleIndex += 1;
            if ((fontState.FontBase.Style & FontStyle.Italic) > 0) styleIndex += 2;
            if (styleIndex > 0)
            {
                var fontSize = (int)Math.Round(fontState.FontBase.Size);
                if (fontSize < fpc_startIndex) fontSize = fpc_startIndex;
                if (fontSize > 52) fontSize = 52;
                var correctionValue = fpc_fontData[fontIndex * 3 + styleIndex - 1, fontSize - fpc_startIndex];
                resultOffset = -correctionValue * fontState.FontBase.Size * 0.025;
            }

            return resultOffset;
        }
#endregion

#region Methods
        [SecuritySafeCritical]
        public static SizeD MeasureText(Graphics g, string text, Font font, RectangleD bounds,
            double lineSpacing, bool wordWrap, bool rightToLeft,
            double scale, double angle, StringTrimming trimming, bool lineLimit, bool allowHtmlTags, StiTextOptions textOptions, double? scaleCorrection = null)
        {
            lock (g)
            {
                var measureSize = new SizeD(0, 0);
                DrawTextBase(g, ref text, font, bounds, Color.Black, Color.Black, lineSpacing,
                    StiTextHorAlignment.Left, StiVertAlignment.Top, wordWrap, rightToLeft, scale, angle,
                    trimming, lineLimit, ref measureSize, false, null, null, allowHtmlTags, null, null, textOptions, scaleCorrection);
                return measureSize;
            }
        }

        [SecuritySafeCritical]
        public static void DrawText(Graphics g, string text, Font font, RectangleD bounds,
            Color foreColor, Color backColor, double lineSpacing,
            StiTextHorAlignment horAlign, StiVertAlignment vertAlign, bool wordWrap, bool rightToLeft,
            double scale, double angle, StringTrimming trimming, bool lineLimit, bool allowHtmlTags, StiTextOptions textOptions = null, double? scaleCorrection = null)
        {
            var measureSize = new SizeD(0, 0);
            DrawTextBase(g, ref text, font, bounds, foreColor, backColor, lineSpacing, horAlign, vertAlign,
                wordWrap, rightToLeft, scale, angle, trimming, lineLimit, ref measureSize, true, null, null, allowHtmlTags, null, null, textOptions, scaleCorrection);
        }

        [SecuritySafeCritical]
        public static string BreakText(Graphics g, ref string text, Font font, RectangleD bounds,
            Color foreColor, Color backColor, double lineSpacing, StiTextHorAlignment horAlign, bool wordWrap, bool rightToLeft,
            double scale, double angle, StringTrimming trimming, bool allowHtmlTags, StiTextOptions textOptions, double? scaleCorrection = null)
        {
            var measureSize = new SizeD(0, 0);
            return DrawTextBase(g, ref text, font, bounds, foreColor, backColor, lineSpacing,
                horAlign, StiVertAlignment.Top, wordWrap, rightToLeft, scale, angle,
                trimming, true, ref measureSize, false, null, null, allowHtmlTags, null, null, textOptions, scaleCorrection);
        }

        [SecuritySafeCritical]
        public static List<string> GetTextLines(Graphics g, ref string text, Font font, RectangleD bounds,
            double lineSpacing, bool wordWrap, bool rightToLeft,
            double scale, double angle, StringTrimming trimming, bool allowHtmlTags, StiTextOptions textOptions, double? scaleCorrection = null)
        {
            var measureSize = new SizeD(0, 0);
            var textLines = new List<string>();
            DrawTextBase(g, ref text, font, bounds, Color.Black, Color.Black, lineSpacing,
                StiTextHorAlignment.Left, StiVertAlignment.Top, wordWrap, rightToLeft, scale, angle,
                trimming, true, ref measureSize, false, textLines, null, allowHtmlTags, null, null, textOptions, scaleCorrection);
            return textLines;
        }

        [SecuritySafeCritical]
        public static List<string> GetTextLinesAndWidths(Graphics g, ref string text, Font font, RectangleD bounds,
            double lineSpacing, bool wordWrap, bool rightToLeft, double scale, double angle,
            StringTrimming trimming, bool allowHtmlTags, ref List<string> textLines, ref List<StiTextRenderer.LineInfo> linesInfo, StiTextOptions textOptions, double? scaleCorrection = null)
        {
            var measureSize = new SizeD(0, 0);
            DrawTextBase(g, ref text, font, bounds, Color.Black, Color.Black, lineSpacing,
                StiTextHorAlignment.Left, StiVertAlignment.Top, wordWrap, rightToLeft, scale, angle,
                trimming, true, ref measureSize, false, textLines, linesInfo, allowHtmlTags, null, null, textOptions, scaleCorrection);
            return textLines;
        }

        [SecuritySafeCritical]
        public static void DrawTextForOutput(Graphics g, string text, Font font, RectangleD bounds,
            Color foreColor, Color backColor, double lineSpacing,
            StiTextHorAlignment horAlign, StiVertAlignment vertAlign, bool wordWrap, bool rightToLeft,
            double scale, double angle, StringTrimming trimming, bool lineLimit, bool allowHtmlTags,
            List<RunInfo> outRunsList, List<StiFontState> outFontsList, StiTextOptions textOptions, double? scaleCorrection = null)
        {
            var measureSize = new SizeD(0, 0);
            DrawTextBase(g, ref text, font, bounds, foreColor, backColor, lineSpacing, horAlign, vertAlign,
                wordWrap, rightToLeft, scale, angle, trimming, lineLimit, ref measureSize, true, null, null,
                allowHtmlTags, outRunsList, outFontsList, textOptions, scaleCorrection);
        }

#region DrawTextBase private
        [SecuritySafeCritical]
        private static string DrawTextBase(Graphics g, ref string text, Font font, RectangleD bounds,
            Color foreColor, Color backColor, double lineSpacing,
            StiTextHorAlignment horAlign, StiVertAlignment vertAlign, bool wordWrap, bool rightToLeft,
            double scale, double angle, StringTrimming trimming, bool lineLimit,
            ref SizeD measureSize, bool needDraw, List<string> textLinesArray, List<StiTextRenderer.LineInfo> textLinesInfo, bool allowHtmlTags,
            List<RunInfo> outRunsList, List<StiFontState> outFontsList, StiTextOptions textOptions, double? scaleCorrection = null)
        {
            var regionRect = new RectangleD(
                //bounds.X + g.Transform.OffsetX + 1,   //fix 02.12.2008
                bounds.X + g.Transform.OffsetX + 0,
                bounds.Y + g.Transform.OffsetY,
                bounds.Width + 1,
                bounds.Height + 1);
            double sideRectOffset = PrecisionMode2Enabled ? 1.5 + (font.SizeInPoints * StiDpiHelperGraphicsScale * scaleCorrection.GetValueOrDefault(1) > 6 ? (font.SizeInPoints * StiDpiHelperGraphicsScale * scaleCorrection.GetValueOrDefault(1) - 6) * 0.2 : 0) : 1.5;
            var textRect = new RectangleD(
                regionRect.X + sideRectOffset * scale,
                regionRect.Y,
                (double)Math.Round((decimal)(bounds.Width - sideRectOffset * 2 * scale), precisionDigits),
                bounds.Height);
            var textSize = new SizeD(textRect.Width, textRect.Height); //8.81102 width

            angle %= 360;
            if (angle < 0) angle = 360 + angle;
            bool isNotRightAngle = (angle != 0) && (angle != 90) && (angle != 180) && (angle != 270);

            var baseFont = font;
            var baseScale = scale;
            if (PrecisionModeEnabled)
            {
                font = StiFontCollection.CreateFont(font.Name, (float)(font.Size * PrecisionModeFactor), font.Style);
                scale = baseScale / PrecisionModeFactor;
            }

            var gUnit = g.PageUnit;
            var dpiX = g.DpiX;
            var dpiY = g.DpiY;
            var pageF = g.ClipBounds;
            var pageScale = g.PageScale;

            //backingImage == System.Drawing.Imaging.Metafile

#region Get hidden field
            Image backingImage = null;
            var typeFormat1 = g.GetType();
            var infoFormat1 = typeFormat1.GetField("backingImage", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.NonPublic);
            if (infoFormat1 != null)
            {
                var obj1 = infoFormat1.GetValue(g); //backingImage
                if (obj1 != null)
                    backingImage = obj1 as Image;
            }
#endregion

            var useGdiPlus = backingImage != null && backingImage is Metafile || outRunsList != null;
            var writtenText = text;
            var breakText = string.Empty;
            var runs = new List<RunInfo>();

            StiFontState[] fontList = null;
            StiHtmlState[] stateList = null;
            int[] stateOrder = null;
            var currentStateIndex = 0;
            if (!string.IsNullOrEmpty(text) && foreColor != Color.Transparent && font != null && scale > 0.00001)
            {
                if (horAlign == StiTextHorAlignment.Width) wordWrap = true;

                var forceWidthAlign = text.EndsWith(StiForceWidthAlignTag, StringComparison.InvariantCulture);
                if (forceWidthAlign)
                {
                    text = text.Substring(0, text.Length - StiForceWidthAlignTag.Length);
                    writtenText = text;
                }

                var originalText = text;

#region Make states list
                var baseTagsState = new StiHtmlTagsState(
                    baseFont.Bold,
                    baseFont.Italic,
                    baseFont.Underline,
                    baseFont.Strikeout,
                    baseFont.SizeInPoints,
                    baseFont.Name,
                    foreColor,
                    backColor,
                    false,
                    false,
                    0,
                    0,
                    lineSpacing,
                    horAlign);
                var baseState = new StiHtmlState(
                    baseTagsState,
                    0);

                if (!allowHtmlTags)
                {
#region CheckMissingGlyphs
                    if (AllowCheckMissingGlyphs)
                    {
                        try
                        {
                            var storedSB = baseState.Text;
                            baseState.Text = new StringBuilder(text);
                            var newStates = CheckMissingGlyphs(baseState);
                            baseState.Text = storedSB;
                            if (newStates != null)
                            {
                                allowHtmlTags = true;
                                text = ConvertSymbolsToTags(text);
                                originalText = text;
                            }
                        }
                        catch
                        {
                            AllowCheckMissingGlyphs = false;
                        }
                    }
#endregion
                }

                if (allowHtmlTags)
                {
                    var states = ParseHtmlToStates(text, baseState, !needDraw);
                    stateList = new StiHtmlState[states.Count];
                    var sb = new StringBuilder();
                    var orders = new List<int>();
                    for (var index = 0; index < states.Count; index++)
                    {
                        var state = states[index];
                        stateList[index] = state;

                        var sbTemp = PrepareStateText(state.Text);
                        sb.Append(sbTemp);

                        for (var indexOrder = 0; indexOrder < sbTemp.Length; indexOrder++)
                        {
                            orders.Add(index);
                        }

                        if (state.TS.TextAlign == StiTextHorAlignment.Width)
                        {
                            state.TS.WordSpacing = 0;
                        }
                    }

                    text = sb.ToString();
                    stateOrder = new int[orders.Count];
                    for (var index = 0; index < stateOrder.Length; index++)
                    {
                        stateOrder[index] = orders[index];
                    }
                }
                else
                {
                    stateList = new StiHtmlState[1];
                    stateList[0] = baseState;
                    stateOrder = new int[text.Length];
                    for (var index = 0; index < stateOrder.Length; index++)
                    {
                        stateOrder[index] = 0;
                    }
                }
#endregion

#region Make fonts list
                var tempFontList = new List<StiFontState>();
                for (var indexState = 0; indexState < stateList.Length; indexState++)
                {
                    var ffontSize = (float)(PrecisionModeEnabled ? stateList[indexState].TS.FontSize * PrecisionModeFactor : stateList[indexState].TS.FontSize);
                    if (scaleCorrection != null)
                    {
                        ffontSize = (float)(ffontSize * StiDpiHelperGraphicsScale * scaleCorrection.Value);
                    }

                    stateList[indexState].FontIndex = GetFontIndex(
                        stateList[indexState].TS.FontName,
                        ffontSize,
                        stateList[indexState].TS.Bold,
                        stateList[indexState].TS.Italic,
                        stateList[indexState].TS.Underline,
                        stateList[indexState].TS.Strikeout,
                        stateList[indexState].TS.Superscript || stateList[indexState].TS.Subscript,
                        tempFontList);
                }

                fontList = new StiFontState[tempFontList.Count];
                for (var indexFont = 0; indexFont < fontList.Length; indexFont++)
                {
                    fontList[indexFont] = tempFontList[indexFont];

                    ushort[] glyphs = (ushort[])GetFontGlyphs(tempFontList[indexFont].FontBase);
                    if ((glyphs.Length > 160) && (glyphs[160] == 0xffff))
                    {
                        fontList[indexFont].HasNbspGlyf = false;
                    }
                }
#endregion

                for (var indexFont = 0; indexFont < fontList.Length; indexFont++)
                {
                    fontList[indexFont].FontScaled = null;
                    fontList[indexFont].hFontScaled = IntPtr.Zero;
                    fontList[indexFont].hScriptCache = IntPtr.Zero;
                    fontList[indexFont].hScriptCacheScaled = IntPtr.Zero;
                    if ((scale != 1 || PrecisionModeEnabled) && needDraw)
                    {
                        fontList[indexFont].FontScaled = new Font(
                            StiFontCollection.GetFontFamily(fontList[indexFont].FontBase.FontFamily.Name),
                            (float)(fontList[indexFont].FontBase.Size * scale),
                            fontList[indexFont].FontBase.Style,
                            fontList[indexFont].FontBase.Unit,
                            fontList[indexFont].FontBase.GdiCharSet,
                            fontList[indexFont].FontBase.GdiVerticalFont);
                        fontList[indexFont].hFontScaled = fontList[indexFont].FontScaled.ToHfont();
                    }
                }

                try
                {
                    var hdc = g.GetHdc();
                    try
                    {
#region get OUTLINETEXTMETRIC - calculate LineHeight
                        for (var indexFont = 0; indexFont < fontList.Length; indexFont++)
                        {
                            var otm = GetOutlineTextMetricsCached(fontList[indexFont].FontName, fontList[indexFont].FontBase.Style, hdc);

                            var lineHeightTextMetric = (otm.otmTextMetrics.tmAscent + otm.otmTextMetrics.tmDescent + otm.otmTextMetrics.tmExternalLeading) / MaxFontSize * fontList[indexFont].FontBase.SizeInPoints;
                            var ascend = otm.otmTextMetrics.tmAscent / MaxFontSize * fontList[indexFont].FontBase.SizeInPoints * scale;
                            var descend = otm.otmTextMetrics.tmDescent / MaxFontSize * fontList[indexFont].FontBase.SizeInPoints * scale;
                            var typoOffset = 0d;
                            if ((otm.otmfsSelection & 0x80) > 0)    //USE_TYPO_METRICS
                            {
                                typoOffset = ascend;
                                lineHeightTextMetric = (otm.otmAscent - otm.otmDescent + otm.otmLineGap) / MaxFontSize * fontList[indexFont].FontBase.SizeInPoints;
                                ascend = otm.otmAscent / MaxFontSize * fontList[indexFont].FontBase.SizeInPoints * scale;
                                descend = -otm.otmDescent / MaxFontSize * fontList[indexFont].FontBase.SizeInPoints * scale;
                                typoOffset = ascend - typoOffset;
                            }
                            var lineHeight = lineHeightTextMetric * scale;
                            var lineHeightWithSpacing = lineHeight * lineSpacing;
                            var emValue = fontList[indexFont].FontBase.SizeInPoints;
                            fontList[indexFont].LineHeight = lineHeight;
                            fontList[indexFont].Ascend = ascend;
                            fontList[indexFont].Descend = descend;
                            fontList[indexFont].EmValue = emValue;
                            fontList[indexFont].TypoOffset = typoOffset;
                        }
#endregion

                        for (var indexFont = 0; indexFont < fontList.Length; indexFont++)
                        {
                            fontList[indexFont].hFont = fontList[indexFont].FontBase.ToHfont();
                        }

                        bool gdiError;
                        var oldGraphMode = 0;
                        var oldMapMode = 0;
                        var oldXForm = new XFORM(1, 0, 0, 1, 0, 0);

                        try
                        {
#region textSize correction
                            if (((angle > 45) && (angle < 135)) || ((angle > 225) && (angle < 315)))
                            {
                                var tempValue = textSize.Width;
                                textSize.Width = textSize.Height;
                                textSize.Height = tempValue;
                            }

                            var angleRad = -angle * Math.PI / 180;
#endregion

                            var oldRegion = IntPtr.Zero;
                            var newRegion = IntPtr.Zero;
                            var resultGetClip = 0;

                            if (needDraw)
                            {
                                oldGraphMode = SetGraphicsMode(hdc, GM_ADVANCED);
                                if (oldGraphMode == 0) ThrowError(3);

                                if (Compatibility2009)
                                {
#region set world transformation
                                    float scaleX = 1;
                                    float scaleY = 1;
                                    if ((gUnit != GraphicsUnit.Display) && (gUnit != GraphicsUnit.Pixel))
                                    {
#region scale for print
                                        oldMapMode = SetMapMode(hdc, MM_ANISOTROPIC);
                                        if (oldMapMode == 0) ThrowError(4);

                                        var maxX = pageF.Width + (pageF.X > 0 ? pageF.X : 0);
                                        var maxY = pageF.Height + (pageF.Y > 0 ? pageF.Y : 0);

                                        scaleX = dpiX / 100f;
                                        scaleY = dpiY / 100f;

                                        var maxXP = maxX * scaleX;
                                        var maxYP = maxY * scaleY;

                                        var windowSize = new SIZE();
                                        gdiError = SetWindowExtEx(
                                            hdc,
                                            (int)maxX,
                                            (int)maxY,
                                            out windowSize);
                                        if (!gdiError) ThrowError(5);

                                        var viewportSize = new SIZE();
                                        gdiError = SetViewportExtEx(
                                            hdc,
                                            (int)maxXP,
                                            (int)maxYP,
                                            out viewportSize);
                                        if (!gdiError) ThrowError(6);
#endregion
                                    }

                                    gdiError = GetWorldTransform(hdc, out oldXForm);
                                    if (!gdiError) ThrowError(7);

                                    //translate dx, dy
                                    var newXForm1 = new XFORM(1, 0, 0, 1,
                                        textRect.Left + textRect.Width / 2f,
                                        textRect.Top + textRect.Height / 2f);
                                    gdiError = ModifyWorldTransform(hdc, ref newXForm1, MWT_LEFTMULTIPLY);
                                    if (!gdiError) ThrowError(8);

                                    //rotate
                                    var newXForm2 = new XFORM(
                                        Math.Cos(angleRad),
                                        Math.Sin(angleRad),
                                        -Math.Sin(angleRad),
                                        Math.Cos(angleRad),
                                        0, 0);
                                    gdiError = ModifyWorldTransform(hdc, ref newXForm2, MWT_LEFTMULTIPLY);
                                    if (!gdiError) ThrowError(9);

                                    //translate dx, dy
                                    var newXForm3 = new XFORM(1, 0, 0, 1,
                                        -textSize.Width / 2f,
                                        -textSize.Height / 2f);
                                    gdiError = ModifyWorldTransform(hdc, ref newXForm3, MWT_LEFTMULTIPLY);
                                    if (!gdiError) ThrowError(10);
#endregion

#region set drawing parameters
                                    if (!foreColor.IsEmpty)
                                    {
                                        SetTextColor(hdc, ColorToWin32(foreColor));
                                    }

                                    var newMode1 = (backColor.IsEmpty || (backColor == Color.Transparent)) ? DeviceContextBackgroundMode.Transparent : DeviceContextBackgroundMode.Opaque;
                                    SetBkMode(hdc, (int)newMode1);
                                    if (newMode1 != DeviceContextBackgroundMode.Transparent)
                                    {
                                        SetBkColor(hdc, ColorToWin32(backColor));
                                    }

                                    resultGetClip = GetClipRgn(hdc, oldRegion);
                                    newRegion = CreateRectRgn(
                                        (int)(regionRect.Left * scaleX),
                                        (int)(regionRect.Top * scaleY),
                                        (int)(regionRect.Right * scaleX),
                                        (int)(regionRect.Bottom * scaleY));
                                    SelectClipRgn(hdc, newRegion);
#endregion
                                }
                            }

                            SelectObject(hdc, fontList[0].hFont);
                            var lpRect = new RECT(textRect.ToRectangle());

#region prepare text
                            var linesInfo = new List<LineInfo>();
                            //IntPtr scriptCache = IntPtr.Zero;

#region get lines info
                            var posCurrent = 0;
                            while (posCurrent < text.Length)
                            {
                                var pair = new LineInfo();
                                pair.Begin = posCurrent;
                                //find end line
                                while ((posCurrent < text.Length) && (text[posCurrent] != '\r') && (text[posCurrent] != '\n'))
                                {
                                    posCurrent++;
                                }

                                pair.End = posCurrent;
                                //trim spaces right
                                while ((pair.End > pair.Begin + 1) && (Char.IsWhiteSpace(text[pair.End - 1])))
                                {
                                    pair.End--;
                                }

                                linesInfo.Add(pair);
                                //find next line
                                posCurrent++;
                                if ((posCurrent < text.Length) &&
                                    ((text[posCurrent] == '\r') || (text[posCurrent] == '\n')) &&
                                    (text[posCurrent - 1] != text[posCurrent]))
                                {
                                    posCurrent++;
                                }
                            }
#endregion

                            if (stateList.Length > 0 && stateList[0].TS.Indent < 0 && linesInfo.Count > 0)
                            {
                                linesInfo[0].Indent = -stateList[0].TS.Indent;
                                if (stateList[0].ListLevels != null)
                                {
                                    stateList[0].TS.Indent = stateList[0].ListLevels.Count;
                                }
                            }

#region calculate line height, ellipsis and tabs
                            var lpRectForCalculate = new RECT(0, 0, Int32.MaxValue, Int32.MaxValue);
                            var flagsForCalculate = TextFormatFlags.Default | TextFormatFlags.CalculateRectangle;
                            DrawTextExW(hdc, "…", 1, ref lpRectForCalculate, (int)flagsForCalculate, new DRAWTEXTPARAMS());
                            var ellipsisWidth = (int)(lpRectForCalculate.Size.Width * scale);

                            double tabSpaceWidth = 1;
#endregion

                            // !!! переделать все fontList[0].LineHeight

#region LineLimit; for HtmlTags works incorrectly
                            var linesCountCalcDouble = (textSize.Height - fontList[0].LineHeight) / (fontList[0].LineHeight * lineSpacing) + 1;
                            var linesCountCalc = (int)linesCountCalcDouble;
                            if (!lineLimit)
                            {
                                if (allowHtmlTags)
                                {
                                    linesCountCalc = linesInfo.Count;
                                }
                                else
                                {
                                    linesCountCalc++;
                                }
                            }

                            var linesCountLimit = linesCountCalc;
#endregion

#region wordwrap and line length calculation
                            {
                                var wordWrapPoints = new List<LineInfo>();
                                var lastLineIndex = 0;
                                for (var indexLine = 0; indexLine < linesInfo.Count; indexLine++)
                                {
                                    var textLinePair = linesInfo[indexLine];
                                    var textLine = text.Substring(textLinePair.Begin, textLinePair.Length);
                                    var textLen = textLine.Length;

                                    var indentIndex = stateList[stateOrder[textLinePair.Begin]].TS.Indent;
                                    var indentCount = indentIndex * 10;
                                    var indentCalcSize = 0;
                                    var indentSize = 0;
                                    var isIndent = indentCount > 0;
                                    var isWrapped = false;
                                    if (textLinePair.Indent > 0)
                                    {
                                        indentSize = textLinePair.Indent;
                                        indentCalcSize = textLinePair.Indent;
                                        isWrapped = true;
                                    }

                                    if (textLen == 0)
                                    {
                                        var pairWrapPoint = new LineInfo();
                                        pairWrapPoint.Begin += textLinePair.Begin;
                                        wordWrapPoints.Add(pairWrapPoint);
                                        lastLineIndex = wordWrapPoints.Count;
                                        continue;
                                    }

                                    if (textLen > 0)
                                    {
#region scan for bracket and etc..
                                        //scan for bracket
                                        var nowrapList = new int[textLen]; //1 - nowrap, 2 - nowrapBegin
                                        //bool existNowrapPoints = false;
                                        var posInText = 0;
                                        while (posInText < textLen)
                                        {
                                            //find first bracket in string
                                            while (posInText < textLen)
                                            {
                                                var cat = char.GetUnicodeCategory(textLine[posInText]);
                                                if (((cat == UnicodeCategory.OpenPunctuation) || (cat == UnicodeCategory.ClosePunctuation) ||
                                                    (cat == UnicodeCategory.InitialQuotePunctuation) || (cat == UnicodeCategory.FinalQuotePunctuation)) && !(textLine[posInText] == 0x2019))
                                                    break;
                                                posInText++;
                                            }

                                            if (posInText < textLen) //bracket founded
                                            {
                                                var posBegin = posInText;
                                                var posEnd = posInText;
                                                if (char.GetUnicodeCategory(textLine[posInText]) == UnicodeCategory.OpenPunctuation ||
                                                    char.GetUnicodeCategory(textLine[posInText]) == UnicodeCategory.InitialQuotePunctuation)
                                                {
#region OpenPunctuation - Word - ClosePunctuation
                                                    //scan left part
                                                    while (posInText < textLen)
                                                    {
                                                        var cat = char.GetUnicodeCategory(textLine[posInText]);
                                                        if (!((cat == UnicodeCategory.OpenPunctuation) || (cat == UnicodeCategory.InitialQuotePunctuation) || (cat == UnicodeCategory.SpaceSeparator)))
                                                            break;
                                                        posInText++;
                                                    }

                                                    //scan center part
                                                    if ((posInText < textLen) &&
                                                        (char.GetUnicodeCategory(textLine[posInText]) != UnicodeCategory.ClosePunctuation) &&
                                                        (char.GetUnicodeCategory(textLine[posInText]) != UnicodeCategory.FinalQuotePunctuation))
                                                    {
                                                        //skip one word and spaces after him
                                                        while (posInText < textLen)
                                                        {
                                                            var cat = char.GetUnicodeCategory(textLine[posInText]);
                                                            if (cat == UnicodeCategory.SpaceSeparator || isWordWrapSymbol2(textLine, posInText) || isCJKWordWrap(textLine, posInText))
                                                                break;
                                                            posInText++;
                                                        }

                                                        while (posInText < textLen)
                                                        {
                                                            var cat = char.GetUnicodeCategory(textLine[posInText]);
                                                            if (cat != UnicodeCategory.SpaceSeparator)
                                                                break;
                                                            posInText++;
                                                        }

                                                        if ((posInText < textLen) && (char.GetUnicodeCategory(textLine[posInText]) == UnicodeCategory.DashPunctuation)) posInText++;
                                                    }

                                                    //scan right part
                                                    if ((posInText < textLen) &&
                                                        ((char.GetUnicodeCategory(textLine[posInText]) == UnicodeCategory.ClosePunctuation) ||
                                                         (char.GetUnicodeCategory(textLine[posInText]) == UnicodeCategory.FinalQuotePunctuation)))
                                                    {
                                                        while (posInText < textLen)
                                                        {
                                                            var cat = char.GetUnicodeCategory(textLine[posInText]);
                                                            if (!((cat == UnicodeCategory.ClosePunctuation) || (cat == UnicodeCategory.FinalQuotePunctuation) ||
                                                                  (cat == UnicodeCategory.OtherPunctuation) || (cat == UnicodeCategory.SpaceSeparator)))
                                                                break;
                                                            posInText++;
                                                        }
                                                    }

                                                    posEnd = posInText;
#endregion
                                                }
                                                else
                                                {
#region Word - ClosePunctuation
                                                    //scan center part - search one word before bracket
                                                    posInText--;
                                                    //skip SpaceSeparators
                                                    while (posInText >= 0)
                                                    {
                                                        var cat = char.GetUnicodeCategory(textLine[posInText]);
                                                        if (cat != UnicodeCategory.SpaceSeparator)
                                                            break;
                                                        posInText--;
                                                    }

                                                    //skip one word
                                                    while (posInText >= 0)
                                                    {
                                                        var cat = char.GetUnicodeCategory(textLine[posInText]);
                                                        if (cat == UnicodeCategory.SpaceSeparator || nowrapList[posInText] != 0 || isCJKWordWrap(textLine, posInText))
                                                            break;
                                                        posInText--;
                                                    }

                                                    posBegin = posInText + (isCJKWordWrap(textLine, posInText) ? 0 : 1);

                                                    //scan right part
                                                    posInText = posEnd;
                                                    while (posInText < textLen)
                                                    {
                                                        var cat = char.GetUnicodeCategory(textLine[posInText]);
                                                        if (!((cat == UnicodeCategory.ClosePunctuation) || (cat == UnicodeCategory.FinalQuotePunctuation) ||
                                                              (cat == UnicodeCategory.OtherPunctuation) || (cat == UnicodeCategory.SpaceSeparator)))
                                                            break;
                                                        posInText++;
                                                    }

                                                    posEnd = posInText;
#endregion
                                                }

                                                //remove spaces at end of wrap string
                                                while ((posEnd > posBegin) && (char.GetUnicodeCategory(textLine[posEnd - 1]) == UnicodeCategory.SpaceSeparator))
                                                {
                                                    posEnd--;
                                                }

                                                //existNowrapPoints = true;
                                                nowrapList[posBegin] = 2;
                                                for (var indexBr = posBegin + 1; indexBr < posEnd; indexBr++)
                                                {
                                                    nowrapList[indexBr] = 1;
                                                }
                                            }
                                        }

                                        //scan for NonBreakingSpace
                                        var flag2 = false;
                                        for (var indexChar = 0; indexChar < textLen; indexChar++)
                                        {
                                            if (textLine[indexChar] == '\x2011' || textLine[indexChar] == '\xA0')
                                            {
                                                flag2 = true;
                                                nowrapList[indexChar] = 1;
                                                var indexCh2 = indexChar;
                                                var flag3 = true;
                                                while (indexCh2 > 0 && isNotWordWrapSymbol2(textLine, indexCh2 - 1))
                                                {
                                                    indexCh2--;
                                                    if (nowrapList[indexCh2] == 0)
                                                    {
                                                        nowrapList[indexCh2] = 1;
                                                    }
                                                    else
                                                    {
                                                        flag3 = false;
                                                        break;
                                                    }
                                                }

                                                if (flag3) nowrapList[indexCh2] = 2;
                                                indexCh2 = indexChar;
                                                while (indexCh2 + 1 < textLen && (isNotWordWrapSymbol2(textLine, indexCh2 + 1) || textLine[indexCh2 + 1] == '\x2011' || textLine[indexCh2 + 1] == '\xA0'))
                                                {
                                                    indexCh2++;
                                                    nowrapList[indexCh2] = 1;
                                                }

                                                indexChar = indexCh2;
                                            }
                                        }

                                        if (flag2)
                                        {
                                            textLine = textLine.Replace('\x2011', '-');
                                        }

                                        //scan for OtherPunctuation, MathSymbol, CurrencySymbol
                                        for (var indexChar = 0; indexChar < textLen; indexChar++)
                                        {
                                            if (isNotWordWrapSymbol(textLine, indexChar))
                                            {
                                                if (indexChar > 0 && isNotWordWrapSymbol2(textLine, indexChar - 1))
                                                {
                                                    nowrapList[indexChar] = 1;

                                                    //mark symbols after this position
                                                    var indexCh2 = indexChar;
                                                    if (!isWordWrapSymbol2(textLine, indexCh2) && !isCJKSymbol(textLine, indexCh2))
                                                    {
                                                        indexCh2++;
                                                        while (indexCh2 < textLine.Length && char.IsLetterOrDigit(textLine[indexCh2]) && !isCJKSymbol(textLine, indexCh2))
                                                        {
                                                            if (nowrapList[indexCh2] == 0)
                                                            {
                                                                nowrapList[indexCh2] = 1;
                                                            }
                                                            else
                                                            {
                                                                break;
                                                            }

                                                            indexCh2++;
                                                        }
                                                    }

                                                    indexCh2 = indexChar;
                                                    var flag3 = false;
                                                    var flag4 = true;
                                                    while (indexCh2 > 0 && isNotWordWrapSymbol2(textLine, indexCh2 - 1) && !isWordWrapSymbol2(textLine, indexCh2 - 1) && flag4)
                                                    {
                                                        flag3 = true;
                                                        indexCh2--;
                                                        if (nowrapList[indexCh2] == 0)
                                                        {
                                                            nowrapList[indexCh2] = 1;
                                                        }
                                                        else
                                                        {
                                                            flag3 = false;
                                                            break;
                                                        }

                                                        flag4 = !isCJKSymbol(textLine, indexCh2);
                                                    }

                                                    if (flag3) nowrapList[indexCh2] = 2;
                                                }
                                                else if ((indexChar < textLine.Length - 1) && isNotWordWrapSymbol2(textLine, indexChar + 1))
                                                {
                                                    var flag3 = nowrapList[indexChar] == 0;

                                                    //mark symbols after this position
                                                    var indexCh2 = indexChar;
                                                    if (!isWordWrapSymbol2(textLine, indexCh2))
                                                    {
                                                        indexCh2++;
                                                        while (indexCh2 < textLine.Length && char.IsLetterOrDigit(textLine[indexCh2]))
                                                        {
                                                            if (nowrapList[indexCh2] == 0)
                                                            {
                                                                nowrapList[indexCh2] = 1;
                                                            }
                                                            else
                                                            {
                                                                break;
                                                            }

                                                            indexCh2++;
                                                        }
                                                    }

                                                    if (flag3) nowrapList[indexChar] = 2;
                                                }
                                            }
                                        }
#endregion

                                        var lineWidths = new int[textLen];

#region ScriptItemize
                                        SCRIPT_ITEM[] scriptItemList = null;
                                        var scriptItemCount = 0;
                                        var error = 0;
                                        var cMaxItems = 10;
                                        do
                                        {
                                            cMaxItems *= 10;
                                            var psControl = new SCRIPT_CONTROL();
                                            var psState = new SCRIPT_STATE();
                                            //scriptItemList = new SCRIPT_ITEM[cMaxItems];
                                            psState.uBidiLevel = (ushort)(rightToLeft ? 1 : 0);

                                            var buf = Marshal.AllocHGlobal(sizeofScriptItem * (cMaxItems + 1) + 2);

                                            error = ScriptItemize(
                                                textLine,
                                                textLine.Length,
                                                cMaxItems,
                                                ref psControl,
                                                ref psState,
                                                //ref scriptItemList[0],
                                                buf,
                                                out scriptItemCount);
                                            if ((error != 0) && (error != E_OUTOFMEMORY))
                                            {
                                                Marshal.FreeHGlobal(buf);
                                                ThrowError(11, error);
                                            }

                                            if (error == 0)
                                            {
                                                scriptItemList = new SCRIPT_ITEM[scriptItemCount + 1];
                                                var offset = buf;
                                                for (var indexItem = 0; indexItem < scriptItemCount + 1; indexItem++)
                                                {
                                                    scriptItemList[indexItem] = (SCRIPT_ITEM)Marshal.PtrToStructure(offset, typeof(SCRIPT_ITEM));
                                                    offset = (IntPtr)((Int64)offset + sizeofScriptItem);
                                                }
                                            }

                                            Marshal.FreeHGlobal(buf);
                                        } while (error == E_OUTOFMEMORY);
#endregion

#region Check for: '-' at end of word, break between letter and digit
                                        var tempList = new List<SCRIPT_ITEM>(scriptItemList);
                                        for (var indexScriptItem = scriptItemList.Length - 2; indexScriptItem > 0; indexScriptItem--)
                                        {
                                            var posChar = scriptItemList[indexScriptItem].iCharPos;
                                            if ((posChar == scriptItemList[indexScriptItem + 1].iCharPos - 1) && (textLine[posChar] == '-') && char.IsLetter(textLine, posChar - 1) ||
                                                char.IsDigit(textLine, posChar) && char.IsLetter(textLine, posChar - 1))
                                            {
                                                tempList.RemoveAt(indexScriptItem);
                                            }
                                        }

                                        //fix for U+2019, later need to rewrite wordwrap logic
                                        for (var indexScriptItem = tempList.Count - 3; indexScriptItem > 0; indexScriptItem--)
                                        {
                                            var posChar = tempList[indexScriptItem].iCharPos;
                                            if ((posChar == tempList[indexScriptItem + 1].iCharPos - 1) && (textLine[posChar] == 0x2019) && char.IsLetter(textLine, posChar - 1) && char.IsLetter(textLine, posChar + 1))
                                            {
                                                tempList.RemoveAt(indexScriptItem);
                                                tempList.RemoveAt(indexScriptItem);
                                            }
                                        }

                                        if (tempList.Count < scriptItemList.Length)
                                        {
                                            scriptItemList = new SCRIPT_ITEM[tempList.Count];
                                            for (var indexItem = 0; indexItem < tempList.Count; indexItem++)
                                            {
                                                scriptItemList[indexItem] = tempList[indexItem];
                                            }

                                            scriptItemCount = scriptItemList.Length - 1;
                                        }

                                        tempList.Clear();
#endregion

#region break runs in nowrapBegin points
                                        //if (existNowrapPoints)
                                        {
                                            var newScriptItemList = new List<SCRIPT_ITEM>();
                                            newScriptItemList.Add(scriptItemList[0]);
                                            var itemIndex = 0;
                                            for (var indexChar = 0; indexChar < textLen; indexChar++)
                                            {
                                                if (indexChar == scriptItemList[itemIndex + 1].iCharPos)
                                                {
                                                    itemIndex++;
                                                    newScriptItemList.Add(scriptItemList[itemIndex]);
                                                    continue;
                                                }

                                                if ((nowrapList[indexChar] == 2) && (indexChar != 0))
                                                {
                                                    var si = scriptItemList[itemIndex];
                                                    si.iCharPos = indexChar;
                                                    newScriptItemList.Add(si);
                                                    continue;
                                                }

                                                if ((indexChar > 0) && (stateOrder[textLinePair.Begin + indexChar] != stateOrder[textLinePair.Begin + indexChar - 1]))
                                                {
                                                    var si = scriptItemList[itemIndex];
                                                    si.iCharPos = indexChar;
                                                    newScriptItemList.Add(si);
                                                    continue;
                                                }
                                            }

                                            newScriptItemList.Add(scriptItemList[scriptItemCount]);
                                            scriptItemList = new SCRIPT_ITEM[newScriptItemList.Count];
                                            for (var indexRun = 0; indexRun < newScriptItemList.Count; indexRun++)
                                            {
                                                scriptItemList[indexRun] = newScriptItemList[indexRun];
                                            }

                                            scriptItemCount = newScriptItemList.Count - 1;
                                        }
#endregion

                                        var bufLen = textLen * 2;
                                        if (bufLen < 20) bufLen = 20;
                                        var sumLen = 0;
                                        var lastLineOffset = 0;

                                        var nowrapLastRunIndex = 0;
                                        var nowrapLastUsedRun = 0;

                                        var softHypenWidth = -1;
                                        double remainder = 0;

                                        // process each "run" of text 
                                        for (var indexRun = 0; indexRun < scriptItemCount; indexRun++)
                                        {
#region process run
                                            var glyphIndexList = new ushort[bufLen];
                                            var glyphClusterList = new ushort[textLen];
                                            //SCRIPT_VISATTR[] scriptVisAttrList = new SCRIPT_VISATTR[bufLen];
                                            var scriptVisAttrList = Marshal.AllocHGlobal(sizeofScriptVisattr * bufLen);
                                            var advanceWidthList = new int[bufLen];
                                            ABC abc;
                                            int glyphIndexCount;

                                            var runCharPos = scriptItemList[indexRun].iCharPos;
                                            var runCharLen = scriptItemList[indexRun + 1].iCharPos - runCharPos;
                                            var runText = textLine.Substring(runCharPos, runCharLen);

                                            currentStateIndex = stateOrder[textLinePair.Begin + runCharPos];
                                            var currentFontState = fontList[stateList[currentStateIndex].FontIndex];

                                            SelectObject(hdc, currentFontState.hFont);

                                            if ((nowrapList[runCharPos] == 2) && (indexRun != nowrapLastUsedRun))
                                            {
                                                nowrapLastRunIndex = indexRun;
                                            }

#region ScriptShape
                                            error = ScriptShape(
                                                hdc,
                                                ref currentFontState.hScriptCache,
                                                runText,
                                                runCharLen,
                                                bufLen,
                                                ref scriptItemList[indexRun].a,
                                                glyphIndexList,
                                                glyphClusterList,
                                                //ref scriptVisAttrList[0],
                                                scriptVisAttrList,
                                                out glyphIndexCount
                                            );
                                            if (error == E_SCRIPT_NOT_IN_FONT) //fix
                                            {
                                                error = 0;
                                                glyphIndexCount = runCharLen;
                                                for (var tIndex = 0; tIndex < glyphIndexCount; tIndex++)
                                                {
                                                    glyphClusterList[tIndex] = (ushort)tIndex;
                                                }

                                                scriptItemList[indexRun].a.packed = 0;
                                            }

                                            if ((error != 0) && (error != E_SCRIPT_NOT_IN_FONT))
                                            {
                                                Marshal.FreeHGlobal(scriptVisAttrList);
                                                ThrowError(12, error);
                                            }
#endregion

                                            var goff = Marshal.AllocHGlobal(sizeofGoffset * glyphIndexCount);

#region ScriptPlace
                                            error = ScriptPlace(
                                                hdc,
                                                ref currentFontState.hScriptCache,
                                                glyphIndexList,
                                                glyphIndexCount,
                                                //ref scriptVisAttrList[0],
                                                scriptVisAttrList,
                                                ref scriptItemList[indexRun].a,
                                                advanceWidthList,
                                                //ref goff[0],
                                                goff,
                                                out abc
                                            );


                                            //todo 
                                            //var bufff = new byte[glyphIndexCount * sizeofGoffset];
                                            //Marshal.Copy(goff, bufff, 0, glyphIndexCount * sizeofGoffset);


                                            Marshal.FreeHGlobal(goff);
                                            if (error != 0)
                                            {
                                                Marshal.FreeHGlobal(scriptVisAttrList);
                                                ThrowError(13, error);
                                            }
#endregion

                                            Marshal.FreeHGlobal(scriptVisAttrList);

                                            if (CorrectionEnabled && !Compatibility2009)
                                            {
#region AdvanceWidthList correction
                                                var allGlyphWidth = GetFontWidth(currentFontState);
                                                if (allGlyphWidth.Length > 0)
                                                {
                                                    if (PrecisionMode3Enabled)
                                                    {
                                                        var fontScale = MaxFontSize / currentFontState.EmValue;
                                                        double sumD = remainder;
                                                        int sumI = 0;
                                                        for (var indexGlyph = 0; indexGlyph < glyphIndexCount; indexGlyph++)
                                                        {
                                                            if (advanceWidthList[indexGlyph] == 0) continue;
                                                            int tempGlyphIndex = glyphIndexList[indexGlyph];
                                                            if (tempGlyphIndex >= allGlyphWidth.Length) tempGlyphIndex = allGlyphWidth.Length - 1;
                                                            sumD += allGlyphWidth[tempGlyphIndex] / fontScale;
                                                            int width = (int)(sumD - sumI);
                                                            sumI += width;
                                                            advanceWidthList[indexGlyph] = width;
                                                        }
                                                        if (((sumD - sumI) > 0.5) && (indexRun < scriptItemCount - 1))
                                                        {
                                                            advanceWidthList[glyphIndexCount - 1]++;
                                                            sumI++;
                                                        }
                                                        remainder = sumD - sumI;
                                                    }
                                                    else
                                                    if (PrecisionMode2Enabled)
                                                    {
                                                        var fontScale = MaxFontSize / currentFontState.EmValue;
                                                        double sumD = remainder;
                                                        int sumI = 0;
                                                        for (var indexGlyph = 0; indexGlyph < glyphIndexCount; indexGlyph++)
                                                        {
                                                            if (advanceWidthList[indexGlyph] == 0) continue;
                                                            int tempGlyphIndex = glyphIndexList[indexGlyph];
                                                            if (tempGlyphIndex >= allGlyphWidth.Length) tempGlyphIndex = allGlyphWidth.Length - 1;
                                                            sumD += allGlyphWidth[tempGlyphIndex] / fontScale;
                                                            int width = (int)(sumD - sumI);
                                                            sumI += width;
                                                            advanceWidthList[indexGlyph] = width;
                                                        }
                                                        if ((sumD - sumI) > 0.5)
                                                        {
                                                            advanceWidthList[glyphIndexCount - 1]++;
                                                            sumI++;
                                                        }
                                                        remainder = sumD - sumI;
                                                    }
                                                    else
                                                    {
                                                        //double summm1 = 0;
                                                        //for (int indexGlyph = 0; indexGlyph < glyphIndexCount; indexGlyph++)
                                                        //{
                                                        //    summm1 += advanceWidthList[indexGlyph];
                                                        //}
                                                        //double summm2 = 0;
                                                        var fontScale = MaxFontSize / currentFontState.EmValue;
                                                        for (var indexGlyph = 0; indexGlyph < glyphIndexCount; indexGlyph++)
                                                        {
                                                            if (advanceWidthList[indexGlyph] == 0) continue;
                                                            int tempGlyphIndex = glyphIndexList[indexGlyph];
                                                            if (tempGlyphIndex >= allGlyphWidth.Length) tempGlyphIndex = allGlyphWidth.Length - 1;
                                                            var width = allGlyphWidth[tempGlyphIndex] / fontScale;
                                                            if (advanceWidthList[indexGlyph] < width - 0.4)
                                                            {
                                                                var newWidth = (int)Math.Round(width);
                                                                if (advanceWidthList[indexGlyph] >= newWidth) newWidth++;
                                                                advanceWidthList[indexGlyph] = newWidth;
                                                            }
                                                            else
                                                            {
                                                                var percent = advanceWidthList[indexGlyph] / width;
                                                                if (advanceWidthList[indexGlyph] - width > 1 && percent > 1.1)
                                                                {
                                                                    advanceWidthList[indexGlyph] = (int)Math.Ceiling(width);
                                                                }
                                                            }

                                                            //summm2 += advanceWidthList[indexGlyph];
                                                        }
                                                    }
                                                }
#endregion
                                            }

#region Apply LetterSpacing and WordSpacing
                                            if (stateList[currentStateIndex].TS.LetterSpacing != 0)
                                            {
                                                //double addedLetterSpacing = currentFontState.EmValue * stateList[currentStateIndex].TS.LetterSpacing;
                                                var addedLetterSpacing = currentFontState.EmValue * 1.35 * stateList[currentStateIndex].TS.LetterSpacing;
                                                var ssumWidthInt = 0;
                                                double ssumWidthDouble = 0;
                                                for (var indexGlyph = 0; indexGlyph < glyphIndexCount; indexGlyph++)
                                                {
                                                    ssumWidthDouble += advanceWidthList[indexGlyph] + addedLetterSpacing;
                                                    var glyphWidthScaled = (int)ssumWidthDouble - ssumWidthInt;
                                                    ssumWidthInt += glyphWidthScaled;
                                                    advanceWidthList[indexGlyph] = glyphWidthScaled;
                                                }
                                            }

                                            if (stateList[currentStateIndex].TS.WordSpacing != 0)
                                            {
                                                var addedWordSpacing = currentFontState.EmValue * stateList[currentStateIndex].TS.WordSpacing;
                                                var ssumWidthInt = 0;
                                                double ssumWidthDouble = 0;
                                                for (var indexChar = 0; indexChar < runCharLen; indexChar++)
                                                {
                                                    if (char.IsWhiteSpace(runText[indexChar]))
                                                    {
                                                        ssumWidthDouble += advanceWidthList[glyphClusterList[indexChar]] + addedWordSpacing;
                                                        var glyphWidthScaled = (int)ssumWidthDouble - ssumWidthInt;
                                                        ssumWidthInt += glyphWidthScaled;
                                                        advanceWidthList[glyphClusterList[indexChar]] += glyphWidthScaled;
                                                    }
                                                }
                                            }
#endregion

#region calculate chars widths
                                            //int runLength = (int)(abc.abcA + abc.abcB + abc.abcC);
                                            var runLength = 0;
                                            for (var indexChar = 0; indexChar < runCharLen; indexChar++)
                                            {
                                                if (runText[indexChar] == '\xAD') //soft hypen
                                                {
                                                    softHypenWidth = advanceWidthList[indexChar];
                                                    advanceWidthList[indexChar] = 0;
                                                }

                                                var currentScriptLen = 0;
                                                if (runText[indexChar] == '\t')
                                                {
                                                    currentScriptLen = GetTabsWidth(textOptions, tabSpaceWidth, sumLen + runLength);
                                                }
                                                else
                                                {
                                                    int clusterNumber = glyphClusterList[indexChar]; //index of first glyph
                                                    //find last char in cluster
                                                    while ((indexChar < runCharLen - 1) && (glyphClusterList[indexChar + 1] == clusterNumber))
                                                    {
                                                        indexChar++;
                                                    }

                                                    var indexGlyphBegin = clusterNumber;
                                                    var indexGlyphEnd = clusterNumber;
                                                    if (scriptItemList[indexRun].a.fRTL)
                                                    {
                                                        indexGlyphBegin = (indexChar + 1 < runCharLen ? glyphClusterList[indexChar + 1] + 1 : 0);
                                                        indexGlyphEnd++;
                                                    }
                                                    else
                                                    {
                                                        indexGlyphEnd = (indexChar + 1 < runCharLen ? glyphClusterList[indexChar + 1] : glyphIndexCount);
                                                    }

                                                    for (var indexGlyph = indexGlyphBegin; indexGlyph < indexGlyphEnd; indexGlyph++)
                                                    {
                                                        currentScriptLen += advanceWidthList[indexGlyph];
                                                    }
                                                }

                                                runLength += currentScriptLen;
                                                lineWidths[runCharPos + indexChar] = currentScriptLen;
                                                if (isIndent && runCharPos + indexChar + 1 == indentCount && !isWrapped)
                                                {
                                                    indentCalcSize = sumLen + runLength;
                                                }
                                            }
#endregion

#region calculate boundsWidth
                                            double boundsWidth = Int32.MaxValue;
                                            var needTrim = false;
                                            if (trimming != StringTrimming.None)
                                            {
                                                boundsWidth = textSize.Width;
                                                if ((!wordWrap) || (wordWrapPoints.Count + 1 == linesCountLimit))
                                                {
                                                    needTrim = true;
                                                }

                                                if ((trimming == StringTrimming.EllipsisCharacter) || (trimming == StringTrimming.EllipsisWord))
                                                {
                                                    if (needTrim) boundsWidth = textSize.Width - ellipsisWidth;
                                                }
                                            }
                                            else
                                            {
                                                if (wordWrap) boundsWidth = textSize.Width - indentSize; // - stateList[currentStateIndex].TS.Indent * 40;
                                            }
#endregion

                                            if (Math.Round((decimal)((sumLen + runLength) * scale), precisionDigits) <= (decimal)boundsWidth)
                                            {
                                                //run fully in rect
                                                sumLen += runLength;
                                                //lineWidths[runCharPos] = runLength;
                                            }
                                            else
                                            {
#region find wordWrap point
#region ScriptBreak
                                                //SCRIPT_LOGATTR[] logAttrList = new SCRIPT_LOGATTR[runCharLen];

                                                var ptrLogAttrList = Marshal.AllocHGlobal(runCharLen + 2);

                                                error = ScriptBreak(
                                                    runText,
                                                    runCharLen,
                                                    ref scriptItemList[indexRun].a,
                                                    //ref logAttrList[0]
                                                    ptrLogAttrList
                                                );
                                                if (error != 0)
                                                {
                                                    Marshal.FreeHGlobal(ptrLogAttrList);
                                                    ThrowError(14, error);
                                                }

                                                var tempBuf = new byte[runCharLen];
                                                Marshal.Copy(ptrLogAttrList, tempBuf, 0, runCharLen);
                                                Marshal.FreeHGlobal(ptrLogAttrList);

                                                var logAttrList = new SCRIPT_LOGATTR[runCharLen];
                                                for (var indexChar = 0; indexChar < runCharLen; indexChar++)
                                                {
                                                    logAttrList[indexChar].packed = tempBuf[indexChar];
                                                    var ch = runText[indexChar];
                                                    //if (ch == '!' || ch == '%' || ch == ',' || ch == '.' || ch == '/' || ch == ';')
                                                    //{
                                                    //    logAttrList[indexChar].fSoftBreak = true;
                                                    //}
                                                    if (ch == '(' || ch == '{')
                                                    {
                                                        logAttrList[indexChar].fSoftBreak = true;
                                                    }

                                                    if (indexChar > 0)
                                                    {
                                                        ch = runText[indexChar - 1];
                                                        if (ch == '!' || ch == '%' || ch == ')' || ch == '}' || ch == '-' || ch == '?')
                                                        {
                                                            logAttrList[indexChar].fSoftBreak = true;
                                                        }

                                                        if (runText[indexChar] == '´' && char.IsLetterOrDigit(ch))
                                                        {
                                                            logAttrList[indexChar].fSoftBreak = false;
                                                        }
                                                    }
                                                }
#endregion

                                                var scriptLen = 0;
                                                var scriptLenSp = 0;
                                                var scriptLastCharPos = 0;
                                                var lineCharOffset = 0;
                                                for (var indexChar = 0; indexChar < runCharLen; indexChar++)
                                                {
#region calculate boundsWidth
                                                    boundsWidth = Int32.MaxValue;
                                                    needTrim = false;
                                                    if (trimming != StringTrimming.None)
                                                    {
                                                        boundsWidth = textSize.Width;
                                                        if ((!wordWrap) || (wordWrapPoints.Count + 1 == linesCountLimit))
                                                        {
                                                            needTrim = true;
                                                        }

                                                        if ((trimming == StringTrimming.EllipsisCharacter) || (trimming == StringTrimming.EllipsisWord))
                                                        {
                                                            if (needTrim) boundsWidth = textSize.Width - ellipsisWidth;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (wordWrap) boundsWidth = textSize.Width - indentSize; // - stateList[currentStateIndex].TS.Indent * 40;
                                                    }
#endregion

                                                    if (logAttrList[indexChar].fSoftBreak)
                                                    {
                                                        //check if the SoftHyphen does fit in bounds
                                                        var needSoftBreak = !((indexChar > 0) && (runText[indexChar - 1] == '\xAD') &&
                                                                               (Math.Round((decimal)((sumLen + scriptLenSp + softHypenWidth) * scale), precisionDigits) > (decimal)boundsWidth));
                                                        if (needSoftBreak)
                                                        {
                                                            scriptLenSp = scriptLen;
                                                            scriptLastCharPos = indexChar;
                                                        }
                                                    }

#region calculate current script length
                                                    if (runText[indexChar] == '\t')
                                                    {
                                                        var tabWidth = GetTabsWidth(textOptions, tabSpaceWidth, sumLen + scriptLen);
                                                        lineWidths[runCharPos + indexChar] = tabWidth;
                                                    }

                                                    int clusterNumber = glyphClusterList[indexChar]; //index of first glyph
                                                    //find last char in cluster
                                                    while ((indexChar < runCharLen - 1) && (glyphClusterList[indexChar + 1] == clusterNumber))
                                                    {
                                                        indexChar++;
                                                    }

                                                    scriptLen += lineWidths[runCharPos + indexChar];
#endregion

                                                    if (!logAttrList[indexChar].fWhiteSpace) scriptLenSp = scriptLen;

                                                    //check if script intersect rect
                                                    if (Math.Round((decimal)((sumLen + scriptLenSp) * scale), precisionDigits) > (decimal)boundsWidth)
                                                    {
#region add line
                                                        if (needTrim)
                                                        {
#region trimming
                                                            if ((trimming == StringTrimming.Character) || (trimming == StringTrimming.EllipsisCharacter))
                                                            {
                                                                textLen = runCharPos + indexChar;
                                                                if (textLen == 0) textLen++;
                                                            }

                                                            if ((trimming == StringTrimming.Word) || (trimming == StringTrimming.EllipsisWord))
                                                            {
                                                                //get last script in rect
                                                                textLen = runCharPos + scriptLastCharPos;
                                                                if (textLen == 0) textLen = runCharPos + indexChar;
                                                                if (textLen == 0) textLen++;
                                                            }

                                                            indexRun = scriptItemCount;

                                                            if (lastLineOffset < textLen)
                                                            {
                                                                var pairTrim = new LineInfo();
                                                                pairTrim.Begin = lastLineOffset;
                                                                pairTrim.End = textLen;
                                                                pairTrim.NeedWidthAlign = false;
                                                                if ((trimming == StringTrimming.EllipsisCharacter) || (trimming == StringTrimming.EllipsisWord))
                                                                {
                                                                    pairTrim.Text = textLine.Substring(pairTrim.Begin, pairTrim.Length) + "…";
                                                                }

                                                                wordWrapPoints.Add(pairTrim);
                                                                lastLineOffset = textLen;
                                                            }
#endregion

                                                            break;
                                                        }

                                                        var needNowrap = false;
                                                        if (nowrapList[runCharPos + indexChar] == 1)
                                                        {
                                                            var indexOfBegin = runCharPos + indexChar - 1;
                                                            while (nowrapList[indexOfBegin] == 1) indexOfBegin--;
                                                            if (indexOfBegin == lastLineOffset)
                                                            {
                                                                needNowrap = true;
                                                            }
                                                        }

                                                        if (((scriptLastCharPos == lineCharOffset) && (sumLen == 0)) || needNowrap)
                                                        {
                                                            //only one word in line; break it
                                                            lineCharOffset = indexChar;
                                                            if ((lineCharOffset == 0) && (!needNowrap)) lineCharOffset++;
                                                            if (runCharPos + lineCharOffset - lastLineOffset == 0) lineCharOffset++;
                                                            if ((lineCharOffset > 0) && (runText[lineCharOffset - 1] == '\xAD')) lineCharOffset--;
                                                        }
                                                        else
                                                        {
                                                            //get last script in rect
                                                            lineCharOffset = scriptLastCharPos;
                                                        }

                                                        var pairWrapPoint = new LineInfo();
                                                        pairWrapPoint.Begin = lastLineOffset;
                                                        pairWrapPoint.End = runCharPos + lineCharOffset;
                                                        pairWrapPoint.NeedWidthAlign = true;
                                                        wordWrapPoints.Add(pairWrapPoint);
                                                        if (isWrapped && StiBaseOptions.AllowHtmlListItemSecondLineIndent) pairWrapPoint.Indent = indentCalcSize;

                                                        if ((pairWrapPoint.End - 1 > 0) && (text[pairWrapPoint.End - 1] == '\xAD') && (softHypenWidth >= 0))
                                                        {
                                                            lineWidths[pairWrapPoint.End - 1] = softHypenWidth;
                                                        }

                                                        if (StiBaseOptions.AllowHtmlListItemSecondLineIndent)
                                                        {
                                                            indentSize = indentCalcSize;
                                                        }
                                                        isWrapped = true;

#region trim
                                                        while ((pairWrapPoint.End > pairWrapPoint.Begin + 1) && (Char.IsWhiteSpace(textLine[pairWrapPoint.End - 1])))
                                                        {
                                                            pairWrapPoint.End--;
                                                        }

                                                        while ((lineCharOffset < runCharLen - 1) && (Char.IsWhiteSpace(textLine[runCharPos + lineCharOffset]) || textLine[runCharPos + lineCharOffset] == '\xAD'))
                                                        {
                                                            lineCharOffset++;
                                                        }
#endregion

                                                        scriptLen = 0;
                                                        scriptLenSp = 0;
                                                        sumLen = 0;
                                                        scriptLastCharPos = lineCharOffset;
                                                        lastLineOffset = runCharPos + lineCharOffset;
                                                        indexChar = lineCharOffset - 1;

                                                        if ((lastLineOffset < textLen) && (nowrapList[lastLineOffset] == 1) && (nowrapLastRunIndex != 0))
                                                        {
                                                            lastLineOffset = scriptItemList[nowrapLastRunIndex].iCharPos;
                                                            pairWrapPoint.End = lastLineOffset;
                                                            indexRun = nowrapLastRunIndex - 1;
                                                            nowrapLastUsedRun = nowrapLastRunIndex;
                                                            nowrapLastRunIndex = 0;
                                                            //trim
                                                            while ((pairWrapPoint.End > pairWrapPoint.Begin + 1) && (Char.IsWhiteSpace(textLine[pairWrapPoint.End - 1])))
                                                            {
                                                                pairWrapPoint.End--;
                                                            }

                                                            break;
                                                        }

                                                        nowrapLastRunIndex = 0;
                                                        nowrapLastUsedRun = 0;
#endregion
                                                    }
                                                }
#endregion

                                                sumLen += scriptLen;
                                                remainder = 0;
                                            }
#endregion
                                        }

#region add last line if need
                                        if (lastLineOffset < textLen)
                                        {
                                            var pairWrapPoint = new LineInfo();
                                            pairWrapPoint.Begin = lastLineOffset;
                                            pairWrapPoint.End = textLen;
                                            pairWrapPoint.NeedWidthAlign = false;
                                            if (((trimming == StringTrimming.EllipsisCharacter) || (trimming == StringTrimming.EllipsisWord)) &&
                                                ((wordWrapPoints.Count + 1 == linesCountLimit) && (indexLine + 1 < linesInfo.Count)))
                                            {
                                                pairWrapPoint.Text = textLine.Substring(pairWrapPoint.Begin, pairWrapPoint.Length) + "…";
                                            }

                                            if (isWrapped && StiBaseOptions.AllowHtmlListItemSecondLineIndent) pairWrapPoint.Indent = indentCalcSize;
                                            wordWrapPoints.Add(pairWrapPoint);
                                        }
#endregion

#region calculate width justify and line width
                                        for (var indexWrapLine = lastLineIndex; indexWrapLine < wordWrapPoints.Count; indexWrapLine++)
                                        {
                                            var line = wordWrapPoints[indexWrapLine];
                                            var lineWidth = 0;
                                            var spaceCount = 0;
                                            for (var indexChar = line.Begin; indexChar < line.End; indexChar++)
                                            {
                                                lineWidth += lineWidths[indexChar];
                                                var ch = text[textLinePair.Begin + indexChar];
                                                if (char.IsWhiteSpace(ch) && ch != '\xA0')
                                                {
                                                    spaceCount++;
                                                }
                                            }

                                            if (line.Text != null) lineWidth += ellipsisWidth;
                                            line.Width = (int)(lineWidth * scale);
                                            line.Begin += textLinePair.Begin;
                                            //if ((line.NeedWidthAlign) && (spaceCount > 0))
                                            if (spaceCount > 0)
                                            {
                                                var lineJustifyOffset = (textSize.Width - line.Width - line.Indent) / (double)spaceCount;
                                                if (lineJustifyOffset > 0)
                                                {
                                                    line.JustifyOffset = lineJustifyOffset;
                                                }
                                                else
                                                {
                                                    line.NeedWidthAlign = false;
                                                }
                                            }
                                        }

                                        lastLineIndex = wordWrapPoints.Count;
#endregion
                                    }
                                }

                                linesInfo = wordWrapPoints;
                            }
#endregion

                            if ((!lineLimit) && (trimming == StringTrimming.None)) linesCountLimit = linesInfo.Count;
#endregion

#region measure text
                            var maxWidth = 0;
                            double fullTextHeight = 0;
                            linesCountCalc = 0;
                            for (var indexLine = 0; indexLine < linesInfo.Count; indexLine++)
                            {
                                var lineInfo = linesInfo[indexLine];
                                if (maxWidth < lineInfo.Width) maxWidth = lineInfo.Width;

                                //calculate max font size for this line
                                var lineInfoBegin = lineInfo.Begin;
                                if ((lineInfoBegin > 0) && (stateOrder[lineInfoBegin] - stateOrder[lineInfoBegin - 1] == 2)) lineInfoBegin--;
                                var fontIndex = stateList[stateOrder[lineInfo.Begin]].FontIndex;
                                if (lineInfo.End - lineInfo.Begin > 0)
                                {
                                    var stateIndex1 = lineInfoBegin + 1 < stateOrder.Length ? stateOrder[lineInfoBegin + 1] : stateOrder[stateOrder.Length - 1];
                                    var stateIndex2 = lineInfo.End < stateOrder.Length ? stateOrder[lineInfo.End] : stateOrder[stateOrder.Length - 1];
                                    for (var indexChar = stateIndex1; indexChar <= stateIndex2; indexChar++)
                                    {
                                        var tempFontIndex = stateList[indexChar].FontIndex;
                                        if (fontList[fontIndex].FontBase.Size < fontList[tempFontIndex].FontBase.Size)
                                        {
                                            fontIndex = tempFontIndex;
                                        }
                                        else if (fontList[fontIndex].LineHeight < fontList[tempFontIndex].LineHeight)
                                        {
                                            fontIndex = tempFontIndex;
                                        }
                                    }
                                }

                                lineInfo.IndexOfMaxFont = fontIndex;

                                //double lineSpace = stateList[stateOrder[lineInfo.End - (lineInfo.Length > 0 ? 1 : 0)]].LineHeight;
                                double lineSpace = 1;
                                if (indexLine != linesInfo.Count - 1)
                                {
                                    var nextLineInfo = linesInfo[indexLine + 1];
                                    var stateIndex = nextLineInfo.Begin;
                                    if (stateIndex > 0) stateIndex--;
                                    lineSpace = stateList[stateOrder[stateIndex]].TS.LineHeight;
                                }

                                lineInfo.LineHeight = fontList[fontIndex].LineHeight * lineSpace;
                                fullTextHeight += lineInfo.LineHeight;
                                if ((fullTextHeight < textRect.Height) ||
                                    (fullTextHeight - lineInfo.LineHeight + fontList[fontIndex].LineHeight < textRect.Height))
                                {
                                    linesCountCalc++;
                                }

                                lineInfo.TextAlignment = stateList[stateOrder[lineInfo.End - (lineInfo.Length > 0 ? 1 : 0)]].TS.TextAlign;

                                //for test only
                                //lineInfo.Text = text.Substring(lineInfo.Begin, lineInfo.Length);
                            }

                            if (lineLimit) linesCountLimit = linesCountCalc;
                            measureSize.Width = maxWidth;
                            measureSize.Height = (int)Math.Round((decimal)fullTextHeight);
                            if (linesInfo.Count == 0) measureSize.Height = 0;
                            if (OptimizeBottomMargin)
                            {
                                if (linesInfo.Count == 1) measureSize.Height += fontList[0].LineHeight * 0.07;
                                else if (linesInfo.Count == 2)
                                    measureSize.Height += fontList[0].LineHeight * 0.085;
                                else if (linesInfo.Count > 2)
                                    measureSize.Height += fontList[0].LineHeight * 0.1;
                            }
                            else
                            {
                                if (linesInfo.Count == 1) measureSize.Height += fontList[0].LineHeight * 0.1;
                                if (linesInfo.Count > 1) measureSize.Height += fontList[0].LineHeight * 0.4;
                            }

                            if (((angle > 45) && (angle < 135)) || ((angle > 225) && (angle < 315)))
                            {
                                var tempValue = measureSize.Width;
                                measureSize.Width = measureSize.Height;
                                measureSize.Height = tempValue;

                                if (measureSize.Height > textRect.Height && wordWrap) measureSize.Height = textRect.Height;
                                if (measureSize.Width > textRect.Width) measureSize.Width = textRect.Width;
                            }
                            else
                            {
                                if (measureSize.Width > textRect.Width && wordWrap) measureSize.Width = textRect.Width;
                                if (measureSize.Height > textRect.Height) measureSize.Height = textRect.Height;
                            }

                            measureSize.Width += sideRectOffset * 2 * baseScale;
#endregion

                            if (textLinesArray != null)
                            {
                                for (var index = 0; index < linesInfo.Count; index++)
                                {
                                    var lineInfo = linesInfo[index];
                                    var textLine = lineInfo.Text;
                                    if (textLine == null) textLine = text.Substring(lineInfo.Begin, lineInfo.Length);

                                    var needEnd = (textLine.Length > 0) && (textLine[textLine.Length - 1] == '\xAD');
                                    textLine = textLine.Replace("\xAD", "") + (needEnd ? "-" : "");

                                    if (lineInfo.Indent > 0) textLine = GetIndentString(lineInfo.Indent) + textLine;
                                    textLinesArray.Add(textLine);
                                    if (textLinesInfo != null) textLinesInfo.Add(lineInfo);
                                }
                            }

                            if (needDraw)
                            {
                                if (!Compatibility2009)
                                {
#region set world transformation
                                    float scaleX = 1;
                                    float scaleY = 1;
                                    if ((gUnit != GraphicsUnit.Display) && (gUnit != GraphicsUnit.Pixel))
                                    {
#region scale for print
                                        oldMapMode = SetMapMode(hdc, MM_ANISOTROPIC);
                                        if (oldMapMode == 0) ThrowError(4);

                                        var maxX = pageF.Width + (pageF.X > 0 ? pageF.X : 0);
                                        var maxY = pageF.Height + (pageF.Y > 0 ? pageF.Y : 0);

                                        scaleX = dpiX / 100f;
                                        scaleY = dpiY / 100f;

                                        if (pageScale != 0.01)
                                        {
                                            scaleX *= pageScale / 0.01f;
                                            scaleY *= pageScale / 0.01f;
                                        }

                                        var maxXP = maxX * scaleX;
                                        var maxYP = maxY * scaleY;

                                        var windowSize = new SIZE();
                                        gdiError = SetWindowExtEx(
                                            hdc,
                                            (int)maxX,
                                            (int)maxY,
                                            out windowSize);
                                        if (!gdiError) ThrowError(5);

                                        var viewportSize = new SIZE();
                                        gdiError = SetViewportExtEx(
                                            hdc,
                                            (int)maxXP,
                                            (int)maxYP,
                                            out viewportSize);
                                        if (!gdiError) ThrowError(6);
#endregion
                                    }

                                    gdiError = GetWorldTransform(hdc, out oldXForm);
                                    if (!gdiError) ThrowError(7);

                                    //translate dx, dy
                                    var newXForm1 = new XFORM(1, 0, 0, 1,
                                        textRect.Left + textRect.Width / 2f,
                                        textRect.Top + textRect.Height / 2f);
                                    gdiError = ModifyWorldTransform(hdc, ref newXForm1, MWT_LEFTMULTIPLY);
                                    if (!gdiError) ThrowError(8);

                                    //rotate
                                    var newXForm2 = new XFORM(
                                        Math.Cos(angleRad),
                                        Math.Sin(angleRad),
                                        -Math.Sin(angleRad),
                                        Math.Cos(angleRad),
                                        0, 0);
                                    gdiError = ModifyWorldTransform(hdc, ref newXForm2, MWT_LEFTMULTIPLY);
                                    if (!gdiError) ThrowError(9);

                                    //translate dx, dy
                                    var newXForm3 = new XFORM(1, 0, 0, 1,
                                        -textSize.Width / 2f,
                                        -textSize.Height / 2f);
                                    gdiError = ModifyWorldTransform(hdc, ref newXForm3, MWT_LEFTMULTIPLY);
                                    if (!gdiError) ThrowError(10);
#endregion

#region set drawing parameters
                                    if (!foreColor.IsEmpty)
                                    {
                                        SetTextColor(hdc, ColorToWin32(foreColor));
                                    }

                                    var newMode1 = (backColor.IsEmpty || (backColor == Color.Transparent)) ? DeviceContextBackgroundMode.Transparent : DeviceContextBackgroundMode.Opaque;
                                    SetBkMode(hdc, (int)newMode1);
                                    if (newMode1 != DeviceContextBackgroundMode.Transparent)
                                    {
                                        SetBkColor(hdc, ColorToWin32(backColor));
                                    }

                                    resultGetClip = GetClipRgn(hdc, oldRegion);
                                    newRegion = CreateRectRgn(
                                        (int)(regionRect.Left * scaleX),
                                        (int)(regionRect.Top * scaleY),
                                        (int)(regionRect.Right * scaleX),
                                        (int)(regionRect.Bottom * scaleY));
                                    SelectClipRgn(hdc, newRegion);
#endregion
                                }

                                var linesCount = linesInfo.Count;
                                if (linesCount > linesCountLimit) linesCount = linesCountLimit;

#region vertical alignment
                                double rectY = 0;
                                double textHeight = 0;
                                for (var indexLine = 0; indexLine < linesCount; indexLine++)
                                {
                                    textHeight += linesInfo[indexLine].LineHeight;
                                }

                                textHeight = (int)textHeight;
                                var vertAlignment = vertAlign;
                                if (isNotRightAngle)
                                {
                                    vertAlignment = StiVertAlignment.Center;
                                }

                                switch (vertAlignment)
                                {
                                    case StiVertAlignment.Center:
                                        rectY += (textSize.Height - textHeight) / 2;
                                        break;

                                    case StiVertAlignment.Bottom:
                                        rectY += textSize.Height - textHeight;
                                        break;
                                }
#endregion

                                //fix for strange GDI behaviour (shift text down if angle not 0/90/180/270)
                                if (isNotRightAngle)
                                    rectY -= 2 * scale;

                                var yposDecimal = (decimal)rectY;
                                for (var indexLine = 0; indexLine < linesCount; indexLine++)
                                {
                                    var textLineInfo = linesInfo[indexLine];
                                    if (textLineInfo.Length > 0)
                                    {
                                        var textLine = text.Substring(textLineInfo.Begin, textLineInfo.Length);
                                        if (textLineInfo.Text != null) textLine = textLineInfo.Text;
                                        if (textLine.IndexOf('\x2011') != -1) textLine = textLine.Replace('\x2011', '-');
                                        //if (textLineInfo.Indent > 0) textLine = GetIndentString(textLineInfo.Indent) + textLine;

                                        textLine = textLine.Substring(0, textLine.Length - 1).Replace('\xAD', '\x200B') + textLine[textLine.Length - 1];

#region horizontal alignment
                                        var rectX = textLineInfo.Indent * scale; // stateList[stateOrder[textLineInfo.Begin]].TS.Indent * 40;

                                        double textLineWidth = textLineInfo.Width;

                                        var lineHorAlign = textLineInfo.TextAlignment;
                                        if (rightToLeft)
                                        {
                                            if (textLineInfo.TextAlignment == StiTextHorAlignment.Left) lineHorAlign = StiTextHorAlignment.Right;
                                            if (textLineInfo.TextAlignment == StiTextHorAlignment.Right) lineHorAlign = StiTextHorAlignment.Left;
                                        }

                                        if (forceWidthAlign && (indexLine == linesInfo.Count - 1))
                                        {
                                            textLineInfo.NeedWidthAlign = true;
                                        }

                                        if ((lineHorAlign == StiTextHorAlignment.Width) && (!textLineInfo.NeedWidthAlign))
                                        {
                                            if (rightToLeft) lineHorAlign = StiTextHorAlignment.Right;
                                            else lineHorAlign = StiTextHorAlignment.Left;
                                        }

                                        if (isNotRightAngle) lineHorAlign = StiTextHorAlignment.Center;

                                        switch (lineHorAlign)
                                        {
                                            case StiTextHorAlignment.Center:
                                                rectX += (textSize.Width - textLineWidth) / 2f;
                                                break;

                                            case StiTextHorAlignment.Right:
                                                rectX += textSize.Width - textLineWidth;
                                                break;
                                        }
#endregion

#region out text
#region ScriptItemize
                                        SCRIPT_ITEM[] scriptItemList = null;
                                        var scriptItemCount = 0;
                                        var error = 0;
                                        var cMaxItems = 10;
                                        do
                                        {
                                            cMaxItems *= 10;
                                            var psControl = new SCRIPT_CONTROL();
                                            var psState = new SCRIPT_STATE();
                                            //scriptItemList = new SCRIPT_ITEM[cMaxItems];
                                            psState.uBidiLevel = (ushort)(rightToLeft ? 1 : 0);

                                            var buf = Marshal.AllocHGlobal(sizeofScriptItem * cMaxItems + 1);

                                            error = ScriptItemize(
                                                textLine,
                                                textLine.Length,
                                                cMaxItems,
                                                ref psControl,
                                                ref psState,
                                                //ref scriptItemList[0],
                                                buf,
                                                out scriptItemCount);
                                            if ((error != 0) && (error != E_OUTOFMEMORY))
                                            {
                                                Marshal.FreeHGlobal(buf);
                                                ThrowError(15, error);
                                            }

                                            scriptItemList = new SCRIPT_ITEM[scriptItemCount + 1];
                                            var offset = buf;
                                            for (var indexItem = 0; indexItem < scriptItemCount + 1; indexItem++)
                                            {
                                                scriptItemList[indexItem] = (SCRIPT_ITEM)Marshal.PtrToStructure(offset, typeof(SCRIPT_ITEM));
                                                offset = (IntPtr)((Int64)offset + sizeofScriptItem);
                                            }

                                            Marshal.FreeHGlobal(buf);
                                        } while (error == E_OUTOFMEMORY);

                                        var store_scriptItemCount = scriptItemCount;
#endregion

#region break runs in nowrapBegin points
                                        //if (existNowrapPoints)
                                        {
                                            var newScriptItemList = new List<SCRIPT_ITEM>();
                                            newScriptItemList.Add(scriptItemList[0]);
                                            var itemIndex = 0;
                                            for (var indexChar = 0; indexChar < textLine.Length; indexChar++)
                                            {
                                                if (indexChar == scriptItemList[itemIndex + 1].iCharPos)
                                                {
                                                    itemIndex++;
                                                    newScriptItemList.Add(scriptItemList[itemIndex]);
                                                    continue;
                                                }

                                                if ((indexChar > 0) && (stateOrder[textLineInfo.Begin + indexChar] != stateOrder[textLineInfo.Begin + indexChar - 1]))
                                                {
                                                    var si = scriptItemList[itemIndex];
                                                    si.iCharPos = indexChar;
                                                    newScriptItemList.Add(si);
                                                    continue;
                                                }
                                            }

                                            newScriptItemList.Add(scriptItemList[scriptItemCount]);
                                            scriptItemList = new SCRIPT_ITEM[newScriptItemList.Count];
                                            for (var indexRun = 0; indexRun < newScriptItemList.Count; indexRun++)
                                            {
                                                scriptItemList[indexRun] = newScriptItemList[indexRun];
                                            }

                                            scriptItemCount = newScriptItemList.Count - 1;
                                        }
#endregion

#region ScriptLayout
                                        var bidiLevel = new byte[scriptItemCount];
                                        var visualToLogicalList = new int[scriptItemCount];
                                        var logicalToVisualList = new int[scriptItemCount];

                                        for (var index = 0; index < scriptItemCount; index++)
                                        {
                                            bidiLevel[index] = (byte)scriptItemList[index].a.s.uBidiLevel;
                                        }

                                        error = ScriptLayout(scriptItemCount, bidiLevel, visualToLogicalList, logicalToVisualList);
                                        if (error != 0)
                                        {
                                            ThrowError(16, error);
                                        }
#endregion

                                        var textLen = textLine.Length;
                                        var bufLen = textLen * 2;
                                        if (bufLen < 20) bufLen = 20;
                                        var sumLen = 0;
                                        var xpos = rectX;
                                        //int ypos = (int)Math.Round(yposDecimal);
                                        double remainder = 0;
                                        double remainderScaled = 0;

                                        // process each "run" of text 
                                        for (var indexRun = 0; indexRun < scriptItemCount; indexRun++)
                                        {
                                            var glyphIndexList = new ushort[bufLen];
                                            var glyphClusterList = new ushort[textLen];
                                            //SCRIPT_VISATTR[] scriptVisAttrList = new SCRIPT_VISATTR[bufLen];
                                            var scriptVisAttrList = Marshal.AllocHGlobal(sizeofScriptVisattr * bufLen);
                                            //GOFFSET[] goff = new GOFFSET[bufLen];
                                            var advanceWidthList = new int[bufLen];
                                            ABC abc;
                                            int glyphIndexCount;

                                            var vidx = visualToLogicalList[indexRun];

                                            var runCharPos = scriptItemList[vidx].iCharPos;
                                            var runCharLen = scriptItemList[vidx + 1].iCharPos - runCharPos;
                                            var runText = textLine.Substring(runCharPos, runCharLen);

                                            currentStateIndex = stateOrder[textLineInfo.Begin + runCharPos];
                                            var currentFontState = fontList[stateList[currentStateIndex].FontIndex];
                                            //IntPtr hhFontScaled = fontList[stateList[currentStateIndex].FontIndex].hFontScaled;
                                            //IntPtr hhFont = fontList[stateList[currentStateIndex].FontIndex].hFont;
                                            var useScale = currentFontState.hFontScaled != IntPtr.Zero;
                                            if (!useGdiPlus)
                                            {
                                                if (useScale)
                                                {
                                                    SelectObject(hdc, currentFontState.hFontScaled);
                                                }
                                                else
                                                {
                                                    SelectObject(hdc, currentFontState.hFont);
                                                }
                                            }

                                            if (!currentFontState.HasNbspGlyf && runText.IndexOf('\xA0') != -1)
                                            {
                                                runText = runText.Replace('\xA0', ' ');
                                            }

                                            var ypos = (double)yposDecimal;
                                            if (stateList[currentStateIndex].TS.Superscript || stateList[currentStateIndex].TS.Subscript)
                                            {
                                                var parentState = fontList[currentFontState.ParentFontIndex];
                                                ypos += fontList[textLineInfo.IndexOfMaxFont].Ascend - parentState.Ascend;
                                                if (stateList[currentStateIndex].TS.Subscript)
                                                {
                                                    ypos += parentState.LineHeight - (currentFontState.Ascend + currentFontState.Descend);
                                                }
                                            }
                                            else
                                            {
                                                ypos += fontList[textLineInfo.IndexOfMaxFont].Ascend - currentFontState.Ascend;
                                            }

                                            if (!useGdiPlus)
                                            {
                                                if (!stateList[currentStateIndex].TS.FontColor.IsEmpty)
                                                {
                                                    SetTextColor(hdc, ColorToWin32(stateList[currentStateIndex].TS.FontColor));
                                                }

                                                var newMode = (stateList[currentStateIndex].TS.BackColor.IsEmpty || (stateList[currentStateIndex].TS.BackColor == Color.Transparent)) ? DeviceContextBackgroundMode.Transparent : DeviceContextBackgroundMode.Opaque;
                                                SetBkMode(hdc, (int)newMode);
                                                if (newMode != DeviceContextBackgroundMode.Transparent)
                                                {
                                                    SetBkColor(hdc, ColorToWin32(stateList[currentStateIndex].TS.BackColor));
                                                }
                                            }

                                            var hScriptCache = useScale ? currentFontState.hScriptCacheScaled : currentFontState.hScriptCache;

#region ScriptShape
                                            error = ScriptShape(
                                                hdc,
                                                ref hScriptCache,
                                                runText,
                                                runCharLen,
                                                textLen * 2,
                                                ref scriptItemList[vidx].a,
                                                glyphIndexList,
                                                glyphClusterList,
                                                //ref scriptVisAttrList[0],
                                                scriptVisAttrList,
                                                out glyphIndexCount
                                            );
                                            if (error == E_SCRIPT_NOT_IN_FONT) //fix
                                            {
                                                error = 0;
                                                glyphIndexCount = runCharLen;
                                                for (var tIndex = 0; tIndex < glyphIndexCount; tIndex++)
                                                {
                                                    glyphClusterList[tIndex] = (ushort)tIndex;
                                                }

                                                scriptItemList[indexRun].a.packed = 0;
                                            }

                                            if ((error != 0) && (error != E_SCRIPT_NOT_IN_FONT))
                                            {
                                                Marshal.FreeHGlobal(scriptVisAttrList);
                                                ThrowError(17, error);
                                            }

                                            var store_error17 = error;
#endregion

                                            var goff = Marshal.AllocHGlobal(sizeofGoffset * bufLen);

                                            //if (currentFontState.hFontScaled != IntPtr.Zero) SelectObject(hdc, currentFontState.hFontScaled);

#region ScriptPlace
                                            var summaryWidthInt = 0;
                                            if (useScale)
                                            {
                                                SelectObject(hdc, currentFontState.hFont);
                                                error = ScriptPlace(
                                                    hdc,
                                                    ref currentFontState.hScriptCache,
                                                    glyphIndexList,
                                                    glyphIndexCount,
                                                    //ref scriptVisAttrList[0],
                                                    scriptVisAttrList,
                                                    ref scriptItemList[vidx].a,
                                                    advanceWidthList,
                                                    //ref goff[0],
                                                    goff,
                                                    out abc
                                                );
                                                if (error != 0)
                                                {
                                                    Marshal.FreeHGlobal(goff);
                                                    Marshal.FreeHGlobal(scriptVisAttrList);
                                                    ThrowError(1801, error);
                                                }

                                                SelectObject(hdc, currentFontState.hFontScaled);

                                                if (CorrectionEnabled && !Compatibility2009)
                                                {
#region AdvanceWidthList correction
                                                    var allGlyphWidth = GetFontWidth(currentFontState);
                                                    if (allGlyphWidth.Length > 0)
                                                    {
                                                        if (PrecisionMode3Enabled)
                                                        {
                                                            var fontScale = MaxFontSize / currentFontState.EmValue;
                                                            double sumD = remainder;
                                                            int sumI = 0;
                                                            for (var indexGlyph = 0; indexGlyph < glyphIndexCount; indexGlyph++)
                                                            {
                                                                if (advanceWidthList[indexGlyph] == 0) continue;
                                                                int tempGlyphIndex = glyphIndexList[indexGlyph];
                                                                if (tempGlyphIndex >= allGlyphWidth.Length) tempGlyphIndex = allGlyphWidth.Length - 1;
                                                                sumD += allGlyphWidth[tempGlyphIndex] / fontScale;
                                                                int width = (int)(sumD - sumI);
                                                                sumI += width;
                                                                advanceWidthList[indexGlyph] = width;
                                                            }
                                                            if ((sumD - sumI) > 0.5)
                                                            {
                                                                advanceWidthList[glyphIndexCount - 1]++;
                                                                sumI++;
                                                            }
                                                            remainder = sumD - sumI;
                                                        }
                                                        else
                                                        if (PrecisionMode2Enabled)
                                                        {
                                                            var fontScale = MaxFontSize / currentFontState.EmValue;
                                                            double sumD = remainder;
                                                            int sumI = 0;
                                                            for (var indexGlyph = 0; indexGlyph < glyphIndexCount; indexGlyph++)
                                                            {
                                                                if (advanceWidthList[indexGlyph] == 0) continue;
                                                                int tempGlyphIndex = glyphIndexList[indexGlyph];
                                                                if (tempGlyphIndex >= allGlyphWidth.Length) tempGlyphIndex = allGlyphWidth.Length - 1;
                                                                sumD += allGlyphWidth[tempGlyphIndex] / fontScale;
                                                                int width = (int)(sumD - sumI);
                                                                sumI += width;
                                                                advanceWidthList[indexGlyph] = width;
                                                            }
                                                            if ((sumD - sumI) > 0.5)
                                                            {
                                                                advanceWidthList[glyphIndexCount - 1]++;
                                                                sumI++;
                                                            }
                                                            remainder = sumD - sumI;
                                                        }
                                                        else
                                                        {
                                                            var fontScale = MaxFontSize / currentFontState.EmValue;
                                                            for (var indexGlyph = 0; indexGlyph < glyphIndexCount; indexGlyph++)
                                                            {
                                                                if (advanceWidthList[indexGlyph] == 0) continue;
                                                                int tempGlyphIndex = glyphIndexList[indexGlyph];
                                                                if (tempGlyphIndex >= allGlyphWidth.Length) tempGlyphIndex = allGlyphWidth.Length - 1;
                                                                var width = allGlyphWidth[tempGlyphIndex] / fontScale;
                                                                if (advanceWidthList[indexGlyph] < width - 0.4)
                                                                {
                                                                    var newWidth = (int)Math.Round(width);
                                                                    if (advanceWidthList[indexGlyph] >= newWidth) newWidth++;
                                                                    advanceWidthList[indexGlyph] = newWidth;
                                                                }
                                                                else
                                                                {
                                                                    var percent = advanceWidthList[indexGlyph] / width;
                                                                    if (advanceWidthList[indexGlyph] - width > 1 && percent > 1.1)
                                                                    {
                                                                        advanceWidthList[indexGlyph] = (int)Math.Ceiling(width);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
#endregion
                                                }

                                                for (var indexGlyph = 0; indexGlyph < glyphIndexCount; indexGlyph++)
                                                {
                                                    summaryWidthInt += advanceWidthList[indexGlyph];
                                                }
                                            }

                                            error = ScriptPlace(
                                                hdc,
                                                ref hScriptCache,
                                                glyphIndexList,
                                                glyphIndexCount,
                                                //ref scriptVisAttrList[0],
                                                scriptVisAttrList,
                                                ref scriptItemList[vidx].a,
                                                advanceWidthList,
                                                //ref goff[0],
                                                goff,
                                                out abc
                                            );
                                            if (error != 0)
                                            {
                                                Marshal.FreeHGlobal(goff);
                                                Marshal.FreeHGlobal(scriptVisAttrList);
                                                ThrowError(18, error);
                                            }

                                            Marshal.FreeHGlobal(scriptVisAttrList);
#endregion

                                            if (CorrectionEnabled && !Compatibility2009)
                                            {
#region AdvanceWidthList correction
                                                var allGlyphWidth = GetFontWidth(currentFontState);
                                                if (allGlyphWidth.Length > 0)
                                                {
                                                    if (PrecisionMode3Enabled)
                                                    {
                                                        var fontScale = MaxFontSize / currentFontState.EmValue / scale;
                                                        int sumIF = 0;
                                                        double sumD = remainderScaled * fontScale;
                                                        for (var indexGlyph = 0; indexGlyph < glyphIndexCount; indexGlyph++)
                                                        {
                                                            int aw = advanceWidthList[indexGlyph];
                                                            if (aw == 0) continue;
                                                            int tempGlyphIndex = glyphIndexList[indexGlyph];
                                                            if (tempGlyphIndex >= allGlyphWidth.Length) tempGlyphIndex = allGlyphWidth.Length - 1;
                                                            int glyphWidth = allGlyphWidth[tempGlyphIndex];
                                                            sumD += glyphWidth;
                                                            if (aw < glyphWidth / fontScale - 0.5)
                                                            {
                                                                var newWidth = (int)Math.Round(glyphWidth / fontScale);
                                                                if (aw >= newWidth) newWidth++;
                                                                aw = newWidth;
                                                            }
                                                            advanceWidthList[indexGlyph] = aw;
                                                            sumIF += aw;
                                                        }
                                                        sumD /= fontScale;

                                                        bool[] spaces = new bool[glyphIndexCount];
                                                        int spacesCount = 0;
                                                        int sumSpaces = 0;
                                                        for (var indexChar = 0; indexChar < runCharLen; indexChar++)
                                                        {
                                                            if (char.IsWhiteSpace(runText[indexChar]))
                                                            {
                                                                int spaceIndex = glyphClusterList[indexChar];
                                                                spaces[spaceIndex] = true;
                                                                sumSpaces += advanceWidthList[spaceIndex];
                                                                spacesCount++;
                                                            }
                                                        }

                                                        if ((spacesCount > 0) && ((sumIF - sumD) < sumSpaces * 0.25))
                                                        {
                                                            double factor = (sumSpaces - (sumIF - sumD)) / sumSpaces;
                                                            double sumDSp = 0;
                                                            int sumISp = 0;
                                                            for (var indexGlyph = 0; indexGlyph < glyphIndexCount; indexGlyph++)
                                                            {
                                                                if (spaces[indexGlyph])
                                                                {
                                                                    int widthI = advanceWidthList[indexGlyph];
                                                                    if (widthI == 0) continue;
                                                                    sumDSp += widthI * factor;
                                                                    int width = (int)Math.Round(sumDSp - sumISp, 0);
                                                                    sumISp += width;
                                                                    advanceWidthList[indexGlyph] = width;
                                                                }
                                                            }
                                                            if ((sumDSp - sumISp) > 0.5)
                                                            {
                                                                advanceWidthList[glyphIndexCount - 1]++;
                                                                sumISp++;
                                                            }
                                                            remainderScaled = sumDSp - sumISp;
                                                        }
                                                        else
                                                        {
                                                            double factor = sumD / sumIF;
                                                            sumD = remainderScaled;
                                                            int sumI = 0;
                                                            for (var indexGlyph = 0; indexGlyph < glyphIndexCount; indexGlyph++)
                                                            {
                                                                int widthI = advanceWidthList[indexGlyph];
                                                                if (widthI == 0) continue;
                                                                sumD += widthI * factor;
                                                                int width = (int)Math.Round(sumD - sumI, 0);
                                                                sumI += width;
                                                                advanceWidthList[indexGlyph] = width;
                                                            }
                                                            if ((sumD - sumI) > 0.5)
                                                            {
                                                                advanceWidthList[glyphIndexCount - 1]++;
                                                                sumI++;
                                                            }
                                                            remainderScaled = sumD - sumI;
                                                        }
                                                    }
                                                    else
                                                    if (PrecisionMode2Enabled)
                                                    {
                                                        var fontScale = MaxFontSize / currentFontState.EmValue / scale;
                                                        int sumIF = 0;
                                                        double sumD = remainderScaled * fontScale;
                                                        for (var indexGlyph = 0; indexGlyph < glyphIndexCount; indexGlyph++)
                                                        {
                                                            sumIF += advanceWidthList[indexGlyph];
                                                            if (advanceWidthList[indexGlyph] == 0) continue;
                                                            int tempGlyphIndex = glyphIndexList[indexGlyph];
                                                            if (tempGlyphIndex >= allGlyphWidth.Length) tempGlyphIndex = allGlyphWidth.Length - 1;
                                                            sumD += allGlyphWidth[tempGlyphIndex];
                                                        }
                                                        sumD /= fontScale;

                                                        bool[] spaces = new bool[glyphIndexCount];
                                                        int spacesCount = 0;
                                                        int sumSpaces = 0;
                                                        for (var indexChar = 0; indexChar < runCharLen; indexChar++)
                                                        {
                                                            if (char.IsWhiteSpace(runText[indexChar]))
                                                            {
                                                                int spaceIndex = glyphClusterList[indexChar];
                                                                spaces[spaceIndex] = true;
                                                                sumSpaces += advanceWidthList[spaceIndex];
                                                                spacesCount++;
                                                            }
                                                        }

                                                        if ((spacesCount > 0) && ((sumIF - sumD) < sumSpaces * 0.25))
                                                        {
                                                            double factor = (sumSpaces - (sumIF - sumD)) / sumSpaces;
                                                            double sumDSp = 0;
                                                            int sumISp = 0;
                                                            for (var indexGlyph = 0; indexGlyph < glyphIndexCount; indexGlyph++)
                                                            {
                                                                if (spaces[indexGlyph])
                                                                {
                                                                    int widthI = advanceWidthList[indexGlyph];
                                                                    if (widthI == 0) continue;
                                                                    sumDSp += widthI * factor;
                                                                    int width = (int)Math.Round(sumDSp - sumISp, 0);
                                                                    sumISp += width;
                                                                    advanceWidthList[indexGlyph] = width;
                                                                }
                                                            }
                                                            if ((sumDSp - sumISp) > 0.5)
                                                            {
                                                                advanceWidthList[glyphIndexCount - 1]++;
                                                                sumISp++;
                                                            }
                                                            remainderScaled = sumDSp - sumISp;
                                                        }
                                                        else
                                                        {
                                                            double factor = sumD / sumIF;
                                                            sumD = remainderScaled;
                                                            int sumI = 0;
                                                            for (var indexGlyph = 0; indexGlyph < glyphIndexCount; indexGlyph++)
                                                            {
                                                                int widthI = advanceWidthList[indexGlyph];
                                                                if (widthI == 0) continue;
                                                                sumD += widthI * factor;
                                                                int width = (int)Math.Round(sumD - sumI, 0);
                                                                sumI += width;
                                                                advanceWidthList[indexGlyph] = width;
                                                            }
                                                            if ((sumD - sumI) > 0.5)
                                                            {
                                                                advanceWidthList[glyphIndexCount - 1]++;
                                                                sumI++;
                                                            }
                                                            remainderScaled = sumD - sumI;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        var fontScale = MaxFontSize / currentFontState.EmValue / scale;
                                                        //double summm1 = 0;
                                                        //for (int indexGlyph = 0; indexGlyph < glyphIndexCount; indexGlyph++)
                                                        //{
                                                        //    summm1 += advanceWidthList[indexGlyph];
                                                        //}
                                                        //double summm2 = 0;
                                                        for (var indexGlyph = 0; indexGlyph < glyphIndexCount; indexGlyph++)
                                                        {
                                                            if (advanceWidthList[indexGlyph] == 0) continue;
                                                            int tempGlyphIndex = glyphIndexList[indexGlyph];
                                                            if (tempGlyphIndex >= allGlyphWidth.Length) tempGlyphIndex = allGlyphWidth.Length - 1;
                                                            var width = allGlyphWidth[tempGlyphIndex] / fontScale;
                                                            if (advanceWidthList[indexGlyph] < width - 0.4)
                                                            {
                                                                var newWidth = (int)Math.Round(width);
                                                                if (advanceWidthList[indexGlyph] >= newWidth) newWidth++;
                                                                advanceWidthList[indexGlyph] = newWidth;
                                                            }
                                                            else
                                                            {
                                                                var percent = advanceWidthList[indexGlyph] / width;
                                                                if (advanceWidthList[indexGlyph] - width > 1 && percent > 1.1)
                                                                {
                                                                    advanceWidthList[indexGlyph] = (int)Math.Ceiling(width);
                                                                }
                                                            }
                                                            //summm2 += advanceWidthList[indexGlyph];
                                                        }
                                                    }
                                                }
#endregion
                                            }

#region AdvanceWidthList correction for scale
                                            if (useScale && !PrecisionMode2Enabled)
                                            {
                                                var summaryScaledInt = 0;
                                                for (var indexGlyph = 0; indexGlyph < glyphIndexCount; indexGlyph++)
                                                {
                                                    summaryScaledInt += advanceWidthList[indexGlyph];
                                                }

                                                if (summaryScaledInt > 0)
                                                {
                                                    var correctionValue = (summaryWidthInt * scale) / summaryScaledInt;
                                                    var ssumWidthInt = 0;
                                                    double ssumWidthDouble = 0;
                                                    //double summm3 = 0;
                                                    for (var indexGlyph = 0; indexGlyph < glyphIndexCount; indexGlyph++)
                                                    {
                                                        ssumWidthDouble += advanceWidthList[indexGlyph] * correctionValue;
                                                        var glyphWidthScaled = (int)ssumWidthDouble - ssumWidthInt;
                                                        ssumWidthInt += glyphWidthScaled;
                                                        advanceWidthList[indexGlyph] = glyphWidthScaled;
                                                        //summm3 += advanceWidthList[indexGlyph];
                                                    }
                                                }
                                            }
#endregion

                                            //if (currentFontState.hFontScaled != IntPtr.Zero) SelectObject(hdc, currentFontState.hFont);

#region store base advanceWidthList
                                            int[] baseAdvanceWidthList = null;
                                            if (outRunsList != null)
                                            {
                                                baseAdvanceWidthList = new int[glyphIndexCount];
                                                for (var indexChar = 0; indexChar < glyphIndexCount; indexChar++)
                                                {
                                                    baseAdvanceWidthList[indexChar] = advanceWidthList[indexChar];
                                                }
                                            }
#endregion

#region Apply LetterSpacing and WordSpacing
                                            double additionalSpacing = 0;
                                            if (stateList[currentStateIndex].TS.LetterSpacing != 0)
                                            {
                                                //double addedLetterSpacing = currentFontState.EmValue * stateList[currentStateIndex].TS.LetterSpacing;
                                                //double addedLetterSpacing = currentFontState.EmValue * 1.35 * stateList[currentStateIndex].TS.LetterSpacing;
                                                var addedLetterSpacing = currentFontState.EmValue * 1.35 * stateList[currentStateIndex].TS.LetterSpacing * scale;
                                                var ssumWidthInt = 0;
                                                double ssumWidthDouble = 0;
                                                for (var indexGlyph = 0; indexGlyph < glyphIndexCount; indexGlyph++)
                                                {
                                                    ssumWidthDouble += advanceWidthList[indexGlyph] + addedLetterSpacing;
                                                    var glyphWidthScaled = (int)ssumWidthDouble - ssumWidthInt;
                                                    ssumWidthInt += glyphWidthScaled;
                                                    advanceWidthList[indexGlyph] = glyphWidthScaled;
                                                }

                                                additionalSpacing += addedLetterSpacing * glyphIndexCount;
                                            }

                                            if (stateList[currentStateIndex].TS.WordSpacing != 0)
                                            {
                                                //double addedWordSpacing = currentFontState.EmValue * stateList[currentStateIndex].TS.WordSpacing;
                                                var addedWordSpacing = currentFontState.EmValue * stateList[currentStateIndex].TS.WordSpacing * scale;
                                                var ssumWidthInt = 0;
                                                double ssumWidthDouble = 0;
                                                for (var indexChar = 0; indexChar < runCharLen; indexChar++)
                                                {
                                                    if (char.IsWhiteSpace(runText[indexChar]))
                                                    {
                                                        ssumWidthDouble += advanceWidthList[glyphClusterList[indexChar]] + addedWordSpacing;
                                                        var glyphWidthScaled = (int)ssumWidthDouble - ssumWidthInt;
                                                        ssumWidthInt += glyphWidthScaled;
                                                        advanceWidthList[glyphClusterList[indexChar]] += glyphWidthScaled;
                                                        additionalSpacing += addedWordSpacing;
                                                    }
                                                }
                                            }
#endregion

#region calculate runLength and check for simplyRun
                                            var runLength = 0;
                                            var simplyRun = !scriptItemList[vidx].a.fRTL;
                                            for (var indexChar = 0; indexChar < runCharLen; indexChar++)
                                            {
                                                var currentScriptLen = 0;
                                                if (runText[indexChar] == '\t')
                                                {
                                                    //currentScriptLen = GetTabsWidth(tFirstTabOffset, tDistanceBetweenTabs, tabSpaceWidth, sumLen + runLength);
                                                    var tabOffset = GetTabsWidth(textOptions, tabSpaceWidth * scale, sumLen + runLength);
                                                    currentScriptLen = tabOffset;
                                                    summaryWidthInt += (int)(tabOffset / scale);
                                                }
                                                else
                                                {
                                                    int clusterNumber = glyphClusterList[indexChar]; //index of first glyph
                                                    //find last char in cluster
                                                    while ((indexChar < runCharLen - 1) && (glyphClusterList[indexChar + 1] == clusterNumber))
                                                    {
                                                        indexChar++;
                                                        simplyRun = false;
                                                    }

                                                    var indexGlyphBegin = clusterNumber;
                                                    var indexGlyphEnd = clusterNumber;
                                                    if (scriptItemList[vidx].a.fRTL)
                                                    {
                                                        indexGlyphBegin = (indexChar + 1 < runCharLen ? glyphClusterList[indexChar + 1] + 1 : 0);
                                                        indexGlyphEnd++;
                                                    }
                                                    else
                                                    {
                                                        indexGlyphEnd = (indexChar + 1 < runCharLen ? glyphClusterList[indexChar + 1] : glyphIndexCount);
                                                    }

                                                    for (var indexGlyph = indexGlyphBegin; indexGlyph < indexGlyphEnd; indexGlyph++)
                                                    {
                                                        currentScriptLen += advanceWidthList[indexGlyph];
                                                    }

                                                    if (indexGlyphEnd - indexGlyphBegin > 1) simplyRun = false;
                                                }

                                                runLength += currentScriptLen;
                                            }
#endregion

                                            //if (angle != 0) simplyRun = false;

#region scale and justify correction
                                            //int sumWidthInt = 0;
                                            //double sumWidthDouble = 0;
                                            //if (currentFontState.hFontScaled != IntPtr.Zero)
                                            //{
                                            //    for (int indexGlyph = 0; indexGlyph < glyphIndexCount; indexGlyph++)
                                            //    {
                                            //        sumWidthDouble += advanceWidthList[indexGlyph] * scale;
                                            //        int glyphWidthScaled = (int)sumWidthDouble - sumWidthInt;
                                            //        sumWidthInt += glyphWidthScaled;
                                            //        advanceWidthList[indexGlyph] = glyphWidthScaled;
                                            //    }
                                            //}

                                            var sumJustifyInt = 0;
                                            double sumJustifyDouble = 0;
                                            if (lineHorAlign == StiTextHorAlignment.Width)
                                            {
                                                for (var indexChar = 0; indexChar < runCharLen; indexChar++)
                                                {
                                                    if (char.IsWhiteSpace(runText[indexChar]) && runText[indexChar] != '\xA0')
                                                    {
                                                        sumJustifyDouble += textLineInfo.JustifyOffset;
                                                        var justifyOffset = (int)(sumJustifyDouble - sumJustifyInt);
                                                        sumJustifyInt += justifyOffset;
                                                        advanceWidthList[glyphClusterList[indexChar]] += justifyOffset;
                                                    }
                                                }
                                            }
#endregion

                                            if (simplyRun && useGdiPlus || outRunsList != null)
                                            //  || ((glyphIndexCount == 0) && (runCharLen > 0)))   //added 2009.03.04 as trial fix
                                            {
                                                var runinfo = new RunInfo();
                                                runinfo.Text = runText;
                                                runinfo.XPos = xpos + (outRunsList != null ? textRect.X - regionRect.X : 0);
                                                runinfo.YPos = ypos;
                                                runinfo.Widths = new int[runCharLen];

#region fill widths table
                                                for (var indexChar = 0; indexChar < runCharLen; indexChar++)
                                                {
                                                    var beginIndexChar = indexChar;
                                                    int clusterNumber = glyphClusterList[indexChar]; //index of first glyph
                                                    //find last char in cluster
                                                    while ((indexChar < runCharLen - 1) && (glyphClusterList[indexChar + 1] == clusterNumber))
                                                    {
                                                        indexChar++;
                                                    }

                                                    var indexGlyphBegin = clusterNumber;
                                                    var indexGlyphEnd = clusterNumber;
                                                    if (scriptItemList[vidx].a.fRTL)
                                                    {
                                                        indexGlyphBegin = (indexChar + 1 < runCharLen ? glyphClusterList[indexChar + 1] + 1 : 0);
                                                        indexGlyphEnd++;
                                                    }
                                                    else
                                                    {
                                                        indexGlyphEnd = (indexChar + 1 < runCharLen ? glyphClusterList[indexChar + 1] : glyphIndexCount);
                                                    }

                                                    if ((beginIndexChar != indexChar) || (indexGlyphEnd - indexGlyphBegin > 1))
                                                    {
                                                        var currentScriptLen = 0;
                                                        for (var indexGlyph = indexGlyphBegin; indexGlyph < indexGlyphEnd; indexGlyph++)
                                                        {
                                                            currentScriptLen += advanceWidthList[indexGlyph];
                                                        }

                                                        if (indexChar != beginIndexChar)
                                                        {
                                                            var charsCount = indexChar - beginIndexChar + 1;
                                                            var tempWidth = currentScriptLen / (double)charsCount;
                                                            double currWidthDouble = 0;
                                                            var currWidthInt = 0;
                                                            for (var tempIndex = 0; tempIndex < charsCount - 1; tempIndex++)
                                                            {
                                                                currWidthDouble += tempWidth;
                                                                var currentValue = (int)Math.Round(currWidthDouble) - currWidthInt;
                                                                currWidthInt += currentValue;
                                                                runinfo.Widths[beginIndexChar + tempIndex] = currentValue;
                                                            }

                                                            runinfo.Widths[indexChar] = currentScriptLen - currWidthInt;
                                                        }
                                                        else
                                                        {
                                                            runinfo.Widths[indexChar] = currentScriptLen;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        runinfo.Widths[indexChar] = advanceWidthList[indexGlyphBegin];
                                                    }
                                                }
#endregion

                                                if (baseAdvanceWidthList == null) baseAdvanceWidthList = new int[glyphIndexCount];

                                                runinfo.GlyphIndexList = new int[glyphIndexCount];
                                                runinfo.GlyphWidths = new int[glyphIndexCount];
                                                runinfo.ScaleList = new double[glyphIndexCount];
                                                for (var indexGlyph = 0; indexGlyph < glyphIndexCount; indexGlyph++)
                                                {
                                                    runinfo.GlyphIndexList[indexGlyph] = glyphIndexList[indexGlyph];
                                                    runinfo.GlyphWidths[indexGlyph] = advanceWidthList[indexGlyph];
                                                    runinfo.ScaleList[indexGlyph] = 1;
                                                    if (baseAdvanceWidthList[indexGlyph] != 0)
                                                    {
                                                        runinfo.ScaleList[indexGlyph] = advanceWidthList[indexGlyph] / (double)baseAdvanceWidthList[indexGlyph];
                                                    }
                                                }

                                                runinfo.TextColor = stateList[currentStateIndex].TS.FontColor;
                                                runinfo.BackColor = stateList[currentStateIndex].TS.BackColor;
                                                runinfo.FontIndex = stateList[currentStateIndex].FontIndex;
                                                runinfo.Href = stateList[currentStateIndex].TS.Href;
                                                runinfo.TextAlign = stateList[currentStateIndex].TS.TextAlign;
                                                runs.Add(runinfo);
                                            }
                                            else
                                            {
                                                //if (currentFontState.hFontScaled != IntPtr.Zero) SelectObject(hdc, currentFontState.hFontScaled);

                                                var posCorrection = GetFontPositionCorrection(currentFontState);
                                                posCorrection += currentFontState.TypoOffset;

#region ScriptTextOut
                                                error = ScriptTextOut(
                                                    hdc,
                                                    ref hScriptCache,
                                                    (int)Math.Round((decimal)xpos),
                                                    (int)Math.Round((decimal)(ypos + posCorrection)),
                                                    0,
                                                    ref lpRect, // no clipping
                                                    ref scriptItemList[vidx].a,
                                                    IntPtr.Zero,
                                                    0,
                                                    glyphIndexList,
                                                    glyphIndexCount,
                                                    advanceWidthList,
                                                    IntPtr.Zero,
                                                    //ref goff[0]
                                                    goff
                                                );
                                                if (error != 0)
                                                {
                                                    Marshal.FreeHGlobal(goff);

#region throw error
                                                    var stb = new StringBuilder();
                                                    try
                                                    {
                                                        stb.Append(string.Format(" indexLine={0}",
                                                            indexLine));
                                                        stb.Append(string.Format(" scriptItemCount={0}",
                                                            store_scriptItemCount));
                                                        stb.Append(string.Format(" indexRun={0}",
                                                            indexRun));
                                                        stb.Append(string.Format(" runCharLen={0}",
                                                            runCharLen));
                                                        stb.Append(string.Format(" runText={0}",
                                                            runText == null ? "null" : "\"" + runText + "\""));
                                                        stb.Append(string.Format(" err17={0}",
                                                            store_error17));
                                                        stb.Append(string.Format(" hdc={0:X8} hScriptCache={1:X8} x={2:X4} y={3:X4}",
                                                            hdc.ToInt64(),
                                                            currentFontState.hScriptCache.ToInt64(),
                                                            (int)Math.Round((decimal)xpos),
                                                            (int)Math.Round((decimal)ypos)));
                                                        stb.Append(string.Format(" lpRect=({0:X4},{1:X4},{2:X4},{3:X4})",
                                                            lpRect.left,
                                                            lpRect.right,
                                                            lpRect.top,
                                                            lpRect.bottom));
                                                        stb.Append(string.Format(" scriptItemList=({0:X8},{1:X8})",
                                                            scriptItemList[vidx].a.packed,
                                                            scriptItemList[vidx].a.s.packed));
                                                        if (glyphIndexList != null)
                                                        {
                                                            stb.Append(string.Format(" glyphIndexList=[{0}]",
                                                                glyphIndexList.Length));
                                                        }
                                                        else
                                                        {
                                                            stb.Append(" glyphIndexList=null");
                                                        }

                                                        stb.Append(string.Format(" glyphIndexCount={0}", glyphIndexCount));
                                                        if (advanceWidthList != null)
                                                        {
                                                            stb.Append(string.Format(" advanceWidthList=[{0}]",
                                                                advanceWidthList.Length));
                                                        }
                                                        else
                                                        {
                                                            stb.Append(" advanceWidthList=null");
                                                        }
                                                        //stb.Append(string.Format(" goff=[{0}]",
                                                        //    goff.Length));

                                                        stb.Append(".\r Parameters: ");
                                                        stb.Append(string.Format(" text={0}",
                                                            text == null ? "null" : "\"" + text + "\""));
                                                        stb.Append(string.Format(" font={0},{1}",
                                                            font.Name, font.Size));
                                                        var style = (font.Bold ? "B" : "") + (font.Italic ? "I" : "") + (font.Underline ? "U" : "");
                                                        if (style.Length > 0) stb.Append("," + style);
                                                        stb.Append(string.Format(" bounds=({0},{1},{2},{3})",
                                                            bounds.Left.ToString().Replace(',', '.'),
                                                            bounds.Right.ToString().Replace(',', '.'),
                                                            bounds.Top.ToString().Replace(',', '.'),
                                                            bounds.Bottom.ToString().Replace(',', '.')));
                                                        stb.Append(string.Format(" lineSpacing={0}",
                                                            lineSpacing));
                                                        stb.Append(string.Format(" wordwrap={0}",
                                                            wordWrap ? "true" : "false"));
                                                        stb.Append(string.Format(" rightToLeft={0}",
                                                            rightToLeft ? "true" : "false"));
                                                        stb.Append(string.Format(" scale={0}",
                                                            scale));
                                                        stb.Append(string.Format(" angle={0}",
                                                            angle));
                                                        stb.Append(string.Format(" trim={0}",
                                                            trimming.ToString()));
                                                        stb.Append(string.Format(" lineLimit={0}",
                                                            lineLimit ? "true" : "false"));
                                                        stb.Append(string.Format(" allowHtmlTags={0}",
                                                            allowHtmlTags ? "true" : "false"));
                                                    }
                                                    catch
                                                    {
                                                        ThrowError(19, error);
                                                    }

                                                    var myEx = new Win32Exception(Marshal.GetLastWin32Error());
                                                    throw new Exception(string.Format("TextRender error at step {0}, code #{1:X8}(#{2:X8}): {3} \r{4}",
                                                        19,
                                                        myEx.ErrorCode,
                                                        error,
                                                        myEx.Message,
                                                        stb.ToString()));
#endregion
                                                }
#endregion

                                                //if (currentFontState.hFontScaled != IntPtr.Zero) SelectObject(hdc, currentFontState.hFont);
                                            }

                                            Marshal.FreeHGlobal(goff);

                                            if (useScale)
                                            {
                                                if (currentFontState.hScriptCacheScaled != hScriptCache) currentFontState.hScriptCacheScaled = hScriptCache;
                                            }
                                            else
                                            {
                                                if (currentFontState.hScriptCache != hScriptCache) currentFontState.hScriptCache = hScriptCache;
                                            }

                                            //xpos += runLength * scale + sumJustifyDouble;
                                            if (useScale)
                                            {
                                                xpos += summaryWidthInt * scale + sumJustifyDouble + additionalSpacing;
                                            }
                                            else
                                            {
                                                xpos += runLength + sumJustifyDouble;
                                            }

                                            sumLen += runLength;
                                        }
#endregion
                                    }

                                    yposDecimal += (decimal)textLineInfo.LineHeight;
                                }

#region restore clipping
                                if (resultGetClip == 1)
                                {
                                    SelectClipRgn(hdc, oldRegion);
                                }

                                if (resultGetClip == 0)
                                {
                                    SelectClipRgn(hdc, IntPtr.Zero);
                                }
#endregion

                                DeleteObject(newRegion);
                            }

                            //ScriptFreeCache(ref scriptCache);

#region break text
                            if (!needDraw)
                            {
                                if (linesInfo.Count > linesCountLimit)
                                {
                                    forceWidthAlign = (linesCountLimit > 0) && (linesInfo[linesCountLimit - 1]).NeedWidthAlign;
                                    if (allowHtmlTags)
                                    {
                                        var lineInfo = linesInfo[linesCountLimit];
                                        var statePos = stateOrder[lineInfo.Begin];
                                        var state = stateList[statePos];

                                        var posLine = lineInfo.Begin;
                                        while ((posLine > 0) && (stateOrder[posLine - 1] == stateOrder[posLine])) posLine--;
                                        var offset = lineInfo.Begin - posLine;
                                        var stateText = PrepareStateText(state.Text).ToString();
                                        if (linesCountLimit > 0)
                                        {
                                            string writtenTextEnd = null;
                                            if ((statePos > 0) && (stateList[statePos - 1].TS.Tag.Tag == StiHtmlTag.ListItem) && (state.TS.Tag.Tag == StiHtmlTag.ListItem))
                                            {
                                                if ((stateList[statePos - 1].ListLevels == null) || (state.ListLevels == null) || (stateList[statePos - 1].ListLevels.Count == state.ListLevels.Count))
                                                {
                                                    writtenTextEnd = "<li>" + stateText.Substring(0, offset);
                                                }
                                                else
                                                {
                                                    writtenTextEnd = (state.ListLevels[state.ListLevels.Count - 1] > 0 ? "<ol>" : "<ul>") + stateText.Substring(0, offset);
                                                }
                                            }
                                            else
                                            {
                                                writtenTextEnd = StateToHtml(state, state, stateText.Substring(0, offset), lineInfo.Indent);
                                            }

                                            writtenText = originalText.Substring(0, state.PosBegin) +
                                                          (offset > 0 ? writtenTextEnd : "") +
                                                          (forceWidthAlign ? StiForceWidthAlignTag : "");
                                        }
                                        else
                                        {
                                            writtenText = string.Empty;
                                        }

                                        var stateIndex = stateOrder[lineInfo.Begin] + 1;
                                        breakText = StateToHtml(state, (state.TS.Tag.Tag == StiHtmlTag.ListItem || state.TS.Tag.Tag == StiHtmlTag.P) && (stateIndex < stateList.Length) ? stateList[stateIndex] : state, stateText.Substring(offset), lineInfo.Indent);
                                        if (state.TS.Tag.Tag == StiHtmlTag.ListItem && stateIndex < stateList.Length)
                                        {
                                            breakText += stateList[stateIndex].Text;
                                            stateIndex++;
                                        }

                                        //fix for join parts of state with missing glyphs
                                        while ((stateIndex < stateList.Length) && (stateList[stateIndex].PosBegin == stateList[stateIndex - 1].PosBegin) && (stateList[stateIndex].Text.ToString() != "\n") && (stateList[stateIndex - 1].Text.ToString() != "\n"))
                                        {
                                            breakText += stateList[stateIndex].Text;
                                            stateIndex++;
                                        }

                                        if (stateIndex < stateList.Length)
                                        {
                                            breakText += originalText.Substring(stateList[stateIndex].PosBegin);
                                        }
                                    }
                                    else
                                    {
                                        LineInfo lineInfo = null;
                                        if (linesCountLimit > 0)
                                        {
                                            lineInfo = linesInfo[linesCountLimit - 1];
                                            writtenText = text.Substring(0, lineInfo.End) + (forceWidthAlign ? StiForceWidthAlignTag : "");
                                        }
                                        else
                                        {
                                            writtenText = string.Empty;
                                        }

                                        lineInfo = linesInfo[linesCountLimit];
                                        breakText = text.Substring(lineInfo.Begin);
                                    }
                                }
                            }
#endregion
                        }
                        finally
                        {
                            if (needDraw)
                            {
#region restore world transformation
                                gdiError = SetWorldTransform(hdc, ref oldXForm);
                                if (!gdiError) ThrowError(20);

                                if (oldMapMode != 0)
                                {
                                    var tempMapMode = SetMapMode(hdc, oldMapMode);
                                    if (tempMapMode == 0) ThrowError(21);
                                }

                                var tempGraphMode = SetGraphicsMode(hdc, oldGraphMode);
                                if (tempGraphMode == 0) ThrowError(22);
#endregion
                            }

                            for (var indexFont = 0; indexFont < fontList.Length; indexFont++)
                            {
                                var error = DeleteObject(fontList[indexFont].hFont);
                                if (!error) ThrowError(23);
                                ScriptFreeCache(ref fontList[indexFont].hScriptCache);
                                if (fontList[indexFont].hScriptCacheScaled != IntPtr.Zero)
                                {
                                    ScriptFreeCache(ref fontList[indexFont].hScriptCacheScaled);
                                }
                            }
                        }
                    }
                    finally
                    {
                        g.ReleaseHdc(hdc);
                    }

                    if (runs.Count > 0)
                    {
                        if (outRunsList != null)
                        {
#region Store data for Wpf
                            outRunsList.Clear();
                            outRunsList.AddRange(runs);
                            outFontsList.Clear();
                            outFontsList.AddRange(fontList);
#endregion
                        }
                        else
                        {
#region draw with GDI+
                            var sf = new StringFormat(StringFormat.GenericTypographic);

                            var oldRegion = g.Clip;
                            var newRegion = new RectangleF(
                                (float)(bounds.X),
                                (float)(bounds.Y),
                                (float)(bounds.Width),
                                (float)(bounds.Height));
                            g.SetClip(newRegion, CombineMode.Intersect);
                            var defaultHint = g.TextRenderingHint;
                            //g.TextRenderingHint = TextRenderingHint.AntiAlias;

                            var gstate = g.Save();

#region Rotate
                            g.TranslateTransform(
                                (float)(bounds.X + sideRectOffset * scale + textRect.Width / 2f),
                                (float)(bounds.Y + textRect.Height / 2f));
                            g.RotateTransform((float)(-angle));
                            g.TranslateTransform(
                                (float)(-textSize.Width / 2f),
                                (float)(-textSize.Height / 2f));
#endregion

#region Draw lines
                            for (var indexRun = 0; indexRun < runs.Count; indexRun++)
                            {
                                var runInfo = runs[indexRun];
                                //double baseX = runInfo.XPos + bounds.X + 1.5 * scale;
                                //double baseY = runInfo.YPos + bounds.Y;
                                var baseX = runInfo.XPos;
                                var baseY = runInfo.YPos;

                                var fontDraw = fontList[runInfo.FontIndex].FontBase;
                                if (fontList[runInfo.FontIndex].hFontScaled != IntPtr.Zero) fontDraw = fontList[runInfo.FontIndex].FontScaled;
                                var brush = new SolidBrush(runInfo.TextColor);

                                if (runInfo.BackColor != Color.Transparent)
                                {
                                    double width = 0;
                                    for (var indexChar = 0; indexChar < runInfo.Text.Length; indexChar++)
                                    {
                                        width += runInfo.Widths[indexChar];
                                    }

                                    var rect = new RectangleF();
                                    rect.X = (float)baseX;
                                    rect.Y = (float)baseY;
                                    rect.Width = (float)width;
                                    rect.Height = (float)fontList[runInfo.FontIndex].LineHeight;
                                    g.FillRectangle(new SolidBrush(runInfo.BackColor), rect);
                                }

                                for (var indexChar = 0; indexChar < runInfo.Text.Length; indexChar++)
                                {
                                    var ch = runInfo.Text[indexChar];
                                    //if (ch == ' ') ch = '_';
                                    g.DrawString(
                                        ch.ToString(),
                                        fontDraw,
                                        brush,
                                        (float)(baseX),
                                        (float)(baseY),
                                        sf);
                                    baseX += runInfo.Widths[indexChar];
                                }
                            }
#endregion

                            g.Restore(gstate);

                            g.TextRenderingHint = defaultHint;
                            g.Clip = oldRegion;
#endregion
                        }
                    }
                }
                catch
                {
                }
                finally
                {
                    for (var indexFont = 0; indexFont < fontList.Length; indexFont++)
                    {
                        if (fontList[indexFont].hFontScaled != IntPtr.Zero)
                        {
                            var error = DeleteObject(fontList[indexFont].hFontScaled);
                            if (!error) ThrowError(24);
                        }
                    }
                }
            }

            text = breakText;
            return writtenText;
        }

        private static OUTLINETEXTMETRIC GetOutlineTextMetricsCached(string fontName, FontStyle fontStyle, IntPtr hdc)
        {
            var st = fontName + "*" + (char)(48 + (int)fontStyle);
            var obj = outlineTextMetricsCache[st];
            if (obj == null)
            {
                var otm = new OUTLINETEXTMETRIC();

                lock (lockOutlineTextMetricsCache)
                {
                    using (var tempFont = StiFontCollection.CreateFont(fontName, MaxFontSize, fontStyle))
                    {
                        var hTempFont = tempFont.ToHfont();
                        try
                        {
                            SelectObject(hdc, hTempFont);

                            var cbSize = GetOutlineTextMetrics(hdc, 0, IntPtr.Zero);
                            if (cbSize == 0) ThrowError(1);
                            var buffer = Marshal.AllocHGlobal((int)cbSize);
                            try
                            {
                                if (GetOutlineTextMetrics(hdc, cbSize, buffer) != 0)
                                {
                                    otm = (OUTLINETEXTMETRIC)Marshal.PtrToStructure(buffer, typeof(OUTLINETEXTMETRIC));
                                }
                            }
                            finally
                            {
                                Marshal.FreeHGlobal(buffer);
                            }
                        }
                        finally
                        {
                            var error = DeleteObject(hTempFont);
                            if (!error) ThrowError(2);
                        }
                    }

                    outlineTextMetricsCache[st] = otm;
                }

                return otm;
            }

            return (OUTLINETEXTMETRIC)obj;
        }

        //private static bool isWordWrapSymbol(string text, int pos)
        //{
        //    char sym1 = text[pos];
        //    if (sym1 == '(' || sym1 == '{') return true;
        //    if (pos > 0)
        //    {
        //        char sym2 = text[pos - 1];
        //        if (sym2 == '!' || sym2 == '%' || sym2 == ')' || sym2 == '}' || sym2 == '-' || sym2 == '?') return true;
        //    }
        //    return false;
        //}
        private static bool isWordWrapSymbol2(string text, int pos)
        {
            var sym1 = text[pos];
            //return (sym1 == '(' || sym1 == '{' || sym1 == '!' || sym1 == '%' || sym1 == ')' || sym1 == '}' || sym1 == '-' || sym1 == '?');
            return (sym1 == '!' || sym1 == '%' || sym1 == ')' || sym1 == '}' || sym1 == '-' || sym1 == '?' ||
                    sym1 == '）' || sym1 == '：' || sym1 == '、' || sym1 == '，' || sym1 == '。'); //CJK punctuation
        }

        private static bool isNotWordWrapSymbol(string text, int pos)
        {
            var uc = char.GetUnicodeCategory(text[pos]);
            var result = uc == UnicodeCategory.OtherPunctuation || uc == UnicodeCategory.MathSymbol || uc == UnicodeCategory.CurrencySymbol || uc == UnicodeCategory.OtherSymbol || uc == UnicodeCategory.ConnectorPunctuation || uc == UnicodeCategory.InitialQuotePunctuation || uc == UnicodeCategory.FinalQuotePunctuation;
            if (pos > 0 && isWordWrapSymbol2(text, pos - 1)) result = false;
            return result;
        }

        private static bool isNotWordWrapSymbol2(string text, int pos)
        {
            var uc = char.GetUnicodeCategory(text[pos]);
            var result = uc == UnicodeCategory.OtherPunctuation || uc == UnicodeCategory.MathSymbol || uc == UnicodeCategory.CurrencySymbol || uc == UnicodeCategory.OtherSymbol || uc == UnicodeCategory.ConnectorPunctuation || uc == UnicodeCategory.InitialQuotePunctuation || uc == UnicodeCategory.FinalQuotePunctuation;
            return result || char.IsLetterOrDigit(text[pos]);
        }

        private static bool isCJKWordWrap(string text, int pos)
        {
            if ((pos > 0) && isCJKSymbol(text, pos))
            {
                return isCJKSymbol(text, pos - 1);
            }

            return false;
        }

        private static bool isCJKSymbol(string text, int pos)
        {
            var sym = text[pos];
            return (sym >= 0x4e00 && sym <= 0x9fcc) || (sym >= 0x3400 && sym <= 0x4db5); // || (sym >= 0x20000 && sym <= 0x2a6df); //char is only 16bit
        }
#endregion
#endregion
    }
}