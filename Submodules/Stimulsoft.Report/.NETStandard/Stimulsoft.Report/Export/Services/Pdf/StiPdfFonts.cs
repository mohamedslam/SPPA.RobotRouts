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
using System.Xml;
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.IO;
using System.IO.Compression;
using Stimulsoft.Base;
using System.Security;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
using Graphics = Stimulsoft.Drawing.Graphics;
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Image = Stimulsoft.Drawing.Image;
using StringFormat = Stimulsoft.Drawing.StringFormat;
using Region = Stimulsoft.Drawing.Region;
using CharacterRange = Stimulsoft.Drawing.CharacterRange;
#endif

namespace Stimulsoft.Report.Export
{
    #region Class PdfFontInfo
    internal class PdfFontInfo
    {
        public int[] Widths;
        public string[] CharPdfNames;
        public int CH;
        public int XH;
        public int ASC;
        public int DESC;
        public int tmASC;
        public int tmDESC;
        public int tmExternal;
        public int MacAscend;
        public int MacDescend;
        public int MacLineGap;
        public int LLX;
        public int LLY;
        public int URX;
        public int URY;
        public int StemV;
        public int ItalicAngle;
        public int LineGap;
        public uint fsSelection;
        public int UnderscoreSize;
        public int UnderscorePosition;
        public int StrikeoutSize;
        public int StrikeoutPosition;
        public ushort[] UnicodeMap;
        public ushort[] UnicodeMapBack;
        public ushort[] GlyphList;
        public ushort[] GlyphBackList;
        public ushort[] GlyphRtfList;
        public int[] SymsToPDF;
        public int MappedSymbolsCount;
        public bool NeedSyntBold;
        public bool NeedSyntItalic;
        public ushort[] GlyphWidths;
        public byte[] ChildFontsMap;
        public bool UseUnicode;
    }
    #endregion

    #region Class PdfFonts
    [SuppressUnmanagedCodeSecurity]
    internal partial class PdfFonts : PdfFontInfo
    {
        #region DllImport

        [DllImport("GDI32.dll", SetLastError = true)]
        internal static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern uint GetOutlineTextMetrics(IntPtr hdc, uint cbData, IntPtr lpOTM);

        //[DllImport("gdi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        //internal static extern uint GetTextMetrics(IntPtr hdc, IntPtr lptm);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern uint GetGlyphIndices(
            IntPtr hdc,             // handle to DC
            string lpstr,           // string to convert
            int c,                  // number of characters in string
            [In, Out] ushort[] pgi, // array of glyph indices
            uint fl);           // glyph options

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool GetCharABCWidthsI(
            IntPtr hdc,             // handle to DC
            uint giFirst,           // first glyph index in range
            uint cgi,               // count of glyph indices in range
            [In, Out] ushort[] pgi,  // array of glyph indices
            [In, Out] ABC[] lpabc); // array of character widths

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern uint GetFontData(
            IntPtr hdc,         // handle to DC
            uint dwTable,       // metric table name
            uint dwOffset,      // offset into table
            [In, Out] byte[] lpvBuffer, // buffer for returned data
            uint cbData         // length of data
            );

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern uint GetFontData(
            IntPtr hdc,         // handle to DC
            uint dwTable,       // metric table name
            uint dwOffset,      // offset into table
            [In, Out] IntPtr lpvBuffer, // buffer for returned data
            uint cbData         // length of data
            );

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool DeleteObject(IntPtr objectHandle);

        #endregion

        #region GDI structures

