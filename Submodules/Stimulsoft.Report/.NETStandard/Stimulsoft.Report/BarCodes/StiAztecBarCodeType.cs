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
using Stimulsoft.Report.Components;
using Stimulsoft.Report.Export;
using Stimulsoft.Report.PropertyGrid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Text;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#endif

#if STIDRAWING
using Graphics = Stimulsoft.Drawing.Graphics;
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Image = Stimulsoft.Drawing.Image;
#endif

namespace Stimulsoft.Report.BarCodes
{
    /// <summary>
    /// The class describes the Barcode type - Aztec.
    /// </summary>
    [TypeConverter(typeof(Design.StiAztecBarCodeTypeConverter))]
	public class StiAztecBarCodeType : StiBarCodeTypeService
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiAztecBarCodeType
            jObject.AddPropertyFloat("Height", Height, 1f);
            jObject.AddPropertyFloat("Module", Module, 40f);
            jObject.AddPropertyInt("ErrorCorrectionLevel", ErrorCorrectionLevel, StiAztec.Default_EC_Percent);
            jObject.AddPropertyEnum("MatrixSize", MatrixSize, StiAztecSize.Automatic);
            jObject.AddPropertyInt("CodePage", CodePage, StiAztec.Default_EC_Percent);

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

                    case "ErrorCorrectionLevel":
                        this.ErrorCorrectionLevel = property.DeserializeInt();
                        break;

                    case "MatrixSize":
                        this.MatrixSize = property.DeserializeEnum<StiAztecSize>();
                        break;

                    case "CodePage":
                        this.CodePage = property.DeserializeInt();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject

        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiAztecBarCodeType;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            var list = new[]
            {
                propHelper.ErrorCorrectionLevel(),
                propHelper.MatrixSize(),
                propHelper.Module()
            };
            objHelper.Add(StiPropertyCategories.Main, list);

