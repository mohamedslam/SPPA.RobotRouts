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
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.Components;
using System.Security;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Metafile = Stimulsoft.Drawing.Imaging.Metafile;
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Font = Stimulsoft.Drawing.Font;
using Matrix = Stimulsoft.Drawing.Drawing2D.Matrix;
#endif

namespace Stimulsoft.Report.Export.Tools
{
    [SuppressUnmanagedCodeSecurity]
    internal class StiPdfMetafileRender
    {
        private StiPdfExportService pdfService = null;
        private PdfFonts pdfFont = null;
        private StreamWriter pageStream = null;
        private bool useUnicodeMode = false;

        private const double hiToTwips = 0.72;	//convert from hi(hundredths of inch) to points
        private const double fontCorrectValue = 0.955;
        private int precision_digits_font = 3;
        private const float italicAngleTanValue = 0.325f;   //18 degrees
        private const float boldFontStrokeWidthValue = 0.031f;

        private string ConvertToString(double Value)
        {
            return pdfService.ConvertToString(Value);
        }
        private string ConvertToString(double Value, int precision)
        {
            return pdfService.ConvertToString(Value, precision);
        }


        #region Used GDI structures

        internal struct SIZEL
        {
            public int cX;	//LONG == signed int
            public int cY;	//LONG
        }

        internal struct POINTL
        {
            public int X; //LONG
            public int Y; //LONG
        }

        //		internal struct POINTS 
        //		{ 
        //			public short X; //LONG
        //			public short Y; //LONG
        //		}

        internal struct RECTL
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        //		internal struct PANOSE 
        //		{
        //			public byte bFamilyType;
        //			public byte bSerifStyle;
        //			public byte bWeight;
        //			public byte bProportion;
        //			public byte bContrast;
        //			public byte bStrokeVariation;
        //			public byte ArmStyle;
        //			public byte bLetterform;
        //			public byte bMidline;
        //			public byte bXHeight;
        //		}

        //ms-help://MS.VSCC.2003/MS.MSDNQTR.2003FEB.1033/gdi/fontext_1wmq.htm
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct LOGFONT
        {
            public int lfHeight;
            public int lfWidth;
            public int lfEscapement;
            public int lfOrientation;
            public int lfWeight;
            public byte lfItalic;
            public byte lfUnderline;
            public byte lfStrikeOut;
            public byte lfCharSet;
            public byte lfOutPrecision;
            public byte lfClipPrecision;
            public byte lfQuality;
            public byte lfPitchAndFamily;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string lfFaceName; //tchar[LF_FACESIZE]
        }

        //ms-help://MS.VSCC.2003/MS.MSDNQTR.2003FEB.1033/gdi/fontext_04tu.htm
        //		internal struct EXTLOGFONT 
        //		{ 
        //			public LOGFONT elfLogFont; 
        //			public string   elfFullName; //tchar[LF_FULLFACESIZE]
        //			public string   elfStyle; //tchar[LF_FACESIZE]
        //			public uint   elfVersion; 
        //			public uint   elfStyleSize; 
        //			public uint   elfMatch; 
        //			public uint   elfReserved; 
        //			public uint   elfVendorId; //byte[4]
        //			public uint   elfCulture; 
        //			public PANOSE  elfPanose; 
        //		} 

        //ms-help://MS.VSCC.2003/MS.MSDNQTR.2003FEB.1033/gdi/metafile_5t2q.htm
        internal struct tagENHMETAHEADER
        {
            //			public uint iType; 
            //			public uint nSize; 
            public RECTL rclBounds; //Specifies the dimensions, in device units, of the smallest rectangle that can be drawn around 
            //the picture stored in the metafile. This rectangle is supplied by graphics device interface (GDI).
            //Its dimensions include the right and bottom edges. 
            public RECTL rclFrame;	//Specifies the dimensions, in .01 millimeter units, of a rectangle that surrounds the picture stored
            //in the metafile. This rectangle must be supplied by the application that creates the metafile.
            //Its dimensions include the right and bottom edges. 
            public uint dSignature;
            public uint nVersion;
            public uint nBytes;
            public uint nRecords;
            public ushort nHandles;
            public ushort sReserved;
            public uint nDescription;
            public uint offDescription;
            public uint nPalEntries;
            public SIZEL szlDevice;		//Specifies the resolution of the reference device, in pixels.
            public SIZEL szlMillimeters;	//Specifies the resolution of the reference device, in millimeters.
            public uint cbPixelFormat;
            public uint offPixelFormat;
            public uint bOpenGL;
            //			public SIZEL szlMicrometers;	//Windows 98/Me, Windows 2000/XP: Size of the reference device in micrometers.
        }

        //ms-help://MS.VSCC.2003/MS.MSDNQTR.2003FEB.1033/gdi/metafile_9fua.htm
        internal class tagEMREXTTEXTOUT
        {
            public RECTL Bounds;			//Bounding rectangle, in device units. 
            public uint iGraphicsMode;	//Current graphics mode. This member can be either the GM_COMPATIBLE or GM_ADVANCED value. 
            public float exScale;		//X-scaling factor from page units to .01mm units if the graphics mode is the GM_COMPATIBLE value.
            public float eyScale;		//Y-scaling factor from page units to .01mm units if the graphics mode is the GM_COMPATIBLE value.
            //	EMRTEXT emrtext :
            public POINTL Reference;		//Logical coordinates of the reference point used to position the string. 
            public uint nChars;			//Number of characters in string. 
            public uint offString;		//Offset to string
            public uint fOptions;			//Value specifying how to use the application-defined rectangle. This member can be a combination of the ETO_CLIPPED and ETO_OPAQUE values. 
            public RECTL rcl;				//Optional clipping and/or opaquing rectangle, in logical units.
            public uint offDx;			//Offset to intercharacter spacing array. 
        }

        //ms-help://MS.VSCC.2003/MS.MSDNQTR.2004APR.1033/gdi/cordspac_4tma.htm
        internal struct XFORM
        {
            public float eM11;
            public float eM12;
            public float eM21;
            public float eM22;
            public float eDx;
            public float eDy;

            public XFORM(float eM11, float eM12, float eM21, float eM22, float eDx, float eDy)
            {
                this.eM11 = eM11;
                this.eM12 = eM12;
                this.eM21 = eM21;
                this.eM22 = eM22;
                this.eDx = eDx;
                this.eDy = eDy;
            }
        }

        //ms-help://MS.VSCC.2003/MS.MSDNQTR.2004APR.1033/gdi/metafile_4xde.htm
        //ms-help://MS.VSCC.2003/MS.MSDNQTR.2004APR.1033/gdi/bitmaps_0fzo.htm - dwRop info
        internal struct tagEMRBITBLT
        {
            public RECTL rclBounds;
            public int xDest;
            public int yDest;
            public int cxDest;
            public int cyDest;
            public uint dwRop;
            public int xSrc;
            public int ySrc;
            public XFORM xformSrc;
            public uint crBkColorSrc;
            public uint iUsageSrc;
            public uint offBmiSrc;
            public uint cbBmiSrc;
            public uint offBitsSrc;
            public uint cbBitsSrc;
        }

        internal struct tagEMRALPHABLEND
        {
            public RECTL rclBounds;         // Inclusive-inclusive bounds in device units
            public int xDest;
            public int yDest;
            public int cxDest;
            public int cyDest;
            public uint dwRop;
            public int xSrc;
            public int ySrc;
            public XFORM xformSrc;          // Source DC transform
            public uint crBkColorSrc;        // Source DC BkColor in RGB
            public uint iUsageSrc;          // Source bitmap info color table usage (DIB_RGB_COLORS)
            public uint offBmiSrc;          // Offset to the source BITMAPINFO structure
            public uint cbBmiSrc;           // Size of the source BITMAPINFO structure
            public uint offBitsSrc;         // Offset to the source bitmap bits
            public uint cbBitsSrc;          // Size of the source bitmap bits
            public int cxSrc;
            public int cySrc;
        }

        //ms-help://MS.VSCC.2003/MS.MSDNQTR.2003FEB.1033/gdi/metafile_6x9u.htm
        internal struct tagEMRSTRETCHDIBITS
        {
            public RECTL rclBounds;
            public int xDest;
            public int yDest;
            public int xSrc;
            public int ySrc;
            public int cxSrc;
            public int cySrc;
            public uint offBmiSrc;
            public uint cbBmiSrc;
            public uint offBitsSrc;
            public uint cbBitsSrc;
            public uint iUsageSrc;
            public uint dwRop;
            public int cxDest;
            public int cyDest;
        }


        internal const uint ETO_OPAQUE = 0x0002;
        internal const uint ETO_CLIPPED = 0x0004;
        internal const uint ETO_GLYPH_INDEX = 0x0010;
        internal const uint ETO_PDY = 0x2000;

        internal const int LF_FULLFACESIZE = 64;
        internal const int LF_FACESIZE = 32;

        internal const int GM_COMPATIBLE = 1;
        internal const int GM_ADVANCED = 2;

        /* Ternary raster operations */
        internal const uint SRCCOPY = 0x00CC0020; // dest = source                    (DWORD)
        internal const uint SRCPAINT = 0x00EE0086; /* dest = source OR dest           */
        internal const uint SRCAND = 0x008800C6; /* dest = source AND dest          */
        internal const uint SRCINVERT = 0x00660046; /* dest = source XOR dest          */
        internal const uint SRCERASE = 0x00440328; /* dest = source AND (NOT dest )   */
        internal const uint NOTSRCCOPY = 0x00330008; /* dest = (NOT source)             */
        internal const uint NOTSRCERASE = 0x001100A6; /* dest = (NOT src) AND (NOT dest) */
        internal const uint MERGECOPY = 0x00C000CA; /* dest = (source AND pattern)     */
        internal const uint MERGEPAINT = 0x00BB0226; /* dest = (NOT source) OR dest     */
        internal const uint PATCOPY = 0x00F00021; /* dest = pattern                  */
        internal const uint PATPAINT = 0x00FB0A09; /* dest = DPSnoo                   */
        internal const uint PATINVERT = 0x005A0049; /* dest = pattern XOR dest         */
        internal const uint DSTINVERT = 0x00550009; /* dest = (NOT dest)               */
        internal const uint BLACKNESS = 0x00000042; /* dest = BLACK                    */
        internal const uint WHITENESS = 0x00FF0062; /* dest = WHITE                    */
        internal const uint NOMIRRORBITMAP = 0x80000000; /* Do not Mirror the bitmap in this call */
        internal const uint CAPTUREBLT = 0x40000000; /* Include layered windows */

        internal const uint TA_NOUPDATECP = 0;
        internal const uint TA_UPDATECP = 1;
        internal const uint TA_LEFT = 0;
        internal const uint TA_RIGHT = 2;
        internal const uint TA_CENTER = 6;
        internal const uint TA_TOP = 0;
        internal const uint TA_BOTTOM = 8;
        internal const uint TA_BASELINE = 24;
        internal const uint TA_RTLREADING = 256;
        internal const uint TA_MASK = TA_BASELINE + TA_CENTER + TA_UPDATECP + TA_RTLREADING;

        internal const uint BKMODE_TRANSPARENT = 1;
        internal const uint BKMODE_OPAQUE = 2;

        #endregion

        #region Data read procedures
        private byte[] dataArray = null;
        private int dataOffset;

        private Int32 ReadInt32()
        {
            Int32 value = BitConverter.ToInt32(dataArray, dataOffset);
            dataOffset += 4;
            return value;
        }
        private Int16 ReadInt16()
        {
            Int16 value = BitConverter.ToInt16(dataArray, dataOffset);
            dataOffset += 2;
            return value;
        }
        private UInt32 ReadUInt32()
        {
            UInt32 value = BitConverter.ToUInt32(dataArray, dataOffset);
            dataOffset += 4;
            return value;
        }
        private UInt16 ReadUInt16()
        {
            UInt16 value = BitConverter.ToUInt16(dataArray, dataOffset);
            dataOffset += 2;
            return value;
        }
        private byte ReadUInt8()
        {
            byte value = dataArray[dataOffset];
            dataOffset++;
            return value;
        }
        private float ReadSingle()
        {
            float value = BitConverter.ToSingle(dataArray, dataOffset);
            dataOffset += 4;
            return value;
        }
        private string ReadStringTchar(int bufSize)
        {
            StringBuilder value = new StringBuilder();
            int offset = 0;
            while ((BitConverter.ToUInt16(dataArray, dataOffset + offset * 2) != 0) && (offset < bufSize))
            {
                value.Append(BitConverter.ToChar(dataArray, dataOffset + offset * 2));
                offset++;
            }
            dataOffset += bufSize * 2;
            return value.ToString();
        }
        #endregion

        #region Internal structures
        internal class EmfFontInfo
        {
            public int internalFontNumber;
            public LOGFONT elfLogFont;
        }
        internal class EmfBrushInfo
        {
            public uint lbStyle;
            public uint lbColor;
            public uint lbHatch;
        }
        //ms-help://MS.VSCC.2003/MS.MSDNQTR.2003FEB.1033/gdi/pens_0k8i.htm
        internal class EmfPenInfo
        {
            public uint lopnStyle;
            public int lopnWidth;
            public uint lopnColor;
        }
        internal class GDIobjectInfo
        {
            public uint ihNumber;
            public EmfFontInfo Font;
            public EmfBrushInfo Brush;
            public EmfPenInfo Pen;
        }

        internal struct POINTD
        {
            public double X;
            public double Y;
        }
        #endregion

