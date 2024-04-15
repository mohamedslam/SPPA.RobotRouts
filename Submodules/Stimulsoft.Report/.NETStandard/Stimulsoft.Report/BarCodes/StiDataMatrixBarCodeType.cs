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
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.BarCodes.Design;
using Stimulsoft.Report.PropertyGrid;
using System;
using System.ComponentModel;
using System.Drawing;
#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

namespace Stimulsoft.Report.BarCodes
{
    /// <summary>
    /// The class describes the Barcode type - DataMatrix.
    /// </summary>
    [TypeConverter(typeof(StiDataMatrixBarCodeTypeConverter))]
    public class StiDataMatrixBarCodeType : StiBarCodeTypeService
    {
        #region class StiDataMatrixException
        protected class StiDataMatrixException : Exception
        {
            public StiDataMatrixException()
            {
            }
            public StiDataMatrixException(string message)
                : base(message)
            {
            }
            public StiDataMatrixException(string message, Exception inner)
                : base(message, inner)
            {
            }
        }
        #endregion

        #region class StiDataMatrix
        protected class StiDataMatrix
        {
            #region Enums
            public enum CommandCode
            {
                Padding = 129,
                ModeC40 = 230,
                ModeBinary = 231,
                FNC1 = 232,
                ModeX12 = 238,
                ModeText = 239,
                ModeEdifact = 240,
                EscapeToAscii = 254
            }
            #endregion

            #region Class ReedSolomon
            public class ReedSolomon
            {
                private int logmod;
                private int rlen;

                private int[] log = null;
                private int[] alog = null;
                private int[] rspoly = null;

                public ReedSolomon(int nsym)
                {
                    //Calculate log/alog tables
                    logmod = 255;
                    log = new int[logmod + 1];
                    alog = new int[logmod];
                    int p = 1;
                    for (int v = 0; v < logmod; v++)
                    {
                        alog[v] = p;
                        log[p] = v;
                        p <<= 1;
                        if (p > 255)
                        {
                            p ^= 301;
                        }
                    }

                    //Calculate rspoly table
                    rlen = nsym;
                    rspoly = new int[nsym + 1];
                    int index = 1;
                    rspoly[0] = 1;
                    for (int i = 1; i <= nsym; i++)
                    {
                        rspoly[i] = 1;
                        for (int k = i - 1; k > 0; k--)
                        {
                            if (rspoly[k] != 0)
                            {
                                rspoly[k] = alog[(log[rspoly[k]] + index) % logmod];
                            }
                            rspoly[k] ^= rspoly[k - 1];
                        }
                        rspoly[0] = alog[(log[rspoly[0]] + index) % logmod];
                        index++;
                    }
                }

                public byte[] Encode(int len, byte[] data)
                {
                    var res = new byte[rlen];
                    for (int i = 0; i < len; i++)
                    {
                        int m = res[rlen - 1] ^ data[i];
                        for (int k = rlen - 1; k > 0; k--)
                        {
                            if ((m != 0) && (rspoly[k] != 0))
                            {
                                res[k] = (byte)(res[k - 1] ^ alog[(log[m] + log[rspoly[k]]) % logmod]);
                            }
                            else
                            {
                                res[k] = res[k - 1];
                            }
                        }
                        if ((m != 0) && (rspoly[0] != 0))
                        {
                            res[0] = (byte)alog[(log[m] + log[rspoly[0]]) % logmod];
                        }
                        else
                        {
                            res[0] = 0;
                        }
                    }
                    return res;
                }

            }
            #endregion

            #region Ecc200ListItem
            private struct ecc200ListItem
            {
                public int Height;
                public int Width;
                public int FH;
                public int FW;
                public int Bytes;
                public int Datablock;
                public int RSblock;

                public ecc200ListItem(int h, int w, int fh, int fw, int bytes, int datablock, int rsblock)
                {
                    Height = h;
                    Width = w;
                    FH = fh;
                    FW = fw;
                    Bytes = bytes;
                    Datablock = datablock;
                    RSblock = rsblock;
                }
            }
            #endregion

            #region Properties
            public byte[] Matrix => grid;

            public int Width => gridWidth;

            public int Height => gridHeight;

            public string ErrorMessage => errorMessage;
            #endregion

            #region Fields
            private int gridWidth = 0;
            private int gridHeight = 0;
            private byte[] grid = null;
            private string errorMessage = null;
            private ecc200ListItem[] ecc200List = null;
            private bool processTilde = false;
            #endregion

            #region Methods.DataMatrixPlacement
            private void DataMatrixPlacementbit(int[] array, int numRows, int numColumns, int row, int column, int pos, byte b)
            {
                if (row < 0)
                {
                    row += numRows;
                    column += 4 - ((numRows + 4) % 8);
                }
                if (column < 0)
                {
                    column += numColumns;
                    row += 4 - ((numColumns + 4) % 8);
                }
                array[row * numColumns + column] = (pos << 3) + b;
            }

