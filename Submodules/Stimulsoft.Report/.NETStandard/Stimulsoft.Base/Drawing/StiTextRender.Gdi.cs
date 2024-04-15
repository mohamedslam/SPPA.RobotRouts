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
using System.Drawing;
using System.Runtime.InteropServices;

namespace Stimulsoft.Base.Drawing
{
    public partial class StiTextRenderer
    {
        #region Dll imports

        [DllImport("gdi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		internal static extern uint GetOutlineTextMetrics(IntPtr hdc, uint cbData, IntPtr lpOTM);

		[DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		private static extern int DrawTextExW(IntPtr hDC, string lpszString, int nCount, ref RECT lpRect, int nFormat, [In, Out] DRAWTEXTPARAMS param);

		[DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr SelectObject(IntPtr hdc, IntPtr obj);

		[DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool DeleteObject(IntPtr objectHandle);

		[DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
		private static extern int SetTextColor(IntPtr hDC, int crColor);

		[DllImport("gdi32.dll", EntryPoint = "SetBkMode", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
		private static extern int SetBkMode(IntPtr hDC, int nBkMode);

		[DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
		private static extern int SetBkColor(IntPtr hDC, int clr);

		[DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
		private static extern IntPtr CreateRectRgn(int x1, int y1, int x2, int y2);

		[DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
		private static extern int GetClipRgn(IntPtr hDC, IntPtr hRgn);

		[DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
		private static extern int SelectClipRgn(IntPtr hDC, IntPtr hRgn);

		[DllImport("gdi32.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
		private static extern bool GetWorldTransform(
			IntPtr hdc,
			out XFORM lpXform
			);

		[DllImport("gdi32.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
		private static extern bool SetWorldTransform(
			IntPtr hdc,
			ref XFORM lpXform
			);

		[DllImport("gdi32.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
		private static extern bool ModifyWorldTransform(
			IntPtr hdc,
			ref XFORM lpXform,
			UInt32 iMode
			);

		[DllImport("gdi32.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
		private static extern int SetGraphicsMode(
			IntPtr hdc,
			int iMode
			);

		[DllImport("gdi32.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
		private static extern int SetMapMode(
			IntPtr hdc,		// handle to device context
			int fnMapMode	// new mapping mode
			);

		[DllImport("gdi32.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
		private static extern bool GetWindowExtEx(
			IntPtr hdc,		// handle to device context
			out SIZE lpSize	// viewport dimensions
			);

		[DllImport("gdi32.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
		private static extern bool SetWindowExtEx(
			IntPtr hdc,		// handle to device context
			int nXExtent,	// new horizontal window extent
			int nYExtent,	// new vertical window extent
			out SIZE lpSize	// original viewport extent
			);

		[DllImport("gdi32.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
		private static extern bool GetViewportExtEx(
			IntPtr hdc,		// handle to device context
			out SIZE lpSize	// viewport dimensions
			);

		[DllImport("gdi32.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
		private static extern bool SetViewportExtEx(
			IntPtr hdc,		// handle to device context
			int nXExtent,	// new horizontal viewport extent
			int nYExtent,	// new vertical viewport extent
			out SIZE lpSize	// original viewport extent
			);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool GetCharABCWidthsI(
            IntPtr hdc,				// handle to DC
            uint giFirst,			// first glyph index in range
            uint cgi,				// count of glyph indices in range
            [In, Out] ushort[] pgi,  // array of glyph indices
            [In, Out] ABC[] lpabc); // array of character widths

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern uint GetGlyphIndices(
            IntPtr hdc,				// handle to DC
            string lpstr,			// string to convert
            int c,					// number of characters in string
            [In, Out] ushort[] pgi,	// array of glyph indices
            uint fl);			// glyph options


		[DllImport("usp10.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
		private static extern int ScriptItemize(
			string pwcInChars,
			int cInChars,
			int cMaxItems,
			ref SCRIPT_CONTROL psControls,
			ref SCRIPT_STATE psState,
			//ref SCRIPT_ITEM scriptItemList,
            IntPtr scriptItemList,
			out int scriptItemCount
			);

		//E_OUTOFMEMORY result если не хватило scriptItemList

		//HRESULT WINAPI ScriptItemize(
		//    const WCHAR *pwcInChars,
		//    int cInChars,
		//    int cMaxItems,
		//    const SCRIPT_CONTROL *psControl,
		//    const SCRIPT_STATE *psState,
		//    SCRIPT_ITEM *scriptItemList,
		//    int *scriptItemCount
		//);

		[DllImport("usp10.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
		private static extern int ScriptLayout(
			int cRuns,
			byte[] pbLevel,
			int[] piVisualToLogical,
			int[] piLogicalToVisual
			);

		//HRESULT WINAPI ScriptLayout(
		//  int cRuns, 
		//  const BYTE *pbLevel, 
		//  int *piVisualToLogical, 
		//  int *piLogicalToVisual 
		//);

		[DllImport("usp10.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
		private static extern int ScriptShape(
			IntPtr hdc,				// handle to DC
			ref IntPtr psc,         // SCRIPT_CACHE
			string pwcChars,
			int cChars,
			int cMaxGlyphs,
			ref SCRIPT_ANALYSIS psa,
			ushort[] pwOutGlyphs,
			ushort[] pwLogClust,
			//ref SCRIPT_VISATTR psva,
            IntPtr psva,
			out int pcGlyphs
			);

		//HRESULT WINAPI ScriptShape(
		//  HDC hdc, 
		//  SCRIPT_CACHE *psc, 
		//  const WCHAR *pwcChars, 
		//  int cChars, 
		//  int cMaxGlyphs, 
		//  SCRIPT_ANALYSIS *psa, 
		//  WORD *pwOutGlyphs, 
		//  WORD *pwLogClust, 
		//  SCRIPT_VISATTR *psva, 
		//  int *pcGlyphs 
		//);

		[DllImport("usp10.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
		private static extern int ScriptPlace(
			IntPtr hdc,				// handle to DC
			ref IntPtr psc,         // SCRIPT_CACHE
			ushort[] pwGlyphs,
			int pcGlyphs,
			//ref SCRIPT_VISATTR psva,
            IntPtr psva,
			ref SCRIPT_ANALYSIS psa,
			int[] piAdvance,
			//ref GOFFSET pGoffset,
            IntPtr pGoffset,
			out ABC pABC
			);

		//HRESULT WINAPI ScriptPlace(
		//  HDC hdc, 
		//  SCRIPT_CACHE *psc, 
		//  const WORD *pwGlyphs, 
		//  int cGlyphs, 
		//  const SCRIPT_VISATTR *psva, 
		//  SCRIPT_ANALYSIS *psa, 
		//  int *piAdvance, 
		//  GOFFSET *pGoffset, 
		//  ABC *pABC 
		//);

		[DllImport("usp10.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
		private static extern int ScriptTextOut(
			IntPtr hdc,				// handle to DC
			ref IntPtr psc,         // SCRIPT_CACHE
			int x,
			int y,
			uint fuOptions,
			ref RECT lprc,
			ref SCRIPT_ANALYSIS psa,
			IntPtr pwcReserved,
			int iReserved,
			ushort[] pwGlyphs,
			int cGlyphs,
			int[] piAdvance,
			IntPtr piJustify,
			//ref GOFFSET pGoffset
            IntPtr pGoffset
			);

		//HRESULT WINAPI ScriptTextOut(
		//  const HDC hdc, 
		//  SCRIPT_CACHE *psc, 
		//  int x, 
		//  int y, 
		//  UINT fuOptions, 
		//  const RECT *lprc, 
		//  const SCRIPT_ANALYSIS *psa, 
		//  const WCHAR *pwcReserved, 
		//  int iReserved, 
		//  const WORD *pwGlyphs, 
		//  int cGlyphs, 
		//  const int *piAdvance, 
		//  const int *piJustify, 
		//  const GOFFSET *pGoffset 
		//);

		[DllImport("usp10.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
		private static extern int ScriptFreeCache(ref IntPtr psc);

		//HRESULT WINAPI ScriptFreeCache(
		//  SCRIPT_CACHE *psc 
		//);

		[DllImport("usp10.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
		private static extern int ScriptBreak(
			string pwcInChars,
			int cInChars,
			ref SCRIPT_ANALYSIS psa,
			//ref SCRIPT_LOGATTR psla
            IntPtr psla
			);

		//HRESULT WINAPI ScriptBreak( 
		//  const WCHAR *pwcChars, 
		//  int cChars, 
		//  const SCRIPT_ANALYSIS *psa, 
		//  SCRIPT_LOGATTR *psla 
		//);

        #endregion

        #region Structures

        private static int sizeofScriptItem = Marshal.SizeOf(typeof(SCRIPT_ITEM));
        private static int sizeofScriptVisattr = Marshal.SizeOf(typeof(SCRIPT_VISATTR));
        private static int sizeofGoffset = Marshal.SizeOf(typeof(GOFFSET));

        #region struct GOFFSET
        //typedef struct tagGOFFSET {
		//  LONG  du;
		//  LONG  dv;
		//} GOFFSET;

		[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Auto)]
		private struct GOFFSET
		{
            [FieldOffset(0)]
			public Int32 du;
            [FieldOffset(4)]
            public Int32 dv;
		}
        #endregion

        #region struct SCRIPT_VISATTR
		//typedef struct tag_SCRIPT_VISATTR { 
		//  WORD uJustification :4; 
		//  WORD fClusterStart :1; 
		//  WORD fDiacritic :1; 
		//  WORD fZeroWidth :1; 
		//  WORD fReserved :1; 
		//  WORD fShapeReserved :8; 
		//} SCRIPT_VISATTR;

		[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Auto)]
		private struct SCRIPT_VISATTR
		{
            [FieldOffset(0)]
			private UInt16 packed;

			internal ushort uJustification
			{
				get { return (ushort)(packed & 0x000F); }
			}
			internal ushort fClusterStart
			{
				get { return (ushort)((packed & 0x0010) >> 4); }
			}
			internal ushort fDiacritic
			{
				get { return (ushort)((packed & 0x0020) >> 5); }
			}
			internal ushort fZeroWidth
			{
				get { return (ushort)((packed & 0x0040) >> 6); }
			}
			internal ushort fReserved
			{
				get { return (ushort)((packed & 0x0080) >> 7); }
			}
			internal ushort fShapeReserved
			{
				get { return (ushort)((packed & 0xFF00) >> 8); }
			}
		}
        #endregion

        #region struct SCRIPT_STATE
		//typedef struct tag_SCRIPT_STATE { 
		//  WORD uBidiLevel :5; 
		//  WORD fOverrideDirection :1; 
		//  WORD fInhibitSymSwap :1; 
		//  WORD fCharShape :1; 
		//  WORD fDigitSubstitute :1; 
		//  WORD fInhibitLigate :1; 
		//  WORD fDisplayZWG :1; 
		//  WORD fArabicNumContext :1; 
		//  WORD fGcpClusters :1; 
		//  WORD fReserved :1; 
		//  WORD fEngineReserved :2; 
		//} SCRIPT_STATE;

		[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Auto)]
			private struct SCRIPT_STATE
		{
			//internal ushort packed;
            [FieldOffset(0)]
            internal UInt16 packed;

			internal ushort uBidiLevel
			{
				get { return (ushort)(packed & 0x001F); }
				set { packed = (ushort)((packed & 0xFFE0) | (value & 0x001F)); }
			}
			internal ushort fOverrideDirection
			{
				get { return (ushort)((packed & 0x0020) >> 5); }
			}
			internal ushort fInhibitSymSwap
			{
				get { return (ushort)((packed & 0x0040) >> 6); }
			}
			internal ushort fCharShape
			{
				get { return (ushort)((packed & 0x0080) >> 7); }
			}
			internal ushort fDigitSubstitute
			{
				get { return (ushort)((packed & 0x0100) >> 8); }
			}
			internal ushort fInhibitLigate
			{
				get { return (ushort)((packed & 0x0200) >> 9); }
			}
			internal ushort fDisplayZWG
			{
				get { return (ushort)((packed & 0x0400) >> 10); }
			}
			internal ushort fArabicNumContext
			{
				get { return (ushort)((packed & 0x0800) >> 11); }
			}
			internal ushort fGcpClusters
			{
				get { return (ushort)((packed & 0x1000) >> 12); }
			}
			internal ushort fReserved
			{
				get { return (ushort)((packed & 0x2000) >> 13); }
			}
			internal ushort fEngineReserved
			{
				get { return (ushort)((packed & 0xC000) >> 14); }
			}
		}
        #endregion

        #region struct SCRIPT_ANALYSIS
		//typedef struct tag_SCRIPT_ANALYSIS {
		//  WORD eScript      :10; 
		//  WORD fRTL          :1; 
		//  WORD fLayoutRTL    :1; 
		//  WORD fLinkBefore   :1; 
		//  WORD fLinkAfter    :1; 
		//  WORD fLogicalOrder :1; 
		//  WORD fNoGlyphIndex :1; 
		//  SCRIPT_STATE s ; 
		//} SCRIPT_ANALYSIS;

		[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Auto)]
		private struct SCRIPT_ANALYSIS
		{
			//internal ushort packed;
            [FieldOffset(0)]
            internal UInt16 packed;
            [FieldOffset(2)]
            public SCRIPT_STATE s;

			internal ushort eScript
			{
				get { return (ushort)(packed & 0x03FF); }
			}
			internal bool fRTL
			{
				get { return (packed & 0x0400) > 0; }
			}
			internal ushort fLayoutRTL
			{
				get { return (ushort)((packed & 0x0800) >> 11); }
			}
			internal ushort fLinkBefore
			{
				get { return (ushort)((packed & 0x1000) >> 12); }
			}
			internal ushort fLinkAfter
			{
				get { return (ushort)((packed & 0x2000) >> 13); }
			}
			internal ushort fLogicalOrder
			{
				get { return (ushort)((packed & 0x4000) >> 14); }
			}
			internal ushort fNoGlyphIndex
			{
				get { return (ushort)((packed & 0x8000) >> 15); }
			}
		}
        #endregion

        #region struct SCRIPT_ITEM
		[StructLayout(LayoutKind.Explicit, Pack=8, CharSet = CharSet.Auto)]
		private struct SCRIPT_ITEM
		{
            //internal int iCharPos;
            [FieldOffset(0)]
            internal Int32 iCharPos;
            [FieldOffset(4)]
            internal SCRIPT_ANALYSIS a;
		}
        #endregion

        #region struct SCRIPT_CONTROL
		//typedef struct tag_SCRIPT_CONTROL { 
		//  DWORD uDefaultLanguage :16; 
		//  DWORD fContextDigits :1; 
		//  DWORD fInvertPreBoundDir :1; 
		//  DWORD fInvertPostBoundDir :1; 
		//  DWORD fLinkStringBefore :1; 
		//  DWORD fLinkStringAfter :1; 
		//  DWORD fNeutralOverride :1; 
		//  DWORD fNumericOverride :1; 
		//  DWORD fLegacyBidiClass :1; 
		//  DWORD fReserved :8; 
		//} SCRIPT_CONTROL;

		[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Auto)]
		private struct SCRIPT_CONTROL
		{
            [FieldOffset(0)]
			internal UInt16 uDefaultLanguage;
            [FieldOffset(2)]
            private byte packed;
            [FieldOffset(3)]
            internal byte fReserved;

			SCRIPT_CONTROL(uint data)
			{
				uDefaultLanguage = (ushort)(data & 0x0000FFFF);
				packed = (byte)((data & 0x00FF0000) >> 16);
				fReserved = (byte)((data & 0xFF000000) >> 24);
			}

			internal ushort fContextDigits
			{
				get { return (ushort)(packed & 0x0001); }
			}
			internal ushort fInvertPreBoundDir
			{
				get { return (ushort)((packed & 0x0002) >> 1); }
			}
			internal ushort fInvertPostBoundDir
			{
				get { return (ushort)((packed & 0x0004) >> 2); }
			}
			internal ushort fLinkStringBefore
			{
				get { return (ushort)((packed & 0x0008) >> 3); }
			}
			internal ushort fLinkStringAfter
			{
				get { return (ushort)((packed & 0x0010) >> 4); }
			}
			internal ushort fNeutralOverride
			{
				get { return (ushort)((packed & 0x0020) >> 5); }
			}
			internal ushort fNumericOverride
			{
				get { return (ushort)((packed & 0x0040) >> 6); }
			}
			internal ushort fLegacyBidiClass
			{
				get { return (ushort)((packed & 0x0080) >> 7); }
			}
		}
        #endregion

        #region struct SCRIPT_LOGATTR
		//typedef struct tag_SCRIPT_LOGATTR { 
		//  BYTE fSoftBreak :1; 
		//  BYTE fWhiteSpace :1; 
		//  BYTE fCharStop :1; 
		//  BYTE fWordStop :1; 
		//  BYTE fInvalid :1; 
		//  BYTE fReserved :3; 
		//} SCRIPT_LOGATTR;

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		private struct SCRIPT_LOGATTR
		{
			internal byte packed;

			internal bool fSoftBreak
			{
				get
                {
                    return ((packed & 0x0001) > 0 ? true : false);
                }
                set
                {
                    packed |= 0x0001;
                    if (!value)
                        packed ^= 0x0001;
                }
			}
			internal bool fWhiteSpace
			{
				get { return (((packed & 0x0002) >> 1) > 0 ? true : false); }
			}
			internal bool fCharStop
			{
				get { return (((packed & 0x0004) >> 2) > 0 ? true : false); }
			}
			internal bool fWordStop
			{
				get { return (((packed & 0x0008) >> 3) > 0 ? true : false); }
			}
			internal bool fInvalid
			{
				get { return (((packed & 0x0010) >> 4) > 0 ? true : false); }
			}
			internal ushort fReserved
			{
				get { return (ushort)((packed & 0x00E0) >> 5); }
			}
		}
        #endregion
        
        #region RECT
		[StructLayout(LayoutKind.Sequential)]
		private struct RECT
		{
			public int left;
			public int top;
			public int right;
			public int bottom;
			public RECT(int left, int top, int right, int bottom)
			{
				this.left = left;
				this.top = top;
				this.right = right;
				this.bottom = bottom;
			}

			public RECT(Rectangle r)
			{
				this.left = r.Left;
				this.top = r.Top;
				this.right = r.Right;
				this.bottom = r.Bottom;
			}

			public static RECT FromXYWH(int x, int y, int width, int height)
			{
				return new RECT(x, y, x + width, y + height);
			}

			public Size Size
			{
				get
				{
					return new Size(this.right - this.left, this.bottom - this.top);
				}
			}

			public Rectangle ToRectangle()
			{
				return new Rectangle(this.left, this.top, this.right - this.left, this.bottom - this.top);
			}
		}
        #endregion

        #region DRAWTEXTPARAMS
		[StructLayout(LayoutKind.Sequential)]
			internal class DRAWTEXTPARAMS
		{
			private int cbSize;
			public int iTabLength = 0;
			public int iLeftMargin = 0;
			public int iRightMargin = 0;
			public int uiLengthDrawn = 0;
			public DRAWTEXTPARAMS()
			{
				this.cbSize = Marshal.SizeOf(typeof(DRAWTEXTPARAMS));
			}
		}
        #endregion

        #region ABC
		private struct ABC
		{
			internal int abcA;
			internal uint abcB;
			internal int abcC;

			internal ABC(int abcA, uint abcB, int abcC)
			{
				this.abcA = abcA;
				this.abcB = abcB;
				this.abcC = abcC;
			}
		}
        #endregion

        #region XFORM
		[StructLayout(LayoutKind.Sequential)]
		private struct XFORM
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
			public XFORM(double eM11, double eM12, double eM21, double eM22, double eDx, double eDy)
			{
				this.eM11 = (float)eM11;
				this.eM12 = (float)eM12;
				this.eM21 = (float)eM21;
				this.eM22 = (float)eM22;
				this.eDx = (float)eDx;
				this.eDy = (float)eDy;
			}
		}
        #endregion

		#region SIZE
		[StructLayout(LayoutKind.Sequential)]
		private struct SIZE
		{
			public int cx;
			public int cy;

			public SIZE(int cx, int cy)
			{
				this.cx = cx;
				this.cy = cy;
			}
		}
		#endregion

		#region POINT
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		private struct POINT 
		{
			public int x;
			public int y;

			internal POINT(int x, int y)
			{
				this.x = x;
				this.y = y;
			}
		}
		#endregion

		#region PANOSE
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		private struct PANOSE 
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
		#endregion

		#region TEXTMETRIC
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		private struct TEXTMETRIC 
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
		#endregion

		#region OUTLINETEXTMETRIC
		[StructLayout(LayoutKind.Sequential)]
		private struct OUTLINETEXTMETRIC 
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
			public uint otmpFamilyName;		//string offset
			public uint otmpFaceName;		//string offset
			public uint otmpStyleName;		//string offset
			public uint otmpFullName;		//string offset
		}
		#endregion

        #endregion

        #region Constants

        #region TextFormatFlags
		/// <summary>
		/// DrawText Format Flags
		/// </summary>
		[Flags]
		internal enum TextFormatFlags
		{
			Bottom = 8,
			CalculateRectangle = 0x400,
			Default = 0,
			EndEllipsis = 0x8000,
			ExpandTabs = 0x40,
			ExternalLeading = 0x200,
			HidePrefix = 0x100000,
			HorizontalCenter = 1,
			Internal = 0x1000,
			Left = 0,
			ModifyString = 0x10000,
			NoClipping = 0x100,
			NoFullWidthCharacterBreak = 0x80000,
			NoPrefix = 0x800,
			PathEllipsis = 0x4000,
			PrefixOnly = 0x200000,
			Right = 2,
			RightToLeft = 0x20000,
			SingleLine = 0x20,
			TabStop = 0x80,
			TextBoxControl = 0x2000,
			Top = 0,
			VerticalCenter = 4,
			WordBreak = 0x10,
			WordEllipsis = 0x40000
		}
        #endregion

		/// <summary>
		/// Device Context Background Mode
		/// </summary>
		internal enum DeviceContextBackgroundMode
		{
			Opaque = 2,
			Transparent = 1
		}

		/// <summary>
		/// Script doesn't exist in font
		/// </summary>
		private const int E_SCRIPT_NOT_IN_FONT = -2147220992; //0x80040200
		/// <summary>
		/// Ran out of memory
		/// </summary>
		private const int E_OUTOFMEMORY = -2147024882; //0x8007000E

		/// <summary>
		/// XForm stuff
		/// </summary>
		private const int MWT_IDENTITY = 1;
		private const int MWT_LEFTMULTIPLY = 2;
		private const int MWT_RIGHTMULTIPLY = 3;

		/// <summary>
		/// Graphics Modes
		/// </summary>
		private const int GM_COMPATIBLE = 1;
		private const int GM_ADVANCED = 2;

		/// <summary>
		/// Mapping Modes
		/// </summary>
		private const int MM_TEXT = 1;
		private const int MM_LOMETRIC = 2;
		private const int MM_HIMETRIC = 3;
		private const int MM_LOENGLISH = 4;
		private const int MM_HIENGLISH = 5;
		private const int MM_TWIPS = 6;
		private const int MM_ISOTROPIC = 7;
		private const int MM_ANISOTROPIC = 8;

        private const uint GDI_ERROR = 0xFFFFFFFF;

        #endregion        
    }
}