        #region Internal methods
        private void RemoveObject(uint ihNumber)
        {
            //check array, if element with ihNumber already exists
            for (int index = GDIobjects.Count - 1; index >= 0; index--)
            {
                GDIobjectInfo gdiObject = (GDIobjectInfo)GDIobjects[index];
                if (gdiObject.ihNumber == ihNumber)
                {
                    GDIobjects.RemoveAt(index);
                }
            }
        }

        //		/* Stock Logical Objects */
        //		#define WHITE_BRUSH         0
        //		#define LTGRAY_BRUSH        1
        //		#define GRAY_BRUSH          2
        //		#define DKGRAY_BRUSH        3
        //		#define BLACK_BRUSH         4
        //		#define NULL_BRUSH          5
        //		#define HOLLOW_BRUSH        NULL_BRUSH
        //		#define WHITE_PEN           6
        //		#define BLACK_PEN           7
        //		#define NULL_PEN            8
        //		#define OEM_FIXED_FONT      10
        //		#define ANSI_FIXED_FONT     11
        //		#define ANSI_VAR_FONT       12
        //		#define SYSTEM_FONT         13
        //		#define DEVICE_DEFAULT_FONT 14
        //		#define DEFAULT_PALETTE     15
        //		#define SYSTEM_FIXED_FONT   16

        private void SelectObject(uint ihNumber)
        {
            //			if (((ihNumber & 0x80000000) != 0) && (ihNumber != 0x80000005) && (ihNumber != 0x80000008))
            //			{
            //				ihNumber = 0xFFFFFFFF;	//stock
            //			}
            if (ihNumber == 0x80000005)
            {
                gdiSelectedBrush = 0xFFFFFFFF;	//NULL_BRUSH
                return;
            }
            if (ihNumber == 0x80000008)
            {
                gdiSelectedPen = 0xFFFFFFFF;	//NULL_PEN
                return;
            }
            //check array, if element with ihNumber exists
            for (int index = GDIobjects.Count - 1; index >= 0; index--)
            {
                GDIobjectInfo gdiObject = (GDIobjectInfo)GDIobjects[index];
                if (gdiObject.ihNumber == ihNumber)
                {
                    //gdiSelectedObject = (uint)index;
                    if (gdiObject.Pen != null) gdiSelectedPen = ihNumber;
                    if (gdiObject.Brush != null) gdiSelectedBrush = ihNumber;
                    if (gdiObject.Font != null) gdiSelectedFont = ihNumber;
                    return;
                }
            }
            //gdiSelectedObject = 0;
            //return;
        }

        private int GetGdiObjectIndex(uint ihNumber)
        {
            //check array, if element with ihNumber exists
            for (int index = GDIobjects.Count - 1; index >= 0; index--)
            {
                GDIobjectInfo gdiObject = (GDIobjectInfo)GDIobjects[index];
                if (gdiObject.ihNumber == ihNumber)
                {
                    return index;
                }
            }
            return 0;
        }

        private void SetPdfColor(uint Color)
        {
            Color tempColor = ConvertColorFromGdi(Color);
            pdfService.SetStrokeColor(tempColor);
        }

        private void SetPdfBackColor(uint Color)
        {
            Color tempColor = ConvertColorFromGdi(Color);
            pdfService.SetNonStrokeColor(tempColor);
        }

        private Color ConvertColorFromGdi(uint gdiColor)
        {
            Color newColor = Color.FromArgb(
                (int)((gdiColor & 0x0000FFu)),
                (int)((gdiColor & 0x00FF00u) >> 8),
                (int)((gdiColor & 0xFF0000u) >> 16));
            return newColor;
        }

        private void WriteTextToStream(tagEMREXTTEXTOUT extText, StringBuilder Text, int[] charsOffset)
        {
            #region Write text
            if ((GDIobjects.Count > 0) && (extText.nChars > 0))
            {
                float fontSize = 0;
                float sizeInPt = 1;
                float baselineOffset = 0;
                Font tempFont = null;
                int escapement = 0;
                GDIobjectInfo gdiObject = (GDIobjectInfo)GDIobjects[GetGdiObjectIndex(gdiSelectedFont)];
                if (gdiObject.Font != null)
                {
                    int fnt = gdiObject.Font.internalFontNumber;
                    pdfFont.CurrentFont = fnt;
                    tempFont = Font.FromLogFont((object)gdiObject.Font.elfLogFont);
                    sizeInPt = tempFont.SizeInPoints;
                    fontSize = (Math.Abs(gdiObject.Font.elfLogFont.lfHeight) + sizeInPt) / 2;

                    float tmASC = pdfFont.tmASC;
                    float tmDESC = pdfFont.tmDESC;
                    if (tempFont.Name == "Cambria Math")
                    {
                        tmASC *= 4.7f;
                        tmDESC *= 4.7f;
                    }

                    if (gdiObject.Font.elfLogFont.lfHeight < 0)
                    {
                        //fontSize = Math.Abs(gdiObject.Font.elfLogFont.lfHeight) * (pdfFont.tmASC + pdfFont.tmExternal) / 1000;
                        fontSize = Math.Abs(gdiObject.Font.elfLogFont.lfHeight) * (tmASC + pdfFont.tmExternal * 0.1f) / 1000;
                        sizeInPt = (float)(Math.Abs(gdiObject.Font.elfLogFont.lfHeight) * hiToTwips / fontCorrectValue);
                    }
                    fontSize *= (float)StiDpiHelper.GraphicsScale;
                    //sizeInPt *= (float)StiDpiHelper.GraphicsScale;
                    //pageStream.WriteLine("/F{0} {1} Tf", fnt, ConvertToString(sizeInPt * fontCorrectValue, precision_digits_font)); //empiric value
                    pageStream.WriteLine("/F{0} {1} Tf", fnt, ConvertToString(sizeInPt * fontCorrectValue * emfScaleX / hiToTwips, precision_digits_font)); //empiric value
                    //baselineOffset = pdfFont.tmDESC / (float)(pdfFont.tmASC + pdfFont.tmDESC + pdfFont.tmExternal) * fontSize;
                    baselineOffset = tmDESC / (float)(tmASC + tmDESC + pdfFont.tmExternal) * Math.Abs(gdiObject.Font.elfLogFont.lfHeight);
                    escapement = gdiObject.Font.elfLogFont.lfEscapement;
                }

                if ((extText.fOptions & ETO_OPAQUE) != 0)
                {
                    SetPdfBackColor(emfBackColor);	//fill color
                    pageStream.WriteLine("{0} {1} {2} {3} re f",
                        ConvertToString(emfX + extText.rcl.Left * emfScaleX),
                        ConvertToString(emfY - extText.rcl.Bottom * emfScaleY),
                        ConvertToString((extText.rcl.Right - extText.rcl.Left) * emfScaleX),
                        ConvertToString((extText.rcl.Bottom - extText.rcl.Top) * emfScaleY));
                }

                SetPdfBackColor(emfTextColor);

                #region correct text line length
                bool useGlyphIndex = (extText.fOptions & ETO_GLYPH_INDEX) != 0;
                StringBuilder stt = new StringBuilder();
                int summEmf = 0;
                int summPdf = 0;
                //double dpiScale = 1 / StiDpiHelper.GraphicsScale;
                stt.Append((useUnicodeMode ? "<" : "("));
                for (int index = 0; index < Text.Length; index++)
                {
                    int currentChar = (int)Text[index];
                    if (useGlyphIndex)
                    {
                        currentChar = pdfFont.GlyphBackList[currentChar];
                    }
                    int code = pdfFont.UnicodeMap[currentChar];
                    int currentCharWidth = 1000;	//default width of symbols
                    if (code >= 32)
                    {
                        currentCharWidth = pdfFont.Widths[code - 32];
                    }
                    int charsOffsetValue = charsOffset[index];
                    summPdf += currentCharWidth;
                    summEmf += charsOffsetValue;
                    //double correctionValue = - (summEmf * emfScale / ((sizeInPt * fontCorrectValue) / 1000) - summPdf);
                    double correctionValue = -(summEmf * hiToTwips / ((sizeInPt * fontCorrectValue) / 1000) - summPdf);
                    summPdf += -(int)correctionValue;
                    if (useUnicodeMode)
                    {
                        int glyph = pdfFont.GlyphList[code];
                        if (useGlyphIndex) glyph = (int)Text[index];
                        if (glyph == 0xFFFF) glyph = 0x0000;
                        stt.Append(glyph.ToString("X4"));
                        if (index < Text.Length - 1)
                        {
                            stt.Append(">" + ConvertToString(correctionValue) + "<");
                        }
                    }
                    else
                    {
                        stt.Append(StiPdfExportService.ConvertToEscapeSequencePlusTabs(((char)code).ToString()));
                        if (index < Text.Length - 1)
                        {
                            stt.Append(")" + ConvertToString(correctionValue) + "(");
                        }
                    }
                }
                stt.Append((useUnicodeMode ? ">" : ")"));
                #endregion

                byte[] charr = new byte[stt.Length];
                for (int tempIndex = 0; tempIndex < stt.Length; tempIndex++)
                {
                    charr[tempIndex] = (byte)stt[tempIndex];
                }
                pageStream.WriteLine("BT");
                double xCoord = emfX + extText.Reference.X * emfScaleX;
                double smallCorrection = 0.02 + sizeInPt * emfScaleX * 0.008;
                double yCoord = emfY + smallCorrection - (extText.Reference.Y + fontSize) * emfScaleY;
                if (emfTextAlign == TA_BOTTOM)
                {
                    yCoord = emfY + smallCorrection - (extText.Reference.Y - baselineOffset) * emfScaleY;
                }
                if (emfTextAlign == TA_BASELINE)
                {
                    yCoord = emfY + smallCorrection - extText.Reference.Y * emfScaleY;
                }

                Matrix matrix = null;
                if (escapement != 0)
                {
                    double AngleInRadians = escapement / 10f * Math.PI / 180f;
                    matrix = new Matrix(
                        (float)Math.Cos(AngleInRadians),
                        (float)Math.Sin(AngleInRadians),
                        (float)-Math.Sin(AngleInRadians),
                        (float)Math.Cos(AngleInRadians),
                        (float)xCoord,
                        (float)yCoord);
                }

                if (pdfFont.NeedSyntItalic && tempFont.Italic)
                {
                    if (matrix != null)
                    {
                        Matrix matrix2 = new Matrix(1, 0, italicAngleTanValue, 1, 0, 0);
                        matrix2.Multiply(matrix, MatrixOrder.Append);
                    }
                    else
                    {
                        Matrix matrix2 = new Matrix(1, 0, italicAngleTanValue, 1, (float)xCoord, (float)yCoord);
                        matrix = matrix2;
                    }
                }

                if (matrix != null)
                {
                    pageStream.WriteLine("{0} {1} {2} {3} {4} {5} Tm",
                        ConvertToString(matrix.Elements[0]),
                        ConvertToString(matrix.Elements[1]),
                        ConvertToString(matrix.Elements[2]),
                        ConvertToString(matrix.Elements[3]),
                        ConvertToString(matrix.Elements[4]),
                        ConvertToString(matrix.Elements[5]));
                }
                else
                {
                    pageStream.WriteLine("{0} {1} Td", ConvertToString(xCoord), ConvertToString(yCoord));
                }

                if (pdfFont.NeedSyntBold && tempFont.Bold)
                {
                    pageStream.WriteLine("{0} w 2 Tr", ConvertToString(boldFontStrokeWidthValue * sizeInPt * fontCorrectValue * emfScale / hiToTwips, precision_digits_font));
                    SetPdfColor(emfTextColor);
                }

                pageStream.Write("[");
                pageStream.Flush();
                pdfService.memoryPageStream.Write(charr, 0, charr.Length);
                pageStream.WriteLine("] TJ");
                pageStream.WriteLine("ET");

                if (pdfFont.NeedSyntBold && tempFont.Bold)
                {
                    pageStream.WriteLine("0 Tr");
                }

                #region underline text
                if ((tempFont != null) && (tempFont.Underline))
                {
                    #region calculate text line length in pt
                    double summ = 0;
                    for (int index = 0; index < Text.Length; index++)
                    {
                        if ((byte)Text[index] >= 32)
                        {
                            summ += pdfFont.Widths[(byte)Text[index] - 32];
                        }
                    }
                    summ = summ * (sizeInPt * fontCorrectValue) / 1000;
                    #endregion

                    pageStream.WriteLine("{0} w", ConvertToString(sizeInPt * 0.09));
                    SetPdfBackColor(emfTextColor);
                    pageStream.WriteLine("{0} {1} m",
                        ConvertToString(xCoord),
                        ConvertToString(yCoord - sizeInPt * 0.115));
                    pageStream.WriteLine("{0} {1} l S",
                        ConvertToString(xCoord + summ),
                        ConvertToString(yCoord - sizeInPt * 0.115));
                }
                #endregion

            }
            #endregion
        }

        private double GetTransformedCX(int x)
        {
            double newPointX = (x * baseXForm.eM11);
            return newPointX;
        }
        private double GetTransformedCY(int y)
        {
            double newPointY = (y * baseXForm.eM22);
            return newPointY;
        }
        private double GetTransformedX(int x)
        {
            double newPointX = (x * baseXForm.eM11 - emfOffsetX);
            return newPointX;
        }
        private double GetTransformedY(int y)
        {
            double newPointY = (y * baseXForm.eM22 - emfOffsetY);
            return newPointY;
        }
        private POINTD GetTransformedXY(int x, int y)
        {
            POINTD newPoint = new POINTD();
            newPoint.X = (x * baseXForm.eM11 - emfOffsetX);
            newPoint.Y = (y * baseXForm.eM22 - emfOffsetY);
            return newPoint;
        }
        private double GetTransformedW(int w)
        {
            double newW = w * ((baseXForm.eM11 + baseXForm.eM22) / 2);
            return newW;
            //return (double)w;
        }