            private void DataMatrixPlacementBlock(int[] array, int numRows, int numColumns, int row, int column, int pos)
            {
                DataMatrixPlacementbit(array, numRows, numColumns, row - 2, column - 2, pos, 7);
                DataMatrixPlacementbit(array, numRows, numColumns, row - 2, column - 1, pos, 6);
                DataMatrixPlacementbit(array, numRows, numColumns, row - 1, column - 2, pos, 5);
                DataMatrixPlacementbit(array, numRows, numColumns, row - 1, column - 1, pos, 4);
                DataMatrixPlacementbit(array, numRows, numColumns, row - 1, column - 0, pos, 3);
                DataMatrixPlacementbit(array, numRows, numColumns, row - 0, column - 2, pos, 2);
                DataMatrixPlacementbit(array, numRows, numColumns, row - 0, column - 1, pos, 1);
                DataMatrixPlacementbit(array, numRows, numColumns, row - 0, column - 0, pos, 0);
            }

            private void DataMatrixPlacementCornerA(int[] array, int numRows, int numColumns, int pos)
            {
                DataMatrixPlacementbit(array, numRows, numColumns, numRows - 1, 0, pos, 7);
                DataMatrixPlacementbit(array, numRows, numColumns, numRows - 1, 1, pos, 6);
                DataMatrixPlacementbit(array, numRows, numColumns, numRows - 1, 2, pos, 5);
                DataMatrixPlacementbit(array, numRows, numColumns, 0, numColumns - 2, pos, 4);
                DataMatrixPlacementbit(array, numRows, numColumns, 0, numColumns - 1, pos, 3);
                DataMatrixPlacementbit(array, numRows, numColumns, 1, numColumns - 1, pos, 2);
                DataMatrixPlacementbit(array, numRows, numColumns, 2, numColumns - 1, pos, 1);
                DataMatrixPlacementbit(array, numRows, numColumns, 3, numColumns - 1, pos, 0);
            }

            private void DataMatrixPlacementCornerB(int[] array, int numRows, int numColumns, int pos)
            {
                DataMatrixPlacementbit(array, numRows, numColumns, numRows - 3, 0, pos, 7);
                DataMatrixPlacementbit(array, numRows, numColumns, numRows - 2, 0, pos, 6);
                DataMatrixPlacementbit(array, numRows, numColumns, numRows - 1, 0, pos, 5);
                DataMatrixPlacementbit(array, numRows, numColumns, 0, numColumns - 4, pos, 4);
                DataMatrixPlacementbit(array, numRows, numColumns, 0, numColumns - 3, pos, 3);
                DataMatrixPlacementbit(array, numRows, numColumns, 0, numColumns - 2, pos, 2);
                DataMatrixPlacementbit(array, numRows, numColumns, 0, numColumns - 1, pos, 1);
                DataMatrixPlacementbit(array, numRows, numColumns, 1, numColumns - 1, pos, 0);
            }

            private void DataMatrixPlacementCornerC(int[] array, int numRows, int numColumns, int pos)
            {
                DataMatrixPlacementbit(array, numRows, numColumns, numRows - 3, 0, pos, 7);
                DataMatrixPlacementbit(array, numRows, numColumns, numRows - 2, 0, pos, 6);
                DataMatrixPlacementbit(array, numRows, numColumns, numRows - 1, 0, pos, 5);
                DataMatrixPlacementbit(array, numRows, numColumns, 0, numColumns - 2, pos, 4);
                DataMatrixPlacementbit(array, numRows, numColumns, 0, numColumns - 1, pos, 3);
                DataMatrixPlacementbit(array, numRows, numColumns, 1, numColumns - 1, pos, 2);
                DataMatrixPlacementbit(array, numRows, numColumns, 2, numColumns - 1, pos, 1);
                DataMatrixPlacementbit(array, numRows, numColumns, 3, numColumns - 1, pos, 0);
            }

            private void DataMatrixPlacementCornerD(int[] array, int numRows, int numColumns, int pos)
            {
                DataMatrixPlacementbit(array, numRows, numColumns, numRows - 1, 0, pos, 7);
                DataMatrixPlacementbit(array, numRows, numColumns, numRows - 1, numColumns - 1, pos, 6);
                DataMatrixPlacementbit(array, numRows, numColumns, 0, numColumns - 3, pos, 5);
                DataMatrixPlacementbit(array, numRows, numColumns, 0, numColumns - 2, pos, 4);
                DataMatrixPlacementbit(array, numRows, numColumns, 0, numColumns - 1, pos, 3);
                DataMatrixPlacementbit(array, numRows, numColumns, 1, numColumns - 3, pos, 2);
                DataMatrixPlacementbit(array, numRows, numColumns, 1, numColumns - 2, pos, 1);
                DataMatrixPlacementbit(array, numRows, numColumns, 1, numColumns - 1, pos, 0);
            }

