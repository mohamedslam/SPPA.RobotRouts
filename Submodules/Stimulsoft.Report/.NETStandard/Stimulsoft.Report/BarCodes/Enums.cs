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

namespace Stimulsoft.Report.BarCodes
{
	#region StiCheckSum
	public enum StiCheckSum
	{
		Yes,
		No
	}
	#endregion

	#region StiPlesseyCheckSum
	public enum StiPlesseyCheckSum
	{
		None,
		Modulo10,
		Modulo11
	}
	#endregion

	#region StiDataMatrixSize
	public enum StiDataMatrixSize
	{
		Automatic = -1,
		s10x10,
		s12x12,
		s8x18,
		s14x14,
		s8x32,
		s16x16,
		s12x26,
		s18x18,
		s20x20,
		s12x36,
		s22x22,
		s16x36,
		s24x24,
		s26x26,
		s16x48,
		s32x32,
		s36x36,
		s40x40,
		s44x44,
		s48x48,
		s52x52,
		s64x64,
		s72x72,
		s80x80,
		s88x88,
		s96x96,
		s104x104,
		s120x120,
		s132x132,
		s144x144
	}
	#endregion

	#region StiDataMatrixEncodingType
	public enum StiDataMatrixEncodingType
	{
		Ascii,
		C40,
		Text,
		X12,
		Edifact,
		Binary
	}
	#endregion

	#region StiPdf417EncodingMode
	public enum StiPdf417EncodingMode
	{
		//Auto,
		Text,
		Numeric,
		Byte
	}
	#endregion

	#region StiPdf417ErrorsCorrectionLevel
	public enum StiPdf417ErrorsCorrectionLevel
	{
		Automatic = -1,
		Level0 = 0, 
		Level1 = 1, 
		Level2 = 2, 
		Level3 = 3, 
		Level4 = 4, 
		Level5 = 5, 
		Level6 = 6, 
		Level7 = 7, 
		Level8 = 8 
	}
	#endregion

	#region StiEanSupplementType
	public enum StiEanSupplementType
	{
		None,
		TwoDigit,
		FiveDigit
	}
	#endregion

	#region StiCode11CheckSum
	public enum StiCode11CheckSum
	{
		None,
		OneDigit,
		TwoDigits,
		Auto
	}
	#endregion

    #region StiQRCodeSize
    public enum StiQRCodeSize
    {
        Automatic = 0,
        v1,
        v2,
        v3,
        v4,
        v5,
        v6,
        v7,
        v8,
        v9,
        v10,
        v11,
        v12,
        v13,
        v14,
        v15,
        v16,
        v17,
        v18,
        v19,
        v20,
        v21,
        v22,
        v23,
        v24,
        v25,
        v26,
        v27,
        v28,
        v29,
        v30,
        v31,
        v32,
        v33,
        v34,
        v35,
        v36,
        v37,
        v38,
        v39,
        v40
    }
    #endregion

    #region StiQRCodeErrorCorrectionLevel
    public enum StiQRCodeErrorCorrectionLevel
    {
        Level1,  //L
        Level2,  //M
        Level3,  //Q
        Level4   //H
    }
    #endregion

    #region StiQRCodeECIMode
    public enum StiQRCodeECIMode
    {
        Cp437 = 2,
        ISO_8859_1 = 3,
        ISO_8859_2 = 4,
        ISO_8859_3 = 5,
        ISO_8859_4 = 6,
        ISO_8859_5 = 7,
        ISO_8859_6 = 8,
        ISO_8859_7 = 9,
        ISO_8859_8 = 10,
        ISO_8859_9 = 11,
        ISO_8859_11 = 13,
        ISO_8859_13 = 15,
        ISO_8859_15 = 17,
        Shift_JIS = 20,
        Windows_1250 = 21,
        Windows_1251 = 22,
        Windows_1252 = 23,
        Windows_1256 = 24,
        UTF_8 = 26
    }
    #endregion

    #region StiMaxicodeMode
    public enum StiMaxicodeMode
    {
        Mode2 = 2,
        Mode3 = 3,
        Mode4 = 4,
        Mode5 = 5,
        Mode6 = 6
    }
    #endregion

    #region StiAztecSize
    public enum StiAztecSize
    {
        Automatic = 0,
        Compact1 = -1,
        Compact2 = -2,
        Compact3 = -3,
        Compact4 = -4,
        v1 = 1,
        v2,
        v3,
        v4,
        v5,
        v6,
        v7,
        v8,
        v9,
        v10,
        v11,
        v12,
        v13,
        v14,
        v15,
        v16,
        v17,
        v18,
        v19,
        v20,
        v21,
        v22,
        v23,
        v24,
        v25,
        v26,
        v27,
        v28,
        v29,
        v30,
        v31,
        v32
    }
    #endregion

    #region StiQRCodeBodyShapeType
    public enum StiQRCodeBodyShapeType
    {
        Square,
        RoundedSquare,
        Dot,
        Circle,
        Diamond,
        Star,
        ZebraHorizontal,
        ZebraVertical,
        ZebraCross1,
        ZebraCross2,
        Circular,
        DockedDiamonds
    }
    #endregion

    #region StiQRCodeEyeFrameShapeType
    public enum StiQRCodeEyeFrameShapeType
    {
        Square,
        Dots,
        Circle,
        Round,
        Round1,
        Round3
    }
    #endregion

    #region StiQRCodeEyeBallShapeType
    public enum StiQRCodeEyeBallShapeType
    {
        Square,
        Dots,
        Circle,
        Round,
        Round1,
        Round3,
        Star,
        ZebraHorizontal,
        ZebraVertical
    }
    #endregion

}