        private void SetViewport()
        {
            if ((viewportFlags == 0x0F) && ((emfMapMode == 7) || (emfMapMode == 8)))
            {
                float scaleX = (float)(viewportExt.X / windowExt.X);
                float scaleY = (float)(viewportExt.Y / windowExt.Y);

                float em11 = scaleX;
                //float em12 = 0;
                //float em21 = 0;
                float em22 = scaleY;
                float edx = (float)(emfX * (1 - scaleX) + (viewportOrg.X - windowOrg.X * scaleX) * emfScaleX);
                float edy = (float)(emfY * (1 - scaleY) - (viewportOrg.Y - windowOrg.Y * scaleY) * emfScaleY);

                pageStream.WriteLine("{0} {1} {2} {3} {4} {5} cm",
                    ConvertToString(em11, 7),
                    ConvertToString(0),
                    ConvertToString(0),
                    ConvertToString(em22, 7),
                    ConvertToString(edx, 6),
                    ConvertToString(edy, 6));

                //viewportBack = new XFORM(em11, em12, em21, em22, edx, edy);

                viewportFlags = 0;
            }
        }

        private void AddStockGDIobjects()
        {
            //empty object with index 0
            GDIobjectInfo gdiObject = new GDIobjectInfo();
            gdiObject.ihNumber = 0xFFFFFFFF;
            RemoveObject(gdiObject.ihNumber);
            GDIobjects.Add(gdiObject);

            //WHITE_BRUSH
            gdiObject = new GDIobjectInfo();
            gdiObject.ihNumber = 0x80000000;
            EmfBrushInfo emfb = new EmfBrushInfo();
            emfb.lbStyle = 0;
            emfb.lbColor = 0x00FFFFFF;
            emfb.lbHatch = 0;
            gdiObject.Brush = emfb;
            RemoveObject(gdiObject.ihNumber);
            GDIobjects.Add(gdiObject);

            //LTGRAY_BRUSH
            gdiObject = new GDIobjectInfo();
            gdiObject.ihNumber = 0x80000001;
            emfb = new EmfBrushInfo();
            emfb.lbStyle = 0;
            emfb.lbColor = 0x00C0C0C0;
            emfb.lbHatch = 0;
            gdiObject.Brush = emfb;
            RemoveObject(gdiObject.ihNumber);
            GDIobjects.Add(gdiObject);

            //GRAY_BRUSH
            gdiObject = new GDIobjectInfo();
            gdiObject.ihNumber = 0x80000002;
            emfb = new EmfBrushInfo();
            emfb.lbStyle = 0;
            emfb.lbColor = 0x00808080;
            emfb.lbHatch = 0;
            gdiObject.Brush = emfb;
            RemoveObject(gdiObject.ihNumber);
            GDIobjects.Add(gdiObject);

            //DKGRAY_BRUSH
            gdiObject = new GDIobjectInfo();
            gdiObject.ihNumber = 0x80000003;
            emfb = new EmfBrushInfo();
            emfb.lbStyle = 0;
            emfb.lbColor = 0x00404040;
            emfb.lbHatch = 0;
            gdiObject.Brush = emfb;
            RemoveObject(gdiObject.ihNumber);
            GDIobjects.Add(gdiObject);

            //BLACK_BRUSH
            gdiObject = new GDIobjectInfo();
            gdiObject.ihNumber = 0x80000004;
            emfb = new EmfBrushInfo();
            emfb.lbStyle = 0;
            emfb.lbColor = 0x00000000;
            emfb.lbHatch = 0;
            gdiObject.Brush = emfb;
            RemoveObject(gdiObject.ihNumber);
            GDIobjects.Add(gdiObject);

            //WHITE_PEN
            gdiObject = new GDIobjectInfo();
            gdiObject.ihNumber = 0x80000006;
            EmfPenInfo emfp = new EmfPenInfo();
            emfp.lopnStyle = 0;
            emfp.lopnWidth = 0;
            emfp.lopnColor = 0x00FFFFFF;
            gdiObject.Pen = emfp;
            RemoveObject(gdiObject.ihNumber);
            GDIobjects.Add(gdiObject);

            //BLACK_PEN
            gdiObject = new GDIobjectInfo();
            gdiObject.ihNumber = 0x80000007;
            emfp = new EmfPenInfo();
            emfp.lopnStyle = 0;
            emfp.lopnWidth = 0;
            emfp.lopnColor = 0x00000000;
            gdiObject.Pen = emfp;
            RemoveObject(gdiObject.ihNumber);
            GDIobjects.Add(gdiObject);

            SelectObject(0x80000000);   //white brush
            SelectObject(0x80000007);   //black pen
        }
        #endregion

        #region Variables

        //from gdi
        private uint emfSelectedObject = 0;
        private uint gdiSelectedPen = 0;
        private uint gdiSelectedBrush = 0;
        private uint gdiSelectedFont = 0;
        private POINTD emfCurrentPoint;
        private uint emfBackColor = 0;
        private uint emfTextColor = 0;
        private uint emfBkMode = 0;	//TRANSPARENT 1, OPAQUE 2
        private uint emfPolyFillMode = 0;	//ALTERNATE 1, WINDING 2
        private uint emfMapMode = 0;	//MM_ISOTROPIC 7, MM_ANISOTROPIC 8
        private uint emfTextAlign = 0;

        private const double penWidthScale = 1.2;
        private const double penWidthDefault = 0.1;

        private PointD viewportOrg;
        private PointD viewportExt;
        private PointD windowOrg;
        private PointD windowExt;
        private uint viewportFlags = 0;
        //private object viewportBack = null;
        //private Stack viewportStack = null;

        private double emfX = 0;
        private double emfY = 0;
        private int emfOffsetX = 0;
        private int emfOffsetY = 0;
        private double emfScale = 0;
        private double emfScaleX = 0;
        private double emfScaleY = 0;
        private XFORM baseXForm = new XFORM();
        private Stimulsoft.Report.Export.StiPdfExportService.StiPdfData emfComponent;
        internal ArrayList GDIobjects = new ArrayList();
        private bool makePath = false;
        private ArrayList emfPath = null;

        private Stack dcStack = null;
        private DCData dcData = null;

        private class DCData
        {
            public uint emfBackColor = 0;
            public uint emfTextColor = 0;
            public uint emfBkMode = 0;
            public uint emfPolyFillMode = 0;
            public uint emfMapMode = 0;
            public uint emfTextAlign = 0;
        }

        internal bool assembleData = true;
        internal bool centerEmf = false;

        private Bitmap emfBmp = null;
        private Graphics emfGr = null;
        private Graphics.EnumerateMetafileProc m_delegate = null;
        private Point m_destPoint = new Point(0, 0);

        private static object lockMetafileFlag = new object();
        #endregion