            private int[] DataMatrixPlacement(int numRows, int numColumns)
            {
                var array = new int[numColumns * numRows];
                int row = 4;
                int column = 0;
                int pos = 1;
                do
                {
                    // check corner
                    if (row == numRows && column == 0)
                        DataMatrixPlacementCornerA(array, numRows, numColumns, pos++);
                    if ((row == numRows - 2) && (column == 0) && ((numColumns % 4) != 0))
                        DataMatrixPlacementCornerB(array, numRows, numColumns, pos++);
                    if (row == numRows - 2 && (column == 0) && (numColumns % 8) == 4)
                        DataMatrixPlacementCornerC(array, numRows, numColumns, pos++);
                    if (row == numRows + 4 && column == 2 && !((numColumns % 8) != 0))
                        DataMatrixPlacementCornerD(array, numRows, numColumns, pos++);
                    // up/right
                    do
                    {
                        if (row < numRows && column >= 0 && !(array[row * numColumns + column] != 0))
                            DataMatrixPlacementBlock(array, numRows, numColumns, row, column, pos++);
                        row -= 2;
                        column += 2;
                    }
                    while (row >= 0 && column < numColumns);
                    row++;
                    column += 3;
                    // down/left
                    do
                    {
                        if (row >= 0 && column < numColumns && !(array[row * numColumns + column] != 0))
                            DataMatrixPlacementBlock(array, numRows, numColumns, row, column, pos++);
                        row += 2;
                        column -= 2;
                    }
                    while (row < numRows && column >= 0);
                    row += 3;
                    column++;
                }
                while (row < numRows || column < numColumns);
                //fill unfilled corner
                if (array[numRows * numColumns - 1] == 0)
                {
                    array[numRows * numColumns - 1] = array[numRows * numColumns - numColumns - 2] = 1;
                }
                return array;
            }
            #endregion

            #region Methods.MakeEcc200Blocks
            private void MakeEcc200Blocks(byte[] binary, int bytes, int datablock, int rsblock)
            {
                var reedSolomon = new ReedSolomon(rsblock);
                int blocks = (bytes + 2) / datablock;
                for (int b = 0; b < blocks; b++)
                {
                    var buf = new byte[256];
                    int p = 0;
                    for (int n = b; n < bytes; n += blocks)
                    {
                        buf[p++] = binary[n];
                    }
                    var ecc = reedSolomon.Encode(p, buf);
                    p = rsblock - 1;    //reversed
                    for (int n = b; n < rsblock * blocks; n += blocks)
                    {
                        binary[bytes + n] = ecc[p--];
                    }
                }
            }
            #endregion

            #region Methods.DataMatrixEncode
            private int DataMatrixEncode(byte[] output, int matrixLength, int[] barcode, StiDataMatrixEncodingType encoding)
            {
                int outPos = 0;
                int barPos = 0;

                var input = StiBarCodeTypeService.UnpackTilde(barcode, processTilde);

                if ((barPos < input.Length) && (input[barPos] == (int)BarcodeCommandCode.Fnc1))
                {
                    output[outPos++] = (byte)CommandCode.FNC1;
                    barPos++;
                }

                if (barcode.Length > 0)
                {
                    switch (encoding)
                    {
                        case StiDataMatrixEncodingType.C40:
                        case StiDataMatrixEncodingType.Text:
                        case StiDataMatrixEncodingType.X12:
                            {
                                EncodeCTX(output, matrixLength, input, ref encoding, ref barPos, ref outPos);
                                break;
                            }

                        case StiDataMatrixEncodingType.Edifact:
                            {
                                EncodeE(output, matrixLength, input, ref barPos, ref outPos);
                                break;
                            }

                        case StiDataMatrixEncodingType.Ascii:
                            {
                                EncodeA(output, matrixLength, input, ref barPos, ref outPos);
                                break;
                            }

                        case StiDataMatrixEncodingType.Binary:
                            {
                                EncodeB(output, matrixLength, input, ref barPos, ref outPos);
                                break;
                            }
                    }
                }

                int codeLength = outPos;
                if ((outPos < matrixLength) && (encoding != StiDataMatrixEncodingType.Ascii))
                {
                    output[outPos++] = (byte)CommandCode.EscapeToAscii;
                }
                if (outPos < matrixLength)
                {
                    output[outPos++] = (byte)CommandCode.Padding;
                }
                while (outPos < matrixLength)
                {   // more padding
                    int v = 129 + (((outPos + 1) * 149) % 253) + 1; // see Annex H
                    if (v > 254) v -= 254;
                    output[outPos++] = (byte)v;
                }
                if ((outPos > matrixLength) || (barPos < input.Length))
                    return 0;   // did not fit
                return codeLength;      // OK 
            }