        internal const uint GDI_ERROR = 0xFFFFFFFF;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct ABC
        {
            public int abcA;
            public uint abcB;
            public int abcC;

            internal ABC(int abcA, uint abcB, int abcC)
            {
                this.abcA = abcA;
                this.abcB = abcB;
                this.abcC = abcC;
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct ABCFLOAT
        {
            public float abcfA;
            public float abcfB;
            public float abcfC;

            internal ABCFLOAT(float abcfA, float abcfB, float abcfC)
            {
                this.abcfA = abcfA;
                this.abcfB = abcfB;
                this.abcfC = abcfC;
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct POINT
        {
            public int x;
            public int y;

            internal POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;

            internal RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct PANOSE
        {
            public byte bFamilyType;
            public byte bSerifStyle;
            public byte bWeight;
            public byte bProportion;
            public byte bContrast;
            public byte bStrokeVariation;
            public byte ArmStyle;
            public byte bLetterform;
            public byte bMidline;
            public byte bXHeight;

            internal PANOSE(
                byte bFamilyType,
                byte bSerifStyle,
                byte bWeight,
                byte bProportion,
                byte bContrast,
                byte bStrokeVariation,
                byte ArmStyle,
                byte bLetterform,
                byte bMidline,
                byte bXHeight)
            {
                this.bFamilyType = bFamilyType;
                this.bSerifStyle = bSerifStyle;
                this.bWeight = bWeight;
                this.bProportion = bProportion;
                this.bContrast = bContrast;
                this.bStrokeVariation = bStrokeVariation;
                this.ArmStyle = ArmStyle;
                this.bLetterform = bLetterform;
                this.bMidline = bMidline;
                this.bXHeight = bXHeight;
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct TEXTMETRIC
        {
            public int tmHeight;
            public int tmAscent;
            public int tmDescent;
            public int tmInternalLeading;
            public int tmExternalLeading;
            public int tmAveCharWidth;
            public int tmMaxCharWidth;
            public int tmWeight;
            public int tmOverhang;
            public int tmDigitizedAspectX;
            public int tmDigitizedAspectY;
            public char tmFirstChar;
            public char tmLastChar;
            public char tmDefaultChar;
            public char tmBreakChar;
            public byte tmItalic;
            public byte tmUnderlined;
            public byte tmStruckOut;
            public byte tmPitchAndFamily;
            public byte tmCharSet;

            internal TEXTMETRIC(
                int tmHeight,
                int tmAscent,
                int tmDescent,
                int tmInternalLeading,
                int tmExternalLeading,
                int tmAveCharWidth,
                int tmMaxCharWidth,
                int tmWeight,
                int tmOverhang,
                int tmDigitizedAspectX,
                int tmDigitizedAspectY,

                char tmFirstChar,
                char tmLastChar,
                char tmDefaultChar,
                char tmBreakChar,

                byte tmItalic,
                byte tmUnderlined,
                byte tmStruckOut,
                byte tmPitchAndFamily,
                byte tmCharSet)
            {
                this.tmHeight = tmHeight;
                this.tmAscent = tmAscent;
                this.tmDescent = tmDescent;
                this.tmInternalLeading = tmInternalLeading;
                this.tmExternalLeading = tmExternalLeading;
                this.tmAveCharWidth = tmAveCharWidth;
                this.tmMaxCharWidth = tmMaxCharWidth;
                this.tmWeight = tmWeight;
                this.tmOverhang = tmOverhang;
                this.tmDigitizedAspectX = tmDigitizedAspectX;
                this.tmDigitizedAspectY = tmDigitizedAspectY;

                this.tmFirstChar = tmFirstChar;
                this.tmLastChar = tmLastChar;
                this.tmDefaultChar = tmDefaultChar;
                this.tmBreakChar = tmBreakChar;

                this.tmItalic = tmItalic;
                this.tmUnderlined = tmUnderlined;
                this.tmStruckOut = tmStruckOut;
                this.tmPitchAndFamily = tmPitchAndFamily;
                this.tmCharSet = tmCharSet;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct OUTLINETEXTMETRIC
        {
            public uint otmSize;
            public TEXTMETRIC otmTextMetrics;
            public byte otmFiller;
            public PANOSE otmPanoseNumber;
            public uint otmfsSelection;
            public uint otmfsType;
            public int otmsCharSlopeRise;
            public int otmsCharSlopeRun;
            public int otmItalicAngle;
            public uint otmEMSquare;
            public int otmAscent;
            public int otmDescent;
            public uint otmLineGap;
            public uint otmsCapEmHeight;
            public uint otmsXHeight;
            public RECT otmrcFontBox;
            public int otmMacAscent;
            public int otmMacDescent;
            public uint otmMacLineGap;
            public uint otmusMinimumPPEM;
            public POINT otmptSubscriptSize;
            public POINT otmptSubscriptOffset;
            public POINT otmptSuperscriptSize;
            public POINT otmptSuperscriptOffset;
            public uint otmsStrikeoutSize;
            public int otmsStrikeoutPosition;
            public int otmsUnderscoreSize;
            public int otmsUnderscorePosition;
            public IntPtr otmpFamilyName;   //string offset
            public IntPtr otmpFaceName;     //string offset
            public IntPtr otmpStyleName;    //string offset
            public IntPtr otmpFullName;     //string offset
        }
        #endregion

        #region Struct pfontInfo
        internal struct pfontInfo
        {
            public string Name;
            public string PdfName;
            public bool Bold;
            public bool Italic;
            public int Number;
            public Font Font;
            public int ParentFontNumber;
            public byte[] ChildFontsMap;
            public bool UseUnicode;
        }
        #endregion

        #region Fields
        internal const ushort FirstMappedSymbol = 64;
        internal const float Factor = 0.366f;

        //max symbols for output. StandardMode=255, UnicodeMode=32767
        internal ushort MaxSymbols = 32767;
        internal bool forceUseUnicode = true;

        private OUTLINETEXTMETRIC otm; // = new OUTLINETEXTMETRIC();
        private int[] WIDTHS;
        internal PdfFontInfo[] fonts = null;
        public ArrayList UnicodeMapsList = null;
        public ArrayList GlyphMapsList = null;
        public bool standardPdfFonts = true;
        public ArrayList fontList;
        public Hashtable fontHash = new Hashtable();
        private FontsInfoStore fontsInfoStore = null;
        internal StiOpenTypeHelper OpenTypeHelper = new StiOpenTypeHelper();

        private static ushort[,] StandardFontWidths = null;
        private static string[] StandardFontCharsNames = null;
        private static int[,] StandardFontInfo = null;
        private static int[] uniMap = null;

        private static Hashtable FontsDataCache = new Hashtable();
        private Hashtable localFontsDataCache = new Hashtable();
        #endregion

        #region Property CurrentFont
        private int currentFont = -1;
        public int CurrentFont
        {
            get
            {
                return currentFont;
            }
            set
            {
                if (currentFont != value)
                {
                    currentFont = value;

                    if ((fonts != null) && (fonts.Length > 0))
                    {
                        Widths = fonts[currentFont].Widths;
                        CH = fonts[currentFont].CH;
                        XH = fonts[currentFont].XH;
                        ASC = fonts[currentFont].ASC;
                        DESC = fonts[currentFont].DESC;
                        tmASC = fonts[currentFont].tmASC;
                        tmDESC = fonts[currentFont].tmDESC;
                        tmExternal = fonts[currentFont].tmExternal;
                        MacAscend = fonts[currentFont].MacAscend;
                        MacDescend = fonts[currentFont].MacDescend;
                        MacLineGap = fonts[currentFont].MacLineGap;
                        LLX = fonts[currentFont].LLX;
                        LLY = fonts[currentFont].LLY;
                        URX = fonts[currentFont].URX;
                        URY = fonts[currentFont].URY;
                        StemV = fonts[currentFont].StemV;
                        ItalicAngle = fonts[currentFont].ItalicAngle;
                        LineGap = fonts[currentFont].LineGap;
                        fsSelection = fonts[currentFont].fsSelection;
                        UnderscoreSize = fonts[currentFont].UnderscoreSize;
                        UnderscorePosition = fonts[currentFont].UnderscorePosition;
                        StrikeoutSize = fonts[currentFont].StrikeoutSize;
                        StrikeoutPosition = fonts[currentFont].StrikeoutPosition;
                        UnicodeMap = fonts[currentFont].UnicodeMap;
                        UnicodeMapBack = fonts[currentFont].UnicodeMapBack;
                        GlyphList = fonts[currentFont].GlyphList;
                        GlyphBackList = fonts[currentFont].GlyphBackList;
                        GlyphRtfList = fonts[currentFont].GlyphRtfList;
                        SymsToPDF = fonts[currentFont].SymsToPDF;
                        CharPdfNames = fonts[currentFont].CharPdfNames;
                        MappedSymbolsCount = fonts[currentFont].MappedSymbolsCount;
                        NeedSyntBold = fonts[currentFont].NeedSyntBold;
                        NeedSyntItalic = fonts[currentFont].NeedSyntItalic;
                        GlyphWidths = fonts[currentFont].GlyphWidths;
                        ChildFontsMap = fonts[currentFont].ChildFontsMap;
                        UseUnicode = fonts[currentFont].UseUnicode;
                    }
                }
            }
        }
        #endregion

        #region Methods

        #region MakeGlyphBackMap
        public ushort[] MakeGlyphBackMap(Font font, bool useFontInfoStore)
        {
            if (font == null) return null;
            ushort[] backMap = null;

            FontsInfoStore.FontInfo fontInfo = null;
            if (useFontInfoStore || StiOptions.Export.Pdf.ForceUseFontsInfoStoreForWysiwyg)
            {
                fontInfo = fontsInfoStore.GetFontInfo(font);
            }
            if (fontInfo == null)
            {
                if (StiOptions.Engine.FullTrust)
                {
                    #region Get data from GDI
                    object obj = Stimulsoft.Base.Drawing.StiTextRenderer.GetFontGlyphs(font);
                    ushort[] rtcode = obj as ushort[];
                    if (rtcode == null || rtcode.Length == 0) ThrowError(1);

                    //make table
                    ushort[] tempBackMap = new ushort[65536];
                    for (int index = 65535; index >= 32; index--)
                    {
                        int glyphIndex = rtcode[index];
                        tempBackMap[glyphIndex] = (ushort)index;
                    }

                    backMap = tempBackMap;
                    #endregion
                }
                else
                {
                    #region Get data directly from ttf-font
                    byte[] fontData = null;
                    GetFontDataBuf(font, out fontData, false);
                    var cmap = OpenTypeHelper.GetCharToGlyphTable(fontData, font.Name);

                    ushort[] tempBackMap = new ushort[65536];
                    for (uint index = 65535; index >= 32; index--)
                    {
                        uint glyphIndex = cmap.ContainsKey(index) ? cmap[index] : 0xFFFF;
                        tempBackMap[glyphIndex] = (ushort)index;
                    }
                    backMap = tempBackMap;
                    #endregion
                }
            }
            else
            {
                #region get data from store
                ushort[] rtcode = fontInfo.Glyphs;

                //make table
                ushort[] tempBackMap = new ushort[65536];
                for (int index = 65535 - 1; index >= 32; index--)
                {
                    int glyphIndex = rtcode[index];
                    tempBackMap[glyphIndex] = (ushort)index;
                }
                backMap = tempBackMap;
                #endregion
            }
            return backMap;
        }
        #endregion

        #region GetFontMetrics
        public void GetFontMetrics(Font font, PdfFontInfo currentFontInfo, ushort[] glyphMap, bool isWpf)
        {
            otm = new OUTLINETEXTMETRIC();
            WIDTHS = new int[MaxSymbols + 1];
            if (font == null) return;

            var isGdi = true;
#if STIDRAWING
            isGdi = Graphics.GraphicsEngine == Stimulsoft.Drawing.GraphicsEngine.Gdi;
#endif

            bool internalFont = IsFontStimulsoft(font.Name) || SignatureFonts.StiSignatureFontsHelper.IsSignatureFont(font.Name);
            if (StiOptions.Engine.FullTrust && StiOptions.Export.Pdf.AllowInvokeWindowsLibraries && !internalFont && isGdi)
            {
                GetFontMetricsFullTrust(font, currentFontInfo, glyphMap, isWpf);
            }
            else
            {
                int errorStep = 0;
                try
                {
                    //using (Font tempFont = StiFontCollection.CreateFont(font.Name, 2048, font.Style))
                    using (Font tempFont = new Font(font.FontFamily, 2048, font.Style))
                    using (Image img = new Bitmap(1, 1))
                    using (Graphics g = Graphics.FromImage(img))
                    using (StringFormat sf = new StringFormat(StringFormat.GenericDefault))
                    {
                        sf.FormatFlags |= StringFormatFlags.NoWrap;

                        //get text metrics
                        string str = "0\n1";
                        CharacterRange[] cr = new CharacterRange[1];
                        cr[0] = new CharacterRange(2, 1);
                        sf.SetMeasurableCharacterRanges(cr);

                        errorStep = 10;

                        Region[] ranges = g.MeasureCharacterRanges(str, tempFont, new RectangleF(0, 0, 10000000, 10000000), sf);
                        RectangleF rect = ranges[0].GetBounds(g);

                        errorStep = 20;

                        double symHeight = rect.Height;
                        double lineHeight = rect.Y;
                        double wwBase = 0;

                        if (!(StiOptions.Export.Pdf.AllowInvokeWindowsLibraries && isGdi))
                        {
                            var cfHeight = isGdi ? 0.987 : 1;
                            symHeight = g.MeasureString("D\r\nD\r\nD\r\nD\r\nD\r\nD\r\nD\r\nD\r\nD\r\nD", tempFont, 10000000, sf).Height / 10 * cfHeight;
                            lineHeight = symHeight;
                            wwBase = g.MeasureString("DD", tempFont, 10000000, sf).Width;
                        }

                        if (StiOptions.Engine.TextLineSpacingScale != 1)
                        {
                            symHeight *= StiOptions.Engine.TextLineSpacingScale;
                            lineHeight *= StiOptions.Engine.TextLineSpacingScale;
                        }

                        double ascendCf = 0.8;
                        double descendCf = 0.2;
                        if (tempFont.Name == "Teddy Bear")
                        {
                            ascendCf = 0.7;
                            descendCf = 0.3;
                        }

                        otm.otmTextMetrics.tmAscent = (int)(symHeight * ascendCf);
                        otm.otmTextMetrics.tmDescent = (int)(symHeight * descendCf);
                        otm.otmTextMetrics.tmExternalLeading = (int)(lineHeight - symHeight);
                        otm.otmsStrikeoutPosition = (int)(symHeight * 0.25);
                        otm.otmsStrikeoutSize = (uint)(symHeight * 0.05);
                        otm.otmsUnderscorePosition = -(int)(symHeight * 0.1);
                        otm.otmsUnderscoreSize = (int)(symHeight * 0.07);

                        errorStep = 30;

                        //get symbols widths
                        StringBuilder sb = new StringBuilder();
                        for (int index = 0; index < currentFontInfo.MappedSymbolsCount; index++)
                        {
                            sb.Append((char)currentFontInfo.UnicodeMapBack[index]);
                        }
                        str = sb.ToString();

                        ranges = new Region[str.Length];

                        errorStep = 40;

                        if (StiOptions.Export.Pdf.AllowInvokeWindowsLibraries && isGdi)
                        {
                            int indexRange = 0;
                            while (indexRange < str.Length)
                            {
                                int count = str.Length - indexRange;
                                if (count > 32) count = 32;

                                cr = new CharacterRange[count];
                                for (int index2 = 0; index2 < count; index2++)
                                {
                                    cr[index2].First = indexRange + index2;
                                    cr[index2].Length = 1;
                                }
                                sf.SetMeasurableCharacterRanges(cr);

                                g.MeasureCharacterRanges(str, tempFont, new RectangleF(0, 0, 10000000, 10000000), sf)
                                    .CopyTo(ranges, indexRange);

                                indexRange += count;
                            }
                        }

                        errorStep = 50;

                        for (int index = 0; index < currentFontInfo.MappedSymbolsCount; index++)
                        {
                            double fl = 0;
                            if (StiOptions.Export.Pdf.AllowInvokeWindowsLibraries && isGdi)
                            {
                                fl = ranges[index].GetBounds(g).Width * Factor * StiDpiHelper.GraphicsScale;
                            }
                            else
                            {
                                var ww = g.MeasureString("D" + str.Substring(index, 1) + "D", tempFont, 10000000, sf).Width - wwBase;

                                if (isGdi)
                                {
                                    fl = ww * 0.971 * Factor * StiDpiHelper.GraphicsScale;
                                }
                                else
                                {
                                    fl = ww * 0.99985 * Factor * StiDpiHelper.GraphicsScale;
                                }
                            }
                            if (char.GetUnicodeCategory(str[index]) == global::System.Globalization.UnicodeCategory.NonSpacingMark) fl = 0;
                            WIDTHS[index] = (int)(Math.Round(fl));
                        }

                        errorStep = 60;

                        #region Get glyphs list
                        if (currentFontInfo.UseUnicode)
                        {
                            byte[] fontData = null;
                            GetFontDataBuf(font, out fontData, glyphMap[0xffff] == 0);
                            if (fontData == null)
                                throw new Exception($"Font data not found.");

                            errorStep = 70;

                            var cmap = OpenTypeHelper.GetCharToGlyphTable(fontData, font.Name);
                            if (cmap == null || cmap.Count == 0)
                                throw new Exception($"CMAP table not found.");

                            errorStep = 80;

                            //fix for some fonts
                            bool isSymbolFont = IsFontSymbolic(font.Name);
                            if (!isSymbolFont && cmap.Count <= 224)
                            {
                                uint minv = 65535;
                                uint maxv = 0;
                                foreach (var de in cmap)
                                {
                                    if (de.Key < minv) minv = de.Key;
                                    if (de.Key > maxv) maxv = de.Key;
                                }
                                if (minv >= 0xF020 && maxv <= 0xF0FF) isSymbolFont = true;
                            }

                            uint offset = isSymbolFont ? 0xF000u : 0u;

                            for (int index = 0; index < currentFontInfo.MappedSymbolsCount; index++)
                            {
                                uint charCode = currentFontInfo.UnicodeMapBack[index];
                                if ((charCode >= 32) && (charCode <= 255))
                                {
                                    charCode += offset;
                                }
                                currentFontInfo.GlyphList[index] = (ushort)(cmap.ContainsKey(charCode) ? cmap[charCode] : 0xFFFF);
                            }
                        }
                        #endregion

                        #region Check for NeedSynt
                        if (StiFontCollection.IsCustomFont(font.Name))
                        {
                            if (font.Bold && font.Italic)
                            {
                                if (!StiFontCollection.IsStyleAvailable(font.Name, FontStyle.Bold | FontStyle.Italic))
                                {
                                    if (StiFontCollection.IsStyleAvailable(font.Name, FontStyle.Bold))
                                    {
                                        NeedSyntItalic = true;
                                    }
                                    else if (StiFontCollection.IsStyleAvailable(font.Name, FontStyle.Italic))
                                    {
                                        NeedSyntBold = true;
                                    }
                                    else
                                    {
                                        NeedSyntBold = true;
                                        NeedSyntItalic = true;
                                    }
                                }
                            }
                            else if (font.Bold)
                            {
                                if (!StiFontCollection.IsStyleAvailable(font.Name, FontStyle.Bold))
                                {
                                    NeedSyntBold = true;
                                }
                            }
                            else if (font.Italic)
                            {
                                if (!StiFontCollection.IsStyleAvailable(font.Name, FontStyle.Italic))
                                {
                                    NeedSyntItalic = true;
                                }
                            }
                        }
                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    string str = string.Format("Exception in PdfFonts.GetFontMetrics, font '{0}' step #{1}: '{2}'", font.Name, errorStep, ex.Message);
                    StiLogService.Write(this.GetType(), str);
                    StiLogService.Write(this.GetType(), ex.Message);
                    throw new Exception(str);
                }
            }
        }

        public void GetFontMetricsFullTrust(Font font, PdfFontInfo currentFontInfo, ushort[] glyphMap, bool isWpf)
        {
            FontsInfoStore.FontInfo fontInfo = null;
            if (glyphMap[0xffff] == 0 || StiOptions.Export.Pdf.ForceUseFontsInfoStoreForWysiwyg)
            {
                fontInfo = fontsInfoStore.GetFontInfo(font);
            }
            if (fontInfo == null)
            {
                #region get data from GDI
                Graphics gr = StiOptions.Export.Pdf.AllowInvokeWindowsLibraries ? Graphics.FromHwnd(IntPtr.Zero) : Graphics.FromImage(new Bitmap(1, 1));
                try
                {
                    IntPtr hdc = gr.GetHdc();
                    try
                    {
                        //using (Font tempFont = StiFontCollection.CreateFont(font.Name, 2048, font.Style))
                        using (Font tempFont = new Font(font.FontFamily, 2048, font.Style))
                        {
                            IntPtr hFont = tempFont.ToHfont();
                            try
                            {
                                IntPtr oldObj = SelectObject(hdc, hFont);

                                uint cbSize = GetOutlineTextMetrics(hdc, 0, IntPtr.Zero);
                                if (cbSize == 0) ThrowError(3);

                                IntPtr buffer = Marshal.AllocHGlobal((int)cbSize);
                                try
                                {
                                    if (GetOutlineTextMetrics(hdc, cbSize, buffer) != 0)
                                    {
                                        otm =
                                            (OUTLINETEXTMETRIC)
                                                Marshal.PtrToStructure(buffer, typeof(OUTLINETEXTMETRIC));

                                        long otmLong = buffer.ToInt64();
                                        string fullName =
                                            Marshal.PtrToStringUni(new IntPtr(otmLong + (Int64)otm.otmpFullName));
                                        string familyName =
                                            Marshal.PtrToStringUni(new IntPtr(otmLong + (Int64)otm.otmpFamilyName));
                                        //string faceName = Marshal.PtrToStringUni(new IntPtr(otmLong + (Int64)otm.otmpFaceName));
                                        string styleName = Marshal.PtrToStringUni(new IntPtr(otmLong + (Int64)otm.otmpStyleName));

                                        bool isBold = (fullName.ToLowerInvariant().IndexOf("bold", StringComparison.InvariantCulture) != -1) ||
                                            (styleName.ToLowerInvariant().IndexOf("bold", StringComparison.InvariantCulture) != -1) ||
                                            (styleName.ToLowerInvariant().IndexOf("полужирный", StringComparison.InvariantCulture) != -1);
                                        bool isItalic = (fullName.ToLowerInvariant().IndexOf("italic", StringComparison.InvariantCulture) != -1) ||
                                            (styleName.ToLowerInvariant().IndexOf("italic", StringComparison.InvariantCulture) != -1) ||
                                            (styleName.ToLowerInvariant().IndexOf("курсив", StringComparison.InvariantCulture) != -1);

                                        bool needSyntBold = false;
                                        if (font.Bold)
                                        {
                                            if (!isBold) needSyntBold = true;
                                            if (familyName.ToLowerInvariant().IndexOf("bold", StringComparison.InvariantCulture) != -1) needSyntBold = true;
                                        }
                                        bool needSyntItalic = false;
                                        if (font.Italic)
                                        {
                                            if (!isItalic) needSyntItalic = true;
                                            //if (familyName.ToLowerInvariant().IndexOf("italic", StringComparison.InvariantCulture) != -1) needSyntItalic = true;
                                        }
                                        NeedSyntBold = needSyntBold;
                                        NeedSyntItalic = needSyntItalic;

                                        if (font.Name == "Cambria Math")
                                        {
                                            otm.otmTextMetrics.tmAscent = (int)(otm.otmTextMetrics.tmAscent / 4.7);
                                            otm.otmTextMetrics.tmDescent = (int)(otm.otmTextMetrics.tmDescent / 4.7);
                                        }

                                    }
                                }
                                finally
                                {
                                    Marshal.FreeHGlobal(buffer);
                                }

                                //fix for some fonts
                                int offset = 0;
                                if (IsFontSymbolic(font.Name)) offset = 0xF000;

                                StringBuilder sb = new StringBuilder();
                                for (int index = 0; index < currentFontInfo.MappedSymbolsCount; index++)
                                {
                                    int charCode = currentFontInfo.UnicodeMapBack[index];
                                    if ((charCode >= 32) && (charCode <= 255))
                                    {
                                        sb.Append((char)(offset + charCode));
                                    }
                                    else
                                    {
                                        sb.Append((char)charCode);
                                    }
                                }

                                string str = sb.ToString();
                                int count = str.Length;
                                ushort[] rtcode = new ushort[count];
                                uint res = GetGlyphIndices(hdc, str, count, rtcode, 1);
                                if (res == GDI_ERROR) ThrowError(4);

                                for (int index = 0; index < currentFontInfo.MappedSymbolsCount; index++)
                                {
                                    currentFontInfo.GlyphList[index] = rtcode[index];
                                }

                                #region add glyphs from glyphMap
                                if (glyphMap != null && glyphMap[0xFFFF] != 0)
                                {
                                    int glyphCounter = 0;
                                    for (int index = 0; index < 65535; index++)
                                    {
                                        if ((glyphMap[index] != 0) && (currentFontInfo.GlyphBackList[index] == 0))
                                        {
                                            glyphCounter++;
                                        }
                                    }
                                    int mapPos = rtcode.Length;
                                    ushort[] newRtCode = new ushort[mapPos + glyphCounter];
                                    Array.Copy(rtcode, 0, newRtCode, 0, mapPos);
                                    for (int index = 0; index < 65535; index++)
                                    {
                                        if ((glyphMap[index] != 0) && (currentFontInfo.GlyphBackList[index] == 0))
                                        {
                                            newRtCode[mapPos] = (ushort)index;
                                            mapPos++;
                                        }
                                    }
                                    rtcode = newRtCode;
                                    count = newRtCode.Length;
                                }
                                #endregion

                                ABC[] abc = new ABC[count];
                                bool gdiErr = GetCharABCWidthsI(hdc, 0, (uint)count, rtcode, abc);
                                if (gdiErr == false) ThrowError(5);

                                #region store glyphs widths in glyphMap

                                double scaleF = isWpf || StiDpiHelper.NeedFontScaling ? StiDpiHelper.GraphicsScale : 1;

                                if (glyphMap != null && glyphMap[0xFFFF] != 0)
                                {
                                    int mapPos = currentFontInfo.MappedSymbolsCount;
                                    for (int index = 0; index < 65536; index++)
                                    {
                                        if ((glyphMap[index] != 0) && (currentFontInfo.GlyphBackList[index] == 0))
                                        {
                                            double fl = (abc[mapPos].abcA + abc[mapPos].abcB + abc[mapPos].abcC) * Factor * scaleF;
                                            glyphMap[index] = (ushort)(Math.Round(fl));
                                            mapPos++;
                                        }
                                    }
                                    currentFontInfo.GlyphWidths = glyphMap;
                                }
                                #endregion

                                for (int index = 0; index < currentFontInfo.MappedSymbolsCount; index++)
                                {
                                    double fl = (abc[index].abcA + abc[index].abcB + abc[index].abcC) * Factor * scaleF;
                                    WIDTHS[index] = (int)(Math.Round(fl));
                                }

                                SelectObject(hdc, oldObj);
                            }
                            finally
                            {
                                bool error = DeleteObject(hFont);
                                //if (!error) ThrowError(6);
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
                }
                #endregion
            }
            else
            {
                #region get data from store
                int cbSize = fontInfo.OTM.Length;
                IntPtr buffer = Marshal.AllocHGlobal(cbSize);
                try
                {
                    Marshal.Copy(fontInfo.OTM, 0, buffer, cbSize);
                    otm = (OUTLINETEXTMETRIC)Marshal.PtrToStructure(buffer, typeof(OUTLINETEXTMETRIC));
                    NeedSyntBold = fontInfo.NeedSyntBold;
                    NeedSyntItalic = fontInfo.NeedSyntItalic;
                }
                finally
                {
                    Marshal.FreeHGlobal(buffer);
                }

                ushort[] rtcode = fontInfo.Glyphs;
                for (int index = 0; index < currentFontInfo.MappedSymbolsCount; index++)
                {
                    int charIndex = currentFontInfo.UnicodeMapBack[index];
                    currentFontInfo.GlyphList[index] = rtcode[charIndex];
                    WIDTHS[index] = fontInfo.Widths[charIndex];
                }
                #endregion
            }
        }
        #endregion

        #region GetFontDataFromGdi
        private void GetFontDataFromGdi(Font font, out byte[] buff, bool useFontInfoStore)
        {
            buff = new byte[0];
            if (font == null) return;

            if (IsFontStimulsoft(font.Name))
            {
                using (var fontStream = typeof(Helpers.StiFontIconsHelper).Assembly.GetManifestResourceStream("Stimulsoft.Base.FontIcons.Stimulsoft.ttf"))
                {
                    if (fontStream != null)
                    {
                        buff = new byte[fontStream.Length];
                        fontStream.Read(buff, 0, buff.Length);
                        return;
                    }
                }
            }

            if (SignatureFonts.StiSignatureFontsHelper.IsSignatureFont(font.Name))
            {
                var dataSign = SignatureFonts.StiSignatureFontsHelper.GetFontDataByFamilyName(font.Name);
                if (dataSign != null) buff = dataSign;
                return;
            }

            FontsInfoStore.FontInfo fontInfo = null;
            if (useFontInfoStore || StiOptions.Export.Pdf.ForceUseFontsInfoStoreForWysiwyg)
            {
                fontInfo = fontsInfoStore.GetFontInfo(font);
            }
            if (fontInfo == null)
            {
                var fontData = StiFontCollection.GetCustomFontData(font.Name, font.Style);
                if (fontData != null)
                {
                    buff = fontData;
                }
                else
                {
                    var isGdi = true;
#if STIDRAWING
                    isGdi = Graphics.GraphicsEngine == Stimulsoft.Drawing.GraphicsEngine.Gdi;
#endif
                    if (StiOptions.Engine.FullTrust && StiOptions.Export.Pdf.AllowInvokeWindowsLibraries && isGdi)
                    {
                        GetFontDataFromGdiFullTrust(font, out buff);
                    }
                }
            }
            else
            {
                #region get data from store
                buff = fontInfo.Data;
                #endregion
            }
        }

        private static void GetFontDataFromGdiFullTrust(Font font, out byte[] buff)
        {
            Graphics gr = StiOptions.Export.Pdf.AllowInvokeWindowsLibraries ? Graphics.FromHwnd(IntPtr.Zero) : Graphics.FromImage(new Bitmap(1, 1));
            try
            {
                IntPtr hdc = gr.GetHdc();
                try
                {
                    //using (Font tempFont = StiFontCollection.CreateFont(font.Name, 2048, font.Style))
                    using (Font tempFont = new Font(font.FontFamily, 2048, font.Style))
                    {
                        IntPtr hFont = tempFont.ToHfont();
                        try
                        {
                            IntPtr oldObj = SelectObject(hdc, hFont);

                            //check for .ttc (TrueTypeCollection)
                            uint cbData = GetFontData(hdc, 0x66637474, 0, IntPtr.Zero, 0);
                            if (cbData == GDI_ERROR)
                            {
                                //font is simple TrueType
                                cbData = GetFontData(hdc, 0, 0, IntPtr.Zero, 0);
                                if (cbData == GDI_ERROR) ThrowError(8);
                                buff = new byte[cbData];
                                uint res = GetFontData(hdc, 0, 0, buff, cbData);
                                if (res == GDI_ERROR) ThrowError(9);
                                if (res != cbData) ThrowError(10);
                            }
                            else
                            {
                                //font is TrueTypeCollection
                                buff = new byte[cbData];
                                uint res = GetFontData(hdc, 0x66637474, 0, buff, cbData);
                            }

                            SelectObject(hdc, oldObj);
                        }
                        finally
                        {
                            bool error = DeleteObject(hFont);
                            //if (!error) ThrowError(7);
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
            }
        }
        #endregion

        #region InitFontsData()
        public void InitFontsData(bool isWpf)
        {
            //make additional fonts for standard mode
            int fontsCount = fontList.Count;
            for (int indexFont = 0; indexFont < fontsCount; indexFont++)
            {
                pfontInfo currFont = (pfontInfo)fontList[indexFont];

                bool useUnicode = forceUseUnicode;
                if (useUnicode)
                {
                    byte[] buff = null;
                    GetFontDataBuf(currFont.Font, out buff, false, true);
                    if (buff == null || buff.Length == 0)
                    {
                        useUnicode = false;
                    }
                }
                currFont.UseUnicode = useUnicode;
                fontList[indexFont] = currFont;

                if (!useUnicode)
                {
                    ushort[] map = (ushort[])UnicodeMapsList[indexFont];
                    int mapSymbol = FirstMappedSymbol;
                    for (int indexChar = FirstMappedSymbol; indexChar < 65536; indexChar++)
                    {
                        if (map[indexChar] > 0) mapSymbol++;
                    }
                    if (mapSymbol > 255)
                    {
                        #region add fonts
                        currFont = (pfontInfo)fontList[indexFont];
                        ushort[] newMap = null;
                        byte[] fontsMap = new byte[65536];
                        byte currentFontNumber = (byte)fontList.Count;
                        for (int indexChar = 0; indexChar < FirstMappedSymbol; indexChar++) fontsMap[indexChar] = currentFontNumber;

                        bool needAddNewFontInfo = true;
                        mapSymbol = FirstMappedSymbol;
                        for (int indexChar = FirstMappedSymbol; indexChar < 65536; indexChar++)
                        {
                            if (needAddNewFontInfo)
                            {
                                pfontInfo fi = new pfontInfo();
                                fi.Name = currFont.Name;
                                fi.PdfName = currFont.PdfName;
                                fi.Bold = currFont.Bold;
                                fi.Italic = currFont.Italic;
                                fi.Number = currFont.Number;
                                fi.Font = currFont.Font;
                                fi.ParentFontNumber = indexFont;
                                fi.UseUnicode = useUnicode;
                                fontList.Add(fi);

                                newMap = new ushort[65536];
                                Array.Copy(map, 0, newMap, 0, 64);
                                UnicodeMapsList.Add(newMap);
                                GlyphMapsList.Add(new ushort[65536]);
                                currentFontNumber = (byte)(fontList.Count - 1);
                                mapSymbol = FirstMappedSymbol;
                                needAddNewFontInfo = false;
                            }
                            if (map[indexChar] > 0)
                            {
                                newMap[indexChar] = 1;
                                fontsMap[indexChar] = currentFontNumber;
                                mapSymbol++;
                                if (mapSymbol == 254) needAddNewFontInfo = true;
                            }
                        }
                        currFont.ChildFontsMap = fontsMap;
                        fontList[indexFont] = currFont;
                        #endregion
                    }
                }
            }

            //init fonts array
            fonts = new PdfFontInfo[fontList.Count];
            for (int indexFont = 0; indexFont < fontList.Count; indexFont++)
            {
                fonts[indexFont] = new PdfFontInfo();
                PdfFontInfo currentFontInfo = fonts[indexFont];
                currentFontInfo.UnicodeMap = (ushort[])UnicodeMapsList[indexFont];
                currentFontInfo.UnicodeMapBack = new ushort[MaxSymbols + 1];
                currentFontInfo.UseUnicode = ((pfontInfo)fontList[indexFont]).UseUnicode;

                #region prepare GlyphBackList, GlyphRtfList tables and store known symbols in UnicodeMap
                ushort[] tempGlyphMap = (ushort[])GlyphMapsList[indexFont];
                ushort[] tempBackMap = new ushort[65536];
                if (tempGlyphMap[65535] != 0)
                {
                    pfontInfo tmpfinf = (pfontInfo)fontList[indexFont];
                    currentFontInfo.GlyphBackList = MakeGlyphBackMap(tmpfinf.Font, false);
                    int currentGlyph = 0;
                    for (int index = 0; index < 65535; index++)
                    {
                        if (tempGlyphMap[index] != 0)
                        {
                            currentFontInfo.UnicodeMap[currentFontInfo.GlyphBackList[index]] = 1;
                            tempBackMap[currentGlyph] = (ushort)index;
                            currentGlyph++;
                        }
                    }
                    currentFontInfo.GlyphRtfList = new ushort[currentGlyph];
                    Array.Copy(tempBackMap, 0, currentFontInfo.GlyphRtfList, 0, currentGlyph);
                }
                #endregion

                #region prepare UnicodeMap table
                ushort mappedSymbol = FirstMappedSymbol;
                for (int indexSymbol = FirstMappedSymbol; indexSymbol < 65536; indexSymbol++)
                {
                    if (currentFontInfo.UnicodeMap[indexSymbol] != 0)
                    {
                        currentFontInfo.UnicodeMap[indexSymbol] = mappedSymbol;
                        currentFontInfo.UnicodeMapBack[mappedSymbol] = (ushort)indexSymbol;
                        mappedSymbol++;
                        if (mappedSymbol > MaxSymbols) mappedSymbol = MaxSymbols;
                        if (!currentFontInfo.UseUnicode && mappedSymbol == 92)
                        {
                            currentFontInfo.UnicodeMapBack[mappedSymbol] = (ushort)indexSymbol;
                            mappedSymbol++;
                        }
                    }
                }
                ushort tempIndexSymbol = 0x2022;  //bullet
                currentFontInfo.UnicodeMap[tempIndexSymbol] = mappedSymbol;
                currentFontInfo.UnicodeMapBack[mappedSymbol] = tempIndexSymbol;
                mappedSymbol++;

                if (!currentFontInfo.UseUnicode && IsFontSymbolic(((pfontInfo)fontList[indexFont]).Name))
                {
                    mappedSymbol = 256;
                    for (ushort indexSymbol = FirstMappedSymbol; indexSymbol < mappedSymbol; indexSymbol++)
                    {
                        currentFontInfo.UnicodeMap[indexSymbol] = indexSymbol;
                        currentFontInfo.UnicodeMapBack[indexSymbol] = indexSymbol;
                    }
                    for (uint indexSymbol = mappedSymbol; indexSymbol <= 65535; indexSymbol++)
                    {
                        currentFontInfo.UnicodeMap[indexSymbol] = 0;
                    }
                }

                currentFontInfo.MappedSymbolsCount = mappedSymbol;

                for (ushort indexSymbol = 0; indexSymbol < FirstMappedSymbol; indexSymbol++)
                {
                    currentFontInfo.UnicodeMap[indexSymbol] = indexSymbol;
                    currentFontInfo.UnicodeMapBack[indexSymbol] = indexSymbol;
                }
                #endregion

                #region init SymsToPDF array and char names
                currentFontInfo.CharPdfNames = new string[mappedSymbol];
                currentFontInfo.SymsToPDF = new int[mappedSymbol];
                for (int indexTemp = 32; indexTemp < currentFontInfo.MappedSymbolsCount; indexTemp++)
                {
                    int offset = -1;
                    for (int indexOffset = 0; indexOffset < StandardFontNumWidths; indexOffset++)
                    {
                        if (currentFontInfo.UnicodeMapBack[indexTemp] == StandardFontWidths[indexOffset, 0])
                        {
                            offset = indexOffset;
                            break;
                        }
                    }
                    currentFontInfo.SymsToPDF[indexTemp] = offset;

                    string st = "uni" + currentFontInfo.UnicodeMapBack[indexTemp].ToString("X4");
                    if (uniMap[currentFontInfo.UnicodeMapBack[indexTemp]] != -1)
                    {
                        st = StandardFontCharsNames[uniMap[currentFontInfo.UnicodeMapBack[indexTemp]]].Substring(4);
                    }
                    currentFontInfo.CharPdfNames[indexTemp] = st;
                }
                #endregion

                //fill array with glyph indicies and width data
                currentFontInfo.GlyphList = new ushort[mappedSymbol];

                #region get font info from GDI
                pfontInfo tmpfi = (pfontInfo)fontList[indexFont];
                fonts[indexFont].Widths = new int[mappedSymbol];

                GetFontMetrics(tmpfi.Font, currentFontInfo, tempGlyphMap, isWpf);
                for (int tempIndex = 0; tempIndex < mappedSymbol - 32; tempIndex++)
                {
                    fonts[indexFont].Widths[tempIndex] = WIDTHS[32 + tempIndex];
                }

                fonts[indexFont].CH = (int)Math.Round(otm.otmsCapEmHeight * Factor);
                fonts[indexFont].XH = 0;
                fonts[indexFont].ASC = (int)Math.Round(otm.otmAscent * Factor);
                fonts[indexFont].DESC = (int)Math.Round(otm.otmDescent * Factor);
                fonts[indexFont].tmASC = (int)Math.Round(otm.otmTextMetrics.tmAscent * Factor);
                fonts[indexFont].tmDESC = (int)Math.Round(otm.otmTextMetrics.tmDescent * Factor);
                fonts[indexFont].tmExternal = (int)Math.Round(otm.otmTextMetrics.tmExternalLeading * Factor);
                fonts[indexFont].LLX = (int)Math.Round(otm.otmrcFontBox.left * Factor);
                fonts[indexFont].LLY = (int)Math.Round(otm.otmrcFontBox.bottom * Factor);
                fonts[indexFont].URX = (int)Math.Round(otm.otmrcFontBox.right * Factor);
                fonts[indexFont].URY = (int)Math.Round(otm.otmrcFontBox.top * Factor);
                fonts[indexFont].StemV = (int)Math.Round(0 * Factor);
                fonts[indexFont].ItalicAngle = (int)Math.Round((float)otm.otmItalicAngle / 10);
                fonts[indexFont].LineGap = (int)Math.Round(otm.otmLineGap * Factor);
                fonts[indexFont].MacAscend = (int)Math.Round(otm.otmMacAscent * Factor);
                fonts[indexFont].MacDescend = (int)Math.Round(otm.otmMacDescent * Factor);
                fonts[indexFont].MacLineGap = (int)Math.Round(otm.otmMacLineGap * Factor);
                fonts[indexFont].fsSelection = otm.otmfsSelection;
                fonts[indexFont].UnderscoreSize = (int)Math.Round(otm.otmsUnderscoreSize * Factor);
                fonts[indexFont].UnderscorePosition = (int)Math.Round(otm.otmsUnderscorePosition * Factor);
                fonts[indexFont].StrikeoutSize = (int)Math.Round(otm.otmsStrikeoutSize * Factor);
                fonts[indexFont].StrikeoutPosition = (int)Math.Round(otm.otmsStrikeoutPosition * Factor);
                fonts[indexFont].NeedSyntBold = NeedSyntBold;
                fonts[indexFont].NeedSyntItalic = NeedSyntItalic;
                #endregion

                if (standardPdfFonts == true)
                {
                    #region replace with standard PDF fonts info
                    for (int tempIndex = 32; tempIndex < currentFontInfo.MappedSymbolsCount; tempIndex++)
                    {
                        if (fonts[indexFont].SymsToPDF[tempIndex] != -1)
                        {
                            fonts[indexFont].Widths[tempIndex - 32] =
                                StandardFontWidths[fonts[indexFont].SymsToPDF[tempIndex], tmpfi.Number + 1];
                        }
                    }
                    fonts[indexFont].ItalicAngle = StandardFontInfo[0, tmpfi.Number] / 10;
                    fonts[indexFont].LLX = StandardFontInfo[1, tmpfi.Number];
                    fonts[indexFont].LLY = StandardFontInfo[2, tmpfi.Number];
                    fonts[indexFont].URX = StandardFontInfo[3, tmpfi.Number];
                    fonts[indexFont].URY = StandardFontInfo[4, tmpfi.Number];
                    fonts[indexFont].CH = StandardFontInfo[5, tmpfi.Number];
                    fonts[indexFont].XH = StandardFontInfo[6, tmpfi.Number];
                    fonts[indexFont].ASC = StandardFontInfo[7, tmpfi.Number];
                    fonts[indexFont].DESC = StandardFontInfo[8, tmpfi.Number];
                    fonts[indexFont].StemV = 0;
                    #endregion
                }
                if (((pfontInfo)fontList[indexFont]).ChildFontsMap != null)
                {
                    currentFontInfo.MappedSymbolsCount = 255;
                    currentFontInfo.ChildFontsMap = ((pfontInfo)fontList[indexFont]).ChildFontsMap;
                }
            }

            currentFont = -1;
            CurrentFont = 0;
        }
        #endregion

        #region GetFontNumber
        /// <summary>
        /// Returns number of font in table of fonts.
        /// </summary>
        public int GetFontNumber(Font incomingFont)
        {
            int fontNumber = -1;

            string incomingFontName = null;
            object obj = fontHash[incomingFont];
            if (obj == null)
            {
                incomingFontName = incomingFont.Name;
                fontHash[incomingFont] = incomingFontName;
            }
            else
            {
                incomingFontName = (string)obj;
            }

            #region check fonts table for font
            if (fontList.Count > 0)
            {
                for (int index = 0; index < fontList.Count; index++)
                {
                    pfontInfo tmpfi = (pfontInfo)fontList[index];
                    if ((tmpfi.Name == incomingFontName) &&
                        (tmpfi.Bold == incomingFont.Bold) &&
                        (tmpfi.Italic == incomingFont.Italic))
                    {
                        fontNumber = index;
                        //							if (usePdfFontsOnly == true)
                        //							{
                        //								fontNumber = tmpfi.Number;
                        //							}
                        break;
                    }
                }
            }
            #endregion

            if (fontNumber == -1)
            {
                #region add to fonts table
                pfontInfo fi = new pfontInfo();
                fi.Name = incomingFont.Name;
                fi.Bold = incomingFont.Bold;
                fi.Italic = incomingFont.Italic;
                fi.Font = incomingFont;
                fi.ParentFontNumber = -1;

                fontNumber = 0;
                //This font will be use by default
                foreach (string st in family_Helvetica) if (fi.Name == st) fontNumber = 0;
                foreach (string st in family_Times_Roman) if (fi.Name == st) fontNumber = 4;
                foreach (string st in family_Courier) if (fi.Name == st) fontNumber = 8;
                //		foreach (string st in family_Symbol		 ) if (fi.Name == st) fontNumber = 12;
                //		foreach (string st in family_ZapfDingbats) if (fi.Name == st) fontNumber = 12+1;
                if (fontNumber < 12)
                {
                    if (fi.Bold == true) fontNumber += 1;
                    if (fi.Italic == true) fontNumber += 2;
                }
                fi.Number = fontNumber;
                fi.PdfName = PdfFontName[fontNumber];

                fontList.Add(fi);
                fontNumber = fontList.Count - 1;
                #endregion

                UnicodeMapsList.Add(new ushort[65536]);
                GlyphMapsList.Add(new ushort[65536]);
            }

            CurrentFont = fontNumber;

            return fontNumber;
        }
        #endregion

        #region StoreUnicodeSymbolsInMap
        public void StoreUnicodeSymbolsInMap(StringBuilder sb)
        {
            if ((sb.Length > 0) && (currentFont != -1))
            {
                ushort[] map = (ushort[])UnicodeMapsList[currentFont];
                string st = sb.ToString();
                for (int indexChar = 0; indexChar < st.Length; indexChar++)
                {
                    map[st[indexChar]] = 1;
                }
            }
        }
        #endregion

        #region StoreGlyphsInMap
        public void StoreGlyphsInMap(StringBuilder sb)
        {
            if ((sb.Length > 0) && (currentFont != -1))
            {
                ushort[] map = (ushort[])GlyphMapsList[currentFont];
                string st = sb.ToString();
                for (int indexChar = 0; indexChar < st.Length; indexChar++)
                {
                    map[st[indexChar]] = 1;
                }
                map[0xffff] = 1;
            }
        }
        #endregion

        public static bool IsFontStimulsoft(string name)
        {
            return name == "Stimulsoft";
        }
        public static bool IsFontSymbolic(string name)
        {
            return name == "Symbol" || name == "Wingdings" || name == "Wingdings 2" || name == "Wingdings 3";
        }

        #region Methods.ThrowError
        private static void ThrowError(int step)
        {
            Win32Exception myEx = new Win32Exception(Marshal.GetLastWin32Error());
            throw new Exception(string.Format("PdfFonts error at point {0}, code #{1:X8}: {2}", step, myEx.ErrorCode, myEx.Message));
        }
        private static void ThrowError(int step, int error)
        {
            Win32Exception myEx = new Win32Exception(Marshal.GetLastWin32Error());
            throw new Exception(string.Format("PdfFonts error at point {0}, code #{1:X8}(#{2:X8}): {3}", step, myEx.ErrorCode, error, myEx.Message));
        }
        #endregion

        #region Clear
        public void Clear()
        {
            fonts = null;
            fontList = null;
            UnicodeMapsList = null;
            GlyphMapsList = null;

            Widths = null;
            CharPdfNames = null;
            UnicodeMap = null;
            UnicodeMapBack = null;
            GlyphList = null;
            GlyphBackList = null;
            GlyphRtfList = null;
            SymsToPDF = null;
            GlyphWidths = null;

            fontsInfoStore.Clear();
            localFontsDataCache.Clear();
        }
        #endregion

        #endregion

        #region FontsCaches
        public void GetFontDataBuf(Font font, out byte[] buff, bool useFontInfoStore, bool onlyCheck = false)
        {
            string fontName = string.Format("{0}_{1}", font.Name, font.Style.ToString());
            byte[] temp = null;

            if (StiOptions.Export.Pdf.AllowFontsCache)
            {
                lock (FontsDataCache)
                {
                    if (FontsDataCache.ContainsKey(fontName))
                    {
                        temp = (byte[])FontsDataCache[fontName];
                    }
                    else
                    {
                        GetFontDataFromGdi(font, out temp, useFontInfoStore);
                        FontsDataCache[fontName] = temp;
                    }
                }
                if (onlyCheck)
                {
                    buff = temp;
                    return;
                }
                buff = new byte[temp.Length];
                Array.Copy(temp, buff, temp.Length);
            }
            else
            {
                if (localFontsDataCache.ContainsKey(fontName))
                {
                    temp = (byte[])localFontsDataCache[fontName];
                }
                else
                {
                    GetFontDataFromGdi(font, out temp, useFontInfoStore);
                    localFontsDataCache[fontName] = temp;
                }
                if (onlyCheck)
                {
                    buff = temp;
                    return;
                }
                buff = new byte[temp.Length];
                Array.Copy(temp, buff, temp.Length);
            }
        }

        public static void ClearFontsCache()
        {
            lock (FontsDataCache)
            {
                FontsDataCache.Clear();
            }
        }
        #endregion

        #region this
        public PdfFonts()
        {
            fontList = new ArrayList();
            UnicodeMapsList = new ArrayList();
            GlyphMapsList = new ArrayList();

            InitStandardFontWidths();
            InitStandardFontCharsNames();
            InitStandardFontInfo();
            InitCharNamesUniMap();

            fontsInfoStore = new FontsInfoStore();
        }
        #endregion
    }
    #endregion

    #region Class FontsInfoStore
    public class FontsInfoStore
    {
        #region Structures
        internal class FontInfo
        {
            public FontStyleInfo[] StylesInfo = null;
            public bool NeedSyntBold = false;
            public bool NeedSyntItalic = false;
            public byte[] Data = null;
            public byte[] OTM = null;
            public ushort[] Glyphs = null;
            public ushort[] Widths = null;

            public void SetIndex(int index)
            {
                NeedSyntBold = false;
                NeedSyntItalic = false;

                int newIndex = index;

                if ((index == 1) && StylesInfo[index].NeedSyntBold)
                {
                    newIndex = 0;
                    NeedSyntBold = true;
                }
                if ((index == 2) && StylesInfo[index].NeedSyntItalic)
                {
                    newIndex = 0;
                    NeedSyntItalic = true;
                }
                if ((index == 3) && (StylesInfo[index].NeedSyntBold || StylesInfo[index].NeedSyntItalic))
                {
                    if (!StylesInfo[1].NeedSyntBold)
                    {
                        newIndex = 1;
                        NeedSyntItalic = true;
                    }
                    else
                    {
                        if (!StylesInfo[2].NeedSyntItalic)
                        {
                            newIndex = 2;
                            NeedSyntBold = true;
                        }
                        else
                        {
                            newIndex = 0;
                            NeedSyntBold = true;
                            NeedSyntItalic = true;
                        }
                    }
                }

                this.OTM = StylesInfo[newIndex].OTM;
                this.Glyphs = StylesInfo[newIndex].Glyphs;
                this.Widths = StylesInfo[newIndex].Widths;
                this.Data = StylesInfo[newIndex].Data;
            }

            public FontInfo()
            {
                StylesInfo = new FontStyleInfo[4];
                for (int index = 0; index < 4; index++)
                {
                    StylesInfo[index] = new FontStyleInfo();
                }
            }
        }

        internal class FontStyleInfo
        {
            public bool NeedSyntBold = false;
            public bool NeedSyntItalic = false;
            public byte[] Data = null;
            public byte[] OTM = null;
            public ushort[] Glyphs = null;
            public ushort[] Widths = null;
        }
        #endregion

        #region Fields
        private static Hashtable Store = new Hashtable();
        private Hashtable Cache = null;
        #endregion

        #region Methods public
        public static void LoadFontInfoToStore(string fontName, byte[] data)
        {
            Store[fontName] = data;
        }
        public static void LoadFontInfoToStore(string fontName, Stream stream)
        {
            byte[] buf = new byte[stream.Length];
            stream.Read(buf, 0, buf.Length);
            Store[fontName] = buf;
        }
        public static void LoadFontInfoToStore(string fontName, string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            byte[] buf = new byte[fs.Length];
            fs.Read(buf, 0, buf.Length);
            fs.Close();
            Store[fontName] = buf;
        }
        public static void ClearFontsInfoStore()
        {
            Store.Clear();
        }
        #endregion

        #region Methods internal
        internal void Clear()
        {
            Cache.Clear();
        }

        internal FontInfo GetFontInfo(Font font)
        {
            FontInfo fontInfo = null;
            if (Cache.ContainsKey(font.Name))
            {
                fontInfo = (FontInfo)Cache[font.Name];
            }
            else
            {
                if (Store.ContainsKey(font.Name))
                {
                    fontInfo = new FontInfo();

                    #region load data from store
                    byte[] buf = (byte[])Store[font.Name];
                    MemoryStream ms = new MemoryStream(buf);
                    GZipStream gs = new GZipStream(ms, CompressionMode.Decompress, true);
                    buf = new byte[14];
                    gs.Read(buf, 0, 14);
                    fontInfo.StylesInfo[0].NeedSyntBold = (buf[6] == (byte)'1');
                    fontInfo.StylesInfo[1].NeedSyntBold = (buf[7] == (byte)'1');
                    fontInfo.StylesInfo[2].NeedSyntBold = (buf[8] == (byte)'1');
                    fontInfo.StylesInfo[3].NeedSyntBold = (buf[9] == (byte)'1');
                    fontInfo.StylesInfo[0].NeedSyntItalic = (buf[6] == (byte)'1');
                    fontInfo.StylesInfo[1].NeedSyntItalic = (buf[7] == (byte)'1');
                    fontInfo.StylesInfo[2].NeedSyntItalic = (buf[8] == (byte)'1');
                    fontInfo.StylesInfo[3].NeedSyntItalic = (buf[9] == (byte)'1');
                    ReadFontStyle(gs, fontInfo, 0);
                    ReadFontStyle(gs, fontInfo, 1);
                    ReadFontStyle(gs, fontInfo, 2);
                    ReadFontStyle(gs, fontInfo, 3);
                    gs.Close();
                    ms.Close();
                    #endregion

                    Cache.Add(font.Name, fontInfo);
                }
            }
            if (fontInfo != null)
            {
                int index = 0;
                if (font.Bold) index += 1;
                if (font.Italic) index += 2;
                fontInfo.SetIndex(index);
            }
            return fontInfo;
        }

        private static void ReadFontStyle(GZipStream gs, FontInfo fontInfo, int styleIndex)
        {
            if (!(fontInfo.StylesInfo[styleIndex].NeedSyntBold && fontInfo.StylesInfo[styleIndex].NeedSyntItalic))
            {
                //OTM
                byte[] bufHeader = new byte[9];
                gs.Read(bufHeader, 0, 9);
                int len = BitConverter.ToInt32(bufHeader, 5);
                byte[] bufData = new byte[len];
                gs.Read(bufData, 0, len);
                fontInfo.StylesInfo[styleIndex].OTM = bufData;

                //GLPH
                bufHeader = new byte[9];
                gs.Read(bufHeader, 0, 9);
                len = BitConverter.ToInt32(bufHeader, 5);
                bufData = new byte[len * 2];
                gs.Read(bufData, 0, bufData.Length);
                ushort[] bufGlph = new ushort[len];
                int offset = 0;
                for (int index = 0; index < len; index++)
                {
                    bufGlph[index] = BitConverter.ToUInt16(bufData, offset);
                    offset += 2;
                }
                fontInfo.StylesInfo[styleIndex].Glyphs = bufGlph;

                //WDTH
                bufHeader = new byte[9];
                gs.Read(bufHeader, 0, 9);
                len = BitConverter.ToInt32(bufHeader, 5);
                bufData = new byte[len * 2];
                gs.Read(bufData, 0, bufData.Length);
                ushort[] bufWdth = new ushort[len];
                offset = 0;
                for (int index = 0; index < len; index++)
                {
                    bufWdth[index] = BitConverter.ToUInt16(bufData, offset);
                    offset += 2;
                }
                fontInfo.StylesInfo[styleIndex].Widths = bufWdth;

                //DATA
                bufHeader = new byte[9];
                gs.Read(bufHeader, 0, 9);
                len = BitConverter.ToInt32(bufHeader, 5);
                bufData = new byte[len];
                gs.Read(bufData, 0, len);
                fontInfo.StylesInfo[styleIndex].Data = bufData;
            }
        }
        #endregion

        public FontsInfoStore()
        {
            Cache = new Hashtable();
        }
    }
    #endregion

}