        #region Common
        private bool MetafileCallback(
            EmfPlusRecordType recordType,
            int flags,
            int dataSize,
            IntPtr data,
            PlayRecordCallback callbackData)
        {
            dataArray = null;
            dataOffset = 0;
            if (data != IntPtr.Zero)
            {
                // Copy the unmanaged record to a managed byte buffer 
                // that can be used by PlayRecord.
                dataArray = new byte[dataSize];
                Marshal.Copy(data, dataArray, 0, dataSize);
            }

            if (assembleData)
            {
                #region Assemble data

                switch (recordType)
                {
                    case EmfPlusRecordType.EmfExtTextOutA:
                        #region read data
                        {
                            RECTL Bounds;			//Bounding rectangle, in device units. 
                            uint iGraphicsMode;	//Current graphics mode. This member can be either the GM_COMPATIBLE or GM_ADVANCED value. 
                            float exScale;		//X-scaling factor from page units to .01mm units if the graphics mode is the GM_COMPATIBLE value.
                            float eyScale;		//Y-scaling factor from page units to .01mm units if the graphics mode is the GM_COMPATIBLE value.
                            //	EMRTEXT emrtext :
                            POINTL Reference;		//Logical coordinates of the reference point used to position the string. 
                            uint nChars;			//Number of characters in string. 
                            uint offString;		//Offset to string
                            uint fOptions;			//Value specifying how to use the application-defined rectangle. This member can be a combination of the ETO_CLIPPED and ETO_OPAQUE values. 
                            RECTL rcl;				//Optional clipping and/or opaquing rectangle, in logical units.
                            uint offDx;			//Offset to intercharacter spacing array. 

                            Bounds.Left = ReadInt32();
                            Bounds.Top = ReadInt32();
                            Bounds.Right = ReadInt32();
                            Bounds.Bottom = ReadInt32();
                            iGraphicsMode = ReadUInt32(); //0x10
                            exScale = ReadSingle();
                            eyScale = ReadSingle();

                            Reference.X = ReadInt32();
                            Reference.Y = ReadInt32();
                            nChars = ReadUInt32();
                            offString = ReadUInt32();
                            fOptions = ReadUInt32();
                            rcl.Left = ReadInt32();
                            rcl.Top = ReadInt32();
                            rcl.Right = ReadInt32();
                            rcl.Bottom = ReadInt32();
                            offDx = ReadUInt32();

                            StringBuilder Text = new StringBuilder();
                            int[] charsOffset = new int[nChars];
                            for (int index = 0; index < nChars; index++)
                            {
                                Text.Append((char)dataArray[(int)(offString - 8 + index)]);
                                charsOffset[index] = BitConverter.ToChar(dataArray, (int)(offDx - 8 + index * 4));
                            }

                            //prepare current font
                            if (GDIobjects.Count > 0)
                            {
                                GDIobjectInfo gdiObject = (GDIobjectInfo)GDIobjects[GetGdiObjectIndex(gdiSelectedFont)];
                                if (gdiObject.Font != null)
                                {
                                    int fnt = gdiObject.Font.internalFontNumber;
                                    pdfFont.CurrentFont = fnt;
                                }
                            }

                            //store symbols
                            if ((fOptions & ETO_GLYPH_INDEX) == 0)
                            {
                                pdfFont.StoreUnicodeSymbolsInMap(Text);
                            }
                            else
                            {
                                pdfFont.StoreGlyphsInMap(Text);
                            }
                        }
                        #endregion
                        break;

                    case EmfPlusRecordType.EmfExtTextOutW:
                        #region read data
                        {
                            RECTL Bounds;			//Bounding rectangle, in device units. 
                            uint iGraphicsMode;	//Current graphics mode. This member can be either the GM_COMPATIBLE or GM_ADVANCED value. 
                            float exScale;		//X-scaling factor from page units to .01mm units if the graphics mode is the GM_COMPATIBLE value.
                            float eyScale;		//Y-scaling factor from page units to .01mm units if the graphics mode is the GM_COMPATIBLE value.
                            //	EMRTEXT emrtext :
                            POINTL Reference;		//Logical coordinates of the reference point used to position the string. 
                            uint nChars;			//Number of characters in string. 
                            uint offString;		//Offset to string
                            uint fOptions;			//Value specifying how to use the application-defined rectangle. This member can be a combination of the ETO_CLIPPED and ETO_OPAQUE values. 
                            RECTL rcl;				//Optional clipping and/or opaquing rectangle, in logical units.
                            uint offDx;			//Offset to intercharacter spacing array. 

                            Bounds.Left = ReadInt32();
                            Bounds.Top = ReadInt32();
                            Bounds.Right = ReadInt32();
                            Bounds.Bottom = ReadInt32();
                            iGraphicsMode = ReadUInt32(); //0x10
                            exScale = ReadSingle();
                            eyScale = ReadSingle();

                            Reference.X = ReadInt32();
                            Reference.Y = ReadInt32();
                            nChars = ReadUInt32();
                            offString = ReadUInt32();
                            fOptions = ReadUInt32();
                            rcl.Left = ReadInt32();
                            rcl.Top = ReadInt32();
                            rcl.Right = ReadInt32();
                            rcl.Bottom = ReadInt32();
                            offDx = ReadUInt32();

                            StringBuilder Text = new StringBuilder();
                            int[] charsOffset = new int[nChars];
                            for (int index = 0; index < nChars; index++)
                            {
                                Text.Append(BitConverter.ToChar(dataArray, (int)(offString - 8 + index * 2)));
                                charsOffset[index] = BitConverter.ToChar(dataArray, (int)(offDx - 8 + index * 4));
                            }

                            //prepare current font
                            if (GDIobjects.Count > 0)
                            {
                                GDIobjectInfo gdiObject = (GDIobjectInfo)GDIobjects[GetGdiObjectIndex(gdiSelectedFont)];
                                if (gdiObject.Font != null)
                                {
                                    int fnt = gdiObject.Font.internalFontNumber;
                                    pdfFont.CurrentFont = fnt;
                                }
                            }

                            //store symbols
                            if ((fOptions & ETO_GLYPH_INDEX) == 0)
                            {
                                pdfFont.StoreUnicodeSymbolsInMap(Text);
                            }
                            else
                            {
                                pdfFont.StoreGlyphsInMap(Text);
                            }

                        }
                        #endregion
                        break;

                    case EmfPlusRecordType.EmfExtCreateFontIndirect:
                        #region read data
                        {
                            uint ihFont;		//Index to the font in handle table
                            LOGFONT elfLogFont = new LOGFONT();
                            ihFont = ReadUInt32();
                            elfLogFont.lfHeight = ReadInt32();
                            elfLogFont.lfWidth = ReadInt32();
                            elfLogFont.lfEscapement = ReadInt32();
                            elfLogFont.lfOrientation = ReadInt32();
                            elfLogFont.lfWeight = ReadInt32();
                            elfLogFont.lfItalic = ReadUInt8();
                            elfLogFont.lfUnderline = ReadUInt8();
                            elfLogFont.lfStrikeOut = ReadUInt8();
                            elfLogFont.lfCharSet = ReadUInt8();
                            elfLogFont.lfOutPrecision = ReadUInt8();
                            elfLogFont.lfClipPrecision = ReadUInt8();
                            elfLogFont.lfQuality = ReadUInt8();
                            elfLogFont.lfPitchAndFamily = ReadUInt8();
                            elfLogFont.lfFaceName = ReadStringTchar(LF_FACESIZE);

                            if (elfLogFont.lfHeight == 0) break;	//bad font info

                            FontStyle tempFontStyle = FontStyle.Regular;
                            if (elfLogFont.lfWeight > 500) tempFontStyle |= FontStyle.Bold;
                            if (elfLogFont.lfItalic != 0) tempFontStyle |= FontStyle.Italic;
                            if (elfLogFont.lfUnderline != 0) tempFontStyle |= FontStyle.Underline;
                            Font tempFont = null;
                            try
                            {
                                tempFont = new Font(
                                    StiFontCollection.GetFontFamily(elfLogFont.lfFaceName),
                                    Math.Abs(elfLogFont.lfHeight),		//may be it correct (abs)
                                    tempFontStyle);
                            }
                            catch
                            {
                                tempFont = new Font(
                                    "Arial",
                                    Math.Abs(elfLogFont.lfHeight),		//may be it correct (abs)
                                    tempFontStyle);
                            }
                            elfLogFont.lfFaceName = tempFont.Name;

                            //prepare font structure
                            GDIobjectInfo gdiObject = new GDIobjectInfo();
                            gdiObject.ihNumber = ihFont;
                            EmfFontInfo emff = new EmfFontInfo();
                            emff.internalFontNumber = pdfFont.GetFontNumber(tempFont);
                            emff.elfLogFont = elfLogFont;
                            gdiObject.Font = emff;
                            RemoveObject(gdiObject.ihNumber);
                            GDIobjects.Add(gdiObject);

                        }
                        #endregion
                        break;

                    case EmfPlusRecordType.EmfStretchDIBits:
                        {
                            #region read data
                            tagEMRSTRETCHDIBITS DIBits;
                            DIBits.rclBounds.Left = ReadInt32();
                            DIBits.rclBounds.Top = ReadInt32();
                            DIBits.rclBounds.Right = ReadInt32();
                            DIBits.rclBounds.Bottom = ReadInt32();
                            DIBits.xDest = ReadInt32();
                            DIBits.yDest = ReadInt32();
                            DIBits.xSrc = ReadInt32();
                            DIBits.ySrc = ReadInt32();
                            DIBits.cxSrc = ReadInt32();
                            DIBits.cySrc = ReadInt32();
                            DIBits.offBmiSrc = ReadUInt32();
                            DIBits.cbBmiSrc = ReadUInt32();
                            DIBits.offBitsSrc = ReadUInt32();
                            DIBits.cbBitsSrc = ReadUInt32();
                            DIBits.iUsageSrc = ReadUInt32();
                            DIBits.dwRop = ReadUInt32();
                            DIBits.cxDest = ReadInt32();
                            DIBits.cyDest = ReadInt32();
                            #endregion

                            #region make bitmap
                            dataArray[DIBits.offBmiSrc - 8 - 14 + 0] = 0x42;	//"B"
                            dataArray[DIBits.offBmiSrc - 8 - 14 + 1] = 0x4D;	//"M"
                            BitConverter.GetBytes((uint)(DIBits.cbBmiSrc + DIBits.cbBitsSrc + 14)).CopyTo(dataArray, (int)DIBits.offBmiSrc - 8 - 14 + 2);
                            BitConverter.GetBytes((uint)0).CopyTo(dataArray, (int)DIBits.offBmiSrc - 8 - 14 + 6);
                            BitConverter.GetBytes((uint)(DIBits.cbBmiSrc + 14)).CopyTo(dataArray, (int)DIBits.offBmiSrc - 8 - 14 + 10);
                            MemoryStream mem = new MemoryStream(
                                dataArray,
                                (int)(DIBits.offBmiSrc - 8 - 14),
                                (int)(DIBits.cbBmiSrc + DIBits.cbBitsSrc + 14));
                            Bitmap bmp = new Bitmap(mem);
                            #endregion

                            #region write data
                            if (bmp != null)
                            {
                                //imagesCounter++;
                                pdfService.StoreImageData(bmp, 0, false, false);
                            }
                            mem.Close();
                            bmp.Dispose();
                            #endregion
                        }
                        break;

                    case EmfPlusRecordType.EmfAlphaBlend:
                        {
                            #region read data
                            tagEMRALPHABLEND alpha;
                            alpha.rclBounds.Left = ReadInt32();
                            alpha.rclBounds.Top = ReadInt32();
                            alpha.rclBounds.Right = ReadInt32();
                            alpha.rclBounds.Bottom = ReadInt32();
                            alpha.xDest = (int)GetTransformedX(ReadInt32());
                            alpha.yDest = (int)GetTransformedY(ReadInt32());
                            alpha.cxDest = (int)GetTransformedCX(ReadInt32());
                            alpha.cyDest = (int)GetTransformedCY(ReadInt32());
                            alpha.dwRop = ReadUInt32();
                            alpha.xSrc = ReadInt32();
                            alpha.ySrc = ReadInt32();
                            alpha.xformSrc.eM11 = ReadSingle();
                            alpha.xformSrc.eM12 = ReadSingle();
                            alpha.xformSrc.eM21 = ReadSingle();
                            alpha.xformSrc.eM22 = ReadSingle();
                            alpha.xformSrc.eDx = ReadSingle();
                            alpha.xformSrc.eDy = ReadSingle();
                            alpha.crBkColorSrc = ReadUInt32();
                            alpha.iUsageSrc = ReadUInt32();
                            alpha.offBmiSrc = ReadUInt32();
                            alpha.cbBmiSrc = ReadUInt32();
                            alpha.offBitsSrc = ReadUInt32();
                            alpha.cbBitsSrc = ReadUInt32();
                            alpha.cxSrc = ReadInt32();
                            alpha.cySrc = ReadInt32();
                            #endregion

                            #region make bitmap
                            dataArray[alpha.offBmiSrc - 8 - 14 + 0] = 0x42;	//"B"
                            dataArray[alpha.offBmiSrc - 8 - 14 + 1] = 0x4D;	//"M"
                            BitConverter.GetBytes((uint)(alpha.cbBmiSrc + alpha.cbBitsSrc + 14)).CopyTo(dataArray, (int)alpha.offBmiSrc - 8 - 14 + 2);
                            BitConverter.GetBytes((uint)0).CopyTo(dataArray, (int)alpha.offBmiSrc - 8 - 14 + 6);
                            BitConverter.GetBytes((uint)(alpha.cbBmiSrc + 14)).CopyTo(dataArray, (int)alpha.offBmiSrc - 8 - 14 + 10);
                            MemoryStream mem = new MemoryStream(
                                dataArray,
                                (int)(alpha.offBmiSrc - 8 - 14),
                                (int)(alpha.cbBmiSrc + alpha.cbBitsSrc + 14));
                            Bitmap bmp = new Bitmap(mem);
                            #endregion

                            #region write data
                            if (bmp != null)
                            {
                                //imagesCounter++;
                                pdfService.StoreImageData(bmp, 0, false, false);
                            }
                            mem.Close();
                            bmp.Dispose();
                            #endregion
                        }
                        break;

                    case EmfPlusRecordType.EmfSelectObject:
                        emfSelectedObject = ReadUInt32();
                        SelectObject(emfSelectedObject);
                        break;

                    case EmfPlusRecordType.EmfDeleteObject:
                        RemoveObject(ReadUInt32());
                        break;

                }
                #endregion
            }
            else
            {
                #region Prepare data

                //ms-help://MS.VSCC.2003/MS.MSDNQTR.2003FEB.1033/winprog/winprog/windows_data_types.htm
                //ms-help://MS.VSCC.2003/MS.MSDNQTR.2003FEB.1033/gdi/metafile_5hkj.htm
                //ms-help://MS.VSCC.2003/MS.MSDNQTR.2004APR.1033/gdi/metafile_5hkj.htm
                //ms-help://MS.VSCC.v80/MS.MSDN.v80/MS.WIN32COM.v10.en/gdi/metafile_5hkj.htm

                if ((viewportFlags == 0x0F) &&
                    (recordType != EmfPlusRecordType.EmfSetViewportOrgEx) &&
                    (recordType != EmfPlusRecordType.EmfSetViewportExtEx) &&
                    (recordType != EmfPlusRecordType.EmfSetWindowOrgEx) &&
                    (recordType != EmfPlusRecordType.EmfSetWindowExtEx))
                {
                    SetViewport();
                }

                switch (recordType)
                {

                    case EmfPlusRecordType.EmfHeader:
                        #region read data
                        tagENHMETAHEADER header;

                        header.rclBounds.Left = ReadInt32();
                        header.rclBounds.Top = ReadInt32();
                        header.rclBounds.Right = ReadInt32();
                        header.rclBounds.Bottom = ReadInt32();
                        header.rclFrame.Left = ReadInt32();
                        header.rclFrame.Top = ReadInt32();
                        header.rclFrame.Right = ReadInt32();
                        header.rclFrame.Bottom = ReadInt32();
                        header.dSignature = ReadUInt32();
                        header.nVersion = ReadUInt32();
                        header.nBytes = ReadUInt32();
                        header.nRecords = ReadUInt32();
                        header.nHandles = ReadUInt16();
                        header.sReserved = ReadUInt16();
                        header.nDescription = ReadUInt32();
                        header.offDescription = ReadUInt32();
                        header.nPalEntries = ReadUInt32();
                        header.szlDevice.cX = ReadInt32();
                        header.szlDevice.cY = ReadInt32();
                        header.szlMillimeters.cX = ReadInt32();
                        header.szlMillimeters.cY = ReadInt32();
                        header.cbPixelFormat = ReadUInt32();
                        header.offPixelFormat = ReadUInt32();
                        header.bOpenGL = ReadUInt32();
                        //						#if (WINVER >= 0x0500)
                        //						header.szlMicrometers.cX = ReadInt32(); 
                        //						header.szlMicrometers.cY = ReadInt32(); 

                        emfOffsetX = header.rclBounds.Left;
                        emfOffsetY = header.rclBounds.Top;
                        int emfWidth = header.rclBounds.Right - header.rclBounds.Left;
                        int emfHeight = header.rclBounds.Bottom - header.rclBounds.Top;
                        if (emfOffsetX != 0)
                        {
                            emfWidth += emfOffsetX * 2;
                            emfOffsetX = 0;
                        }
                        if (emfOffsetY != 0)
                        {
                            emfHeight += emfOffsetY * 2;
                            emfOffsetY = 0;
                        }
                        //double emfScaleX = emfComponent.Width / emfWidth;
                        //double emfScaleY = emfComponent.Height / emfHeight;
                        //emfScale = emfScaleX;
                        //if (emfScaleY < emfScaleX) emfScale = emfScaleY;

                        emfScale = hiToTwips;
                        emfScaleX = hiToTwips * 0.996;
                        emfScaleY = hiToTwips;
                        if (pdfService.report.RenderedWith == StiRenderedWith.Wpf)
                            emfScaleY *= StiOptions.Export.Pdf.WpfRichTextVerticalScaling;

                        double dpiX = (header.szlDevice.cX / (double)header.szlMillimeters.cX * 25.4);
                        double dpiY = (header.szlDevice.cY / (double)header.szlMillimeters.cY * 25.4);
                        double dpi = (dpiX + dpiY) / 2f;
                        double k1 = header.szlDevice.cX / (double)header.szlDevice.cY;
                        double k2 = header.szlMillimeters.cX / (double)header.szlMillimeters.cY;
                        double kk = Math.Abs((k1 - k2) / k1);
                        if ((dpi > 110) && (kk < 0.04))
                        {
                            emfScale *= 96f / dpi;
                            emfScaleX *= 96f / dpiX;
                            emfScaleY *= 96f / dpiY;
                        }

                        RectangleD tempRectD = new RectangleD();
                        if (emfComponent.Component is StiRichText)
                        {
                            tempRectD = (emfComponent.Component as StiRichText).ConvertTextMargins(tempRectD, false);
                        }
                        RectangleD rectRichD = new RectangleD(
                            emfComponent.X + tempRectD.X * hiToTwips,
                            emfComponent.Y + (-tempRectD.Bottom) * hiToTwips,
                            emfComponent.Width + tempRectD.Width * hiToTwips,
                            emfComponent.Height + tempRectD.Height * hiToTwips);

                        //emfX = emfComponent.X + (centerEmf ? (emfComponent.Width - emfWidth * emfScale) / 2f : 0);
                        //emfY = emfComponent.Y + emfComponent.Height - (centerEmf ? (emfComponent.Height - emfHeight * emfScale) / 2f : 0);
                        emfX = rectRichD.X + (centerEmf ? (rectRichD.Width - emfWidth * emfScale) / 2f : 0);
                        emfY = rectRichD.Y + rectRichD.Height - (centerEmf ? (rectRichD.Height - emfHeight * emfScale) / 2f : 0);

                        baseXForm = new XFORM(1, 0, 0, 1, 0, 0);
                        viewportFlags = 0;
                        //viewportBack = null;
                        //viewportStack = new Stack();
                        emfMapMode = 0;
                        dcStack = new Stack();
                        #endregion
                        break;

                    case EmfPlusRecordType.EmfSetBkMode:
                        emfBkMode = ReadUInt32();
                        break;

                    case EmfPlusRecordType.EmfSetMapMode:
                        emfMapMode = ReadUInt32();
                        break;

                    case EmfPlusRecordType.EmfSetTextAlign:
                        emfTextAlign = ReadUInt32();
                        break;

                    case EmfPlusRecordType.EmfExtTextOutA:
                        {
                            #region read data
                            tagEMREXTTEXTOUT extText = new tagEMREXTTEXTOUT();

                            extText.Bounds.Left = ReadInt32();
                            extText.Bounds.Top = ReadInt32();
                            extText.Bounds.Right = ReadInt32();
                            extText.Bounds.Bottom = ReadInt32();
                            extText.iGraphicsMode = ReadUInt32(); //0x10	GM_COMPATIBLE = 1, GM_ADVANCED = 2	// ms-help://MS.VSCC.2003/MS.MSDNQTR.2003FEB.1033/gdi/cordspac_2yud.htm
                            extText.exScale = ReadSingle();
                            extText.eyScale = ReadSingle();
                            extText.Reference.X = (int)GetTransformedX(ReadInt32());
                            extText.Reference.Y = (int)GetTransformedY(ReadInt32());
                            extText.nChars = ReadUInt32();
                            extText.offString = ReadUInt32();
                            extText.fOptions = ReadUInt32();
                            extText.rcl.Left = (int)GetTransformedX(ReadInt32());
                            extText.rcl.Top = (int)GetTransformedY(ReadInt32());
                            extText.rcl.Right = (int)GetTransformedX(ReadInt32());
                            extText.rcl.Bottom = (int)GetTransformedY(ReadInt32());
                            extText.offDx = ReadUInt32();

                            StringBuilder Text = new StringBuilder();
                            int[] charsOffset = new int[extText.nChars];
                            for (int index = 0; index < extText.nChars; index++)
                            {
                                Text.Append((char)dataArray[(int)(extText.offString - 8 + index)]);
                                charsOffset[index] = BitConverter.ToChar(dataArray, (int)(extText.offDx - 8 + index * 4));
                            }
                            #endregion

                            WriteTextToStream(extText, Text, charsOffset);
                        }
                        break;

                    case EmfPlusRecordType.EmfExtTextOutW:
                        {
                            #region read data
                            tagEMREXTTEXTOUT extText = new tagEMREXTTEXTOUT();

                            extText.Bounds.Left = ReadInt32();
                            extText.Bounds.Top = ReadInt32();
                            extText.Bounds.Right = ReadInt32();
                            extText.Bounds.Bottom = ReadInt32();
                            extText.iGraphicsMode = ReadUInt32(); //0x10	GM_COMPATIBLE = 1, GM_ADVANCED = 2	// ms-help://MS.VSCC.2003/MS.MSDNQTR.2003FEB.1033/gdi/cordspac_2yud.htm
                            extText.exScale = ReadSingle();
                            extText.eyScale = ReadSingle();
                            extText.Reference.X = (int)GetTransformedX(ReadInt32());
                            extText.Reference.Y = (int)GetTransformedY(ReadInt32());
                            extText.nChars = ReadUInt32();
                            extText.offString = ReadUInt32();
                            extText.fOptions = ReadUInt32();
                            extText.rcl.Left = (int)GetTransformedX(ReadInt32());
                            extText.rcl.Top = (int)GetTransformedY(ReadInt32());
                            extText.rcl.Right = (int)GetTransformedX(ReadInt32());
                            extText.rcl.Bottom = (int)GetTransformedY(ReadInt32());
                            extText.offDx = ReadUInt32();

                            StringBuilder Text = new StringBuilder();
                            int[] charsOffset = new int[extText.nChars];

                            int offDxStep = (extText.fOptions & ETO_PDY) > 0 ? 8 : 4;
                            for (int index = 0; index < extText.nChars; index++)
                            {
                                Text.Append(BitConverter.ToChar(dataArray, (int)(extText.offString - 8 + index * 2)));
                                charsOffset[index] = BitConverter.ToInt32(dataArray, (int)(extText.offDx - 8 + index * offDxStep));
                            }
                            #endregion

                            WriteTextToStream(extText, Text, charsOffset);
                        }
                        break;

                    case EmfPlusRecordType.EmfExtCreateFontIndirect:
                        #region read data
                        {
                            uint ihFont;		//Index to the font in handle table
                            LOGFONT elfLogFont;
                            ihFont = ReadUInt32();
                            elfLogFont.lfHeight = ReadInt32();
                            elfLogFont.lfWidth = ReadInt32();
                            elfLogFont.lfEscapement = ReadInt32();
                            elfLogFont.lfOrientation = ReadInt32();
                            elfLogFont.lfWeight = ReadInt32();
                            elfLogFont.lfItalic = ReadUInt8();
                            elfLogFont.lfUnderline = ReadUInt8();
                            elfLogFont.lfStrikeOut = ReadUInt8();
                            elfLogFont.lfCharSet = ReadUInt8();
                            elfLogFont.lfOutPrecision = ReadUInt8();
                            elfLogFont.lfClipPrecision = ReadUInt8();
                            elfLogFont.lfQuality = ReadUInt8();
                            elfLogFont.lfPitchAndFamily = ReadUInt8();
                            elfLogFont.lfFaceName = ReadStringTchar(LF_FACESIZE);

                            if (elfLogFont.lfHeight == 0) break;	//bad font info

                            elfLogFont.lfCharSet = 0;	//fix for some system

                            FontStyle tempFontStyle = FontStyle.Regular;
                            if (elfLogFont.lfWeight > 500) tempFontStyle |= FontStyle.Bold;
                            if (elfLogFont.lfItalic != 0) tempFontStyle |= FontStyle.Italic;
                            if (elfLogFont.lfUnderline != 0) tempFontStyle |= FontStyle.Underline;
                            Font tempFont = null;
                            try
                            {
                                tempFont = new Font(
                                    StiFontCollection.GetFontFamily(elfLogFont.lfFaceName),
                                    Math.Abs(elfLogFont.lfHeight),		//may be it correct (abs)
                                    tempFontStyle);
                            }
                            catch
                            {
                                tempFont = new Font(
                                    "Arial",
                                    Math.Abs(elfLogFont.lfHeight),		//may be it correct (abs)
                                    tempFontStyle);
                            }
                            elfLogFont.lfFaceName = tempFont.Name;

                            //prepare font structure
                            GDIobjectInfo gdiObject = new GDIobjectInfo();
                            gdiObject.ihNumber = ihFont;
                            EmfFontInfo emff = new EmfFontInfo();
                            emff.internalFontNumber = pdfFont.GetFontNumber(tempFont);
                            emff.elfLogFont = elfLogFont;
                            gdiObject.Font = emff;
                            RemoveObject(gdiObject.ihNumber);
                            GDIobjects.Add(gdiObject);
                        }
                        #endregion
                        break;

                    case EmfPlusRecordType.EmfCreateBrushIndirect:
                        #region read data
                        {
                            uint ihBrush = ReadUInt32();
                            uint lbStyle = ReadUInt32();	//Specifies the brush style
                            uint lbColor = ReadUInt32(); ;	//Specifies the color in which the brush is to be drawn
                            uint lbHatch = ReadUInt32();	//Specifies a hatch style. The meaning depends on the brush style defined by lbStyle. 
                            //prepare brush structure
                            GDIobjectInfo gdiObject = new GDIobjectInfo();
                            gdiObject.ihNumber = ihBrush;
                            EmfBrushInfo emfb = new EmfBrushInfo();
                            emfb.lbStyle = lbStyle;
                            emfb.lbColor = lbColor;
                            emfb.lbHatch = lbHatch;
                            gdiObject.Brush = emfb;
                            RemoveObject(gdiObject.ihNumber);
                            GDIobjects.Add(gdiObject);
                        }
                        #endregion
                        break;

                    case EmfPlusRecordType.EmfCreatePen:
                        #region read data
                        {
                            POINTL lopnWidth;
                            uint ihPen = ReadUInt32();
                            uint lopnStyle = ReadUInt32();
                            lopnWidth.X = ReadInt32();	//pen width
                            lopnWidth.Y = ReadInt32();	//not used
                            uint lopnColor = ReadUInt32();
                            //prepare pen structure
                            GDIobjectInfo gdiObject = new GDIobjectInfo();
                            gdiObject.ihNumber = ihPen;
                            EmfPenInfo emfp = new EmfPenInfo();
                            emfp.lopnStyle = lopnStyle;
                            emfp.lopnWidth = lopnWidth.X;
                            emfp.lopnColor = lopnColor;
                            gdiObject.Pen = emfp;
                            RemoveObject(gdiObject.ihNumber);
                            GDIobjects.Add(gdiObject);
                        }
                        #endregion
                        break;

                    case EmfPlusRecordType.EmfExtCreatePen:
                        #region read data
                        {
                            uint ihPen = ReadUInt32();
                            uint offBmi = ReadUInt32();
                            uint cbBmi = ReadUInt32();
                            uint offBits = ReadUInt32();
                            uint cbBits = ReadUInt32();
                            //ms-help://MS.VSCC.2003/MS.MSDNQTR.2003FEB.1033/gdi/pens_07zm.htm
                            //EXTLOGPEN
                            uint elpPenStyle = ReadUInt32();
                            uint elpWidth = ReadUInt32();
                            uint elpBrushStyle = ReadUInt32();
                            uint elpColor = ReadUInt32();
                            uint elpHatch = ReadUInt32();
                            uint elpNumEntries = ReadUInt32();
                            uint[] elpStyleEntry = new uint[elpNumEntries];
                            for (int index = 0; index < elpNumEntries; index++)
                            {
                                elpStyleEntry[index] = ReadUInt32();
                            }

                            //prepare pen structure
                            GDIobjectInfo gdiObject = new GDIobjectInfo();
                            gdiObject.ihNumber = ihPen;
                            EmfPenInfo emfp = new EmfPenInfo();
                            emfp.lopnStyle = elpPenStyle;
                            emfp.lopnWidth = (int)elpWidth;
                            emfp.lopnColor = elpColor;
                            gdiObject.Pen = emfp;
                            RemoveObject(gdiObject.ihNumber);
                            GDIobjects.Add(gdiObject);
                        }
                        #endregion
                        break;

                    case EmfPlusRecordType.EmfSelectObject:
                        emfSelectedObject = ReadUInt32();
                        SelectObject(emfSelectedObject);
                        break;

                    case EmfPlusRecordType.EmfDeleteObject:
                        RemoveObject(ReadUInt32());
                        break;

                    case EmfPlusRecordType.EmfSetTextColor:
                        emfTextColor = ReadUInt32();
                        break;

                    case EmfPlusRecordType.EmfSetBkColor:
                        emfBackColor = ReadUInt32();
                        break;

                    case EmfPlusRecordType.EmfMoveToEx:
                        #region read data
                        emfCurrentPoint = GetTransformedXY(ReadInt32(), ReadInt32());
                        if (makePath)
                        {
                            string stPoint = string.Format("{0} {1} m",
                                ConvertToString(emfX + emfCurrentPoint.X * emfScaleX),
                                ConvertToString(emfY - emfCurrentPoint.Y * emfScaleY));
                            emfPath.Add(stPoint);
                        }
                        #endregion
                        break;

                    case EmfPlusRecordType.EmfLineTo:
                        #region read data
                        POINTD newPoint = GetTransformedXY(ReadInt32(), ReadInt32());
                        if (GDIobjects.Count > 0)
                        {
                            string stPoint = string.Format("{0} {1} m",
                                ConvertToString(emfX + emfCurrentPoint.X * emfScaleX),
                                ConvertToString(emfY - emfCurrentPoint.Y * emfScaleY));
                            string stLine = string.Format("{0} {1} l",
                                ConvertToString(emfX + newPoint.X * emfScaleX),
                                ConvertToString(emfY - newPoint.Y * emfScaleY));
                            if (makePath)
                            {
                                emfPath.Add(stLine);
                            }
                            else
                            {
                                GDIobjectInfo gdiObject = (GDIobjectInfo)GDIobjects[GetGdiObjectIndex(gdiSelectedPen)];
                                if (gdiObject.Pen != null)
                                {
                                    SetPdfColor(gdiObject.Pen.lopnColor);
                                    double penWidth = GetTransformedW(gdiObject.Pen.lopnWidth) * emfScale;
                                    if (penWidth == 0) penWidth = penWidthDefault;
                                    pageStream.WriteLine("{0} w", ConvertToString(penWidth * penWidthScale));
                                    pageStream.WriteLine(stPoint);
                                    pageStream.WriteLine(stLine + " S");
                                }
                            }
                        }
                        emfCurrentPoint.X = newPoint.X;
                        emfCurrentPoint.Y = newPoint.Y;
                        #endregion
                        break;

                    case EmfPlusRecordType.EmfRectangle:
                        {
                            #region read data
                            RECTL rclBoxL;
                            rclBoxL.Left = ReadInt32();
                            rclBoxL.Top = ReadInt32();
                            rclBoxL.Right = ReadInt32();
                            rclBoxL.Bottom = ReadInt32();
                            RectangleD rclBox = new RectangleD(
                                GetTransformedX(rclBoxL.Left),
                                GetTransformedY(rclBoxL.Top),
                                GetTransformedX(rclBoxL.Right) - GetTransformedX(rclBoxL.Left),
                                GetTransformedY(rclBoxL.Bottom) - GetTransformedY(rclBoxL.Top));
                            #endregion

                            #region write data
                            if (GDIobjects.Count > 0)
                            {
                                bool needDraw = false;
                                bool needFill = false;
                                GDIobjectInfo gdiObject1 = (GDIobjectInfo)GDIobjects[GetGdiObjectIndex(gdiSelectedPen)];
                                if (gdiObject1.Pen != null)
                                {
                                    SetPdfColor(gdiObject1.Pen.lopnColor);
                                    double penWidth = GetTransformedW(gdiObject1.Pen.lopnWidth) * emfScale;
                                    if (penWidth == 0) penWidth = penWidthDefault;
                                    pageStream.WriteLine("{0} w", ConvertToString(penWidth * penWidthScale));
                                    needDraw = true;
                                }
                                GDIobjectInfo gdiObject2 = (GDIobjectInfo)GDIobjects[GetGdiObjectIndex(gdiSelectedBrush)];
                                if (gdiObject2.Brush != null)
                                {
                                    SetPdfBackColor(gdiObject2.Brush.lbColor);
                                    needFill = true;
                                }
                                ArrayList path = new ArrayList();
                                string stPoint = string.Format("{0} {1} m",
                                    ConvertToString(emfX + rclBox.Left * emfScaleX),
                                    ConvertToString(emfY - rclBox.Bottom * emfScaleY));
                                path.Add(stPoint);
                                string stLine = string.Format("{0} {1} l",
                                    ConvertToString(emfX + rclBox.Left * emfScaleX),
                                    ConvertToString(emfY - rclBox.Top * emfScaleY));
                                path.Add(stLine);
                                stLine = string.Format("{0} {1} l",
                                    ConvertToString(emfX + rclBox.Right * emfScaleX),
                                    ConvertToString(emfY - rclBox.Top * emfScaleY));
                                path.Add(stLine);
                                stLine = string.Format("{0} {1} l",
                                    ConvertToString(emfX + rclBox.Right * emfScaleX),
                                    ConvertToString(emfY - rclBox.Bottom * emfScaleY));
                                path.Add(stLine);
                                string stCommand = (needFill ? (needDraw ? "h B" : "h f") : (needDraw ? "h S" : string.Empty));
                                if (stCommand != string.Empty)
                                {
                                    for (int indexLine = 0; indexLine < path.Count; indexLine++)
                                    {
                                        pageStream.WriteLine((string)path[indexLine]);
                                    }
                                    pageStream.WriteLine(stCommand);
                                }
                                path.Clear();
                            }
                            #endregion
                        }
                        break;

                    case EmfPlusRecordType.EmfPolyBezier16:
                    case EmfPlusRecordType.EmfPolyBezierTo16:
                        {
                            #region read data
                            RECTL rclBounds;
                            rclBounds.Left = ReadInt32();
                            rclBounds.Top = ReadInt32();
                            rclBounds.Right = ReadInt32();
                            rclBounds.Bottom = ReadInt32();
                            uint cpts = ReadUInt32();
                            //						POINTS[] apts = new POINTS[cpts];
                            POINTD[] apts = new POINTD[cpts];
                            for (int index = 0; index < cpts; index++)
                            {
                                apts[index] = GetTransformedXY(ReadInt16(), ReadInt16());
                                //							POINTL point = GetTransformedXY(ReadInt16(), ReadInt16());
                                //							apts[index].X = (short)point.X;
                                //							apts[index].Y = (short)point.Y;
                            }
                            #endregion

                            #region write data
                            if (GDIobjects.Count > 0)
                            {
                                bool needDraw = false;
                                GDIobjectInfo gdiObject = (GDIobjectInfo)GDIobjects[GetGdiObjectIndex(gdiSelectedPen)];
                                if (gdiObject.Pen != null) needDraw = true;
                                POINTD startPoint = emfCurrentPoint;
                                int indexPoint = 0;
                                if (recordType == EmfPlusRecordType.EmfPolyBezier16)
                                {
                                    startPoint = apts[0];
                                    indexPoint++;
                                }
                                string stPoint = string.Format("{0} {1} m",
                                    ConvertToString(emfX + startPoint.X * emfScaleX),
                                    ConvertToString(emfY - startPoint.Y * emfScaleY));
                                if (!makePath && needDraw)
                                {
                                    SetPdfColor(gdiObject.Pen.lopnColor);
                                    double penWidth = GetTransformedW(gdiObject.Pen.lopnWidth) * emfScale;
                                    if (penWidth == 0) penWidth = penWidthDefault;
                                    pageStream.WriteLine("{0} w", ConvertToString(penWidth * penWidthScale));
                                    pageStream.WriteLine(stPoint);
                                }
                                if (makePath && recordType == EmfPlusRecordType.EmfPolyBezier16)
                                {
                                    emfPath.Add(stPoint);
                                }
                                while (indexPoint < cpts)
                                {
                                    string stLine = string.Format("{0} {1} {2} {3} {4} {5} c ",
                                        ConvertToString(emfX + apts[indexPoint + 0].X * emfScaleX),
                                        ConvertToString(emfY - apts[indexPoint + 0].Y * emfScaleY),
                                        ConvertToString(emfX + apts[indexPoint + 1].X * emfScaleX),
                                        ConvertToString(emfY - apts[indexPoint + 1].Y * emfScaleY),
                                        ConvertToString(emfX + apts[indexPoint + 2].X * emfScaleX),
                                        ConvertToString(emfY - apts[indexPoint + 2].Y * emfScaleY));
                                    if (makePath)
                                    {
                                        emfPath.Add(stLine);
                                    }
                                    if (!makePath && needDraw)
                                    {
                                        pageStream.WriteLine(stLine);
                                    }
                                    indexPoint += 3;
                                }
                                if (!makePath && needDraw)
                                {
                                    pageStream.WriteLine("S");
                                }
                                if (recordType == EmfPlusRecordType.EmfPolyBezierTo16)
                                {
                                    emfCurrentPoint.X = apts[indexPoint - 3 + 2].X;
                                    emfCurrentPoint.Y = apts[indexPoint - 3 + 2].Y;
                                }
                            }
                            #endregion
                        }
                        break;

                    case EmfPlusRecordType.EmfPolyline16:
                    case EmfPlusRecordType.EmfPolylineTo16:
                        {
                            #region read data
                            RECTL rclBounds;
                            rclBounds.Left = ReadInt32();
                            rclBounds.Top = ReadInt32();
                            rclBounds.Right = ReadInt32();
                            rclBounds.Bottom = ReadInt32();
                            uint cpts = ReadUInt32();
                            //						POINTS[] apts = new POINTS[cpts];
                            POINTD[] apts = new POINTD[cpts];
                            for (int index = 0; index < cpts; index++)
                            {
                                apts[index] = GetTransformedXY(ReadInt16(), ReadInt16());
                                //							POINTL point = GetTransformedXY(ReadInt16(), ReadInt16());
                                //							apts[index].X = (short)point.X;
                                //							apts[index].Y = (short)point.Y;
                            }
                            #endregion

                            #region write data
                            if (GDIobjects.Count > 0)
                            {
                                bool needDraw = false;
                                GDIobjectInfo gdiObject = (GDIobjectInfo)GDIobjects[GetGdiObjectIndex(gdiSelectedPen)];
                                if (gdiObject.Pen != null) needDraw = true;
                                POINTD startPoint = emfCurrentPoint;
                                int indexPoint = 0;
                                if (recordType == EmfPlusRecordType.EmfPolyline16)
                                {
                                    startPoint = apts[0];
                                    indexPoint++;
                                }
                                string stPoint = string.Format("{0} {1} m",
                                    ConvertToString(emfX + startPoint.X * emfScaleX),
                                    ConvertToString(emfY - startPoint.Y * emfScaleY));
                                if (!makePath && needDraw)
                                {
                                    SetPdfColor(gdiObject.Pen.lopnColor);
                                    double penWidth = GetTransformedW(gdiObject.Pen.lopnWidth) * emfScale;
                                    if (penWidth == 0) penWidth = penWidthDefault;
                                    pageStream.WriteLine("{0} w", ConvertToString(penWidth * penWidthScale));
                                    pageStream.WriteLine(stPoint);
                                }
                                if (makePath && recordType == EmfPlusRecordType.EmfPolyline16)
                                {
                                    emfPath.Add(stPoint);
                                }
                                for (; indexPoint < cpts; indexPoint++)
                                {
                                    string stLine = string.Format("{0} {1} l",
                                        ConvertToString(emfX + apts[indexPoint].X * emfScaleX),
                                        ConvertToString(emfY - apts[indexPoint].Y * emfScaleY));
                                    if (makePath)
                                    {
                                        emfPath.Add(stLine);
                                    }
                                    if (!makePath && needDraw)
                                    {
                                        pageStream.WriteLine(stLine);
                                    }
                                }
                                if (!makePath && needDraw)
                                {
                                    pageStream.WriteLine("S");
                                }
                                if (recordType == EmfPlusRecordType.EmfPolylineTo16)
                                {
                                    emfCurrentPoint.X = apts[cpts - 1].X;
                                    emfCurrentPoint.Y = apts[cpts - 1].Y;
                                }
                            }
                            #endregion
                        }
                        break;

                    case EmfPlusRecordType.EmfSetPolyFillMode:
                        emfPolyFillMode = ReadUInt32();
                        break;

                    case EmfPlusRecordType.EmfPolygon16:
                        {
                            #region read data
                            RECTL rclBounds;
                            rclBounds.Left = ReadInt32();
                            rclBounds.Top = ReadInt32();
                            rclBounds.Right = ReadInt32();
                            rclBounds.Bottom = ReadInt32();
                            uint cpts = ReadUInt32();
                            POINTD[] apts = new POINTD[cpts];
                            for (int index = 0; index < cpts; index++)
                            {
                                apts[index] = GetTransformedXY(ReadInt16(), ReadInt16());
                            }
                            #endregion

                            #region write data
                            if (GDIobjects.Count > 0)
                            {
                                bool needDraw = false;
                                bool needFill = false;
                                GDIobjectInfo gdiObject1 = (GDIobjectInfo)GDIobjects[GetGdiObjectIndex(gdiSelectedPen)];
                                if (gdiObject1.Pen != null) needDraw = true;
                                GDIobjectInfo gdiObject2 = (GDIobjectInfo)GDIobjects[GetGdiObjectIndex(gdiSelectedBrush)];
                                if (gdiObject2.Brush != null) needFill = true;
                                ArrayList path = new ArrayList();
                                POINTD startPoint = apts[0];
                                string stPoint = string.Format("{0} {1} m",
                                    ConvertToString(emfX + startPoint.X * emfScaleX),
                                    ConvertToString(emfY - startPoint.Y * emfScaleY));
                                path.Add(stPoint);
                                for (int indexPoint = 1; indexPoint < cpts; indexPoint++)
                                {
                                    string stLine = string.Format("{0} {1} l",
                                        ConvertToString(emfX + apts[indexPoint].X * emfScaleX),
                                        ConvertToString(emfY - apts[indexPoint].Y * emfScaleY));
                                    path.Add(stLine);
                                }
                                if (needFill)
                                {
                                    SetPdfBackColor(gdiObject2.Brush.lbColor);
                                    for (int indexLine = 0; indexLine < path.Count; indexLine++)
                                    {
                                        pageStream.WriteLine((string)path[indexLine]);
                                    }
                                    pageStream.WriteLine("h f");
                                }
                                if (needDraw)
                                {
                                    SetPdfColor(gdiObject1.Pen.lopnColor);
                                    double penWidth = GetTransformedW(gdiObject1.Pen.lopnWidth) * emfScale;
                                    if (penWidth == 0) penWidth = penWidthDefault;
                                    pageStream.WriteLine("{0} w", ConvertToString(penWidth * penWidthScale));
                                    for (int indexLine = 0; indexLine < path.Count; indexLine++)
                                    {
                                        pageStream.WriteLine((string)path[indexLine]);
                                    }
                                    pageStream.WriteLine("h S");
                                }
                                path.Clear();
                            }
                            #endregion
                        }
                        break;

                    case EmfPlusRecordType.EmfPolyPolygon16:
                        {
                            #region read data
                            RECTL rclBounds;
                            rclBounds.Left = ReadInt32();
                            rclBounds.Top = ReadInt32();
                            rclBounds.Right = ReadInt32();
                            rclBounds.Bottom = ReadInt32();
                            uint nPolys = ReadUInt32();
                            uint cpts = ReadUInt32();
                            uint[] aPolyCounts = new uint[nPolys];
                            for (int index = 0; index < nPolys; index++)
                            {
                                aPolyCounts[index] = ReadUInt32();
                            }
                            POINTD[] apts = new POINTD[cpts];
                            for (int index = 0; index < cpts; index++)
                            {
                                apts[index] = GetTransformedXY(ReadInt16(), ReadInt16());
                            }
                            #endregion

                            #region write data
                            if (GDIobjects.Count > 0)
                            {
                                bool needDraw = false;
                                bool needFill = false;
                                GDIobjectInfo gdiObject1 = (GDIobjectInfo)GDIobjects[GetGdiObjectIndex(gdiSelectedPen)];
                                if (gdiObject1.Pen != null) needDraw = true;
                                GDIobjectInfo gdiObject2 = (GDIobjectInfo)GDIobjects[GetGdiObjectIndex(gdiSelectedBrush)];
                                if (gdiObject2.Brush != null) needFill = true;

                                ArrayList path = new ArrayList();
                                uint indexApts = 0;
                                for (int indexPolygon = 0; indexPolygon < nPolys; indexPolygon++)
                                {
                                    POINTD startPoint = apts[indexApts];
                                    string stPoint = string.Format("{0} {1} m",
                                        ConvertToString(emfX + startPoint.X * emfScaleX),
                                        ConvertToString(emfY - startPoint.Y * emfScaleY));
                                    path.Add(stPoint);
                                    for (int indexPoint = 1; indexPoint < aPolyCounts[indexPolygon]; indexPoint++)
                                    {
                                        string stLine = string.Format("{0} {1} l",
                                            ConvertToString(emfX + apts[indexApts + indexPoint].X * emfScaleX),
                                            ConvertToString(emfY - apts[indexApts + indexPoint].Y * emfScaleY));
                                        path.Add(stLine);
                                    }
                                    path.Add("h");
                                    indexApts += aPolyCounts[indexPolygon];
                                }
                                if (needFill)
                                {
                                    SetPdfBackColor(gdiObject2.Brush.lbColor);
                                    for (int indexLine = 0; indexLine < path.Count; indexLine++)
                                    {
                                        pageStream.WriteLine((string)path[indexLine]);
                                    }
                                    pageStream.WriteLine("f{0}", (emfPolyFillMode == 1 ? "*" : ""));
                                }
                                if (needDraw)
                                {
                                    SetPdfColor(gdiObject1.Pen.lopnColor);
                                    double penWidth = GetTransformedW(gdiObject1.Pen.lopnWidth) * emfScale;
                                    if (penWidth == 0) penWidth = penWidthDefault;
                                    pageStream.WriteLine("{0} w", ConvertToString(penWidth * penWidthScale));
                                    for (int indexLine = 0; indexLine < path.Count; indexLine++)
                                    {
                                        pageStream.WriteLine((string)path[indexLine]);
                                    }
                                    pageStream.WriteLine("S");
                                }
                                path.Clear();
                            }
                            #endregion
                        }
                        break;

                    case EmfPlusRecordType.EmfBitBlt:
                        {
                            #region read data
                            tagEMRBITBLT bitblt;
                            bitblt.rclBounds.Left = ReadInt32();
                            bitblt.rclBounds.Top = ReadInt32();
                            bitblt.rclBounds.Right = ReadInt32();
                            bitblt.rclBounds.Bottom = ReadInt32();
                            bitblt.xDest = (int)GetTransformedX(ReadInt32());
                            bitblt.yDest = (int)GetTransformedY(ReadInt32());
                            bitblt.cxDest = (int)GetTransformedCX(ReadInt32());
                            bitblt.cyDest = (int)GetTransformedCY(ReadInt32());
                            bitblt.dwRop = ReadUInt32();
                            bitblt.xSrc = ReadInt32();
                            bitblt.ySrc = ReadInt32();
                            bitblt.xformSrc.eM11 = ReadSingle();
                            bitblt.xformSrc.eM12 = ReadSingle();
                            bitblt.xformSrc.eM21 = ReadSingle();
                            bitblt.xformSrc.eM22 = ReadSingle();
                            bitblt.xformSrc.eDx = ReadSingle();
                            bitblt.xformSrc.eDy = ReadSingle();
                            bitblt.crBkColorSrc = ReadUInt32();
                            bitblt.iUsageSrc = ReadUInt32();
                            bitblt.offBmiSrc = ReadUInt32();
                            bitblt.cbBmiSrc = ReadUInt32();
                            bitblt.offBitsSrc = ReadUInt32();
                            bitblt.cbBitsSrc = ReadUInt32();
                            #endregion

                            #region write data
                            switch (bitblt.dwRop)
                            {
                                case PATCOPY:
                                    #region dwRop PATCOPY
                                    if (GDIobjects.Count > 0)
                                    {
                                        GDIobjectInfo gdiObject = (GDIobjectInfo)GDIobjects[GetGdiObjectIndex(gdiSelectedBrush)];
                                        if (gdiObject.Brush != null)
                                        {
                                            SetPdfBackColor(gdiObject.Brush.lbColor);	//fill color
                                            pageStream.WriteLine("{0} {1} {2} {3} re f",
                                                ConvertToString(emfX + bitblt.xDest * emfScaleX),
                                                ConvertToString(emfY - (bitblt.yDest + bitblt.cyDest) * emfScaleY),
                                                ConvertToString(bitblt.cxDest * emfScale),
                                                ConvertToString(bitblt.cyDest * emfScale));
                                        }
                                    }
                                    #endregion
                                    break;

                            }
                            #endregion
                        }
                        break;

                    case EmfPlusRecordType.EmfStretchDIBits:
                        {
                            #region read data
                            tagEMRSTRETCHDIBITS DIBits;
                            DIBits.rclBounds.Left = (int)GetTransformedX(ReadInt32());
                            DIBits.rclBounds.Top = (int)GetTransformedY(ReadInt32());
                            DIBits.rclBounds.Right = (int)GetTransformedX(ReadInt32());
                            DIBits.rclBounds.Bottom = (int)GetTransformedY(ReadInt32());
                            DIBits.xDest = ReadInt32();
                            DIBits.yDest = ReadInt32();
                            DIBits.xSrc = ReadInt32();
                            DIBits.ySrc = ReadInt32();
                            DIBits.cxSrc = ReadInt32();
                            DIBits.cySrc = ReadInt32();
                            DIBits.offBmiSrc = ReadUInt32();
                            DIBits.cbBmiSrc = ReadUInt32();
                            DIBits.offBitsSrc = ReadUInt32();
                            DIBits.cbBitsSrc = ReadUInt32();
                            DIBits.iUsageSrc = ReadUInt32();
                            DIBits.dwRop = ReadUInt32();
                            DIBits.cxDest = (int)GetTransformedX(ReadInt32());
                            DIBits.cyDest = (int)GetTransformedY(ReadInt32());
                            #endregion

                            #region write data
                            //pageStream.WriteLine("q");
                            Stimulsoft.Report.Export.StiPdfExportService.StiPdfData pp = new Stimulsoft.Report.Export.StiPdfExportService.StiPdfData();
                            double scale = hiToTwips / emfScale;
                            pp.X = emfX + DIBits.xDest * emfScaleX;
                            pp.Y = emfY - (DIBits.yDest + DIBits.cyDest) * emfScaleY;
                            double resX = ((float)DIBits.cxSrc / (float)(DIBits.cxDest)) * scale;
                            double resY = ((float)DIBits.cySrc / (float)(DIBits.cyDest)) * scale;
                            if ((DIBits.cxSrc == DIBits.cxDest) && (DIBits.cySrc == DIBits.cyDest))
                            {
                                resX = hiToTwips / emfScaleX;
                                resY = hiToTwips / emfScaleY;
                            }
                            pdfService.WriteImageInfo2(pp, resX, resY);
                            //pageStream.WriteLine("Q");
                            #endregion
                        }
                        break;

                    case EmfPlusRecordType.EmfAlphaBlend:
                        {
                            #region read data
                            tagEMRALPHABLEND alpha;
                            alpha.rclBounds.Left = ReadInt32();
                            alpha.rclBounds.Top = ReadInt32();
                            alpha.rclBounds.Right = ReadInt32();
                            alpha.rclBounds.Bottom = ReadInt32();
                            alpha.xDest = (int)GetTransformedX(ReadInt32());
                            alpha.yDest = (int)GetTransformedY(ReadInt32());
                            alpha.cxDest = (int)GetTransformedCX(ReadInt32());
                            alpha.cyDest = (int)GetTransformedCY(ReadInt32());
                            alpha.dwRop = ReadUInt32();
                            alpha.xSrc = ReadInt32();
                            alpha.ySrc = ReadInt32();
                            alpha.xformSrc.eM11 = ReadSingle();
                            alpha.xformSrc.eM12 = ReadSingle();
                            alpha.xformSrc.eM21 = ReadSingle();
                            alpha.xformSrc.eM22 = ReadSingle();
                            alpha.xformSrc.eDx = ReadSingle();
                            alpha.xformSrc.eDy = ReadSingle();
                            alpha.crBkColorSrc = ReadUInt32();
                            alpha.iUsageSrc = ReadUInt32();
                            alpha.offBmiSrc = ReadUInt32();
                            alpha.cbBmiSrc = ReadUInt32();
                            alpha.offBitsSrc = ReadUInt32();
                            alpha.cbBitsSrc = ReadUInt32();
                            alpha.cxSrc = ReadInt32();
                            alpha.cySrc = ReadInt32();
                            #endregion

                            #region write data
                            //pageStream.WriteLine("q");
                            Stimulsoft.Report.Export.StiPdfExportService.StiPdfData pp = new Stimulsoft.Report.Export.StiPdfExportService.StiPdfData();
                            double scale = hiToTwips / emfScale;
                            pp.X = emfX + alpha.xDest * emfScaleX;
                            pp.Y = emfY - (alpha.yDest + alpha.cyDest) * emfScaleY;
                            double resX = ((float)alpha.cxSrc / (float)(alpha.cxDest)) * scale;
                            double resY = ((float)alpha.cySrc / (float)(alpha.cyDest)) * scale;
                            if ((alpha.cxSrc == alpha.cxDest) && (alpha.cySrc == alpha.cyDest))
                            {
                                resX = hiToTwips / emfScaleX;
                                resY = hiToTwips / emfScaleY;
                            }
                            pdfService.WriteImageInfo2(pp, resX, resY);
                            //pageStream.WriteLine("Q");
                            #endregion
                        }
                        break;

                    case EmfPlusRecordType.EmfBeginPath:
                        makePath = true;
                        emfPath = new ArrayList();
                        break;

                    case EmfPlusRecordType.EmfEndPath:
                        makePath = false;
                        break;

                    case EmfPlusRecordType.EmfCloseFigure:
                        emfPath.Add("h");
                        break;

                    case EmfPlusRecordType.EmfSetWorldTransform:
                        #region read data
                        XFORM sxform = new XFORM();
                        sxform.eM11 = ReadSingle();
                        sxform.eM12 = ReadSingle();
                        sxform.eM21 = ReadSingle();
                        sxform.eM22 = ReadSingle();
                        sxform.eDx = ReadSingle();
                        sxform.eDy = ReadSingle();

                        baseXForm = sxform;

                        //						emfScale = baseEmfScale * baseXForm.eM11;
                        #endregion
                        break;

                    case EmfPlusRecordType.EmfModifyWorldTransform:
                        #region read data
                        XFORM mxform = new XFORM();
                        mxform.eM11 = ReadSingle();
                        mxform.eM12 = ReadSingle();
                        mxform.eM21 = ReadSingle();
                        mxform.eM22 = ReadSingle();
                        mxform.eDx = ReadSingle();
                        mxform.eDy = ReadSingle();

                        baseXForm.eM11 *= mxform.eM11;
                        baseXForm.eM22 *= mxform.eM22;

                        //						emfScale = baseEmfScale * baseXForm.eM11;
                        #endregion
                        break;

                    case EmfPlusRecordType.EmfStrokePath:
                        {
                            #region read data
                            RECTL rclBounds;
                            rclBounds.Left = ReadInt32();
                            rclBounds.Top = ReadInt32();
                            rclBounds.Right = ReadInt32();
                            rclBounds.Bottom = ReadInt32();
                            #endregion

                            #region write data
                            if (GDIobjects.Count > 0)
                            {
                                GDIobjectInfo gdiObject = (GDIobjectInfo)GDIobjects[GetGdiObjectIndex(gdiSelectedPen)];
                                if (gdiObject.Pen != null)
                                {
                                    SetPdfColor(gdiObject.Pen.lopnColor);
                                    double penWidth = GetTransformedW(gdiObject.Pen.lopnWidth) * emfScale;
                                    if (penWidth == 0) penWidth = penWidthDefault;
                                    pageStream.WriteLine("{0} w", ConvertToString(penWidth * penWidthScale));
                                    foreach (string st in emfPath)
                                    {
                                        pageStream.WriteLine(st);
                                    }
                                    pageStream.WriteLine("S");
                                    emfPath.Clear();
                                }
                            }
                            #endregion
                        }
                        break;

                    case EmfPlusRecordType.EmfFillPath:
                        {
                            #region read data
                            RECTL rclBounds;
                            rclBounds.Left = ReadInt32();
                            rclBounds.Top = ReadInt32();
                            rclBounds.Right = ReadInt32();
                            rclBounds.Bottom = ReadInt32();
                            #endregion

                            #region write data
                            if (GDIobjects.Count > 0)
                            {
                                GDIobjectInfo gdiObject = (GDIobjectInfo)GDIobjects[GetGdiObjectIndex(gdiSelectedBrush)];
                                if (gdiObject.Brush != null)
                                {
                                    SetPdfBackColor(gdiObject.Brush.lbColor);
                                    foreach (string st in emfPath)
                                    {
                                        pageStream.WriteLine(st);
                                    }
                                    pageStream.WriteLine("f");
                                    emfPath.Clear();
                                }
                            }
                            #endregion
                        }
                        break;

                    case EmfPlusRecordType.EmfSetViewportOrgEx:
                        #region read data
                        viewportOrg = new PointD(0, 0);
                        viewportOrg.X = GetTransformedX(ReadInt32());
                        viewportOrg.Y = GetTransformedY(ReadInt32());
                        viewportFlags |= 0x01;
                        //SetViewport();
                        #endregion
                        break;

                    case EmfPlusRecordType.EmfSetViewportExtEx:
                        #region read data
                        viewportExt = new PointD(0, 0);
                        viewportExt.X = GetTransformedCX(ReadInt32());
                        viewportExt.Y = GetTransformedCY(ReadInt32());
                        viewportFlags |= 0x02;
                        //SetViewport();
                        #endregion
                        break;

                    case EmfPlusRecordType.EmfSetWindowOrgEx:
                        #region read data
                        windowOrg = new PointD(0, 0);
                        windowOrg.X = GetTransformedX(ReadInt32());
                        windowOrg.Y = GetTransformedY(ReadInt32());
                        viewportFlags |= 0x04;
                        //SetViewport();
                        #endregion
                        break;

                    case EmfPlusRecordType.EmfSetWindowExtEx:
                        #region read data
                        windowExt = new PointD(0, 0);
                        windowExt.X = GetTransformedCX(ReadInt32());
                        windowExt.Y = GetTransformedCY(ReadInt32());
                        viewportFlags |= 0x08;
                        //SetViewport();
                        #endregion
                        break;

                    case EmfPlusRecordType.EmfSaveDC:
                        pageStream.WriteLine("q");
                        //viewportStack.Push(viewportBack);
                        //viewportBack = null;
                        dcData = new DCData();
                        dcData.emfBackColor = emfBackColor;
                        dcData.emfBkMode = emfBkMode;
                        dcData.emfMapMode = emfMapMode;
                        dcData.emfPolyFillMode = emfPolyFillMode;
                        dcData.emfTextAlign = emfTextAlign;
                        dcData.emfTextColor = emfTextColor;
                        dcStack.Push(dcData);
                        break;

                    case EmfPlusRecordType.EmfRestoreDC:
                        pageStream.WriteLine("Q");
                        //viewportBack = viewportStack.Pop();
                        if (dcStack.Count > 0)
                        {
                            dcData = (DCData)dcStack.Pop();
                            emfBackColor = dcData.emfBackColor;
                            emfBkMode = dcData.emfBkMode;
                            emfMapMode = dcData.emfMapMode;
                            emfPolyFillMode = dcData.emfPolyFillMode;
                            emfTextAlign = dcData.emfTextAlign;
                            emfTextColor = dcData.emfTextColor;
                        }
                        break;

                    case EmfPlusRecordType.Header:
                    case EmfPlusRecordType.GetDC:
                    case EmfPlusRecordType.EndOfFile:
                    case EmfPlusRecordType.EmfEof:
                        //empty
                        break;

                    case EmfPlusRecordType.EmfIntersectClipRect:
                    case EmfPlusRecordType.EmfExtSelectClipRgn:
                    case EmfPlusRecordType.EmfSetStretchBltMode:
                    case EmfPlusRecordType.EmfSelectPalette:
                    case EmfPlusRecordType.EmfSetROP2:
                    case EmfPlusRecordType.SetPageTransform:
                        //empty
                        break;

                    default:
                        //empty
                        break;
                }
                #endregion
            }
            return true;
        }