            private void EncodeB(byte[] output, int matrixLength, int[] barcode, ref int barPos, ref int outPos)
            {
                #region Init
                output[outPos++] = (byte)CommandCode.ModeBinary;
                if (barcode.Length < 250)
                {
                    output[outPos++] = (byte)barcode.Length;
                }
                else
                {
                    output[outPos++] = (byte)(249 + (barcode.Length / 250));
                    output[outPos++] = (byte)(barcode.Length % 250);
                }
                #endregion

                do
                {
                    output[outPos] = (byte)((barcode[barPos++] & 0xFF) + (((outPos + 1) * 149) % 255) + 1);  // see annex H
                    outPos++;
                }
                while ((barPos < barcode.Length) && (outPos < matrixLength));
            }

            private void EncodeA(byte[] output, int matrixLength, int[] barcode, ref int barPos, ref int outPos)
            {
                while ((barPos < barcode.Length) && (outPos < matrixLength))
                {
                    #region Encode byte
                    if (barcode[barPos] > 255)
                    {
                        if (barcode[barPos] == (int)BarcodeCommandCode.Fnc1) output[outPos++] = (byte)CommandCode.FNC1;
                        barPos++;
                    }
                    else if ((barcode.Length - barPos >= 2) && IsDigit(barcode[barPos]) && IsDigit(barcode[barPos + 1]))
                    {
                        output[outPos++] = (byte)((barcode[barPos] - '0') * 10 + barcode[barPos + 1] - '0' + 130);
                        barPos += 2;
                    }
                    else if (barcode[barPos] > 127)
                    {
                        output[outPos++] = 235;
                        output[outPos++] = (byte)(barcode[barPos++] - 127);
                    }
                    else
                    {
                        output[outPos++] = (byte)(barcode[barPos++] + 1);
                    }
                    #endregion
                }
            }

            private void EncodeE(byte[] output, int matrixLength, int[] barcode, ref int barPos, ref int outPos)
            {
                #region Init
                var outBuf = new byte[4];
                output[outPos++] = (byte)CommandCode.ModeEdifact;
                #endregion

                do
                {
                    #region Encode 4 bytes
                    outBuf[0] = 0;
                    outBuf[1] = 0;
                    outBuf[2] = 0;
                    outBuf[3] = 0;
                    int bufPos = 0;
                    while ((barPos < barcode.Length) && (bufPos < 4))
                    {
                        outBuf[bufPos++] = (byte)(barcode[barPos++] & 0xFF);
                    }
                    if (bufPos < 4)
                    {
                        outBuf[bufPos++] = 0x1f;    //escape to ascii
                    }
                    output[outPos] = (byte)((outBuf[0] & 0x3F) << 2);
                    output[outPos++] |= (byte)((outBuf[1] & 0x30) >> 4);
                    output[outPos] = (byte)((outBuf[1] & 0x0F) << 4);
                    if (bufPos == 2)
                    {
                        outPos++;
                    }
                    else
                    {
                        output[outPos++] |= (byte)((outBuf[2] & 0x3C) >> 2);
                        output[outPos] = (byte)((outBuf[2] & 0x03) << 6);
                        output[outPos++] |= (byte)(outBuf[3] & 0x3F);
                    }
                    #endregion
                }
                while ((barPos < barcode.Length) && (outPos < matrixLength));
            }