            return objHelper;
        }

        #endregion

		#region ServiceName
		/// <summary>
		/// Gets a service name.
		/// </summary>
		public override string ServiceName => "Aztec";
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
				if (value < 5f) module = 5f;
				if (value > 400f) module = 400f;
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

        private int errorCorrectionLevel = StiAztec.Default_EC_Percent;
        /// <summary>
        /// Gets or sets the error correction level.
        /// </summary>
        [Description("Gets or sets the error correction level.")]
        [DefaultValue(StiAztec.Default_EC_Percent)]
        [StiSerializable]
        [StiCategory("BarCode")]
        public int ErrorCorrectionLevel
        {
            get
            {
                return errorCorrectionLevel;
            }
            set
            {
                errorCorrectionLevel = value;
                if (value < 10) errorCorrectionLevel = 10;
                if (value > 95) errorCorrectionLevel = 95;
            }
        }

        /// <summary>
        /// Gets or sets matrix size.
        /// </summary>
        [DefaultValue(StiAztecSize.Automatic)]
        [StiSerializable]
        [Description("Gets or sets matrix size.")]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiCategory("BarCode")]
        public StiAztecSize MatrixSize { get; set; } = StiAztecSize.Automatic;

        private int codePage = StiAztec.Default_Aztec_CodePage;
        /// <summary>
        /// Gets or sets the default codepage.
        /// </summary>
        [Description("Gets or sets the default codepage.")]
        [DefaultValue(StiAztec.Default_Aztec_CodePage)]
        [StiSerializable]
        [StiCategory("BarCode")]
        public int CodePage
        {
            get
            {
                return codePage;
            }
            set
            {
                try
                {
                    var enc = Encoding.GetEncoding(value);
                    if (enc != null) codePage = value;
                }
                catch { }
            }
        }

        internal override float LabelFontHeight => DefaultLabelFontHeight;

        public override bool[] VisibleProperties
        {
            get
            {
                var props = new bool[visiblePropertiesCount];
                props[10] = true;
                props[12] = true;
                props[13] = true;

                return props;
            }
        }
		#endregion

		#region StiAztecException
		protected class StiAztecException : Exception
		{
			public StiAztecException()
			{
			}
			public StiAztecException(string message)
				: base(message)
			{
			}
			public StiAztecException(string message, Exception inner)
				: base(message, inner)
			{
			}
		}
		#endregion

        #region StiAztec
        private sealed class StiAztec
        {
            #region Fields
            internal const int Default_EC_Percent = 33; // default minimal percentage of error check words (According to ISO/IEC 24778:2008, a minimum of 23% + 3 words is recommended)
            internal const StiAztecSize Default_Aztec_Layers = StiAztecSize.Automatic;
            internal const int Default_Aztec_CodePage = 28591;  //"ISO-8859-1"
            #endregion

            public static AztecCode Encode(string contents, int eccPercent = Default_EC_Percent, StiAztecSize layers = Default_Aztec_Layers, int codePage = Default_Aztec_CodePage, bool useEci = false)
            {
                eccPercent = Math.Max(25, Math.Min(eccPercent, 95));

                var encoding = Encoding.GetEncoding(Default_Aztec_CodePage);
                int eci = 0;
                if (useEci)
                {
                    //need to implement TryEncodeByte
                    //eci = 26;
                    //encoding = Encoding.UTF8;
                }

                var byteContent = encoding.GetBytes(contents);

                return Encoder.Encode(byteContent, eci, eccPercent, layers);
            }

            #region AztecCode
            internal sealed class AztecCode
            {
                /// <summary>
                /// Compact or full symbol indicator
                /// </summary>
                public bool IsCompact { get; set; }

                /// <summary>
                /// Size in pixels (width and height)
                /// </summary>
                public int Size { get; set; }

                /// <summary>
                /// Number of levels
                /// </summary>
                public int Layers { get; set; }

                /// <summary>
                /// Number of data codewords
                /// </summary>
                public int CodeWords { get; set; }

                /// <summary>
                /// The symbol image
                /// </summary>
                public StiBarcodeUtils.BitMatrix Matrix { get; set; }
            }
            #endregion

            #region Encoder
            private static class Encoder
            {
                private const int MAX_NB_BITS = 32;
                private const int MAX_NB_BITS_COMPACT = 4;

                private static readonly int[] WORD_SIZE = { 4, 6, 6, 8, 8, 8, 8, 8, 8, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12 };

                public static AztecCode Encode(byte[] data, int eci, int minECCPercent, StiAztecSize userSpecifiedLayers)
                {
                    // High-level encode
                    var bits = new HighLevelEncoder(data, eci).Encode();

                    // stuff bits and choose symbol size
                    int eccBits = bits.Size * minECCPercent / 100 + 11;
                    int totalSizeBits = bits.Size + eccBits;
                    bool compact;
                    int layers;
                    int totalBitsInLayer;
                    int wordSize;
                    BitArray stuffedBits;

                    if (userSpecifiedLayers != Default_Aztec_Layers)
                    {
                        compact = userSpecifiedLayers < 0;
                        layers = Math.Abs((int)userSpecifiedLayers);
                        if (layers > (compact ? MAX_NB_BITS_COMPACT : MAX_NB_BITS))
                        {
                            layers = (compact ? MAX_NB_BITS_COMPACT : MAX_NB_BITS);
                        }
                        totalBitsInLayer = TotalBitsInLayer(layers, compact);
                        wordSize = WORD_SIZE[layers];
                        int usableBitsInLayers = totalBitsInLayer - (totalBitsInLayer % wordSize);
                        stuffedBits = StuffBits(bits, wordSize);
                        if ((stuffedBits.Size + eccBits > usableBitsInLayers) || (compact && stuffedBits.Size > wordSize * 64))
                        {
                            throw new StiAztecException("Too many data for specified layer");
                        }
                    }
                    else
                    {
                        wordSize = 0;
                        stuffedBits = null;
                        for (int i = 0; ; i++)
                        {
                            if (i > MAX_NB_BITS)
                            {
                                throw new StiAztecException("Too many data for an Aztec code");
                            }
                            compact = i <= 3;
                            layers = compact ? i + 1 : i;
                            totalBitsInLayer = TotalBitsInLayer(layers, compact);
                            if (totalSizeBits > totalBitsInLayer)
                            {
                                continue;
                            }
                            if (stuffedBits == null || wordSize != WORD_SIZE[layers])
                            {
                                wordSize = WORD_SIZE[layers];
                                stuffedBits = StuffBits(bits, wordSize);
                            }
                            int usableBitsInLayers = totalBitsInLayer - (totalBitsInLayer % wordSize);
                            if (compact && stuffedBits.Size > wordSize * 64)
                            {
                                continue;
                            }
                            if (stuffedBits.Size + eccBits <= usableBitsInLayers)
                            {
                                break;
                            }
                        }
                    }

                    BitArray messageBits = GenerateCheckWords(stuffedBits, totalBitsInLayer, wordSize);

                    // generate mode message
                    int messageSizeInWords = stuffedBits.Size / wordSize;
                    var modeMessage = GenerateModeMessage(compact, layers, messageSizeInWords);

                    // allocate symbol
                    int baseMatrixSize = (compact ? 11 : 14) + layers * 4; // not including alignment lines
                    var alignmentMap = new int[baseMatrixSize];
                    int matrixSize;
                    if (compact)
                    {
                        // no alignment marks in compact mode, alignmentMap is a no-op
                        matrixSize = baseMatrixSize;
                        for (int i = 0; i < alignmentMap.Length; i++)
                        {
                            alignmentMap[i] = i;
                        }
                    }
                    else
                    {
                        matrixSize = baseMatrixSize + 1 + 2 * ((baseMatrixSize / 2 - 1) / 15);
                        int origCenter = baseMatrixSize / 2;
                        int center = matrixSize / 2;
                        for (int i = 0; i < origCenter; i++)
                        {
                            int newOffset = i + i / 15;
                            alignmentMap[origCenter - i - 1] = center - newOffset - 1;
                            alignmentMap[origCenter + i] = center + newOffset + 1;
                        }
                    }
                    var matrix = new StiBarcodeUtils.BitMatrix(matrixSize);

                    // draw data bits
                    for (int i = 0, rowOffset = 0; i < layers; i++)
                    {
                        int rowSize = (layers - i) * 4 + (compact ? 9 : 12);
                        for (int j = 0; j < rowSize; j++)
                        {
                            int columnOffset = j * 2;
                            for (int k = 0; k < 2; k++)
                            {
                                if (messageBits[rowOffset + columnOffset + k])
                                {
                                    matrix[alignmentMap[i * 2 + k], alignmentMap[i * 2 + j]] = true;
                                }
                                if (messageBits[rowOffset + rowSize * 2 + columnOffset + k])
                                {
                                    matrix[alignmentMap[i * 2 + j], alignmentMap[baseMatrixSize - 1 - i * 2 - k]] = true;
                                }
                                if (messageBits[rowOffset + rowSize * 4 + columnOffset + k])
                                {
                                    matrix[alignmentMap[baseMatrixSize - 1 - i * 2 - k], alignmentMap[baseMatrixSize - 1 - i * 2 - j]] = true;
                                }
                                if (messageBits[rowOffset + rowSize * 6 + columnOffset + k])
                                {
                                    matrix[alignmentMap[baseMatrixSize - 1 - i * 2 - j], alignmentMap[i * 2 + k]] = true;
                                }
                            }
                        }
                        rowOffset += rowSize * 8;
                    }

                    // draw mode message
                    DrawModeMessage(matrix, compact, matrixSize, modeMessage);

                    // draw alignment marks
                    if (compact)
                    {
                        DrawBullsEye(matrix, matrixSize / 2, 5);
                    }
                    else
                    {
                        DrawBullsEye(matrix, matrixSize / 2, 7);
                        for (int i = 0, j = 0; i < baseMatrixSize / 2 - 1; i += 15, j += 16)
                        {
                            for (int k = (matrixSize / 2) & 1; k < matrixSize; k += 2)
                            {
                                matrix[matrixSize / 2 - j, k] = true;
                                matrix[matrixSize / 2 + j, k] = true;
                                matrix[k, matrixSize / 2 - j] = true;
                                matrix[k, matrixSize / 2 + j] = true;
                            }
                        }
                    }

                    return new AztecCode
                    {
                        IsCompact = compact,
                        Size = matrixSize,
                        Layers = layers,
                        CodeWords = messageSizeInWords,
                        Matrix = matrix
                    };
                }

                private static void DrawBullsEye(StiBarcodeUtils.BitMatrix matrix, int center, int size)
                {
                    for (var i = 0; i < size; i += 2)
                    {
                        for (var j = center - i; j <= center + i; j++)
                        {
                            matrix[j, center - i] = true;
                            matrix[j, center + i] = true;
                            matrix[center - i, j] = true;
                            matrix[center + i, j] = true;
                        }
                    }
                    matrix[center - size, center - size] = true;
                    matrix[center - size + 1, center - size] = true;
                    matrix[center - size, center - size + 1] = true;
                    matrix[center + size, center - size] = true;
                    matrix[center + size, center - size + 1] = true;
                    matrix[center + size, center + size - 1] = true;
                }

                internal static BitArray GenerateModeMessage(bool compact, int layers, int messageSizeInWords)
                {
                    var modeMessage = new BitArray();
                    if (compact)
                    {
                        modeMessage.AppendBits(layers - 1, 2);
                        modeMessage.AppendBits(messageSizeInWords - 1, 6);
                        modeMessage = GenerateCheckWords(modeMessage, 28, 4);
                    }
                    else
                    {
                        modeMessage.AppendBits(layers - 1, 5);
                        modeMessage.AppendBits(messageSizeInWords - 1, 11);
                        modeMessage = GenerateCheckWords(modeMessage, 40, 4);
                    }
                    return modeMessage;
                }

                private static void DrawModeMessage(StiBarcodeUtils.BitMatrix matrix, bool compact, int matrixSize, BitArray modeMessage)
                {
                    int center = matrixSize / 2;

                    if (compact)
                    {
                        for (var i = 0; i < 7; i++)
                        {
                            int offset = center - 3 + i;
                            if (modeMessage[i])
                            {
                                matrix[offset, center - 5] = true;
                            }
                            if (modeMessage[i + 7])
                            {
                                matrix[center + 5, offset] = true;
                            }
                            if (modeMessage[20 - i])
                            {
                                matrix[offset, center + 5] = true;
                            }
                            if (modeMessage[27 - i])
                            {
                                matrix[center - 5, offset] = true;
                            }
                        }
                    }
                    else
                    {
                        for (var i = 0; i < 10; i++)
                        {
                            int offset = center - 5 + i + i / 5;
                            if (modeMessage[i])
                            {
                                matrix[offset, center - 7] = true;
                            }
                            if (modeMessage[i + 10])
                            {
                                matrix[center + 7, offset] = true;
                            }
                            if (modeMessage[29 - i])
                            {
                                matrix[offset, center + 7] = true;
                            }
                            if (modeMessage[39 - i])
                            {
                                matrix[center - 7, offset] = true;
                            }
                        }
                    }
                }

                private static BitArray GenerateCheckWords(BitArray bitArray, int totalBits, int wordSize)
                {
                    if (bitArray.Size % wordSize != 0)
                        throw new StiAztecException("size of bit array is not a multiple of the word size");

                    // bitArray is guaranteed to be a multiple of the wordSize, so no padding needed
                    int messageSizeInWords = bitArray.Size / wordSize;

                    var rs = new StiBarcodeUtils.ReedSolomonEncoder(GetGF(wordSize));
                    var totalWords = totalBits / wordSize;
                    var messageWords = BitsToWords(bitArray, wordSize, totalWords);
                    rs.Encode(messageWords, totalWords - messageSizeInWords);

                    var startPad = totalBits % wordSize;
                    var messageBits = new BitArray();
                    messageBits.AppendBits(0, startPad);
                    foreach (var messageWord in messageWords)
                    {
                        messageBits.AppendBits(messageWord, wordSize);
                    }
                    return messageBits;
                }

                private static int[] BitsToWords(BitArray stuffedBits, int wordSize, int totalWords)
                {
                    var message = new int[totalWords];
                    int i;
                    int n;
                    for (i = 0, n = stuffedBits.Size / wordSize; i < n; i++)
                    {
                        int value = 0;
                        for (int j = 0; j < wordSize; j++)
                        {
                            value |= stuffedBits[i * wordSize + j] ? (1 << wordSize - j - 1) : 0;
                        }
                        message[i] = value;
                    }
                    return message;
                }

                private static StiBarcodeUtils.GaloisField GetGF(int wordSize)
                {
                    switch (wordSize)
                    {
                        case 4:
                            return StiBarcodeUtils.GaloisField.Aztec_Param;
                        case 6:
                            return StiBarcodeUtils.GaloisField.Aztec_Data_6;
                        case 8:
                            return StiBarcodeUtils.GaloisField.Aztec_Data_8;
                        case 10:
                            return StiBarcodeUtils.GaloisField.Aztec_Data_10;
                        case 12:
                            return StiBarcodeUtils.GaloisField.Aztec_Data_12;
                        default:
                            throw new StiAztecException("Unsupported word size " + wordSize);
                    }
                }

                internal static BitArray StuffBits(BitArray bits, int wordSize)
                {
                    var res = new BitArray();

                    int n = bits.Size;
                    int mask = (1 << wordSize) - 2;
                    for (int i = 0; i < n; i += wordSize)
                    {
                        int word = 0;
                        for (int j = 0; j < wordSize; j++)
                        {
                            if (i + j >= n || bits[i + j])
                            {
                                word |= 1 << (wordSize - 1 - j);
                            }
                        }
                        if ((word & mask) == mask)
                        {
                            res.AppendBits(word & mask, wordSize);
                            i--;
                        }
                        else if ((word & mask) == 0)
                        {
                            res.AppendBits(word | 1, wordSize);
                            i--;
                        }
                        else
                        {
                            res.AppendBits(word, wordSize);
                        }
                    }

                    return res;
                }

                private static int TotalBitsInLayer(int layers, bool compact)
                {
                    return ((compact ? 88 : 112) + 16 * layers) * layers;
                }
            }
            #endregion

            #region BitArray
            private sealed class BitArray
            {
                private int[] bits;
                private int size;

                public int Size => size;

                public int SizeInBytes => (size + 7) >> 3;

                public bool this[int i]
                {
                    get
                    {
                        return (bits[i >> 5] & (1 << (i & 0x1F))) != 0;
                    }
                    set
                    {
                        if (value)
                            bits[i >> 5] |= 1 << (i & 0x1F);
                    }
                }

                public BitArray()
                {
                    this.size = 0;
                    this.bits = new int[1];
                }

                private void EnsureCapacity(int size)
                {
                    if (size > bits.Length << 5)
                    {
                        int[] newBits = MakeArray(size);
                        Array.Copy(bits, 0, newBits, 0, bits.Length);
                        bits = newBits;
                    }
                }

                public void AppendBit(bool bit)
                {
                    EnsureCapacity(size + 1);
                    if (bit)
                    {
                        bits[size >> 5] |= 1 << (size & 0x1F);
                    }
                    size++;
                }

                public void AppendBits(int value, int numBits)
                {
                    if (numBits < 0 || numBits > 32)
                    {
                        throw new StiAztecException("Num bits must be between 0 and 32");
                    }
                    EnsureCapacity(size + numBits);
                    for (int numBitsLeft = numBits; numBitsLeft > 0; numBitsLeft--)
                    {
                        AppendBit(((value >> (numBitsLeft - 1)) & 0x01) == 1);
                    }
                }

                public void AppendBitArray(BitArray other)
                {
                    int otherSize = other.size;
                    EnsureCapacity(size + otherSize);
                    for (int i = 0; i < otherSize; i++)
                    {
                        AppendBit(other[i]);
                    }
                }

                private static int[] MakeArray(int size)
                {
                    return new int[(size + 31) >> 5];
                }
            }
            #endregion

            #region Tokens
            private abstract class Token
            {
                public static Token EMPTY = new SimpleToken(null, 0, 0);

                private readonly Token previous;
                public Token Previous => previous;

                protected Token(Token previous)
                {
                    this.previous = previous;
                }

                public Token Add(int value, int bitCount)
                {
                    return new SimpleToken(this, value, bitCount);
                }

                public Token AddBinaryShift(int start, int byteCount)
                {
                    //int bitCount = (byteCount * 8) + (byteCount <= 31 ? 10 : byteCount <= 62 ? 20 : 21);
                    return new BinaryShiftToken(this, start, byteCount);
                }

                public abstract void AppendTo(BitArray bitArray, byte[] text);
            }

            private sealed class SimpleToken : Token
            {
                private readonly short value;
                private readonly short bitCount;

                public SimpleToken(Token previous, int value, int bitCount)
                   : base(previous)
                {
                    this.value = (short)value;
                    this.bitCount = (short)bitCount;
                }

                public override void AppendTo(BitArray bitArray, byte[] text)
                {
                    bitArray.AppendBits(value, bitCount);
                }
            }

            private sealed class BinaryShiftToken : Token
            {
                private readonly short binaryShiftStart;
                private readonly short binaryShiftByteCount;

                public BinaryShiftToken(Token previous, int binaryShiftStart, int binaryShiftByteCount)
                   : base(previous)
                {
                    this.binaryShiftStart = (short)binaryShiftStart;
                    this.binaryShiftByteCount = (short)binaryShiftByteCount;
                }

                public override void AppendTo(BitArray bitArray, byte[] text)
                {
                    for (int i = 0; i < binaryShiftByteCount; i++)
                    {
                        if (i == 0 || (i == 31 && binaryShiftByteCount <= 62))
                        {
                            bitArray.AppendBits(31, 5);  // BINARY_SHIFT
                            if (binaryShiftByteCount > 62)
                            {
                                bitArray.AppendBits(binaryShiftByteCount - 31, 16);
                            }
                            else if (i == 0)
                            {
                                bitArray.AppendBits(Math.Min(binaryShiftByteCount, (short)31), 5);
                            }
                            else
                            {
                                bitArray.AppendBits(binaryShiftByteCount - 31, 5);
                            }
                        }
                        bitArray.AppendBits(text[binaryShiftStart + i], 8);
                    }
                }
            }
            #endregion

            #region State
            private sealed class State
            {
                public static readonly State INITIAL_STATE = new State(Token.EMPTY, HighLevelEncoder.MODE_UPPER, 0, 0);

                private readonly int mode;
                private readonly Token token;
                private readonly int binaryShiftByteCount;
                private readonly int bitCount;

                public State(Token token, int mode, int binaryBytes, int bitCount)
                {
                    this.token = token;
                    this.mode = mode;
                    this.binaryShiftByteCount = binaryBytes;
                    this.bitCount = bitCount;
                }

                public int Mode => mode;

                public Token Token => token;

                public int BinaryShiftByteCount => binaryShiftByteCount;

                public int BitCount => bitCount;

                public State AppendFLGn(int eci)
                {
                    State result = ShiftAndAppend(HighLevelEncoder.MODE_PUNCT, 0); // 0: FLG(n)
                    Token token = result.token;
                    int bitsAdded = 3;
                    if (eci < 0)    //FNC1
                    {
                        token = token.Add(0, 3); // 0: FNC1
                    }
                    else
                    {
                        int d1 = eci % 10;
                        int d2 = eci / 10;
                        if (d2 > 0)
                        {
                            token = token.Add(2, 3); // 2 digits
                            token = token.Add(d2 + 2, 4);
                            token = token.Add(d1 + 2, 4);
                            bitsAdded += 2 * 4;
                        }
                        else
                        {
                            token = token.Add(1, 3); // 1 digits
                            token = token.Add(d1 + 2, 4);
                            bitsAdded += 4;
                        }
                    }
                    return new State(token, mode, 0, bitCount + bitsAdded);
                }

                public State LatchAndAppend(int mode, int value)
                {
                    int bitCount = this.bitCount;
                    Token token = this.token;
                    if (mode != this.mode)
                    {
                        int latch = HighLevelEncoder.LATCH_TABLE[this.mode][mode];
                        token = token.Add(latch & 0xFFFF, latch >> 16);
                        bitCount += latch >> 16;
                    }
                    int latchModeBitCount = mode == HighLevelEncoder.MODE_DIGIT ? 4 : 5;
                    token = token.Add(value, latchModeBitCount);
                    return new State(token, mode, 0, bitCount + latchModeBitCount);
                }

                public State ShiftAndAppend(int mode, int value)
                {
                    Token token = this.token;
                    int thisModeBitCount = this.mode == HighLevelEncoder.MODE_DIGIT ? 4 : 5;
                    token = token.Add(HighLevelEncoder.SHIFT_TABLE[this.mode][mode], thisModeBitCount);
                    token = token.Add(value, 5);
                    return new State(token, this.mode, 0, this.bitCount + thisModeBitCount + 5);
                }

                public State AddBinaryShiftChar(int index)
                {
                    Token token = this.token;
                    int mode = this.mode;
                    int bitCount = this.bitCount;
                    if (this.mode == HighLevelEncoder.MODE_PUNCT || this.mode == HighLevelEncoder.MODE_DIGIT)
                    {
                        int latch = HighLevelEncoder.LATCH_TABLE[mode][HighLevelEncoder.MODE_UPPER];
                        token = token.Add(latch & 0xFFFF, latch >> 16);
                        bitCount += latch >> 16;
                        mode = HighLevelEncoder.MODE_UPPER;
                    }
                    int deltaBitCount = (binaryShiftByteCount == 0 || binaryShiftByteCount == 31) ? 18 : (binaryShiftByteCount == 62) ? 9 : 8;
                    State result = new State(token, mode, binaryShiftByteCount + 1, bitCount + deltaBitCount);
                    if (result.binaryShiftByteCount == 2047 + 31)
                    {
                        result = result.EndBinaryShift(index + 1);
                    }
                    return result;
                }

                public State EndBinaryShift(int index)
                {
                    if (binaryShiftByteCount == 0)
                    {
                        return this;
                    }
                    Token token = this.token;
                    token = token.AddBinaryShift(index - binaryShiftByteCount, binaryShiftByteCount);
                    return new State(token, mode, 0, this.bitCount);
                }

                public bool IsBetterThanOrEqualTo(State other)
                {
                    int newModeBitCount = this.bitCount + (HighLevelEncoder.LATCH_TABLE[this.mode][other.mode] >> 16);
                    if (this.binaryShiftByteCount < other.binaryShiftByteCount)
                    {
                        newModeBitCount += CalculateBinaryShiftCost(other) - CalculateBinaryShiftCost(this);
                    }
                    else if (this.binaryShiftByteCount > other.binaryShiftByteCount && other.binaryShiftByteCount > 0)
                    {
                        newModeBitCount += 10;
                    }
                    return newModeBitCount <= other.bitCount;
                }

                public BitArray ToBitArray(byte[] text)
                {
                    var symbols = new LinkedList<Token>();
                    for (Token token = EndBinaryShift(text.Length).token; token != null; token = token.Previous)
                    {
                        symbols.AddFirst(token);
                    }
                    BitArray bitArray = new BitArray();
                    foreach (Token symbol in symbols)
                    {
                        symbol.AppendTo(bitArray, text);
                    }
                    return bitArray;
                }

                private static int CalculateBinaryShiftCost(State state)
                {
                    if (state.binaryShiftByteCount > 62)
                    {
                        return 21; // B/S with extended length
                    }
                    if (state.binaryShiftByteCount > 31)
                    {
                        return 20; // two B/S
                    }
                    if (state.binaryShiftByteCount > 0)
                    {
                        return 10; // one B/S
                    }
                    return 0;
                }
            }
            #endregion

            #region HighLevelEncoder
            private sealed class HighLevelEncoder
            {
                #region Fields, tables
                internal const int MODE_UPPER = 0; // 5 bits
                internal const int MODE_LOWER = 1; // 5 bits
                internal const int MODE_DIGIT = 2; // 4 bits
                internal const int MODE_MIXED = 3; // 5 bits
                internal const int MODE_PUNCT = 4; // 5 bits

                internal static readonly int[][] LATCH_TABLE = new int[][] {
            new[] {
                  0,
                  (5 << 16) + 28, // UPPER -> LOWER
                  (5 << 16) + 30, // UPPER -> DIGIT
                  (5 << 16) + 29, // UPPER -> MIXED
                  (10 << 16) + (29 << 5) + 30, // UPPER -> MIXED -> PUNCT
               },
            new[] {
                  (9 << 16) + (30 << 4) + 14, // LOWER -> DIGIT -> UPPER
                  0,
                  (5 << 16) + 30, // LOWER -> DIGIT
                  (5 << 16) + 29, // LOWER -> MIXED
                  (10 << 16) + (29 << 5) + 30, // LOWER -> MIXED -> PUNCT
               },
            new[] {
                  (4 << 16) + 14, // DIGIT -> UPPER
                  (9 << 16) + (14 << 5) + 28, // DIGIT -> UPPER -> LOWER
                  0,
                  (9 << 16) + (14 << 5) + 29, // DIGIT -> UPPER -> MIXED
                  (14 << 16) + (14 << 10) + (29 << 5) + 30,
                  // DIGIT -> UPPER -> MIXED -> PUNCT
               },
            new[] {
                  (5 << 16) + 29, // MIXED -> UPPER
                  (5 << 16) + 28, // MIXED -> LOWER
                  (10 << 16) + (29 << 5) + 30, // MIXED -> UPPER -> DIGIT
                  0,
                  (5 << 16) + 30, // MIXED -> PUNCT
               },
            new[] {
                  (5 << 16) + 31, // PUNCT -> UPPER
                  (10 << 16) + (31 << 5) + 28, // PUNCT -> UPPER -> LOWER
                  (10 << 16) + (31 << 5) + 30, // PUNCT -> UPPER -> DIGIT
                  (10 << 16) + (31 << 5) + 29, // PUNCT -> UPPER -> MIXED
                  0
               }
            };

                internal static readonly int[][] CHAR_MAP = new int[5][];
                internal static readonly int[][] SHIFT_TABLE = new int[6][]; // mode shift codes, per table
                private readonly byte[] text;
                private readonly int eci;
                #endregion

                #region Static constructor
                static HighLevelEncoder()
                {
                    CHAR_MAP[0] = new int[256];
                    CHAR_MAP[1] = new int[256];
                    CHAR_MAP[2] = new int[256];
                    CHAR_MAP[3] = new int[256];
                    CHAR_MAP[4] = new int[256];

                    SHIFT_TABLE[0] = new int[6];
                    SHIFT_TABLE[1] = new int[6];
                    SHIFT_TABLE[2] = new int[6];
                    SHIFT_TABLE[3] = new int[6];
                    SHIFT_TABLE[4] = new int[6];
                    SHIFT_TABLE[5] = new int[6];

                    CHAR_MAP[MODE_UPPER][' '] = 1;
                    for (int c = 'A'; c <= 'Z'; c++)
                    {
                        CHAR_MAP[MODE_UPPER][c] = c - 'A' + 2;
                    }
                    CHAR_MAP[MODE_LOWER][' '] = 1;
                    for (int c = 'a'; c <= 'z'; c++)
                    {
                        CHAR_MAP[MODE_LOWER][c] = c - 'a' + 2;
                    }
                    CHAR_MAP[MODE_DIGIT][' '] = 1;
                    for (int c = '0'; c <= '9'; c++)
                    {
                        CHAR_MAP[MODE_DIGIT][c] = c - '0' + 2;
                    }
                    CHAR_MAP[MODE_DIGIT][','] = 12;
                    CHAR_MAP[MODE_DIGIT]['.'] = 13;
                    int[] mixedTable = { '\0', ' ', 1, 2, 3, 4, 5, 6, 7, '\b', '\t', '\n', 11, '\f', '\r', 27, 28, 29, 30, 31, '@', '\\', '^', '_', '`', '|', '~', 127 };
                    for (int i = 0; i < mixedTable.Length; i++)
                    {
                        CHAR_MAP[MODE_MIXED][mixedTable[i]] = i;
                    }
                    int[] punctTable = { '\0', '\r', '\0', '\0', '\0', '\0', '!', '\'', '#', '$', '%', '&', '\'', '(', ')', '*', '+', ',', '-', '.', '/', ':', ';', '<', '=', '>', '?', '[', ']', '{', '}' };
                    for (int i = 0; i < punctTable.Length; i++)
                    {
                        if (punctTable[i] > 0)
                        {
                            CHAR_MAP[MODE_PUNCT][punctTable[i]] = i;
                        }
                    }
                    foreach (int[] table in SHIFT_TABLE)
                    {
                        for (int i = 0; i < table.Length; i++)
                        {
                            table[i] = -1;
                        }
                    }
                    SHIFT_TABLE[MODE_UPPER][MODE_PUNCT] = 0;

                    SHIFT_TABLE[MODE_LOWER][MODE_PUNCT] = 0;
                    SHIFT_TABLE[MODE_LOWER][MODE_UPPER] = 28;

                    SHIFT_TABLE[MODE_MIXED][MODE_PUNCT] = 0;

                    SHIFT_TABLE[MODE_DIGIT][MODE_PUNCT] = 0;
                    SHIFT_TABLE[MODE_DIGIT][MODE_UPPER] = 15;
                }
                #endregion

                public HighLevelEncoder(byte[] text, int eci)
                {
                    this.text = text;
                    this.eci = eci;
                }

                public BitArray Encode()
                {
                    State initialState = State.INITIAL_STATE;
                    if (eci != 0)
                    {
                        initialState = initialState.AppendFLGn(eci);
                    }
                    ICollection<State> states = new global::System.Collections.ObjectModel.Collection<State>();
                    states.Add(initialState);
                    for (int index = 0; index < text.Length; index++)
                    {
                        int pairCode;
                        // don't remove the (int) type cast, mono compiler needs it
                        int nextChar = (index + 1 < text.Length) ? (int)text[index + 1] : 0;
                        switch (text[index])
                        {
                            case (byte)'\r':
                                pairCode = nextChar == '\n' ? 2 : 0;
                                break;
                            case (byte)'.':
                                pairCode = nextChar == ' ' ? 3 : 0;
                                break;
                            case (byte)',':
                                pairCode = nextChar == ' ' ? 4 : 0;
                                break;
                            case (byte)':':
                                pairCode = nextChar == ' ' ? 5 : 0;
                                break;
                            default:
                                pairCode = 0;
                                break;
                        }
                        if (pairCode > 0)
                        {
                            states = UpdateStateListForPair(states, index, pairCode);
                            index++;
                        }
                        else
                        {
                            states = UpdateStateListForChar(states, index);
                        }
                    }
                    State minState = null;
                    foreach (var state in states)
                    {
                        if (minState == null)
                        {
                            minState = state;
                        }
                        else
                        {
                            if (state.BitCount < minState.BitCount)
                            {
                                minState = state;
                            }
                        }
                    }
                    return minState.ToBitArray(text);
                }

                private ICollection<State> UpdateStateListForChar(IEnumerable<State> states, int index)
                {
                    var result = new LinkedList<State>();
                    foreach (State state in states)
                    {
                        UpdateStateForChar(state, index, result);
                    }
                    return SimplifyStates(result);
                }

                private void UpdateStateForChar(State state, int index, ICollection<State> result)
                {
                    char ch = (char)(text[index] & 0xFF);
                    bool charInCurrentTable = CHAR_MAP[state.Mode][ch] > 0;
                    State stateNoBinary = null;
                    for (int mode = 0; mode <= MODE_PUNCT; mode++)
                    {
                        int charInMode = CHAR_MAP[mode][ch];
                        if (charInMode > 0)
                        {
                            if (stateNoBinary == null)
                            {
                                stateNoBinary = state.EndBinaryShift(index);
                            }
                            if (!charInCurrentTable || mode == state.Mode || mode == MODE_DIGIT)
                            {
                                var latchState = stateNoBinary.LatchAndAppend(mode, charInMode);
                                result.Add(latchState);
                            }
                            if (!charInCurrentTable && SHIFT_TABLE[state.Mode][mode] >= 0)
                            {
                                var shiftState = stateNoBinary.ShiftAndAppend(mode, charInMode);
                                result.Add(shiftState);
                            }
                        }
                    }
                    if (state.BinaryShiftByteCount > 0 || CHAR_MAP[state.Mode][ch] == 0)
                    {
                        var binaryState = state.AddBinaryShiftChar(index);
                        result.Add(binaryState);
                    }
                }

                private static ICollection<State> UpdateStateListForPair(IEnumerable<State> states, int index, int pairCode)
                {
                    var result = new LinkedList<State>();
                    foreach (State state in states)
                    {
                        UpdateStateForPair(state, index, pairCode, result);
                    }
                    return SimplifyStates(result);
                }

                private static void UpdateStateForPair(State state, int index, int pairCode, ICollection<State> result)
                {
                    State stateNoBinary = state.EndBinaryShift(index);
                    result.Add(stateNoBinary.LatchAndAppend(MODE_PUNCT, pairCode));
                    if (state.Mode != MODE_PUNCT)
                    {
                        result.Add(stateNoBinary.ShiftAndAppend(MODE_PUNCT, pairCode));
                    }
                    if (pairCode == 3 || pairCode == 4)
                    {
                        var digitState = stateNoBinary
                           .LatchAndAppend(MODE_DIGIT, 16 - pairCode) // period or comma in DIGIT
                           .LatchAndAppend(MODE_DIGIT, 1); // space in DIGIT
                        result.Add(digitState);
                    }
                    if (state.BinaryShiftByteCount > 0)
                    {
                        State binaryState = state.AddBinaryShiftChar(index).AddBinaryShiftChar(index + 1);
                        result.Add(binaryState);
                    }
                }

                private static ICollection<State> SimplifyStates(IEnumerable<State> states)
                {
                    var result = new LinkedList<State>();
                    var removeList = new List<State>();
                    foreach (State newState in states)
                    {
                        bool add = true;
                        removeList.Clear();
                        foreach (var oldState in result)
                        {
                            if (oldState.IsBetterThanOrEqualTo(newState))
                            {
                                add = false;
                                break;
                            }
                            if (newState.IsBetterThanOrEqualTo(oldState))
                            {
                                removeList.Add(oldState);
                            }
                        }
                        if (add)
                        {
                            result.AddLast(newState);
                        }
                        foreach (var removeItem in removeList)
                        {
                            result.Remove(removeItem);
                        }
                    }
                    return result;
                }
            }
            #endregion
        }
        #endregion

        #region Methods

        public override void Draw(object context, StiBarCode barCode, RectangleF rect, float zoom)
		{		
			var code = GetCode(barCode);
			//code = CheckCodeSymbols(code, AztecSymbols);
			BarCodeData.Code = code;

            try
            {
                var bm = new StiBarcodeUtils.BitMatrix(1, 1);
                bool assembleData = (context is StiBarCodeExportPainter) && (context as StiBarCodeExportPainter).OnlyAssembleData;
                if (!assembleData)  //speed optimization for pdf
                {
                    bm = StiAztec.Encode(code, ErrorCorrectionLevel, MatrixSize, CodePage, false).Matrix;
                }

                byte[] matrix = new byte[bm.Width * bm.Height];

                for (int y = 0; y < bm.Height; y++)
                {
                    int offset = y * bm.Width;
                    for (int x = 0; x < bm.Width; x++)
                    {
                        matrix[offset + x] = bm[x, y] ? (byte)1 : (byte)0;
                    }
                }

                BarCodeData.MatrixGrid = matrix;
                BarCodeData.MatrixWidth = bm.Width;
                BarCodeData.MatrixHeight = bm.Height;
                BarCodeData.MatrixRatioY = 1;

                //if (dm.ErrorMessage == null)
                //{
                    Draw2DBarCode(context, rect, barCode, zoom);
                //}
                //else
                //{
                //    DrawBarCodeError(context, rect, barCode);
                //}
            }
            catch
            {
                DrawBarCodeError(context, rect, barCode);
            }

		}

        public static Image GetBarcodeImage(string code, int zoom)
        {
            var qr = new StiBarCode();
            qr.BarCodeType = new StiAztecBarCodeType(10, 33, StiAztecSize.Automatic, 28591);
            var BarCodeData = (qr.BarCodeType as StiAztecBarCodeType).BarCodeData;
            BarCodeData.Code = code;

            Image resultImage = null;

            try
            {
                var bm = StiAztec.Encode(code, 33, StiAztecSize.Automatic, 28591).Matrix;

                byte[] matrix = new byte[bm.Width * bm.Height];

                for (int y = 0; y < bm.Height; y++)
                {
                    int offset = y * bm.Width;
                    for (int x = 0; x < bm.Width; x++)
                    {
                        matrix[offset + x] = bm[x, y] ? (byte)1 : (byte)0;
                    }
                }

                BarCodeData.MatrixGrid = matrix;
                BarCodeData.MatrixWidth = bm.Width;
                BarCodeData.MatrixHeight = bm.Height;
                BarCodeData.MatrixRatioY = 1;

                int quietZone = 2;
                if (!qr.ShowQuietZones) quietZone = 0;
                int imageWidth = (BarCodeData.MatrixWidth + quietZone * 2) * zoom;
                int imageHeight = (BarCodeData.MatrixHeight + quietZone * 2) * zoom;
                resultImage = new Bitmap(imageWidth, imageHeight);
                using (var gr = Graphics.FromImage(resultImage))
                {
                    (qr.BarCodeType as StiAztecBarCodeType).Draw2DBarCode(gr, new Rectangle(0, 0, imageWidth, imageHeight), qr, zoom);
                }
            }
            catch
            {
            }

            return resultImage;
        }        
		#endregion

        #region Methods.override
        public override StiBarCodeTypeService CreateNew() => new StiAztecBarCodeType();
        #endregion

        public StiAztecBarCodeType()
            : this(40f, 33, StiAztecSize.Automatic, 28591)
		{
		}

        public StiAztecBarCodeType(float module, int errorCorrectionLevel, StiAztecSize matrixSize, int codePage)
		{
			this.module = module;
            this.ErrorCorrectionLevel = errorCorrectionLevel;
			this.MatrixSize = matrixSize;
            this.CodePage = codePage;
		}
    }
}