        public PdfFonts GetPdfFonts()
        {
            return pdfFont;
        }
        #endregion

        #region Clear
        public void Clear()
        {
            m_delegate = null;
            if (emfGr != null) emfGr.Dispose();
            emfGr = null;
            if (emfBmp != null) emfBmp.Dispose();
            emfBmp = null;

            pdfFont = null;
            pageStream = null;
            pdfService = null;
        }
        #endregion
        

        #region RTF render
        public void RenderRtf(Stimulsoft.Report.Export.StiPdfExportService.StiPdfData pp)
        {
            this.pageStream = pdfService.pageStream;
            emfComponent = pp;
            StiRichText richText = pp.Component as StiRichText;
            lock (lockMetafileFlag)
            {
                if (pdfService.report.RenderedWith == StiRenderedWith.Wpf)
                {
                    double storedHeight = richText.Height;
                    richText.Height *= 1 / StiOptions.Export.Pdf.WpfRichTextVerticalScaling * 1.01;
                    richText.RenderMetafile();
                    richText.Height = storedHeight;
                }
                else
                {
                    richText.RenderMetafile();
                }

                Metafile mf = richText.Image as Metafile;

                if (emfBmp == null) emfBmp = new Bitmap(25, 25);
                if (emfGr == null) emfGr = Graphics.FromImage(emfBmp);
                if (m_delegate == null) m_delegate = new Graphics.EnumerateMetafileProc(MetafileCallback);
                try
                {
                    assembleData = false;
                    centerEmf = false;
                    GDIobjects.Clear();
                    AddStockGDIobjects();
                    pageStream.WriteLine("q");
                    pageStream.WriteLine("{0} {1} {2} {3} re W n",
                        ConvertToString(pp.X),
                        ConvertToString(pp.Y),
                        ConvertToString(pp.Width),
                        ConvertToString(pp.Height));
                    pdfService.PushColorToStack();
                    emfGr.EnumerateMetafile(mf, m_destPoint, m_delegate);
                }
                catch
                {
                }
                finally
                {
                    pageStream.WriteLine("Q");
                    pdfService.PopColorFromStack();
                    GDIobjects.Clear();
                    //mf.Dispose();
                    mf = null;
                    if (richText.Image != null)
                    {
                        richText.Image.Dispose();
                        richText.Image = null;
                    }
                }
            }
        }