            private void EncodeCTX(byte[] output, int matrixLength, int[] barcode, ref StiDataMatrixEncodingType encoding, ref int barPos, ref int outPos)
            {
                #region Init
                var outBuf = new int[6];
                int bufPos = 0;

                string s2 = "!\"#$%&'()*+,-./:;<=>?@[\\]_" + (char)BarcodeCommandCode.Fnc1;
                string s3 = "";
                string e = "";

                if (encoding == StiDataMatrixEncodingType.C40)
                {
                    output[outPos++] = (byte)CommandCode.ModeC40;
                    e = " 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                    s3 = "`abcdefghijklmnopqrstuvwxyz{|}~\x7F";
                }
                if (encoding == StiDataMatrixEncodingType.Text)
                {
                    output[outPos++] = (byte)CommandCode.ModeText;
                    e = " 0123456789abcdefghijklmnopqrstuvwxyz";
                    s3 = "`ABCDEFGHIJKLMNOPQRSTUVWXYZ{|}~\x7F";
                }
                if (encoding == StiDataMatrixEncodingType.X12)
                {
                    output[outPos++] = (byte)CommandCode.ModeX12;
                    e = " 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ\r*>";
                }
                #endregion

                do
                {
                    if ((barPos + 1 == barcode.Length) && (bufPos == 0))
                    {
                        output[outPos++] = (byte)CommandCode.EscapeToAscii;
                        EncodeA(output, matrixLength, barcode, ref barPos, ref outPos);
                        encoding = StiDataMatrixEncodingType.Ascii;
                        return;
                    }

                    #region Encode next symbol
                    int c = barcode[barPos++];
                    if ((c & 0x80) != 0)
                    {
                        if (encoding == StiDataMatrixEncodingType.X12)
                        {
                            throw new StiDataMatrixException(string.Format("Cannot encode char 0x{0:X2} in X12", c));
                        }
                        c &= 0x7f;
                        outBuf[bufPos++] = 1;   //shift2 set
                        outBuf[bufPos++] = 30;  //upper shift
                    }
                    int w_idx = e.IndexOf((char)c);
                    if (w_idx >= 0)
                    {
                        //basic set
                        outBuf[bufPos++] = (w_idx + 3) % 40;
                    }
                    else
                    {
                        #region shift set
                        if (encoding == StiDataMatrixEncodingType.X12)
                        {
                            throw new StiDataMatrixException(string.Format("Cannot encode char 0x{0:X2} in X12", c));
                        }
                        if (c < 32)
                        {
                            // shift 1
                            outBuf[bufPos++] = 0;
                            outBuf[bufPos++] = c;
                        }
                        else
                        {
                            w_idx = s2.IndexOf((char)c);
                            if (w_idx >= 0)
                            {
                                // shift 2
                                outBuf[bufPos++] = 1;
                                outBuf[bufPos++] = w_idx;
                            }
                            else
                            {
                                w_idx = s3.IndexOf((char)c);
                                if (w_idx >= 0)
                                {
                                    outBuf[bufPos++] = 2;
                                    outBuf[bufPos++] = w_idx;
                                }
                                else
                                {
                                    throw new StiDataMatrixException(string.Format("Cannot encode char 0x{0:X2}", c));
                                }
                            }
                        }
                        #endregion
                    }
                    #endregion

                    #region Pad buffer part1
                    if ((barPos == barcode.Length) && ((bufPos % 3) == 2))
                    {
                        outBuf[bufPos++] = 0;   //padding with Shift1
                    }
                    #endregion

                    while (bufPos >= 3)
                    {
                        #region Store 3 bytes
                        int v = outBuf[0] * 1600 + outBuf[1] * 40 + outBuf[2] + 1;
                        output[outPos++] = (byte)(v >> 8);
                        output[outPos++] = (byte)(v & 0xFF);
                        bufPos -= 3;
                        outBuf[0] = outBuf[3];
                        outBuf[1] = outBuf[4];
                        outBuf[2] = outBuf[5];
                        outBuf[3] = 0;
                        outBuf[4] = 0;
                        outBuf[5] = 0;
                        #endregion
                    }
                }
                while ((barPos < barcode.Length) && (outPos < matrixLength));
            }

            private static bool IsDigit(int b)
            {
                return (b >= '0' && b <= '9');
            }

            private static int[] ConvertStringToBytes(string st)
            {
                var chars = st.ToCharArray();
                var buf = new int[chars.Length];
                for (int index = 0; index < chars.Length; index++)
                {
                    int ch = chars[index];
                    if (ch < 256)
                        buf[index] = ch;
                    else
                    {
                        if (ch == (int)BarcodeCommandCode.Fnc1 || ch == (int)BarcodeCommandCode.Fnc2 || ch == (int)BarcodeCommandCode.Fnc3 || ch == (int)BarcodeCommandCode.Fnc4)
                            buf[index] = ch;
                        else
                            buf[index] = (int)'?';
                    }
                }
                return buf;
            }
            #endregion