        public void AssembleRtf(StiComponent component)
        {
            StiRichText richText = component as StiRichText;
            lock (lockMetafileFlag)
            {
                if (pdfService.report.RenderedWith == StiRenderedWith.Wpf)
                {
                    double storedHeight = richText.Height;
                    richText.Height *= 1 / StiOptions.Export.Pdf.WpfRichTextVerticalScaling * 1.01;
                    richText.RenderMetafile();
                    richText.Height = storedHeight;
                }
                else
                {
                    richText.RenderMetafile();
                }

                Metafile mf = richText.Image as Metafile;

                if (emfBmp == null) emfBmp = new Bitmap(25, 25);
                if (emfGr == null) emfGr = Graphics.FromImage(emfBmp);
                if (m_delegate == null) m_delegate = new Graphics.EnumerateMetafileProc(MetafileCallback);
                try
                {
                    assembleData = true;
                    GDIobjects.Clear();
                    emfGr.EnumerateMetafile(mf, m_destPoint, m_delegate);
                }
                catch
                {
                }
                finally
                {
                    GDIobjects.Clear();
                    //mf.Dispose();
                    mf = null;
                    if (richText.Image != null)
                    {
                        richText.Image.Dispose();
                        richText.Image = null;
                    }
                }
            }
        }
        #endregion