            #region Methods.MakeGrid
            private byte[] MakeGrid(int[] barcode, ref int widthOriginal, ref int heightOriginal, StiDataMatrixEncodingType globalEncoding)
            {
                int mWidth = widthOriginal;
                int mHeight = heightOriginal;

                var binary = new byte[3200];
                byte[] grid = null;
                int indexMatrix = 0;

                if (mWidth != 0)
                {
                    #region known matrix size
                    //find suitable matrix
                    for (indexMatrix = 0; indexMatrix < ecc200List.Length; indexMatrix++)
                    {
                        if ((ecc200List[indexMatrix].Width == mWidth) && (ecc200List[indexMatrix].Height == mHeight)) break;
                    }
                    if (indexMatrix == ecc200List.Length)
                    {
                        throw new StiDataMatrixException(string.Format("Invalid size {0}x{1}", mWidth, mHeight));
                    }
                    #endregion
                }
                else
                {
                    #region find a suitable matrix size
                    // find one that fits chosen encoding
                    int res = DataMatrixEncode(binary, 1558, barcode, globalEncoding);
                    for (indexMatrix = 0; indexMatrix < ecc200List.Length; indexMatrix++)
                    {
                        if (res <= ecc200List[indexMatrix].Bytes) break;
                    }
                    if (indexMatrix == ecc200List.Length || (res == 0 && barcode.Length > 0))
                    {
                        throw new StiDataMatrixException("Cannot find suitable size, barcode too long");
                    }
                    mWidth = ecc200List[indexMatrix].Width;
                    mHeight = ecc200List[indexMatrix].Height;
                    #endregion
                }

                if (DataMatrixEncode(binary, ecc200List[indexMatrix].Bytes, barcode, globalEncoding) == 0 && barcode.Length > 0)
                {
                    throw new StiDataMatrixException(string.Format("Barcode too long for {0}x{1}", mWidth, mHeight));
                }

                //make ecc code
                MakeEcc200Blocks(binary, ecc200List[indexMatrix].Bytes, ecc200List[indexMatrix].Datablock, ecc200List[indexMatrix].RSblock);

                #region make placement
                int numColumns = mWidth - 2 * (mWidth / ecc200List[indexMatrix].FW);
                int numRows = mHeight - 2 * (mHeight / ecc200List[indexMatrix].FH);
                var places = DataMatrixPlacement(numRows, numColumns);

                grid = new byte[mWidth * mHeight];

                int x = 0;
                int y = 0;
                for (y = 0; y < mHeight; y += ecc200List[indexMatrix].FH)
                {
                    for (x = 0; x < mWidth; x++)
                        grid[y * mWidth + x] = 1;
                    for (x = 0; x < mWidth; x += 2)
                        grid[(y + ecc200List[indexMatrix].FH - 1) * mWidth + x] = 1;
                }
                for (x = 0; x < mWidth; x += ecc200List[indexMatrix].FW)
                {
                    for (y = 0; y < mHeight; y++)
                        grid[y * mWidth + x] = 1;
                    for (y = 0; y < mHeight; y += 2)
                        grid[y * mWidth + x + ecc200List[indexMatrix].FW - 1] = 1;
                }
                for (y = 0; y < numRows; y++)
                {
                    for (x = 0; x < numColumns; x++)
                    {
                        int v = places[(numRows - y - 1) * numColumns + x];
                        if (v == 1 || v > 7 && (binary[(v >> 3) - 1] & (1 << (v & 7))) != 0)
                        {
                            grid[(1 + y + 2 * (y / (ecc200List[indexMatrix].FH - 2))) * mWidth + 1 + x + 2 * (x / (ecc200List[indexMatrix].FW - 2))] = 1;
                        }
                    }
                }
                #endregion

                widthOriginal = mWidth;
                heightOriginal = mHeight;
                return grid;
            }
            #endregion


            public StiDataMatrix(string message, StiDataMatrixEncodingType globalEncoding, bool useRectangularSymbols, StiDataMatrixSize matrixSize, bool processTilde)
                : this(ConvertStringToBytes(message), globalEncoding, useRectangularSymbols, matrixSize, processTilde)
            {
            }

            public StiDataMatrix(int[] data, StiDataMatrixEncodingType globalEncoding, bool useRectangularSymbols, StiDataMatrixSize matrixSize, bool processTilde)
            {
                this.processTilde = processTilde;
                gridWidth = 0;
                gridHeight = 0;
                byte[] array = null;

                if (matrixSize != StiDataMatrixSize.Automatic) useRectangularSymbols = true;

                #region fill ecc200List
                if (useRectangularSymbols)
                {
                    ecc200List = new ecc200ListItem[]
                    {
                        new ecc200ListItem(10, 10, 10, 10, 3, 3, 5),
                        new ecc200ListItem(12, 12, 12, 12, 5, 5, 7),
                        new ecc200ListItem(8, 18, 8, 18, 5, 5, 7),	//r
						new ecc200ListItem(14, 14, 14, 14, 8, 8, 10),
                        new ecc200ListItem(8, 32, 8, 16, 10, 10, 11),	//r
						new ecc200ListItem(16, 16, 16, 16, 12, 12, 12),
                        new ecc200ListItem(12, 26, 12, 26, 16, 16, 14),	//r
						new ecc200ListItem(18, 18, 18, 18, 18, 18, 14),
                        new ecc200ListItem(20, 20, 20, 20, 22, 22, 18),
                        new ecc200ListItem(12, 36, 12, 18, 22, 22, 18),	//r
						new ecc200ListItem(22, 22, 22, 22, 30, 30, 20),
                        new ecc200ListItem(16, 36, 16, 18, 32, 32, 24),	//r
						new ecc200ListItem(24, 24, 24, 24, 36, 36, 24),
                        new ecc200ListItem(26, 26, 26, 26, 44, 44, 28),
                        new ecc200ListItem(16, 48, 16, 24, 49, 49, 28),	//r
						new ecc200ListItem(32, 32, 16, 16, 62, 62, 36),
                        new ecc200ListItem(36, 36, 18, 18, 86, 86, 42),
                        new ecc200ListItem(40, 40, 20, 20, 114, 114, 48),
                        new ecc200ListItem(44, 44, 22, 22, 144, 144, 56),
                        new ecc200ListItem(48, 48, 24, 24, 174, 174, 68),
                        new ecc200ListItem(52, 52, 26, 26, 204, 102, 42),
                        new ecc200ListItem(64, 64, 16, 16, 280, 140, 56),
                        new ecc200ListItem(72, 72, 18, 18, 368, 92, 36),
                        new ecc200ListItem(80, 80, 20, 20, 456, 114, 48),
                        new ecc200ListItem(88, 88, 22, 22, 576, 144, 56),
                        new ecc200ListItem(96, 96, 24, 24, 696, 174, 68),
                        new ecc200ListItem(104, 104, 26, 26, 816, 136, 56),
                        new ecc200ListItem(120, 120, 20, 20, 1050, 175, 68),
                        new ecc200ListItem(132, 132, 22, 22, 1304, 163, 62),
                        new ecc200ListItem(144, 144, 24, 24, 1558, 156, 62)
                    };
                }
                else
                {
                    ecc200List = new ecc200ListItem[]
                    {
                        new ecc200ListItem(10, 10, 10, 10, 3, 3, 5),
                        new ecc200ListItem(12, 12, 12, 12, 5, 5, 7),
                        new ecc200ListItem(14, 14, 14, 14, 8, 8, 10),
                        new ecc200ListItem(16, 16, 16, 16, 12, 12, 12),
                        new ecc200ListItem(18, 18, 18, 18, 18, 18, 14),
                        new ecc200ListItem(20, 20, 20, 20, 22, 22, 18),
                        new ecc200ListItem(22, 22, 22, 22, 30, 30, 20),
                        new ecc200ListItem(24, 24, 24, 24, 36, 36, 24),
                        new ecc200ListItem(26, 26, 26, 26, 44, 44, 28),
                        new ecc200ListItem(32, 32, 16, 16, 62, 62, 36),
                        new ecc200ListItem(36, 36, 18, 18, 86, 86, 42),
                        new ecc200ListItem(40, 40, 20, 20, 114, 114, 48),
                        new ecc200ListItem(44, 44, 22, 22, 144, 144, 56),
                        new ecc200ListItem(48, 48, 24, 24, 174, 174, 68),
                        new ecc200ListItem(52, 52, 26, 26, 204, 102, 42),
                        new ecc200ListItem(64, 64, 16, 16, 280, 140, 56),
                        new ecc200ListItem(72, 72, 18, 18, 368, 92, 36),
                        new ecc200ListItem(80, 80, 20, 20, 456, 114, 48),
                        new ecc200ListItem(88, 88, 22, 22, 576, 144, 56),
                        new ecc200ListItem(96, 96, 24, 24, 696, 174, 68),
                        new ecc200ListItem(104, 104, 26, 26, 816, 136, 56),
                        new ecc200ListItem(120, 120, 20, 20, 1050, 175, 68),
                        new ecc200ListItem(132, 132, 22, 22, 1304, 163, 62),
                        new ecc200ListItem(144, 144, 24, 24, 1558, 156, 62)
                    };
                }
                #endregion

                if (matrixSize != StiDataMatrixSize.Automatic)
                {
                    var item = ecc200List[(int)matrixSize];
                    gridWidth = item.Width;
                    gridHeight = item.Height;
                }

                try
                {
                    array = MakeGrid(
                        data,
                        ref gridWidth,
                        ref gridHeight,
                        globalEncoding);
                }
                catch (StiDataMatrixException dme)
                {
                    errorMessage = dme.Message;
                }

                if (array != null)
                {
                    //flip Y coordinate
                    grid = new byte[gridWidth * gridHeight];
                    for (int y = 0; y < gridHeight; y++)
                    {
                        int posArray = y * gridWidth;
                        int posGrid = (gridHeight - y - 1) * gridWidth;
                        for (int x = 0; x < gridWidth; x++)
                        {
                            grid[posGrid + x] = array[posArray + x];
                        }
                    }
                }

            }
        }
        #endregion

        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiDataMatrixBarCodeType
            jObject.AddPropertyFloat("Height", Height, 1f);
            jObject.AddPropertyFloat("Module", Module, 40f);
            jObject.AddPropertyEnum("EncodingType", EncodingType, StiDataMatrixEncodingType.Ascii);
            jObject.AddPropertyEnum("MatrixSize", MatrixSize, StiDataMatrixSize.Automatic);
            jObject.AddPropertyBool("UseRectangularSymbols", UseRectangularSymbols);
            jObject.AddPropertyBool("ProcessTilde", ProcessTilde, false);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Module":
                        this.module = property.DeserializeFloat();
                        break;

                    case "Height":
                        this.height = property.DeserializeFloat();
                        break;

                    case "EncodingType":
                        this.EncodingType = property.DeserializeEnum<StiDataMatrixEncodingType>();
                        break;

                    case "MatrixSize":
                        this.MatrixSize = property.DeserializeEnum<StiDataMatrixSize>();
                        break;

                    case "UseRectangularSymbols":
                        this.UseRectangularSymbols = property.DeserializeBool();
                        break;

                    case "ProcessTilde":
                        this.ProcessTilde = property.DeserializeBool();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiDataMatrixBarCodeType;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            var list = new[]
            {
                propHelper.EncodingType(),
                propHelper.MatrixSize(),
                propHelper.Module(),
                propHelper.ProcessTilde(),
                propHelper.UseRectangularSymbols()
            };
            objHelper.Add(StiPropertyCategories.Main, list);