        #region Text with Html-tags render
        public void RenderTextToEmf(Stimulsoft.Report.Export.StiPdfExportService.StiPdfData pp, float imageResolution)
        {
            this.pageStream = pdfService.pageStream;
            emfComponent = pp;
            StiText stiText = pp.Component as StiText;
            float rsImageResolution = imageResolution;
            lock (lockMetafileFlag)
            {
                Metafile mf = stiText.GetImage(ref rsImageResolution, StiExportFormat.Pdf) as Metafile;

                if (emfBmp == null) emfBmp = new Bitmap(25, 25);
                if (emfGr == null) emfGr = Graphics.FromImage(emfBmp);
                if (m_delegate == null) m_delegate = new Graphics.EnumerateMetafileProc(MetafileCallback);
                try
                {
                    assembleData = false;
                    centerEmf = false;
                    GDIobjects.Clear();
                    AddStockGDIobjects();
                    pageStream.WriteLine("q");
                    pageStream.WriteLine("{0} {1} {2} {3} re W n",
                        ConvertToString(pp.X),
                        ConvertToString(pp.Y),
                        ConvertToString(pp.Width),
                        ConvertToString(pp.Height));
                    pdfService.PushColorToStack();
                    emfGr.EnumerateMetafile(mf, m_destPoint, m_delegate);
                }
                catch
                {
                }
                finally
                {
                    pageStream.WriteLine("Q");
                    pdfService.PopColorFromStack();
                    GDIobjects.Clear();
                    mf.Dispose();
                }
            }
        }