            return objHelper;
        }
        #endregion

        #region ServiceName
        /// <summary>
        /// Gets a service name.
        /// </summary>
        public override string ServiceName => "DataMatrix";
        #endregion

        #region Properties
        public override string DefaultCodeValue => "12345678901";

        private float module = 40f;
        /// <summary>
        /// Gets or sets width of the most fine element of the bar code.
        /// </summary>
        [Description("Gets or sets width of the most fine element of the bar code.")]
        [DefaultValue(40f)]
        [StiSerializable]
        [StiCategory("BarCode")]
        public override float Module
        {
            get
            {
                return module;
            }
            set
            {
                module = value;

                if (value < 2f) 
                    module = 2f;

                if (value > 400f) 
                    module = 400f;
            }
        }

        private float height = 1f;
        /// <summary>
        /// Gets os sets height factor of the bar code.
        /// </summary>
        [Description("Gets os sets height factor of the bar code.")]
        [Browsable(false)]
        [DefaultValue(1f)]
        public override float Height
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
            }
        }

        /// <summary>
        /// Gets or sets type of encoding type.
        /// </summary>
        [Description("Gets or sets type of encoding type.")]
        [DefaultValue(StiDataMatrixEncodingType.Ascii)]
        [StiSerializable]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiCategory("BarCode")]
        public StiDataMatrixEncodingType EncodingType { get; set; } = StiDataMatrixEncodingType.Ascii;


        /// <summary>
        /// Gets or sets the flag that indicates whether the data message supports character '~' as the escape.
        /// Escape sequence must have format '~ddd', where number from 0 to 255 (eg. "~000", "~029" etc.) or ~FNC1
        /// </summary>
        [Description("Gets or sets the flag that indicates whether the data message supports character '~' as the escape.")]
        [DefaultValue(false)]
        [StiOrder(110)]
        [StiSerializable]
        [StiCategory("BarCode")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        public bool ProcessTilde { get; set; }


        /// <summary>
        /// Gets or sets matrix size.
        /// </summary>
		[DefaultValue(StiDataMatrixSize.Automatic)]
        [StiSerializable]
        [Description("Gets or sets matrix size.")]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiCategory("BarCode")]
        public StiDataMatrixSize MatrixSize { get; set; } = StiDataMatrixSize.Automatic;


        /// <summary>
        /// Gets or sets value which indicates will RectangularSymbols be used or not.
        /// </summary>
		[DefaultValue(false)]
        [StiSerializable]
        [Description("Gets or sets value which indicates will RectangularSymbols be used or not.")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiCategory("BarCode")]
        public bool UseRectangularSymbols { get; set; }

        internal override float LabelFontHeight => DefaultLabelFontHeight;

        public override bool[] VisibleProperties
        {
            get
            {
                var props = new bool[visiblePropertiesCount];

                props[9] = true;
                props[12] = true;
                props[13] = true;
                props[21] = true;

                return props;
            }
        }
        #endregion

        #region Methods
        public override void Draw(object context, StiBarCode barCode, RectangleF rect, float zoom)
        {
            string code = GetCode(barCode);
            //code = CheckCodeSymbols(code, DataMatrixSymbols);
            BarCodeData.Code = code;

            var dm = new StiDataMatrix(code, EncodingType, UseRectangularSymbols, MatrixSize, ProcessTilde);

            BarCodeData.MatrixGrid = dm.Matrix;
            BarCodeData.MatrixWidth = dm.Width;
            BarCodeData.MatrixHeight = dm.Height;
            BarCodeData.MatrixRatioY = 1;

            if (dm.ErrorMessage == null)
            {
                Draw2DBarCode(context, rect, barCode, zoom);
            }
            else
            {
                DrawBarCodeError(context, rect, barCode, dm.ErrorMessage);
            }

        }
  
        public override StiBarCodeTypeService CreateNew() => new StiDataMatrixBarCodeType();
        #endregion

        public StiDataMatrixBarCodeType() : this(40f, StiDataMatrixEncodingType.Ascii, false, StiDataMatrixSize.Automatic, false)
        {
        }

        public StiDataMatrixBarCodeType(float module, StiDataMatrixEncodingType encodingType, bool useRectangularSymbols, StiDataMatrixSize matrixSize) : this(module, encodingType, useRectangularSymbols, matrixSize, false)
        {
        }

        public StiDataMatrixBarCodeType(float module, StiDataMatrixEncodingType encodingType, bool useRectangularSymbols, StiDataMatrixSize matrixSize, bool processTilde)
		{
			this.module = module;
			this.EncodingType = encodingType;
			this.UseRectangularSymbols = useRectangularSymbols;
			this.MatrixSize = matrixSize;
            this.ProcessTilde = processTilde;
		}
	}
}