        public void AssembleTextToEmf(StiComponent component, float imageResolution)
        {
            StiText stiText = component as StiText;
            float rsImageResolution = imageResolution;
            lock (lockMetafileFlag)
            {
                Metafile mf = stiText.GetImage(ref rsImageResolution, StiExportFormat.Pdf) as Metafile;

                if (emfBmp == null) emfBmp = new Bitmap(25, 25);
                if (emfGr == null) emfGr = Graphics.FromImage(emfBmp);
                if (m_delegate == null) m_delegate = new Graphics.EnumerateMetafileProc(MetafileCallback);
                try
                {
                    assembleData = true;
                    GDIobjects.Clear();
                    emfGr.EnumerateMetafile(mf, m_destPoint, m_delegate);
                }
                catch
                {
                }
                finally
                {
                    GDIobjects.Clear();
                    mf.Dispose();
                }
            }
        }
        #endregion

        //#region Checkbox render
        //public void RenderCheckbox(Stimulsoft.Report.Export.StiPdfExportService.StiPdfData pp)
        //{
        //    this.pageStream = pdfService.pageStream;
        //    emfComponent = pp;
        //    StiCheckBox checkbox = pp.Component as StiCheckBox;
        //    lock (lockMetafileFlag)
        //    {
        //        Metafile mf = checkbox.GetMetafile();
        //        Graphics.EnumerateMetafileProc m_delegate = new Graphics.EnumerateMetafileProc(MetafileCallback);
        //        Point m_destPoint = new Point(0, 0);
        //        Bitmap bmp = new Bitmap(25, 25);            // ???
        //        Graphics gr = Graphics.FromImage(bmp);
        //        try
        //        {
        //            assembleData = false;
        //            centerEmf = true;
        //            GDIobjects.Clear();
        //            pageStream.WriteLine("q");
        //            pdfService.PushColorToStack();
        //            gr.EnumerateMetafile(mf, m_destPoint, m_delegate);
        //        }
        //        catch
        //        {
        //        }
        //        finally
        //        {
        //            centerEmf = false;
        //            pageStream.WriteLine("Q");
        //            pdfService.PopColorFromStack();
        //            GDIobjects.Clear();
        //            gr.Dispose();
        //            bmp.Dispose();
        //            mf.Dispose();
        //        }
        //    }
        //}

        //public void AssembleCheckbox(StiComponent component)
        //{
        //    StiCheckBox checkbox = component as StiCheckBox;
        //    lock (lockMetafileFlag)
        //    {
        //        Metafile mf = checkbox.GetMetafile();
        //        Graphics.EnumerateMetafileProc m_delegate = new Graphics.EnumerateMetafileProc(MetafileCallback);
        //        Point m_destPoint = new Point(0, 0);
        //        Bitmap bmp = new Bitmap(25, 25);
        //        Graphics gr = Graphics.FromImage(bmp);
        //        try
        //        {
        //            assembleData = true;
        //            GDIobjects.Clear();
        //            gr.EnumerateMetafile(mf, m_destPoint, m_delegate);
        //        }
        //        catch
        //        {
        //        }
        //        finally
        //        {
        //            GDIobjects.Clear();
        //            gr.Dispose();
        //            bmp.Dispose();
        //            mf.Dispose();
        //        }
        //    }
        //}
        //#endregion

        #region Metafile render
        //public void RenderMetafile(StiPdfData pp, Metafile mf, StreamWriter pageStream)
        //{
        //    emfComponent = pp;
        //    Graphics.EnumerateMetafileProc m_delegate = new Graphics.EnumerateMetafileProc(MetafileCallback);
        //    Point m_destPoint = new Point(0, 0);
        //    Bitmap bmp = new Bitmap(25, 25);
        //    Graphics gr = Graphics.FromImage(bmp);
        //    try
        //    {
        //        assembleData = false;
        //        centerEmf = false;
        //        GDIobjects.Clear();
        //        pageStream.WriteLine("q");
        //        PushColorToStack();
        //        gr.EnumerateMetafile(mf, m_destPoint, m_delegate);
        //    }
        //    finally
        //    {
        //        centerEmf = false;
        //        pageStream.WriteLine("Q");
        //        PopColorFromStack();
        //        GDIobjects.Clear();
        //        gr.Dispose();
        //    }
        //}

        //public void AssembleMetafile(Metafile mf)
        //{
        //    Graphics.EnumerateMetafileProc m_delegate = new Graphics.EnumerateMetafileProc(MetafileCallback);
        //    Point m_destPoint = new Point(0, 0);
        //    Bitmap bmp = new Bitmap(25, 25);
        //    Graphics gr = Graphics.FromImage(bmp);
        //    try
        //    {
        //        assembleData = true;
        //        GDIobjects.Clear();
        //        gr.EnumerateMetafile(mf, m_destPoint, m_delegate);
        //    }
        //    finally
        //    {
        //        GDIobjects.Clear();
        //        gr.Dispose();
        //    }
        //}
        #endregion

        public StiPdfMetafileRender(StiPdfExportService service, bool useUnicodeMode)
        {
            this.pdfService = service;
            this.pdfFont = service.pdfFont;
            this.useUnicodeMode = useUnicodeMode;
        }
    }
}
