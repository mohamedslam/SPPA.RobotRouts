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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Drawing.Design;
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
using Image = Stimulsoft.Drawing.Image;
using Bitmap = Stimulsoft.Drawing.Bitmap;
using Graphics = Stimulsoft.Drawing.Graphics;
#else
#endif

namespace Stimulsoft.Report.BarCodes
{
    /// <summary>
    /// The class describes the Barcode type - QR Code.
    /// </summary>
    [TypeConverter(typeof(Design.StiQRCodeBarCodeTypeConverter))]
	public partial class StiQRCodeBarCodeType : StiBarCodeTypeService
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiQRCodeBarCodeType
            jObject.AddPropertyFloat("Height", Height, 1f);
            jObject.AddPropertyFloat("Module", Module, 40f);
            jObject.AddPropertyEnum("ErrorCorrectionLevel", ErrorCorrectionLevel, StiQRCodeErrorCorrectionLevel.Level1);
            jObject.AddPropertyEnum("MatrixSize", MatrixSize, StiQRCodeSize.Automatic);
            jObject.AddPropertyBool("ProcessTilde", ProcessTilde, false);
            jObject.AddPropertyDouble("ImageMultipleFactor", ImageMultipleFactor, 1d);
            jObject.AddPropertyImage("Image", Image);

            jObject.AddPropertyBrush("BodyBrush", BodyBrush);
            jObject.AddPropertyBrush("EyeFrameBrush", EyeFrameBrush);
            jObject.AddPropertyBrush("EyeBallBrush", EyeBallBrush);
            jObject.AddPropertyEnum("BodyShape", BodyShape, StiQRCodeBodyShapeType.Square);
            jObject.AddPropertyEnum("EyeFrameShape", EyeFrameShape, StiQRCodeEyeFrameShapeType.Square);
            jObject.AddPropertyEnum("EyeBallShape", EyeBallShape, StiQRCodeEyeBallShapeType.Square);

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
                        this.ErrorCorrectionLevel = property.DeserializeEnum<StiQRCodeErrorCorrectionLevel>();
                        break;

                    case "MatrixSize":
                        this.MatrixSize = property.DeserializeEnum<StiQRCodeSize>();
                        break;

                    case "ProcessTilde":
                        this.ProcessTilde = property.DeserializeBool();
                        break;

                    case "ImageMultipleFactor":
                        this.ImageMultipleFactor = property.DeserializeFloat();
                        break;

                    case "Image":
                        this.Image = property.DeserializeImage();
                        break;

                    case "BodyShape":
                        this.BodyShape = property.DeserializeEnum<StiQRCodeBodyShapeType>();
                        break;
                    case "EyeFrameShape":
                        this.EyeFrameShape = property.DeserializeEnum<StiQRCodeEyeFrameShapeType>();
                        break;
                    case "EyeBallShape":
                        this.EyeBallShape = property.DeserializeEnum<StiQRCodeEyeBallShapeType>();
                        break;

                    case "BodyBrush":
                        this.BodyBrush = property.DeserializeBrush();
                        break;
                    case "EyeFrameBrush":
                        this.EyeFrameBrush = property.DeserializeBrush();
                        break;
                    case "EyeBallBrush":
                        this.EyeBallBrush = property.DeserializeBrush();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject

        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiQRCodeBarCodeType;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            return new StiPropertyCollection
            {
                {
                    StiPropertyCategories.Main,
                    new[]
                    {
                        propHelper.ErrorCorrectionLevel(),
                        propHelper.MatrixSize(),
                        propHelper.Module(),
                        propHelper.ProcessTilde(),
                        propHelper.Image(),
                        propHelper.ImageMultipleFactor(),
                    }
                },
                {
                    StiPropertyCategories.Appearance,
                    new[]
                    {
                        propHelper.BodyBrush(),
                        propHelper.BodyShape(),
                        propHelper.EyeBallBrush(),
                        propHelper.EyeBallShape(),
                        propHelper.EyeFrameBrush(),
                        propHelper.EyeFrameShape(),
                    }
                }
            };
        }

        #endregion

		#region ServiceName
		/// <summary>
		/// Gets a service name.
		/// </summary>
		public override string ServiceName => "QR Code";
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
				if (value < 2f) module = 2f;
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

        /// <summary>
        /// Gets or sets the error correction level.
        /// </summary>
        [Description("Gets or sets the error correction level.")]
        [DefaultValue(StiQRCodeErrorCorrectionLevel.Level1)]
        [StiSerializable]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiCategory("BarCode")]
        public StiQRCodeErrorCorrectionLevel ErrorCorrectionLevel { get; set; } = StiQRCodeErrorCorrectionLevel.Level1;

        /// <summary>
        /// Gets or sets matrix size.
        /// </summary>
        [DefaultValue(StiQRCodeSize.Automatic)]
        [StiSerializable]
        [Description("Gets or sets matrix size.")]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiCategory("BarCode")]
        public StiQRCodeSize MatrixSize { get; set; } = StiQRCodeSize.Automatic;

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
        /// Gets or sets image value.
        /// </summary>
        [StiSerializable]
        [DefaultValue(null)]
        [Description("Gets or sets image value.")]
        [Editor("Stimulsoft.Report.Components.Design.StiSimpleImageEditor, Stimulsoft.Report.Design, " + StiVersion.VersionInfo, typeof(UITypeEditor))]
        [TypeConverter(typeof(Components.Design.StiSimpeImageConverter))]
        [StiOrder(StiPropertyOrder.BarCodeImage)]
        [StiPropertyLevel(StiLevel.Standard)]
        [StiCategory("BarCode")]
        public virtual Image Image { get; set; }

        /// <summary>
        /// Gets or sets value to multiply by it an image size.
        /// </summary>
        [StiSerializable]
        [DefaultValue(1d)]
        [Description("Gets or sets value to multiply by it an image size.")]
        [StiOrder(StiPropertyOrder.BarCodeImageMultipleFactor)]
        [StiPropertyLevel(StiLevel.Standard)]
        [StiCategory("BarCode")]
        public virtual double ImageMultipleFactor { get; set; } = 1d;

        /// <summary>
        /// Gets or sets the body shape type.
        /// </summary>
        [StiCategory("Appearance")]
        [DefaultValue(StiQRCodeBodyShapeType.Square)]
        [StiSerializable]
        [Description("Gets or sets the body shape type.")]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        public StiQRCodeBodyShapeType BodyShape { get; set; } = StiQRCodeBodyShapeType.Square;

        /// <summary>
        /// Gets or sets the EyeFrame shape type.
        /// </summary>
        [StiCategory("Appearance")]
        [DefaultValue(StiQRCodeEyeFrameShapeType.Square)]
        [StiSerializable]
        [Description("Gets or sets the EyeFrame shape type.")]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        public StiQRCodeEyeFrameShapeType EyeFrameShape { get; set; } = StiQRCodeEyeFrameShapeType.Square;

        /// <summary>
        /// Gets or sets the EyeBall shape type.
        /// </summary>
        [StiCategory("Appearance")]
        [DefaultValue(StiQRCodeEyeBallShapeType.Square)]
        [StiSerializable]
        [Description("Gets or sets the EyeBall shape type.")]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        public StiQRCodeEyeBallShapeType EyeBallShape { get; set; } = StiQRCodeEyeBallShapeType.Square;

        [StiCategory("Appearance")]
        [StiSerializable]
        [Description("The brush, which is used to draw body shapes.")]
        [TypeConverter(typeof(StiExpressionBrushConverter))]
        [Editor(StiEditors.ExpressionBrush, typeof(UITypeEditor))]
        public StiBrush BodyBrush { get; set; } = new StiEmptyBrush();

        [StiCategory("Appearance")]
        [StiSerializable]
        [Description("The brush, which is used to draw eye frame shapes.")]
        [TypeConverter(typeof(StiExpressionBrushConverter))]
        [Editor(StiEditors.ExpressionBrush, typeof(UITypeEditor))]
        public StiBrush EyeFrameBrush { get; set; } = new StiEmptyBrush();

        [StiCategory("Appearance")]
        [StiSerializable]
        [Description("The brush, which is used to draw eye ball shapes.")]
        [TypeConverter(typeof(StiExpressionBrushConverter))]
        [Editor(StiEditors.ExpressionBrush, typeof(UITypeEditor))]
        public StiBrush EyeBallBrush { get; set; } = new StiEmptyBrush();

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

		#region StiQRCodeException
		protected class StiQRCodeException : Exception
		{
			public StiQRCodeException()
			{
			}
			public StiQRCodeException(string message)
				: base(message)
			{
			}
			public StiQRCodeException(string message, Exception inner)
				: base(message, inner)
			{
			}
		}
		#endregion

        #region StiQRCode
        protected sealed class StiQRCode
        {
            #region Class QREncoder
            public sealed class QREncoder
            {

                private static readonly int[] ALPHANUMERIC_TABLE = 
                {
                    -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,  // 0x00-0x0f
                    -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,  // 0x10-0x1f
                    36, -1, -1, -1, 37, 38, -1, -1, -1, -1, 39, 40, -1, 41, 42, 43,  // 0x20-0x2f
                    0,   1,  2,  3,  4,  5,  6,  7,  8,  9, 44, -1, -1, -1, -1, -1,  // 0x30-0x3f
                    -1, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24,  // 0x40-0x4f
                    25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, -1, -1, -1, -1, -1   // 0x50-0x5f
                };

                private const string defaultECIByteModeEncoding = "ISO-8859-1";
                private const string BYTE_MODE_UTF8 = "UTF-8";
                private const int QUESTION_MARK_CHAR = 0x3F;

                private static int CalculateMaskPenalty(ByteMatrix matrix)
                {
                    int penalty = 0;
                    penalty += MaskUtil.ApplyMaskPenaltyRule1(matrix);
                    penalty += MaskUtil.ApplyMaskPenaltyRule2(matrix);
                    penalty += MaskUtil.ApplyMaskPenaltyRule3(matrix);
                    penalty += MaskUtil.ApplyMaskPenaltyRule4(matrix);
                    return penalty;
                }

                public static void Encode(string content, ErrorCorrectionLevel ecLevel, StiQRCode qrCode, StiQRCodeSize startVersion, bool processTilde, bool gs1 = false)
                {
                    if (string.IsNullOrEmpty(content))
                    {
                        throw new StiQRCodeException("Found empty contents");
                    }

                    content = StiBarCodeTypeService.UnpackTilde(content, processTilde);

                    if (!gs1 && content.StartsWith(new string((char)BarcodeCommandCode.Fnc1, 1)))
                    {
                        gs1 = true;
                        content = content.Substring(1);
                    }

                    string defaultByteModeEncoding = CharacterSetECI.GetEncodingByNumber((int)StiOptions.Engine.BarcodeQRCodeDefaultByteModeEncoding, defaultECIByteModeEncoding);

                    var encoding = defaultByteModeEncoding;

                    var mode = ChooseMode(content, encoding, gs1);

                    if (mode == Mode.BYTE && defaultByteModeEncoding != BYTE_MODE_UTF8)
                    {
                        encoding = GetEncodingName(content, defaultByteModeEncoding);
                    }

                    bool needAppendECI = (mode == Mode.BYTE) && !defaultECIByteModeEncoding.Equals(encoding) && (encoding != BYTE_MODE_UTF8) && !gs1;

                    var dataBits = new BitVector();
                    AppendBytes(content, mode, dataBits, encoding, gs1);

                    InitQRCode(dataBits.Size(), ecLevel, mode, qrCode, startVersion, needAppendECI, gs1);

                    var headerAndDataBits = new BitVector();

                    if (needAppendECI)
                    {
                        var eci = CharacterSetECI.GetCharacterSetECIByName(encoding);
                        if (eci != null)
                        {
                            AppendECI(eci, headerAndDataBits);
                        }
                    }

                    if (gs1)
                    {
                        AppendModeInfo(Mode.FNC1_FIRST_POSITION, headerAndDataBits);
                    }
                    AppendModeInfo(mode, headerAndDataBits);

                    int numLetters = mode.Equals(Mode.BYTE) ? dataBits.SizeInBytes() : content.Length;
                    AppendLengthInfo(numLetters, qrCode.GetVersion(), mode, headerAndDataBits);
                    headerAndDataBits.AppendBitVector(dataBits);

                    TerminateBits(qrCode.GetNumDataBytes(), headerAndDataBits);

                    var finalBits = new BitVector();
                    InterleaveWithECBytes(headerAndDataBits, qrCode.GetNumTotalBytes(), qrCode.GetNumDataBytes(), qrCode.GetNumRSBlocks(), finalBits);

                    var matrix = new ByteMatrix(qrCode.GetMatrixWidth(), qrCode.GetMatrixWidth());

                    bool optimizeForSpeed = true;
                    if (optimizeForSpeed)
                    {
                        int maskPattern = ChooseMaskPattern2(finalBits, qrCode.GetECLevel(), qrCode.GetVersion(), ref matrix);
                        qrCode.SetMaskPattern(maskPattern);
                    }
                    else
                    {
                        int maskPattern = ChooseMaskPattern(finalBits, qrCode.GetECLevel(), qrCode.GetVersion(), matrix);
                        qrCode.SetMaskPattern(maskPattern);
                        MatrixUtil.BuildMatrix(finalBits, qrCode.GetECLevel(), qrCode.GetVersion(), qrCode.GetMaskPattern(), matrix);
                    }

                    qrCode.SetMatrix(matrix);
                    if (!qrCode.IsValid())
                    {
                        throw new StiQRCodeException("Invalid QR code: " + qrCode.ToString());
                    }
                }

                private static string GetEncodingName(string content, string defaultByteModeEncoding)
                {
                    if (TryEncodeByte(content, defaultByteModeEncoding)) return defaultByteModeEncoding;

                    foreach (KeyValuePair<string, CharacterSetECI> eci in CharacterSetECI.NAME_TO_ECI)
                    {
                        if ((eci.Key != BYTE_MODE_UTF8) && TryEncodeByte(content, eci.Key))
                        {
                            return eci.Key;
                        }
                    }
                    return BYTE_MODE_UTF8;
                }

                private static bool TryEncodeByte(string content, string encodingName)
                {
                    Encoding encoding;
                    try
                    {
                        encoding = Encoding.GetEncoding(encodingName);
                    }
                    catch
                    {
                        return false;
                    }

                    //try to encode full string and then decode it back (strong comparison)
                    var bytes = encoding.GetBytes(content);
                    var stBack = encoding.GetString(bytes);
                    if (content != stBack)
                        return false;

                    //try to encode by one char
                    var currentChar = new char[1];
                    for (int index = 0; index < content.Length; index++)
                    {
                        currentChar[0] = content[index];
                        bytes = encoding.GetBytes(currentChar);
                        int length = bytes.Length;

                        if (currentChar[0] != '?' && length == 1 && (int)bytes[0] == QUESTION_MARK_CHAR)
                            return false;
                        else if (length > 1)
                            return false;
                    }

                    return true;
                }

                private static int GetAlphanumericCode(int code)
                {
                    if (code < ALPHANUMERIC_TABLE.Length)
                    {
                        return ALPHANUMERIC_TABLE[code];
                    }
                    if (code == (int)BarcodeCommandCode.Fnc1) return -2;
                    return -1;
                }

                //public static Mode ChooseMode(String content)
                //{
                //    return ChooseMode(content, null);
                //}

                public static Mode ChooseMode(string content, string encoding, bool gs1)
                {
                    if (!gs1 && "Shift_JIS".Equals(encoding))
                    {
                        return IsOnlyDoubleByteKanji(content) ? Mode.KANJI : Mode.BYTE;
                    }
                    bool hasNumeric = false;
                    bool hasAlphanumeric = false;
                    for (int i = 0; i < content.Length; ++i)
                    {
                        char c = content[i];
                        if (c >= '0' && c <= '9')
                        {
                            hasNumeric = true;
                        }
                        else if (GetAlphanumericCode(c) != -1)
                        {
                            hasAlphanumeric = true;
                        }
                        else
                        {
                            return Mode.BYTE;
                        }
                    }
                    if (hasAlphanumeric)
                    {
                        return Mode.ALPHANUMERIC;
                    }
                    else if (hasNumeric)
                    {
                        return Mode.NUMERIC;
                    }
                    return Mode.BYTE;
                }

                private static bool IsOnlyDoubleByteKanji(String content)
                {
                    byte[] bytes;
                    try
                    {
                        bytes = Encoding.GetEncoding("Shift_JIS").GetBytes(content);
                    }
                    catch
                    {
                        return false;
                    }
                    int length = bytes.Length;
                    if (length % 2 != 0)
                    {
                        return false;
                    }
                    for (int i = 0; i < length; i += 2)
                    {
                        int byte1 = bytes[i] & 0xFF;
                        if ((byte1 < 0x81 || byte1 > 0x9F) && (byte1 < 0xE0 || byte1 > 0xEB))
                        {
                            return false;
                        }
                    }
                    return true;
                }

                private static int ChooseMaskPattern(BitVector bits, ErrorCorrectionLevel ecLevel, int version, ByteMatrix matrix)
                {
                    int minPenalty = int.MaxValue;
                    int bestMaskPattern = -1;
                    for (int maskPattern = 0; maskPattern < StiQRCode.NUM_MASK_PATTERNS; maskPattern++)
                    {
                        MatrixUtil.BuildMatrix(bits, ecLevel, version, maskPattern, matrix);
                        int penalty = CalculateMaskPenalty(matrix);
                        if (penalty < minPenalty)
                        {
                            minPenalty = penalty;
                            bestMaskPattern = maskPattern;
                        }
                    }
                    return bestMaskPattern;
                }

                private static int ChooseMaskPattern2(BitVector bits, ErrorCorrectionLevel ecLevel, int version, ref ByteMatrix matrix)
                {
                    List<ByteMatrix> matrices = new List<ByteMatrix>();
                    matrices.Add(matrix);
                    for (int maskPattern = 1; maskPattern < StiQRCode.NUM_MASK_PATTERNS; maskPattern++)
                    {
                        matrices.Add(new ByteMatrix(matrix.GetWidth(), matrix.GetHeight()));
                    }

                    for (int maskPattern = 0; maskPattern < StiQRCode.NUM_MASK_PATTERNS; maskPattern++)
                    {
                        matrix = matrices[maskPattern];
                        MatrixUtil.ClearMatrix(matrix);
                        MatrixUtil.EmbedBasicPatterns(version, matrix);
                        MatrixUtil.EmbedTypeInfo(ecLevel, maskPattern, matrix);
                        MatrixUtil.MaybeEmbedVersionInfo(version, matrix);
                    }

                    MatrixUtil.EmbedDataBits2(bits, matrices);

                    int minPenalty = int.MaxValue;
                    int bestMaskPattern = -1;
                    for (int maskPattern = 0; maskPattern < StiQRCode.NUM_MASK_PATTERNS; maskPattern++)
                    {
                        int penalty = CalculateMaskPenalty(matrices[maskPattern]);
                        if (penalty < minPenalty)
                        {
                            minPenalty = penalty;
                            bestMaskPattern = maskPattern;
                        }
                    }
                    matrix = matrices[bestMaskPattern];
                    return bestMaskPattern;
                }

                private static void InitQRCode(int numInputBits, ErrorCorrectionLevel ecLevel, Mode mode, StiQRCode qrCode, StiQRCodeSize startVersion, bool needAppendECI, bool gs1)
                {
                    qrCode.SetECLevel(ecLevel);
                    qrCode.SetMode(mode);

                    int versionNum = 1;
                    if (startVersion != StiQRCodeSize.Automatic) versionNum = (int)startVersion;

                    for (; versionNum <= 40; versionNum++)
                    {
                        var version = Version.GetVersionForNumber(versionNum);
                        int numBytes = version.GetTotalCodewords();
                        var ecBlocks = version.GetECBlocksForLevel(ecLevel);
                        int numEcBytes = ecBlocks.GetTotalECCodewords();
                        int numRSBlocks = ecBlocks.GetNumBlocks();
                        int numDataBytes = numBytes - numEcBytes;

                        int numAdditionalBits = 4 + (needAppendECI ? 12 : 0) + (gs1 ? 4 : 0);
                        try
                        {
                            numAdditionalBits += mode.GetCharacterCountBits(version);
                        }
                        catch
                        {
                            numAdditionalBits = 32;
                        }

                        if (numDataBytes * 8 >= numInputBits + numAdditionalBits)
                        {
                            qrCode.SetVersion(versionNum);
                            qrCode.SetNumTotalBytes(numBytes);
                            qrCode.SetNumDataBytes(numDataBytes);
                            qrCode.SetNumRSBlocks(numRSBlocks);
                            qrCode.SetNumECBytes(numEcBytes);
                            qrCode.SetMatrixWidth(version.GetDimensionForVersion());
                            return;
                        }
                    }
                    throw new StiQRCodeException("Cannot find proper rs block info (input data too big?)");
                }

                private static void TerminateBits(int numDataBytes, BitVector bits)
                {
                    int capacity = numDataBytes << 3;
                    if (bits.Size() > capacity)
                    {
                        throw new StiQRCodeException("data bits cannot fit in the QR Code " + bits.Size() + " > " +
                            capacity);
                    }
                    for (int i = 0; i < 4 && bits.Size() < capacity; ++i)
                    {
                        bits.AppendBit(0);
                    }
                    int numBitsInLastByte = bits.Size() % 8;
                    if (numBitsInLastByte > 0)
                    {
                        int numPaddingBits = 8 - numBitsInLastByte;
                        for (int i = 0; i < numPaddingBits; ++i)
                        {
                            bits.AppendBit(0);
                        }
                    }
                    if (bits.Size() % 8 != 0)
                    {
                        throw new StiQRCodeException("Number of bits is not a multiple of 8");
                    }
                    int numPaddingBytes = numDataBytes - bits.SizeInBytes();
                    for (int i = 0; i < numPaddingBytes; ++i)
                    {
                        if (i % 2 == 0)
                        {
                            bits.AppendBits(0xec, 8);
                        }
                        else
                        {
                            bits.AppendBits(0x11, 8);
                        }
                    }
                    if (bits.Size() != capacity)
                    {
                        throw new StiQRCodeException("Bits size does not equal capacity");
                    }
                }

                private static void GetNumDataBytesAndNumECBytesForBlockID(int numTotalBytes, int numDataBytes,
                    int numRSBlocks, int blockID, int[] numDataBytesInBlock,
                    int[] numECBytesInBlock)
                {
                    if (blockID >= numRSBlocks)
                    {
                        throw new StiQRCodeException("Block ID too large");
                    }
                    int numRsBlocksInGroup2 = numTotalBytes % numRSBlocks;
                    int numRsBlocksInGroup1 = numRSBlocks - numRsBlocksInGroup2;
                    int numTotalBytesInGroup1 = numTotalBytes / numRSBlocks;
                    int numTotalBytesInGroup2 = numTotalBytesInGroup1 + 1;
                    int numDataBytesInGroup1 = numDataBytes / numRSBlocks;
                    int numDataBytesInGroup2 = numDataBytesInGroup1 + 1;
                    int numEcBytesInGroup1 = numTotalBytesInGroup1 - numDataBytesInGroup1;
                    int numEcBytesInGroup2 = numTotalBytesInGroup2 - numDataBytesInGroup2;
                    if (numEcBytesInGroup1 != numEcBytesInGroup2)
                    {
                        throw new StiQRCodeException("EC bytes mismatch");
                    }
                    if (numRSBlocks != numRsBlocksInGroup1 + numRsBlocksInGroup2)
                    {
                        throw new StiQRCodeException("RS blocks mismatch");
                    }
                    if (numTotalBytes !=
                        ((numDataBytesInGroup1 + numEcBytesInGroup1) *
                            numRsBlocksInGroup1) +
                            ((numDataBytesInGroup2 + numEcBytesInGroup2) *
                                numRsBlocksInGroup2))
                    {
                        throw new StiQRCodeException("Total bytes mismatch");
                    }

                    if (blockID < numRsBlocksInGroup1)
                    {
                        numDataBytesInBlock[0] = numDataBytesInGroup1;
                        numECBytesInBlock[0] = numEcBytesInGroup1;
                    }
                    else
                    {
                        numDataBytesInBlock[0] = numDataBytesInGroup2;
                        numECBytesInBlock[0] = numEcBytesInGroup2;
                    }
                }

                private static void InterleaveWithECBytes(BitVector bits, int numTotalBytes,
                    int numDataBytes, int numRSBlocks, BitVector result)
                {
                    if (bits.SizeInBytes() != numDataBytes)
                    {
                        throw new StiQRCodeException("Number of bits and data bytes does not match");
                    }

                    int dataBytesOffset = 0;
                    int maxNumDataBytes = 0;
                    int maxNumEcBytes = 0;

                    var blocks = new List<BlockPair>(numRSBlocks);

                    for (int i = 0; i < numRSBlocks; ++i)
                    {
                        var numDataBytesInBlock = new int[1];
                        var numEcBytesInBlock = new int[1];
                        GetNumDataBytesAndNumECBytesForBlockID(
                            numTotalBytes, numDataBytes, numRSBlocks, i,
                            numDataBytesInBlock, numEcBytesInBlock);

                        var dataBytes = new ByteArray();
                        dataBytes.Set(bits.GetArray(), dataBytesOffset, numDataBytesInBlock[0]);
                        var ecBytes = GenerateECBytes(dataBytes, numEcBytesInBlock[0]);
                        blocks.Add(new BlockPair(dataBytes, ecBytes));

                        maxNumDataBytes = Math.Max(maxNumDataBytes, dataBytes.Size());
                        maxNumEcBytes = Math.Max(maxNumEcBytes, ecBytes.Size());
                        dataBytesOffset += numDataBytesInBlock[0];
                    }
                    if (numDataBytes != dataBytesOffset)
                    {
                        throw new StiQRCodeException("Data bytes does not match offset");
                    }

                    for (int i = 0; i < maxNumDataBytes; ++i)
                    {
                        for (int j = 0; j < blocks.Count; ++j)
                        {
                            var dataBytes = blocks[j].GetDataBytes();
                            if (i < dataBytes.Size())
                            {
                                result.AppendBits(dataBytes.At(i), 8);
                            }
                        }
                    }

                    for (int i = 0; i < maxNumEcBytes; ++i)
                    {
                        for (int j = 0; j < blocks.Count; ++j)
                        {
                            var ecBytes = blocks[j].GetErrorCorrectionBytes();
                            if (i < ecBytes.Size())
                            {
                                result.AppendBits(ecBytes.At(i), 8);
                            }
                        }
                    }
                    if (numTotalBytes != result.SizeInBytes())
                    {
                        throw new StiQRCodeException("Interleaving error: " + numTotalBytes + " and " +
                            result.SizeInBytes() + " differ.");
                    }
                }

                private static ByteArray GenerateECBytes(ByteArray dataBytes, int numEcBytesInBlock)
                {
                    int numDataBytes = dataBytes.Size();
                    var toEncode = new int[numDataBytes + numEcBytesInBlock];
                    for (int i = 0; i < numDataBytes; i++)
                    {
                        toEncode[i] = dataBytes.At(i);
                    }
                    new StiBarcodeUtils.ReedSolomonEncoder(StiBarcodeUtils.GaloisField.QRCode_256).Encode(toEncode, numEcBytesInBlock);

                    var ecBytes = new ByteArray(numEcBytesInBlock);
                    for (int i = 0; i < numEcBytesInBlock; i++)
                    {
                        ecBytes.Set(i, toEncode[numDataBytes + i]);
                    }
                    return ecBytes;
                }

                private static void AppendModeInfo(Mode mode, BitVector bits)
                {
                    bits.AppendBits(mode.GetBits(), 4);
                }

                private static void AppendLengthInfo(int numLetters, int version, Mode mode, BitVector bits)
                {
                    int numBits = mode.GetCharacterCountBits(Version.GetVersionForNumber(version));
                    if (numLetters > ((1 << numBits) - 1))
                    {
                        throw new StiQRCodeException(numLetters + "is bigger than" + ((1 << numBits) - 1));
                    }
                    bits.AppendBits(numLetters, numBits);
                }

                private static void AppendBytes(String content, Mode mode, BitVector bits, String encoding, bool gs1)
                {
                    if (mode.Equals(Mode.NUMERIC))
                    {
                        AppendNumericBytes(content, bits);
                    }
                    else if (mode.Equals(Mode.ALPHANUMERIC))
                    {
                        AppendAlphanumericBytes(content, bits, gs1);
                    }
                    else if (mode.Equals(Mode.BYTE))
                    {
                        Append8BitBytes(content, bits, encoding, gs1);
                    }
                    else if (mode.Equals(Mode.KANJI))
                    {
                        AppendKanjiBytes(content, bits);
                    }
                    else
                    {
                        throw new StiQRCodeException("Invalid mode: " + mode);
                    }
                }

                private static void AppendNumericBytes(String content, BitVector bits)
                {
                    int length = content.Length;
                    int i = 0;
                    while (i < length)
                    {
                        int num1 = content[i] - '0';
                        if (i + 2 < length)
                        {
                            int num2 = content[i + 1] - '0';
                            int num3 = content[i + 2] - '0';
                            bits.AppendBits(num1 * 100 + num2 * 10 + num3, 10);
                            i += 3;
                        }
                        else if (i + 1 < length)
                        {
                            int num2 = content[i + 1] - '0';
                            bits.AppendBits(num1 * 10 + num2, 7);
                            i += 2;
                        }
                        else
                        {
                            bits.AppendBits(num1, 4);
                            i++;
                        }
                    }
                }

                private static void AppendAlphanumericBytes(string content, BitVector bits, bool gs1)
                {
                    if (gs1)
                    {
                        content = content.Replace("%", "%%").Replace(new string((char)BarcodeCommandCode.Fnc1, 1), "%");
                    }

                    int length = content.Length;
                    int i = 0;
                    while (i < length)
                    {
                        int code1 = GetAlphanumericCode(content[i]);
                        if (code1 == -1)
                        {
                            throw new StiQRCodeException();
                        }
                        if (i + 1 < length)
                        {
                            int code2 = GetAlphanumericCode(content[i + 1]);
                            if (code2 == -1)
                            {
                                throw new StiQRCodeException();
                            }
                            bits.AppendBits(code1 * 45 + code2, 11);
                            i += 2;
                        }
                        else
                        {
                            bits.AppendBits(code1, 6);
                            i++;
                        }
                    }
                }

                private static void Append8BitBytes(string content, BitVector bits, string encoding, bool gs1)
                {
                    if (gs1)
                    {
                        content = content.Replace((char)BarcodeCommandCode.Fnc1, (char)0x1D);
                        encoding = "Shift_JIS";
                    }

                    byte[] bytes;
                    try
                    {
                        bytes = Encoding.GetEncoding(encoding).GetBytes(content);
                    }
                    catch (Exception uee)
                    {
                        throw new StiQRCodeException(uee.Message);
                    }
                    if ((encoding == BYTE_MODE_UTF8) && StiOptions.Engine.BarcodeQRCodeAllowUnicodeBOM)
                    {
                        bits.AppendBits(0xEF, 8);
                        bits.AppendBits(0xBB, 8);
                        bits.AppendBits(0xBF, 8);
                    }
                    for (int i = 0; i < bytes.Length; ++i)
                    {
                        bits.AppendBits(bytes[i], 8);
                    }
                }

                private static void AppendKanjiBytes(String content, BitVector bits)
                {
                    byte[] bytes;
                    try
                    {
                        bytes = Encoding.GetEncoding("Shift_JIS").GetBytes(content);
                    }
                    catch (Exception uee)
                    {
                        throw new StiQRCodeException(uee.Message);
                    }
                    int length = bytes.Length;
                    for (int i = 0; i < length; i += 2)
                    {
                        int byte1 = bytes[i] & 0xFF;
                        int byte2 = bytes[i + 1] & 0xFF;
                        int code = (byte1 << 8) | byte2;
                        int subtracted = -1;
                        if (code >= 0x8140 && code <= 0x9ffc)
                        {
                            subtracted = code - 0x8140;
                        }
                        else if (code >= 0xe040 && code <= 0xebbf)
                        {
                            subtracted = code - 0xc140;
                        }
                        if (subtracted == -1)
                        {
                            throw new StiQRCodeException("Invalid byte sequence");
                        }
                        int encoded = ((subtracted >> 8) * 0xc0) + (subtracted & 0xff);
                        bits.AppendBits(encoded, 13);
                    }
                }

                private static void AppendECI(CharacterSetECI eci, BitVector bits)
                {
                    bits.AppendBits(Mode.ECI.GetBits(), 4);
                    bits.AppendBits(eci.GetValue(), 8);
                }
            }
            #endregion

            #region ErrorCorrectionLevel
            public sealed class ErrorCorrectionLevel
            {
                public static readonly ErrorCorrectionLevel L = new ErrorCorrectionLevel(0, 0x01, "L");
                public static readonly ErrorCorrectionLevel M = new ErrorCorrectionLevel(1, 0x00, "M");
                public static readonly ErrorCorrectionLevel Q = new ErrorCorrectionLevel(2, 0x03, "Q");
                public static readonly ErrorCorrectionLevel H = new ErrorCorrectionLevel(3, 0x02, "H");

                private static readonly ErrorCorrectionLevel[] FOR_BITS = { M, L, H, Q };

                private int ordinal;
                private int bits;
                private string name;

                private ErrorCorrectionLevel(int ordinal, int bits, String name)
                {
                    this.ordinal = ordinal;
                    this.bits = bits;
                    this.name = name;
                }

                public int Ordinal() => ordinal;

                public int GetBits() => bits;

                public string GetName() => name;

                public override string ToString() => name;

                public static ErrorCorrectionLevel ForBits(int bits)
                {
                    if (bits < 0 || bits >= FOR_BITS.Length)
                    {
                        throw new IndexOutOfRangeException();
                    }
                    return FOR_BITS[bits];
                }
            }
            #endregion

            #region ByteMatrix
            internal sealed class ByteMatrix
            {
                private sbyte[][] bytes;
                private int width;
                private int height;

                public ByteMatrix(int width, int height)
                {
                    bytes = new sbyte[height][];
                    for (int k = 0; k < height; ++k)
                    {
                        bytes[k] = new sbyte[width];
                    }
                    this.width = width;
                    this.height = height;
                }

                public int GetHeight() => height;

                public int GetWidth() => width;

                public sbyte Get(int x, int y) => bytes[y][x];

                public sbyte[][] GetArray() => bytes;

                public void Set(int x, int y, sbyte value)
                {
                    bytes[y][x] = value;
                }

                public void Set(int x, int y, int value)
                {
                    bytes[y][x] = (sbyte)value;
                }

                public void Clear(sbyte value)
                {
                    for (int y = 0; y < height; ++y)
                    {
                        for (int x = 0; x < width; ++x)
                        {
                            bytes[y][x] = value;
                        }
                    }
                }

            }
            #endregion

            #region ByteArray
            public sealed class ByteArray
            {
                private const int INITIAL_SIZE = 32;

                private byte[] bytes;
                private int size;

                public ByteArray()
                {
                    bytes = null;
                    size = 0;
                }

                public ByteArray(int size)
                {
                    bytes = new byte[size];
                    this.size = size;
                }

                public ByteArray(byte[] byteArray)
                {
                    bytes = byteArray;
                    size = bytes.Length;
                }

                public int At(int index)
                {
                    return bytes[index] & 0xff;
                }

                public void Set(int index, int value)
                {
                    bytes[index] = (byte)value;
                }

                public int Size() => size;

                public bool IsEmpty() => size == 0;

                public void AppendByte(int value)
                {
                    if (size == 0 || size >= bytes.Length)
                    {
                        int newSize = Math.Max(INITIAL_SIZE, size << 1);
                        Reserve(newSize);
                    }
                    bytes[size] = (byte)value;
                    size++;
                }

                public void Reserve(int capacity)
                {
                    if (bytes == null || bytes.Length < capacity)
                    {
                        var newArray = new byte[capacity];
                        if (bytes != null)
                        {
                            Array.Copy(bytes, 0, newArray, 0, bytes.Length);
                        }
                        bytes = newArray;
                    }
                }

                public void Set(byte[] source, int offset, int count)
                {
                    bytes = new byte[count];
                    size = count;
                    for (int x = 0; x < count; x++)
                    {
                        bytes[x] = source[offset + x];
                    }
                }

            }
            #endregion

            #region BlockPair
            public sealed class BlockPair
            {
                private ByteArray dataBytes;
                private ByteArray errorCorrectionBytes;

                internal BlockPair(ByteArray data, ByteArray errorCorrection)
                {
                    dataBytes = data;
                    errorCorrectionBytes = errorCorrection;
                }

                public ByteArray GetDataBytes() => dataBytes;

                public ByteArray GetErrorCorrectionBytes() => errorCorrectionBytes;
            }
            #endregion

            #region BitVector
            public sealed class BitVector
            {
                private int sizeInBits;
                private byte[] array;
                private const int DEFAULT_SIZE_IN_BYTES = 32;

                public BitVector()
                {
                    sizeInBits = 0;
                    array = new byte[DEFAULT_SIZE_IN_BYTES];
                }

                public int At(int index)
                {
                    if (index < 0 || index >= sizeInBits)
                    {
                        throw new IndexOutOfRangeException("Bad index: " + index);
                    }
                    int value = array[index >> 3] & 0xff;
                    return (value >> (7 - (index & 0x7))) & 1;
                }

                public int Size() => sizeInBits;

                public int SizeInBytes() => (sizeInBits + 7) >> 3;

                public void AppendBit(int bit)
                {
                    if (!(bit == 0 || bit == 1))
                    {
                        throw new StiQRCodeException("Bad bit");
                    }
                    int numBitsInLastByte = sizeInBits & 0x7;
                    if (numBitsInLastByte == 0)
                    {
                        AppendByte(0);
                        sizeInBits -= 8;
                    }
                    array[sizeInBits >> 3] |= (byte)(bit << (7 - numBitsInLastByte));
                    ++sizeInBits;
                }

                public void AppendBits(int value, int numBits)
                {
                    if (numBits < 0 || numBits > 32)
                    {
                        throw new StiQRCodeException("Num bits must be between 0 and 32");
                    }
                    int numBitsLeft = numBits;
                    while (numBitsLeft > 0)
                    {
                        if ((sizeInBits & 0x7) == 0 && numBitsLeft >= 8)
                        {
                            int newByte = (value >> (numBitsLeft - 8)) & 0xff;
                            AppendByte(newByte);
                            numBitsLeft -= 8;
                        }
                        else
                        {
                            int bit = (value >> (numBitsLeft - 1)) & 1;
                            AppendBit(bit);
                            --numBitsLeft;
                        }
                    }
                }

                public void AppendBitVector(BitVector bits)
                {
                    int size = bits.Size();
                    for (int i = 0; i < size; ++i)
                    {
                        AppendBit(bits.At(i));
                    }
                }

                public void Xor(BitVector other)
                {
                    if (sizeInBits != other.Size())
                    {
                        throw new StiQRCodeException("BitVector sizes don't match");
                    }
                    int sizeInBytes = (sizeInBits + 7) >> 3;
                    for (int i = 0; i < sizeInBytes; ++i)
                    {
                        array[i] ^= other.array[i];
                    }
                }

                public override string ToString()
                {
                    var result = new StringBuilder(sizeInBits);
                    for (int i = 0; i < sizeInBits; ++i)
                    {
                        if (At(i) == 0)
                        {
                            result.Append('0');
                        }
                        else if (At(i) == 1)
                        {
                            result.Append('1');
                        }
                        else
                        {
                            throw new StiQRCodeException("Byte isn't 0 or 1");
                        }
                    }
                    return result.ToString();
                }

                public byte[] GetArray() => array;

                private void AppendByte(int value)
                {
                    if ((sizeInBits >> 3) == array.Length)
                    {
                        var newArray = new byte[(array.Length << 1)];
                        Array.Copy(array, 0, newArray, 0, array.Length);
                        array = newArray;
                    }
                    array[sizeInBits >> 3] = (byte)value;
                    sizeInBits += 8;
                }
            }
            #endregion

            #region Version
            public sealed class Version
            {
                private static readonly int[] VERSION_DECODE_INFO =
                {
                  0x07C94, 0x085BC, 0x09A99, 0x0A4D3, 0x0BBF6,
                  0x0C762, 0x0D847, 0x0E60D, 0x0F928, 0x10B78,
                  0x1145D, 0x12A17, 0x13532, 0x149A6, 0x15683,
                  0x168C9, 0x177EC, 0x18EC4, 0x191E1, 0x1AFAB,
                  0x1B08E, 0x1CC1A, 0x1D33F, 0x1ED75, 0x1F250,
                  0x209D5, 0x216F0, 0x228BA, 0x2379F, 0x24B0B,
                  0x2542E, 0x26A64, 0x27541, 0x28C69
                };

                private static readonly Version[] VERSIONS = BuildVersions();

                private int versionNumber;
                private int[] alignmentPatternCenters;
                private ECBlocks[] ecBlocks;
                private int totalCodewords;

                private Version(int versionNumber,
                                int[] alignmentPatternCenters,
                                ECBlocks ecBlocks1,
                                ECBlocks ecBlocks2,
                                ECBlocks ecBlocks3,
                                ECBlocks ecBlocks4)
                {
                    this.versionNumber = versionNumber;
                    this.alignmentPatternCenters = alignmentPatternCenters;
                    this.ecBlocks = new ECBlocks[] { ecBlocks1, ecBlocks2, ecBlocks3, ecBlocks4 };
                    int total = 0;
                    int ecCodewords = ecBlocks1.GetECCodewordsPerBlock();
                    var ecbArray = ecBlocks1.GetECBlocks();
                    for (int i = 0; i < ecbArray.Length; i++)
                    {
                        var ecBlock = ecbArray[i];
                        total += ecBlock.GetCount() * (ecBlock.GetDataCodewords() + ecCodewords);
                    }
                    this.totalCodewords = total;
                }

                public int GetVersionNumber() => versionNumber;

                public int[] GetAlignmentPatternCenters() => alignmentPatternCenters;

                public int GetTotalCodewords() => totalCodewords;

                public int GetDimensionForVersion() => 17 + 4 * versionNumber;

                public ECBlocks GetECBlocksForLevel(ErrorCorrectionLevel ecLevel) => ecBlocks[ecLevel.Ordinal()];

                public static Version GetProvisionalVersionForDimension(int dimension)
                {
                    if (dimension % 4 != 1)
                    {
                        throw new StiQRCodeException();
                    }
                    try
                    {
                        return GetVersionForNumber((dimension - 17) >> 2);
                    }
                    catch (ArgumentException iae)
                    {
                        throw iae;
                    }
                }

                public static Version GetVersionForNumber(int versionNumber)
                {
                    if (versionNumber < 1 || versionNumber > 40)
                    {
                        throw new StiQRCodeException();
                    }
                    return VERSIONS[versionNumber - 1];
                }

                static Version DecodeVersionInformation(int versionBits)
                {
                    int bestDifference = int.MaxValue;
                    int bestVersion = 0;
                    for (int i = 0; i < VERSION_DECODE_INFO.Length; i++)
                    {
                        int targetVersion = VERSION_DECODE_INFO[i];
                        if (targetVersion == versionBits)
                        {
                            return GetVersionForNumber(i + 7);
                        }
                        int bitsDifference = FormatInformation.NumBitsDiffering(versionBits, targetVersion);
                        if (bitsDifference < bestDifference)
                        {
                            bestVersion = i + 7;
                            bestDifference = bitsDifference;
                        }
                    }
                    if (bestDifference <= 3)
                    {
                        return GetVersionForNumber(bestVersion);
                    }
                    return null;
                }


                public sealed class ECBlocks
                {
                    private int ecCodewordsPerBlock;
                    private ECB[] ecBlocks;

                    public ECBlocks(int ecCodewordsPerBlock, ECB ecBlocks)
                    {
                        this.ecCodewordsPerBlock = ecCodewordsPerBlock;
                        this.ecBlocks = new ECB[] { ecBlocks };
                    }

                    public ECBlocks(int ecCodewordsPerBlock, ECB ecBlocks1, ECB ecBlocks2)
                    {
                        this.ecCodewordsPerBlock = ecCodewordsPerBlock;
                        this.ecBlocks = new ECB[] { ecBlocks1, ecBlocks2 };
                    }

                    public int GetECCodewordsPerBlock() => ecCodewordsPerBlock;

                    public int GetNumBlocks()
                    {
                        int total = 0;
                        for (int i = 0; i < ecBlocks.Length; i++)
                        {
                            total += ecBlocks[i].GetCount();
                        }
                        return total;
                    }

                    public int GetTotalECCodewords()
                    {
                        return ecCodewordsPerBlock * GetNumBlocks();
                    }

                    public ECB[] GetECBlocks() => ecBlocks;
                }

                public sealed class ECB
                {
                    private int count;
                    private int dataCodewords;

                    public ECB(int count, int dataCodewords)
                    {
                        this.count = count;
                        this.dataCodewords = dataCodewords;
                    }

                    public int GetCount() => count;

                    public int GetDataCodewords() => dataCodewords;
                }

                public override string ToString()
                {
                    return versionNumber.ToString();
                }

                private static Version[] BuildVersions()
                {
                    return new Version[]{
                        new Version(1, new int[]{},
                            new ECBlocks(7, new ECB(1, 19)),
                            new ECBlocks(10, new ECB(1, 16)),
                            new ECBlocks(13, new ECB(1, 13)),
                            new ECBlocks(17, new ECB(1, 9))),
                        new Version(2, new int[]{6, 18},
                            new ECBlocks(10, new ECB(1, 34)),
                            new ECBlocks(16, new ECB(1, 28)),
                            new ECBlocks(22, new ECB(1, 22)),
                            new ECBlocks(28, new ECB(1, 16))),
                        new Version(3, new int[]{6, 22},
                            new ECBlocks(15, new ECB(1, 55)),
                            new ECBlocks(26, new ECB(1, 44)),
                            new ECBlocks(18, new ECB(2, 17)),
                            new ECBlocks(22, new ECB(2, 13))),
                        new Version(4, new int[]{6, 26},
                            new ECBlocks(20, new ECB(1, 80)),
                            new ECBlocks(18, new ECB(2, 32)),
                            new ECBlocks(26, new ECB(2, 24)),
                            new ECBlocks(16, new ECB(4, 9))),
                        new Version(5, new int[]{6, 30},
                            new ECBlocks(26, new ECB(1, 108)),
                            new ECBlocks(24, new ECB(2, 43)),
                            new ECBlocks(18, new ECB(2, 15), new ECB(2, 16)),
                            new ECBlocks(22, new ECB(2, 11), new ECB(2, 12))),
                        new Version(6, new int[]{6, 34},
                            new ECBlocks(18, new ECB(2, 68)),
                            new ECBlocks(16, new ECB(4, 27)),
                            new ECBlocks(24, new ECB(4, 19)),
                            new ECBlocks(28, new ECB(4, 15))),
                        new Version(7, new int[]{6, 22, 38},
                            new ECBlocks(20, new ECB(2, 78)),
                            new ECBlocks(18, new ECB(4, 31)),
                            new ECBlocks(18, new ECB(2, 14), new ECB(4, 15)),
                            new ECBlocks(26, new ECB(4, 13), new ECB(1, 14))),
                        new Version(8, new int[]{6, 24, 42},
                            new ECBlocks(24, new ECB(2, 97)),
                            new ECBlocks(22, new ECB(2, 38), new ECB(2, 39)),
                            new ECBlocks(22, new ECB(4, 18), new ECB(2, 19)),
                            new ECBlocks(26, new ECB(4, 14), new ECB(2, 15))),
                        new Version(9, new int[]{6, 26, 46},
                            new ECBlocks(30, new ECB(2, 116)),
                            new ECBlocks(22, new ECB(3, 36), new ECB(2, 37)),
                            new ECBlocks(20, new ECB(4, 16), new ECB(4, 17)),
                            new ECBlocks(24, new ECB(4, 12), new ECB(4, 13))),
                        new Version(10, new int[]{6, 28, 50},
                            new ECBlocks(18, new ECB(2, 68), new ECB(2, 69)),
                            new ECBlocks(26, new ECB(4, 43), new ECB(1, 44)),
                            new ECBlocks(24, new ECB(6, 19), new ECB(2, 20)),
                            new ECBlocks(28, new ECB(6, 15), new ECB(2, 16))),
                        new Version(11, new int[]{6, 30, 54},
                            new ECBlocks(20, new ECB(4, 81)),
                            new ECBlocks(30, new ECB(1, 50), new ECB(4, 51)),
                            new ECBlocks(28, new ECB(4, 22), new ECB(4, 23)),
                            new ECBlocks(24, new ECB(3, 12), new ECB(8, 13))),
                        new Version(12, new int[]{6, 32, 58},
                            new ECBlocks(24, new ECB(2, 92), new ECB(2, 93)),
                            new ECBlocks(22, new ECB(6, 36), new ECB(2, 37)),
                            new ECBlocks(26, new ECB(4, 20), new ECB(6, 21)),
                            new ECBlocks(28, new ECB(7, 14), new ECB(4, 15))),
                        new Version(13, new int[]{6, 34, 62},
                            new ECBlocks(26, new ECB(4, 107)),
                            new ECBlocks(22, new ECB(8, 37), new ECB(1, 38)),
                            new ECBlocks(24, new ECB(8, 20), new ECB(4, 21)),
                            new ECBlocks(22, new ECB(12, 11), new ECB(4, 12))),
                        new Version(14, new int[]{6, 26, 46, 66},
                            new ECBlocks(30, new ECB(3, 115), new ECB(1, 116)),
                            new ECBlocks(24, new ECB(4, 40), new ECB(5, 41)),
                            new ECBlocks(20, new ECB(11, 16), new ECB(5, 17)),
                            new ECBlocks(24, new ECB(11, 12), new ECB(5, 13))),
                        new Version(15, new int[]{6, 26, 48, 70},
                            new ECBlocks(22, new ECB(5, 87), new ECB(1, 88)),
                            new ECBlocks(24, new ECB(5, 41), new ECB(5, 42)),
                            new ECBlocks(30, new ECB(5, 24), new ECB(7, 25)),
                            new ECBlocks(24, new ECB(11, 12), new ECB(7, 13))),
                        new Version(16, new int[]{6, 26, 50, 74},
                            new ECBlocks(24, new ECB(5, 98), new ECB(1, 99)),
                            new ECBlocks(28, new ECB(7, 45), new ECB(3, 46)),
                            new ECBlocks(24, new ECB(15, 19), new ECB(2, 20)),
                            new ECBlocks(30, new ECB(3, 15), new ECB(13, 16))),
                        new Version(17, new int[]{6, 30, 54, 78},
                            new ECBlocks(28, new ECB(1, 107), new ECB(5, 108)),
                            new ECBlocks(28, new ECB(10, 46), new ECB(1, 47)),
                            new ECBlocks(28, new ECB(1, 22), new ECB(15, 23)),
                            new ECBlocks(28, new ECB(2, 14), new ECB(17, 15))),
                        new Version(18, new int[]{6, 30, 56, 82},
                            new ECBlocks(30, new ECB(5, 120), new ECB(1, 121)),
                            new ECBlocks(26, new ECB(9, 43), new ECB(4, 44)),
                            new ECBlocks(28, new ECB(17, 22), new ECB(1, 23)),
                            new ECBlocks(28, new ECB(2, 14), new ECB(19, 15))),
                        new Version(19, new int[]{6, 30, 58, 86},
                            new ECBlocks(28, new ECB(3, 113), new ECB(4, 114)),
                            new ECBlocks(26, new ECB(3, 44), new ECB(11, 45)),
                            new ECBlocks(26, new ECB(17, 21), new ECB(4, 22)),
                            new ECBlocks(26, new ECB(9, 13), new ECB(16, 14))),
                        new Version(20, new int[]{6, 34, 62, 90},
                            new ECBlocks(28, new ECB(3, 107), new ECB(5, 108)),
                            new ECBlocks(26, new ECB(3, 41), new ECB(13, 42)),
                            new ECBlocks(30, new ECB(15, 24), new ECB(5, 25)),
                            new ECBlocks(28, new ECB(15, 15), new ECB(10, 16))),
                        new Version(21, new int[]{6, 28, 50, 72, 94},
                            new ECBlocks(28, new ECB(4, 116), new ECB(4, 117)),
                            new ECBlocks(26, new ECB(17, 42)),
                            new ECBlocks(28, new ECB(17, 22), new ECB(6, 23)),
                            new ECBlocks(30, new ECB(19, 16), new ECB(6, 17))),
                        new Version(22, new int[]{6, 26, 50, 74, 98},
                            new ECBlocks(28, new ECB(2, 111), new ECB(7, 112)),
                            new ECBlocks(28, new ECB(17, 46)),
                            new ECBlocks(30, new ECB(7, 24), new ECB(16, 25)),
                            new ECBlocks(24, new ECB(34, 13))),
                        new Version(23, new int[]{6, 30, 54, 74, 102},
                            new ECBlocks(30, new ECB(4, 121), new ECB(5, 122)),
                            new ECBlocks(28, new ECB(4, 47), new ECB(14, 48)),
                            new ECBlocks(30, new ECB(11, 24), new ECB(14, 25)),
                            new ECBlocks(30, new ECB(16, 15), new ECB(14, 16))),
                        new Version(24, new int[]{6, 28, 54, 80, 106},
                            new ECBlocks(30, new ECB(6, 117), new ECB(4, 118)),
                            new ECBlocks(28, new ECB(6, 45), new ECB(14, 46)),
                            new ECBlocks(30, new ECB(11, 24), new ECB(16, 25)),
                            new ECBlocks(30, new ECB(30, 16), new ECB(2, 17))),
                        new Version(25, new int[]{6, 32, 58, 84, 110},
                            new ECBlocks(26, new ECB(8, 106), new ECB(4, 107)),
                            new ECBlocks(28, new ECB(8, 47), new ECB(13, 48)),
                            new ECBlocks(30, new ECB(7, 24), new ECB(22, 25)),
                            new ECBlocks(30, new ECB(22, 15), new ECB(13, 16))),
                        new Version(26, new int[]{6, 30, 58, 86, 114},
                            new ECBlocks(28, new ECB(10, 114), new ECB(2, 115)),
                            new ECBlocks(28, new ECB(19, 46), new ECB(4, 47)),
                            new ECBlocks(28, new ECB(28, 22), new ECB(6, 23)),
                            new ECBlocks(30, new ECB(33, 16), new ECB(4, 17))),
                        new Version(27, new int[]{6, 34, 62, 90, 118},
                            new ECBlocks(30, new ECB(8, 122), new ECB(4, 123)),
                            new ECBlocks(28, new ECB(22, 45), new ECB(3, 46)),
                            new ECBlocks(30, new ECB(8, 23), new ECB(26, 24)),
                            new ECBlocks(30, new ECB(12, 15), new ECB(28, 16))),
                        new Version(28, new int[]{6, 26, 50, 74, 98, 122},
                            new ECBlocks(30, new ECB(3, 117), new ECB(10, 118)),
                            new ECBlocks(28, new ECB(3, 45), new ECB(23, 46)),
                            new ECBlocks(30, new ECB(4, 24), new ECB(31, 25)),
                            new ECBlocks(30, new ECB(11, 15), new ECB(31, 16))),
                        new Version(29, new int[]{6, 30, 54, 78, 102, 126},
                            new ECBlocks(30, new ECB(7, 116), new ECB(7, 117)),
                            new ECBlocks(28, new ECB(21, 45), new ECB(7, 46)),
                            new ECBlocks(30, new ECB(1, 23), new ECB(37, 24)),
                            new ECBlocks(30, new ECB(19, 15), new ECB(26, 16))),
                        new Version(30, new int[]{6, 26, 52, 78, 104, 130},
                            new ECBlocks(30, new ECB(5, 115), new ECB(10, 116)),
                            new ECBlocks(28, new ECB(19, 47), new ECB(10, 48)),
                            new ECBlocks(30, new ECB(15, 24), new ECB(25, 25)),
                            new ECBlocks(30, new ECB(23, 15), new ECB(25, 16))),
                        new Version(31, new int[]{6, 30, 56, 82, 108, 134},
                            new ECBlocks(30, new ECB(13, 115), new ECB(3, 116)),
                            new ECBlocks(28, new ECB(2, 46), new ECB(29, 47)),
                            new ECBlocks(30, new ECB(42, 24), new ECB(1, 25)),
                            new ECBlocks(30, new ECB(23, 15), new ECB(28, 16))),
                        new Version(32, new int[]{6, 34, 60, 86, 112, 138},
                            new ECBlocks(30, new ECB(17, 115)),
                            new ECBlocks(28, new ECB(10, 46), new ECB(23, 47)),
                            new ECBlocks(30, new ECB(10, 24), new ECB(35, 25)),
                            new ECBlocks(30, new ECB(19, 15), new ECB(35, 16))),
                        new Version(33, new int[]{6, 30, 58, 86, 114, 142},
                            new ECBlocks(30, new ECB(17, 115), new ECB(1, 116)),
                            new ECBlocks(28, new ECB(14, 46), new ECB(21, 47)),
                            new ECBlocks(30, new ECB(29, 24), new ECB(19, 25)),
                            new ECBlocks(30, new ECB(11, 15), new ECB(46, 16))),
                        new Version(34, new int[]{6, 34, 62, 90, 118, 146},
                            new ECBlocks(30, new ECB(13, 115), new ECB(6, 116)),
                            new ECBlocks(28, new ECB(14, 46), new ECB(23, 47)),
                            new ECBlocks(30, new ECB(44, 24), new ECB(7, 25)),
                            new ECBlocks(30, new ECB(59, 16), new ECB(1, 17))),
                        new Version(35, new int[]{6, 30, 54, 78, 102, 126, 150},
                            new ECBlocks(30, new ECB(12, 121), new ECB(7, 122)),
                            new ECBlocks(28, new ECB(12, 47), new ECB(26, 48)),
                            new ECBlocks(30, new ECB(39, 24), new ECB(14, 25)),
                            new ECBlocks(30, new ECB(22, 15), new ECB(41, 16))),
                        new Version(36, new int[]{6, 24, 50, 76, 102, 128, 154},
                            new ECBlocks(30, new ECB(6, 121), new ECB(14, 122)),
                            new ECBlocks(28, new ECB(6, 47), new ECB(34, 48)),
                            new ECBlocks(30, new ECB(46, 24), new ECB(10, 25)),
                            new ECBlocks(30, new ECB(2, 15), new ECB(64, 16))),
                        new Version(37, new int[]{6, 28, 54, 80, 106, 132, 158},
                            new ECBlocks(30, new ECB(17, 122), new ECB(4, 123)),
                            new ECBlocks(28, new ECB(29, 46), new ECB(14, 47)),
                            new ECBlocks(30, new ECB(49, 24), new ECB(10, 25)),
                            new ECBlocks(30, new ECB(24, 15), new ECB(46, 16))),
                        new Version(38, new int[]{6, 32, 58, 84, 110, 136, 162},
                            new ECBlocks(30, new ECB(4, 122), new ECB(18, 123)),
                            new ECBlocks(28, new ECB(13, 46), new ECB(32, 47)),
                            new ECBlocks(30, new ECB(48, 24), new ECB(14, 25)),
                            new ECBlocks(30, new ECB(42, 15), new ECB(32, 16))),
                        new Version(39, new int[]{6, 26, 54, 82, 110, 138, 166},
                            new ECBlocks(30, new ECB(20, 117), new ECB(4, 118)),
                            new ECBlocks(28, new ECB(40, 47), new ECB(7, 48)),
                            new ECBlocks(30, new ECB(43, 24), new ECB(22, 25)),
                            new ECBlocks(30, new ECB(10, 15), new ECB(67, 16))),
                        new Version(40, new int[]{6, 30, 58, 86, 114, 142, 170},
                            new ECBlocks(30, new ECB(19, 118), new ECB(6, 119)),
                            new ECBlocks(28, new ECB(18, 47), new ECB(31, 48)),
                            new ECBlocks(30, new ECB(34, 24), new ECB(34, 25)),
                            new ECBlocks(30, new ECB(20, 15), new ECB(61, 16)))
                    };
                }
            }
            #endregion

            #region StiQRCodeException
            public sealed class StiQRCodeException : Exception
            {
                public StiQRCodeException()
                    : base()
                {
                }

                public StiQRCodeException(string message)
                    : base(message)
                {
                }
            }
            #endregion

            #region CharacterSetECI
            public class CharacterSetECI
            {
                private static object lockNAME_TO_ECI = new object();
                private static Dictionary<string, CharacterSetECI> name_to_eci = null;
                public static Dictionary<string, CharacterSetECI> NAME_TO_ECI
                {
                    get
                    {
                        if (name_to_eci == null)
                        {
                            lock (lockNAME_TO_ECI)
                            {
                                Initialize();
                            }
                        }
                        return name_to_eci;
                    }
                    set
                    {
                        name_to_eci = value;
                    }
                }

                private static void Initialize()
                {
                    var n = new Dictionary<string, CharacterSetECI>();
                    //AddCharacterSet(0, "Cp437", n);
                    //AddCharacterSet(1, "ISO-8859-1", n);
                    AddCharacterSet(2, "Cp437", n);
                    AddCharacterSet(3, "ISO-8859-1", n);
                    AddCharacterSet(4, "ISO-8859-2", n);
                    AddCharacterSet(5, "ISO-8859-3", n);
                    AddCharacterSet(6, "ISO-8859-4", n);
                    AddCharacterSet(7, "ISO-8859-5", n);
                    AddCharacterSet(8, "ISO-8859-6", n);
                    AddCharacterSet(9, "ISO-8859-7", n);
                    AddCharacterSet(10, "ISO-8859-8", n);
                    AddCharacterSet(11, "ISO-8859-9", n);
                    //AddCharacterSet(12, "ISO-8859-10", n);
                    AddCharacterSet(13, "ISO-8859-11", n);
                    AddCharacterSet(15, "ISO-8859-13", n);
                    //AddCharacterSet(16, "ISO-8859-14", n);
                    AddCharacterSet(17, "ISO-8859-15", n);
                    //AddCharacterSet(18, "ISO-8859-16", n);
                    AddCharacterSet(20, "Shift_JIS", n);
                    AddCharacterSet(21, "Windows-1250", n);
                    AddCharacterSet(22, "Windows-1251", n);
                    AddCharacterSet(23, "Windows-1252", n);
                    AddCharacterSet(24, "Windows-1256", n);
                    //25 	UTF-16
                    AddCharacterSet(26, "UTF-8", n);
                    //27 	ISO-646-US
                    //28 	Big5
                    //29 	GB 2312
                    //30 	KSC-5601

                    name_to_eci = n;
                }

                private string encodingName;
                private int value;

                private CharacterSetECI(int value, string encodingName)
                {
                    this.value = value;
                    this.encodingName = encodingName;
                }

                public string GetEncodingName() => encodingName;

                public int GetValue() => value;

                private static void AddCharacterSet(int value, String encodingName, Dictionary<String, CharacterSetECI> n)
                {
                    var eci = new CharacterSetECI(value, encodingName);
                    n[encodingName] = eci;
                }

                public static CharacterSetECI GetCharacterSetECIByName(String name)
                {
                    CharacterSetECI c;
                    NAME_TO_ECI.TryGetValue(name, out c);
                    return c;
                }

                public static string GetEncodingByNumber(int number, string defaultEncoding)
                {
                    foreach (var pair in NAME_TO_ECI)
                    {
                        if (pair.Value.value == number) return pair.Value.encodingName;
                    }
                    return defaultEncoding;
                }
            }
            #endregion

            #region FormatInformation
            public sealed class FormatInformation
            {

                private const int FORMAT_INFO_MASK_QR = 0x5412;

                private static readonly int[][] FORMAT_INFO_DECODE_LOOKUP = {
                    new int[]{0x5412, 0x00},
                    new int[]{0x5125, 0x01},
                    new int[]{0x5E7C, 0x02},
                    new int[]{0x5B4B, 0x03},
                    new int[]{0x45F9, 0x04},
                    new int[]{0x40CE, 0x05},
                    new int[]{0x4F97, 0x06},
                    new int[]{0x4AA0, 0x07},
                    new int[]{0x77C4, 0x08},
                    new int[]{0x72F3, 0x09},
                    new int[]{0x7DAA, 0x0A},
                    new int[]{0x789D, 0x0B},
                    new int[]{0x662F, 0x0C},
                    new int[]{0x6318, 0x0D},
                    new int[]{0x6C41, 0x0E},
                    new int[]{0x6976, 0x0F},
                    new int[]{0x1689, 0x10},
                    new int[]{0x13BE, 0x11},
                    new int[]{0x1CE7, 0x12},
                    new int[]{0x19D0, 0x13},
                    new int[]{0x0762, 0x14},
                    new int[]{0x0255, 0x15},
                    new int[]{0x0D0C, 0x16},
                    new int[]{0x083B, 0x17},
                    new int[]{0x355F, 0x18},
                    new int[]{0x3068, 0x19},
                    new int[]{0x3F31, 0x1A},
                    new int[]{0x3A06, 0x1B},
                    new int[]{0x24B4, 0x1C},
                    new int[]{0x2183, 0x1D},
                    new int[]{0x2EDA, 0x1E},
                    new int[]{0x2BED, 0x1F}
                };

                private static readonly int[] BITS_SET_IN_HALF_BYTE =
                    { 0, 1, 1, 2, 1, 2, 2, 3, 1, 2, 2, 3, 2, 3, 3, 4 };

                private ErrorCorrectionLevel errorCorrectionLevel;
                private byte dataMask;

                private FormatInformation(int formatInfo)
                {
                    errorCorrectionLevel = StiQRCode.ErrorCorrectionLevel.ForBits((formatInfo >> 3) & 0x03);
                    dataMask = (byte)(formatInfo & 0x07);
                }

                public static int NumBitsDiffering(int a, int b)
                {
                    a ^= b;
                    return BITS_SET_IN_HALF_BYTE[a & 0x0F] +
                        BITS_SET_IN_HALF_BYTE[(a >> 4 & 0x0F)] +
                        BITS_SET_IN_HALF_BYTE[(a >> 8 & 0x0F)] +
                        BITS_SET_IN_HALF_BYTE[(a >> 12 & 0x0F)] +
                        BITS_SET_IN_HALF_BYTE[(a >> 16 & 0x0F)] +
                        BITS_SET_IN_HALF_BYTE[(a >> 20 & 0x0F)] +
                        BITS_SET_IN_HALF_BYTE[(a >> 24 & 0x0F)] +
                        BITS_SET_IN_HALF_BYTE[(a >> 28 & 0x0F)];
                }

                public static FormatInformation DecodeFormatInformation(int maskedFormatInfo1, int maskedFormatInfo2)
                {
                    var formatInfo = DoDecodeFormatInformation(maskedFormatInfo1, maskedFormatInfo2);
                    if (formatInfo != null)
                    {
                        return formatInfo;
                    }
                    return DoDecodeFormatInformation(maskedFormatInfo1 ^ FORMAT_INFO_MASK_QR,
                                                     maskedFormatInfo2 ^ FORMAT_INFO_MASK_QR);
                }

                private static FormatInformation DoDecodeFormatInformation(int maskedFormatInfo1, int maskedFormatInfo2)
                {
                    int bestDifference = int.MaxValue;
                    int bestFormatInfo = 0;
                    for (int i = 0; i < FORMAT_INFO_DECODE_LOOKUP.GetLength(0); i++)
                    {
                        var decodeInfo = FORMAT_INFO_DECODE_LOOKUP[i];
                        int targetInfo = decodeInfo[0];
                        if (targetInfo == maskedFormatInfo1 || targetInfo == maskedFormatInfo2)
                        {
                            return new FormatInformation(decodeInfo[1]);
                        }
                        int bitsDifference = NumBitsDiffering(maskedFormatInfo1, targetInfo);
                        if (bitsDifference < bestDifference)
                        {
                            bestFormatInfo = decodeInfo[1];
                            bestDifference = bitsDifference;
                        }
                        if (maskedFormatInfo1 != maskedFormatInfo2)
                        {
                            bitsDifference = NumBitsDiffering(maskedFormatInfo2, targetInfo);
                            if (bitsDifference < bestDifference)
                            {
                                bestFormatInfo = decodeInfo[1];
                                bestDifference = bitsDifference;
                            }
                        }
                    }
                    if (bestDifference <= 3)
                    {
                        return new FormatInformation(bestFormatInfo);
                    }
                    return null;
                }

                public ErrorCorrectionLevel GetErrorCorrectionLevel() => errorCorrectionLevel;

                public byte GetDataMask() => dataMask;

                public override int GetHashCode() => base.GetHashCode();

                public override bool Equals(object o)
                {
                    if (!(o is FormatInformation))
                    {
                        return false;
                    }

                    var other = (FormatInformation)o;
                    return this.errorCorrectionLevel == other.errorCorrectionLevel &&
                        this.dataMask == other.dataMask;
                }
            }
            #endregion
 
            #region Mode
            public sealed class Mode
            {
                public static readonly Mode TERMINATOR = new Mode(new int[] { 0, 0, 0 }, 0x00, "TERMINATOR"); // Not really a mode...
                public static readonly Mode NUMERIC = new Mode(new int[] { 10, 12, 14 }, 0x01, "NUMERIC");
                public static readonly Mode ALPHANUMERIC = new Mode(new int[] { 9, 11, 13 }, 0x02, "ALPHANUMERIC");
                public static readonly Mode STRUCTURED_APPEND = new Mode(new int[] { 0, 0, 0 }, 0x03, "STRUCTURED_APPEND"); // Not supported
                public static readonly Mode BYTE = new Mode(new int[] { 8, 16, 16 }, 0x04, "BYTE");
                public static readonly Mode ECI = new Mode(null, 0x07, "ECI"); // character counts don't apply
                public static readonly Mode KANJI = new Mode(new int[] { 8, 10, 12 }, 0x08, "KANJI");
                public static readonly Mode FNC1_FIRST_POSITION = new Mode(null, 0x05, "FNC1_FIRST_POSITION");
                public static readonly Mode FNC1_SECOND_POSITION = new Mode(null, 0x09, "FNC1_SECOND_POSITION");

                private int[] characterCountBitsForVersions;
                private int bits;
                private string name;

                private Mode(int[] characterCountBitsForVersions, int bits, String name)
                {
                    this.characterCountBitsForVersions = characterCountBitsForVersions;
                    this.bits = bits;
                    this.name = name;
                }

                public static Mode ForBits(int bits)
                {
                    switch (bits)
                    {
                        case 0x0:
                            return TERMINATOR;
                        case 0x1:
                            return NUMERIC;
                        case 0x2:
                            return ALPHANUMERIC;
                        case 0x3:
                            return STRUCTURED_APPEND;
                        case 0x4:
                            return BYTE;
                        case 0x5:
                            return FNC1_FIRST_POSITION;
                        case 0x7:
                            return ECI;
                        case 0x8:
                            return KANJI;
                        case 0x9:
                            return FNC1_SECOND_POSITION;
                        default:
                            throw new StiQRCodeException();
                    }
                }

                public int GetCharacterCountBits(Version version)
                {
                    if (characterCountBitsForVersions == null)
                    {
                        throw new StiQRCodeException("Character count doesn't apply to this mode");
                    }
                    int number = version.GetVersionNumber();
                    int offset;
                    if (number <= 9)
                    {
                        offset = 0;
                    }
                    else if (number <= 26)
                    {
                        offset = 1;
                    }
                    else
                    {
                        offset = 2;
                    }
                    return characterCountBitsForVersions[offset];
                }

                public int GetBits() => bits;

                public string GetName() => name;

                public override string ToString() => name;
            }
            #endregion

            #region MaskUtil
            internal sealed class MaskUtil
            {
                private MaskUtil()
                {
                    // do nothing
                }

                public static int ApplyMaskPenaltyRule1(ByteMatrix matrix)
                {
                    return ApplyMaskPenaltyRule1Internal(matrix, true) + ApplyMaskPenaltyRule1Internal(matrix, false);
                }

                public static int ApplyMaskPenaltyRule2(ByteMatrix matrix)
                {
                    int penalty = 0;
                    var array = matrix.GetArray();
                    int width = matrix.GetWidth();
                    int height = matrix.GetHeight();
                    for (int y = 0; y < height - 1; ++y)
                    {
                        for (int x = 0; x < width - 1; ++x)
                        {
                            int value = array[y][x];
                            if (value == array[y][x + 1] && value == array[y + 1][x] && value == array[y + 1][x + 1])
                            {
                                penalty += 3;
                            }
                        }
                    }
                    return penalty;
                }

                public static int ApplyMaskPenaltyRule3(ByteMatrix matrix)
                {
                    int penalty = 0;
                    var array = matrix.GetArray();
                    int width = matrix.GetWidth();
                    int height = matrix.GetHeight();
                    for (int y = 0; y < height; ++y)
                    {
                        for (int x = 0; x < width; ++x)
                        {
                            if (x + 6 < width &&
                                array[y][x] == 1 &&
                                array[y][x + 1] == 0 &&
                                array[y][x + 2] == 1 &&
                                array[y][x + 3] == 1 &&
                                array[y][x + 4] == 1 &&
                                array[y][x + 5] == 0 &&
                                array[y][x + 6] == 1 &&
                                ((x + 10 < width &&
                                    array[y][x + 7] == 0 &&
                                    array[y][x + 8] == 0 &&
                                    array[y][x + 9] == 0 &&
                                    array[y][x + 10] == 0) ||
                                    (x - 4 >= 0 &&
                                        array[y][x - 1] == 0 &&
                                        array[y][x - 2] == 0 &&
                                        array[y][x - 3] == 0 &&
                                        array[y][x - 4] == 0)))
                            {
                                penalty += 40;
                            }
                            if (y + 6 < height &&
                                array[y][x] == 1 &&
                                array[y + 1][x] == 0 &&
                                array[y + 2][x] == 1 &&
                                array[y + 3][x] == 1 &&
                                array[y + 4][x] == 1 &&
                                array[y + 5][x] == 0 &&
                                array[y + 6][x] == 1 &&
                                ((y + 10 < height &&
                                    array[y + 7][x] == 0 &&
                                    array[y + 8][x] == 0 &&
                                    array[y + 9][x] == 0 &&
                                    array[y + 10][x] == 0) ||
                                    (y - 4 >= 0 &&
                                        array[y - 1][x] == 0 &&
                                        array[y - 2][x] == 0 &&
                                        array[y - 3][x] == 0 &&
                                        array[y - 4][x] == 0)))
                            {
                                penalty += 40;
                            }
                        }
                    }
                    return penalty;
                }

                public static int ApplyMaskPenaltyRule4(ByteMatrix matrix)
                {
                    int numDarkCells = 0;
                    var array = matrix.GetArray();
                    int width = matrix.GetWidth();
                    int height = matrix.GetHeight();
                    for (int y = 0; y < height; ++y)
                    {
                        for (int x = 0; x < width; ++x)
                        {
                            if (array[y][x] == 1)
                            {
                                numDarkCells += 1;
                            }
                        }
                    }
                    int numTotalCells = matrix.GetHeight() * matrix.GetWidth();
                    double darkRatio = (double)numDarkCells / numTotalCells;
                    return Math.Abs((int)(darkRatio * 100 - 50)) / 5 * 10;
                }

                public static bool GetDataMaskBit(int maskPattern, int x, int y)
                {
                    //if (!StiQRCode.IsValidMaskPattern(maskPattern))
                    //{
                    //    throw new ArgumentException("Invalid mask pattern");
                    //}
                    int intermediate, temp;
                    switch (maskPattern)
                    {
                        case 0:
                            intermediate = (y + x) & 0x1;
                            break;
                        case 1:
                            intermediate = y & 0x1;
                            break;
                        case 2:
                            intermediate = x % 3;
                            break;
                        case 3:
                            intermediate = (y + x) % 3;
                            break;
                        case 4:
                            intermediate = ((y >> 1) + (x / 3)) & 0x1;
                            break;
                        case 5:
                            temp = y * x;
                            intermediate = (temp & 0x1) + (temp % 3);
                            break;
                        case 6:
                            temp = y * x;
                            intermediate = (((temp & 0x1) + (temp % 3)) & 0x1);
                            break;
                        case 7:
                            temp = y * x;
                            intermediate = (((temp % 3) + ((y + x) & 0x1)) & 0x1);
                            break;
                        default:
                            throw new StiQRCodeException("Invalid mask pattern: " + maskPattern);
                    }
                    return intermediate == 0;
                }

                private static int ApplyMaskPenaltyRule1Internal(ByteMatrix matrix, bool isHorizontal)
                {
                    int penalty = 0;
                    int numSameBitCells = 0;
                    int prevBit = -1;
                    int iLimit = isHorizontal ? matrix.GetHeight() : matrix.GetWidth();
                    int jLimit = isHorizontal ? matrix.GetWidth() : matrix.GetHeight();
                    var array = matrix.GetArray();
                    for (int i = 0; i < iLimit; ++i)
                    {
                        for (int j = 0; j < jLimit; ++j)
                        {
                            int bit = isHorizontal ? array[i][j] : array[j][i];
                            if (bit == prevBit)
                            {
                                numSameBitCells += 1;
                                if (numSameBitCells == 5)
                                {
                                    penalty += 3;
                                }
                                else if (numSameBitCells > 5)
                                {
                                    penalty += 1;
                                }
                            }
                            else
                            {
                                numSameBitCells = 1;  
                                prevBit = bit;
                            }
                        }
                        numSameBitCells = 0; 
                    }
                    return penalty;
                }
            }
            #endregion

            #region MatrixUtil
            internal sealed class MatrixUtil
            {

                private MatrixUtil()
                {
                    // do nothing
                }

                private static readonly int[][] POSITION_DETECTION_PATTERN =  {
                    new int[]{1, 1, 1, 1, 1, 1, 1},
                    new int[]{1, 0, 0, 0, 0, 0, 1},
                    new int[]{1, 0, 2, 2, 2, 0, 1},
                    new int[]{1, 0, 2, 2, 2, 0, 1},
                    new int[]{1, 0, 2, 2, 2, 0, 1},
                    new int[]{1, 0, 0, 0, 0, 0, 1},
                    new int[]{1, 1, 1, 1, 1, 1, 1}
                };

                private static readonly int[][] HORIZONTAL_SEPARATION_PATTERN = {
                    new int[]{0, 0, 0, 0, 0, 0, 0, 0}
                };

                private static readonly int[][] VERTICAL_SEPARATION_PATTERN = {
                    new int[]{0}, new int[]{0}, new int[]{0}, new int[]{0}, new int[]{0}, new int[]{0}, new int[]{0}
                };

                private static readonly int[][] POSITION_ADJUSTMENT_PATTERN = {
                     new int[]{1, 1, 1, 1, 1},
                     new int[]{1, 0, 0, 0, 1},
                     new int[]{1, 0, 1, 0, 1},
                     new int[]{1, 0, 0, 0, 1},
                     new int[]{1, 1, 1, 1, 1}
                };

                private static readonly int[][] POSITION_ADJUSTMENT_PATTERN_COORDINATE_TABLE = {
                     new int[]{-1, -1, -1, -1,  -1,  -1,  -1},  // Version 1
                     new int[]{ 6, 18, -1, -1,  -1,  -1,  -1},  // Version 2
                     new int[]{ 6, 22, -1, -1,  -1,  -1,  -1},  // Version 3
                     new int[]{ 6, 26, -1, -1,  -1,  -1,  -1},  // Version 4
                     new int[]{ 6, 30, -1, -1,  -1,  -1,  -1},  // Version 5
                     new int[]{ 6, 34, -1, -1,  -1,  -1,  -1},  // Version 6
                     new int[]{ 6, 22, 38, -1,  -1,  -1,  -1},  // Version 7
                     new int[]{ 6, 24, 42, -1,  -1,  -1,  -1},  // Version 8
                     new int[]{ 6, 26, 46, -1,  -1,  -1,  -1},  // Version 9
                     new int[]{ 6, 28, 50, -1,  -1,  -1,  -1},  // Version 10
                     new int[]{ 6, 30, 54, -1,  -1,  -1,  -1},  // Version 11
                     new int[]{ 6, 32, 58, -1,  -1,  -1,  -1},  // Version 12
                     new int[]{ 6, 34, 62, -1,  -1,  -1,  -1},  // Version 13
                     new int[]{ 6, 26, 46, 66,  -1,  -1,  -1},  // Version 14
                     new int[]{ 6, 26, 48, 70,  -1,  -1,  -1},  // Version 15
                     new int[]{ 6, 26, 50, 74,  -1,  -1,  -1},  // Version 16
                     new int[]{ 6, 30, 54, 78,  -1,  -1,  -1},  // Version 17
                     new int[]{ 6, 30, 56, 82,  -1,  -1,  -1},  // Version 18
                     new int[]{ 6, 30, 58, 86,  -1,  -1,  -1},  // Version 19
                     new int[]{ 6, 34, 62, 90,  -1,  -1,  -1},  // Version 20
                     new int[]{ 6, 28, 50, 72,  94,  -1,  -1},  // Version 21
                     new int[]{ 6, 26, 50, 74,  98,  -1,  -1},  // Version 22
                     new int[]{ 6, 30, 54, 78, 102,  -1,  -1},  // Version 23
                     new int[]{ 6, 28, 54, 80, 106,  -1,  -1},  // Version 24
                     new int[]{ 6, 32, 58, 84, 110,  -1,  -1},  // Version 25
                     new int[]{ 6, 30, 58, 86, 114,  -1,  -1},  // Version 26
                     new int[]{ 6, 34, 62, 90, 118,  -1,  -1},  // Version 27
                     new int[]{ 6, 26, 50, 74,  98, 122,  -1},  // Version 28
                     new int[]{ 6, 30, 54, 78, 102, 126,  -1},  // Version 29
                     new int[]{ 6, 26, 52, 78, 104, 130,  -1},  // Version 30
                     new int[]{ 6, 30, 56, 82, 108, 134,  -1},  // Version 31
                     new int[]{ 6, 34, 60, 86, 112, 138,  -1},  // Version 32
                     new int[]{ 6, 30, 58, 86, 114, 142,  -1},  // Version 33
                     new int[]{ 6, 34, 62, 90, 118, 146,  -1},  // Version 34
                     new int[]{ 6, 30, 54, 78, 102, 126, 150},  // Version 35
                     new int[]{ 6, 24, 50, 76, 102, 128, 154},  // Version 36
                     new int[]{ 6, 28, 54, 80, 106, 132, 158},  // Version 37
                     new int[]{ 6, 32, 58, 84, 110, 136, 162},  // Version 38
                     new int[]{ 6, 26, 54, 82, 110, 138, 166},  // Version 39
                     new int[]{ 6, 30, 58, 86, 114, 142, 170}   // Version 40
                };

                private static readonly int[][] TYPE_INFO_COORDINATES = {
                     new int[]{8, 0},
                     new int[]{8, 1},
                     new int[]{8, 2},
                     new int[]{8, 3},
                     new int[]{8, 4},
                     new int[]{8, 5},
                     new int[]{8, 7},
                     new int[]{8, 8},
                     new int[]{7, 8},
                     new int[]{5, 8},
                     new int[]{4, 8},
                     new int[]{3, 8},
                     new int[]{2, 8},
                     new int[]{1, 8},
                     new int[]{0, 8}
                };

                private const int VERSION_INFO_POLY = 0x1f25;  // 1 1111 0010 0101

                private const int TYPE_INFO_POLY = 0x537;
                private const int TYPE_INFO_MASK_PATTERN = 0x5412;

                public static void ClearMatrix(ByteMatrix matrix)
                {
                    matrix.Clear((sbyte)-1);
                }

                public static void BuildMatrix(BitVector dataBits, ErrorCorrectionLevel ecLevel, int version, int maskPattern, ByteMatrix matrix)
                {
                    ClearMatrix(matrix);
                    EmbedBasicPatterns(version, matrix);
                    EmbedTypeInfo(ecLevel, maskPattern, matrix);
                    MaybeEmbedVersionInfo(version, matrix);
                    EmbedDataBits(dataBits, maskPattern, matrix);
                }

                public static void EmbedBasicPatterns(int version, ByteMatrix matrix)
                {
                    EmbedPositionDetectionPatternsAndSeparators(matrix);
                    EmbedDarkDotAtLeftBottomCorner(matrix);

                    MaybeEmbedPositionAdjustmentPatterns(version, matrix);
                    EmbedTimingPatterns(matrix);
                }

                public static void EmbedTypeInfo(ErrorCorrectionLevel ecLevel, int maskPattern, ByteMatrix matrix)
                {
                    var typeInfoBits = new BitVector();
                    MakeTypeInfoBits(ecLevel, maskPattern, typeInfoBits);

                    for (int i = 0; i < typeInfoBits.Size(); ++i)
                    {
                        int bit = typeInfoBits.At(typeInfoBits.Size() - 1 - i);

                        int x1 = TYPE_INFO_COORDINATES[i][0];
                        int y1 = TYPE_INFO_COORDINATES[i][1];
                        matrix.Set(x1, y1, bit);

                        if (i < 8)
                        {
                            int x2 = matrix.GetWidth() - i - 1;
                            int y2 = 8;
                            matrix.Set(x2, y2, bit);
                        }
                        else
                        {
                            int x2 = 8;
                            int y2 = matrix.GetHeight() - 7 + (i - 8);
                            matrix.Set(x2, y2, bit);
                        }
                    }
                }

                public static void MaybeEmbedVersionInfo(int version, ByteMatrix matrix)
                {
                    if (version < 7)
                    {  
                        return;  
                    }
                    var versionInfoBits = new BitVector();
                    MakeVersionInfoBits(version, versionInfoBits);

                    int bitIndex = 6 * 3 - 1;  
                    for (int i = 0; i < 6; ++i)
                    {
                        for (int j = 0; j < 3; ++j)
                        {
                            int bit = versionInfoBits.At(bitIndex);
                            bitIndex--;
                            matrix.Set(i, matrix.GetHeight() - 11 + j, bit);
                            matrix.Set(matrix.GetHeight() - 11 + j, i, bit);
                        }
                    }
                }

                public static void EmbedDataBits(BitVector dataBits, int maskPattern, ByteMatrix matrix)
                {
                    int bitIndex = 0;
                    int direction = -1;
                    int x = matrix.GetWidth() - 1;
                    int y = matrix.GetHeight() - 1;
                    while (x > 0)
                    {
                        if (x == 6)
                        {
                            x -= 1;
                        }
                        while (y >= 0 && y < matrix.GetHeight())
                        {
                            for (int i = 0; i < 2; ++i)
                            {
                                int xx = x - i;
                                if (!IsEmpty(matrix.Get(xx, y)))
                                {
                                    continue;
                                }
                                int bit;
                                if (bitIndex < dataBits.Size())
                                {
                                    bit = dataBits.At(bitIndex);
                                    ++bitIndex;
                                }
                                else
                                {
                                    bit = 0;
                                }

                                if (maskPattern != -1)
                                {
                                    if (MaskUtil.GetDataMaskBit(maskPattern, xx, y))
                                    {
                                        bit ^= 0x1;
                                    }
                                }
                                matrix.Set(xx, y, bit);
                            }
                            y += direction;
                        }
                        direction = -direction; 
                        y += direction;
                        x -= 2; 
                    }
                    if (bitIndex != dataBits.Size())
                    {
                        throw new StiQRCodeException("Not all bits consumed: " + bitIndex + '/' + dataBits.Size());
                    }
                }

                public static void EmbedDataBits2(BitVector dataBits, List<ByteMatrix> matrices)
                {
                    var matrix = matrices[0];
                    int matrixWidth = matrix.GetWidth();
                    int matrixHeight = matrix.GetHeight();

                    int bitIndex = 0;
                    int direction = -1;
                    int x = matrixWidth - 1;
                    int y = matrixHeight - 1;
                    while (x > 0)
                    {
                        if (x == 6)
                        {
                            x -= 1;
                        }
                        while (y >= 0 && y < matrixHeight)
                        {
                            for (int i = 0; i < 2; ++i)
                            {
                                int xx = x - i;
                                if (matrix.Get(xx, y) != -1)    // !IsEmpty
                                {
                                    continue;
                                }
                                int bit;
                                if (bitIndex < dataBits.Size())
                                {
                                    bit = dataBits.At(bitIndex);
                                    ++bitIndex;
                                }
                                else
                                {
                                    bit = 0;
                                }

                                for (int maskPattern = 0; maskPattern < StiQRCode.NUM_MASK_PATTERNS; maskPattern++)
                                {
                                    int bit2 = bit;
                                    if (MaskUtil.GetDataMaskBit(maskPattern, xx, y))
                                    {
                                        bit2 ^= 0x1;
                                    }
                                    matrices[maskPattern].Set(xx, y, bit2);
                                }
                            }
                            y += direction;
                        }
                        direction = -direction;
                        y += direction;
                        x -= 2;
                    }
                    if (bitIndex != dataBits.Size())
                    {
                        throw new StiQRCodeException("Not all bits consumed: " + bitIndex + '/' + dataBits.Size());
                    }
                }

                public static int FindMSBSet(int value)
                {
                    uint val = (uint)value;
                    int numDigits = 0;
                    while (val != 0)
                    {
                        val >>= 1;
                        ++numDigits;
                    }
                    return numDigits;
                }

                public static int CalculateBCHCode(int value, int poly)
                {
                    int msbSetInPoly = FindMSBSet(poly);
                    value <<= msbSetInPoly - 1;
                    while (FindMSBSet(value) >= msbSetInPoly)
                    {
                        value ^= poly << (FindMSBSet(value) - msbSetInPoly);
                    }
                    return value;
                }

                public static void MakeTypeInfoBits(ErrorCorrectionLevel ecLevel, int maskPattern, BitVector bits)
                {
                    if (!StiQRCode.IsValidMaskPattern(maskPattern))
                    {
                        throw new StiQRCodeException("Invalid mask pattern");
                    }
                    int typeInfo = (ecLevel.GetBits() << 3) | maskPattern;
                    bits.AppendBits(typeInfo, 5);

                    int bchCode = CalculateBCHCode(typeInfo, TYPE_INFO_POLY);
                    bits.AppendBits(bchCode, 10);

                    var maskBits = new BitVector();
                    maskBits.AppendBits(TYPE_INFO_MASK_PATTERN, 15);
                    bits.Xor(maskBits);

                    if (bits.Size() != 15)
                    { 
                        throw new StiQRCodeException("should not happen but we got: " + bits.Size());
                    }
                }

                public static void MakeVersionInfoBits(int version, BitVector bits)
                {
                    bits.AppendBits(version, 6);
                    int bchCode = CalculateBCHCode(version, VERSION_INFO_POLY);
                    bits.AppendBits(bchCode, 12);

                    if (bits.Size() != 18)
                    {
                        throw new StiQRCodeException("should not happen but we got: " + bits.Size());
                    }
                }

                private static bool IsEmpty(int value)
                {
                    return value == -1;
                }

                private static bool IsValidValue(int value)
                {
                    return (value == -1 ||  
                        value == 0 ||  
                        value == 1);  
                }

                private static void EmbedTimingPatterns(ByteMatrix matrix)
                {
                    for (int i = 8; i < matrix.GetWidth() - 8; ++i)
                    {
                        int bit = (i + 1) % 2;
                        if (!IsValidValue(matrix.Get(i, 6)))
                        {
                            throw new StiQRCodeException();
                        }
                        if (IsEmpty(matrix.Get(i, 6)))
                        {
                            matrix.Set(i, 6, bit);
                        }
                        if (!IsValidValue(matrix.Get(6, i)))
                        {
                            throw new StiQRCodeException();
                        }
                        if (IsEmpty(matrix.Get(6, i)))
                        {
                            matrix.Set(6, i, bit);
                        }
                    }
                }

                private static void EmbedDarkDotAtLeftBottomCorner(ByteMatrix matrix)
                {
                    if (matrix.Get(8, matrix.GetHeight() - 8) == 0)
                    {
                        throw new StiQRCodeException();
                    }
                    matrix.Set(8, matrix.GetHeight() - 8, 1);
                }

                private static void EmbedHorizontalSeparationPattern(int xStart, int yStart, ByteMatrix matrix)
                {
                    if (HORIZONTAL_SEPARATION_PATTERN[0].Length != 8 || HORIZONTAL_SEPARATION_PATTERN.GetLength(0) != 1)
                    {
                        throw new StiQRCodeException("Bad horizontal separation pattern");
                    }
                    for (int x = 0; x < 8; ++x)
                    {
                        if (!IsEmpty(matrix.Get(xStart + x, yStart)))
                        {
                            throw new StiQRCodeException();
                        }
                        matrix.Set(xStart + x, yStart, HORIZONTAL_SEPARATION_PATTERN[0][x]);
                    }
                }

                private static void EmbedVerticalSeparationPattern(int xStart, int yStart, ByteMatrix matrix)
                {
                    if (VERTICAL_SEPARATION_PATTERN[0].Length != 1 || VERTICAL_SEPARATION_PATTERN.GetLength(0) != 7)
                    {
                        throw new StiQRCodeException("Bad vertical separation pattern");
                    }
                    for (int y = 0; y < 7; ++y)
                    {
                        if (!IsEmpty(matrix.Get(xStart, yStart + y)))
                        {
                            throw new StiQRCodeException();
                        }
                        matrix.Set(xStart, yStart + y, VERTICAL_SEPARATION_PATTERN[y][0]);
                    }
                }

                private static void EmbedPositionAdjustmentPattern(int xStart, int yStart, ByteMatrix matrix)
                {
                    if (POSITION_ADJUSTMENT_PATTERN[0].Length != 5 || POSITION_ADJUSTMENT_PATTERN.GetLength(0) != 5)
                    {
                        throw new StiQRCodeException("Bad position adjustment");
                    }
                    for (int y = 0; y < 5; ++y)
                    {
                        for (int x = 0; x < 5; ++x)
                        {
                            if (!IsEmpty(matrix.Get(xStart + x, yStart + y)))
                            {
                                throw new StiQRCodeException();
                            }
                            matrix.Set(xStart + x, yStart + y, POSITION_ADJUSTMENT_PATTERN[y][x]);
                        }
                    }
                }

                private static void EmbedPositionDetectionPattern(int xStart, int yStart, ByteMatrix matrix)
                {
                    if (POSITION_DETECTION_PATTERN[0].Length != 7 || POSITION_DETECTION_PATTERN.GetLength(0) != 7)
                    {
                        throw new StiQRCodeException("Bad position detection pattern");
                    }
                    for (int y = 0; y < 7; ++y)
                    {
                        for (int x = 0; x < 7; ++x)
                        {
                            if (!IsEmpty(matrix.Get(xStart + x, yStart + y)))
                            {
                                throw new StiQRCodeException();
                            }
                            matrix.Set(xStart + x, yStart + y, POSITION_DETECTION_PATTERN[y][x] * 2);
                        }
                    }
                }

                private static void EmbedPositionDetectionPatternsAndSeparators(ByteMatrix matrix)
                {
                    int pdpWidth = POSITION_DETECTION_PATTERN[0].Length;
                    EmbedPositionDetectionPattern(0, 0, matrix);
                    EmbedPositionDetectionPattern(matrix.GetWidth() - pdpWidth, 0, matrix);
                    EmbedPositionDetectionPattern(0, matrix.GetWidth() - pdpWidth, matrix);

                    int hspWidth = HORIZONTAL_SEPARATION_PATTERN[0].Length;
                    EmbedHorizontalSeparationPattern(0, hspWidth - 1, matrix);
                    EmbedHorizontalSeparationPattern(matrix.GetWidth() - hspWidth,
                        hspWidth - 1, matrix);
                    EmbedHorizontalSeparationPattern(0, matrix.GetWidth() - hspWidth, matrix);

                    int vspSize = VERTICAL_SEPARATION_PATTERN.Length;
                    EmbedVerticalSeparationPattern(vspSize, 0, matrix);
                    EmbedVerticalSeparationPattern(matrix.GetHeight() - vspSize - 1, 0, matrix);
                    EmbedVerticalSeparationPattern(vspSize, matrix.GetHeight() - vspSize,
                        matrix);
                }

                private static void MaybeEmbedPositionAdjustmentPatterns(int version, ByteMatrix matrix)
                {
                    if (version < 2)
                    {  
                        return;
                    }
                    int index = version - 1;
                    int[] coordinates = POSITION_ADJUSTMENT_PATTERN_COORDINATE_TABLE[index];
                    int numCoordinates = POSITION_ADJUSTMENT_PATTERN_COORDINATE_TABLE[index].Length;
                    for (int i = 0; i < numCoordinates; ++i)
                    {
                        for (int j = 0; j < numCoordinates; ++j)
                        {
                            int y = coordinates[i];
                            int x = coordinates[j];
                            if (x == -1 || y == -1)
                            {
                                continue;
                            }
                            if (IsEmpty(matrix.Get(x, y)))
                            {
                                EmbedPositionAdjustmentPattern(x - 2, y - 2, matrix);
                            }
                        }
                    }
                }
            }
            #endregion

            #region Const
            public const int NUM_MASK_PATTERNS = 8;
            #endregion

            #region Variables
            private Mode mode;
            private ErrorCorrectionLevel ecLevel;
            private int version;
            private int matrixWidth;
            private int maskPattern;
            private int numTotalBytes;
            private int numDataBytes;
            private int numECBytes;
            private int numRSBlocks;
            private ByteMatrix matrix;
            #endregion

            #region Methods
            public Mode GetMode() => mode;

            public ErrorCorrectionLevel GetECLevel() => ecLevel;

            public int GetVersion() => version;

            public int GetMatrixWidth() => matrixWidth;

            public int GetMaskPattern() => maskPattern;

            public int GetNumTotalBytes() => numTotalBytes;

            public int GetNumDataBytes() => numDataBytes;

            public int GetNumECBytes() => numECBytes;

            public int GetNumRSBlocks() => numRSBlocks;

            internal ByteMatrix GetMatrix() => matrix;

            public int At(int x, int y)
            {
                int value = matrix.Get(x, y);
                if (!(value == 0 || value == 1))
                {
                    throw new StiQRCodeException("Bad value");
                }
                return value;
            }

            public bool IsValid()
            {
                return
                    mode != null &&
                    ecLevel != null &&
                    version != -1 &&
                    matrixWidth != -1 &&
                    maskPattern != -1 &&
                    numTotalBytes != -1 &&
                    numDataBytes != -1 &&
                    numECBytes != -1 &&
                    numRSBlocks != -1 &&
                    IsValidMaskPattern(maskPattern) &&
                    numTotalBytes == numDataBytes + numECBytes &&
                    matrix != null &&
                    matrixWidth == matrix.GetWidth() &&
                    matrix.GetWidth() == matrix.GetHeight(); 
            }


            public void SetMode(Mode value)
            {
                mode = value;
            }

            public void SetECLevel(ErrorCorrectionLevel value)
            {
                ecLevel = value;
            }

            public void SetVersion(int value)
            {
                version = value;
            }

            public void SetMatrixWidth(int value)
            {
                matrixWidth = value;
            }

            public void SetMaskPattern(int value)
            {
                maskPattern = value;
            }

            public void SetNumTotalBytes(int value)
            {
                numTotalBytes = value;
            }

            public void SetNumDataBytes(int value)
            {
                numDataBytes = value;
            }

            public void SetNumECBytes(int value)
            {
                numECBytes = value;
            }

            public void SetNumRSBlocks(int value)
            {
                numRSBlocks = value;
            }

            internal void SetMatrix(ByteMatrix value)
            {
                matrix = value;
            }

            public static bool IsValidMaskPattern(int maskPattern)
            {
                return maskPattern >= 0 && maskPattern < NUM_MASK_PATTERNS;
            }
            #endregion

            public StiQRCode()
            {
                mode = null;
                ecLevel = null;
                version = -1;
                matrixWidth = -1;
                maskPattern = -1;
                numTotalBytes = -1;
                numDataBytes = -1;
                numECBytes = -1;
                numRSBlocks = -1;
                matrix = null;
            }
        }
        #endregion

		#region Methods

        public override void Draw(object context, StiBarCode barCode, RectangleF rect, float zoom)
		{		
			var code = GetCode(barCode);
            //code = CheckCodeSymbols(code, QRCodeSymbols);
            if (code == "System.Byte[]") code = null;
            BarCodeData.Code = code;

            var errorCorrectionLevel2 = StiQRCode.ErrorCorrectionLevel.L;
            if (ErrorCorrectionLevel == StiQRCodeErrorCorrectionLevel.Level2) errorCorrectionLevel2 = StiQRCode.ErrorCorrectionLevel.M;
            if (ErrorCorrectionLevel == StiQRCodeErrorCorrectionLevel.Level3) errorCorrectionLevel2 = StiQRCode.ErrorCorrectionLevel.Q;
            if (ErrorCorrectionLevel == StiQRCodeErrorCorrectionLevel.Level4) errorCorrectionLevel2 = StiQRCode.ErrorCorrectionLevel.H;

            try
            {
                var qrCode = new StiQRCode();
                bool assembleData = (context is StiBarCodeExportPainter) && (context as StiBarCodeExportPainter).OnlyAssembleData;
                if (!assembleData)  //speed optimization for pdf
                {
                    StiQRCode.QREncoder.Encode(code, errorCorrectionLevel2, qrCode, MatrixSize, ProcessTilde);
                }
                else
                {
                    var matrix1 = new StiQRCode.ByteMatrix(1, 1);
                    matrix1.Set(0, 0, 1);
                    qrCode.SetMatrix(matrix1);
                }
                var bm = qrCode.GetMatrix();

                byte[] matrix = new byte[bm.GetWidth() * bm.GetHeight()];

                for (int y = 0; y < bm.GetHeight(); y++)
                {
                    int offset = y * bm.GetWidth();
                    for (int x = 0; x < bm.GetWidth(); x++)
                    {
                        matrix[offset + x] = (byte)bm.Get(x, y);
                    }
                }

                BarCodeData.MatrixGrid = matrix;
                BarCodeData.MatrixWidth = bm.GetWidth();
                BarCodeData.MatrixHeight = bm.GetHeight();
                BarCodeData.MatrixRatioY = 1;

                //if (dm.ErrorMessage == null)
                //{
                    DrawQRCode(context, rect, barCode, zoom, BodyShape, EyeFrameShape, EyeBallShape);
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
            qr.BarCodeType = new StiQRCodeBarCodeType(10, StiQRCodeErrorCorrectionLevel.Level1, StiQRCodeSize.Automatic);
            var BarCodeData = (qr.BarCodeType as StiQRCodeBarCodeType).BarCodeData;
            BarCodeData.Code = code;

            Image resultImage = null;

            var errorCorrectionLevel2 = StiQRCode.ErrorCorrectionLevel.L;
            //if (errorCorrectionLevel == StiQRCodeErrorCorrectionLevel.Level2) errorCorrectionLevel2 = StiQRCode.ErrorCorrectionLevel.M;
            //if (errorCorrectionLevel == StiQRCodeErrorCorrectionLevel.Level3) errorCorrectionLevel2 = StiQRCode.ErrorCorrectionLevel.Q;
            //if (errorCorrectionLevel == StiQRCodeErrorCorrectionLevel.Level4) errorCorrectionLevel2 = StiQRCode.ErrorCorrectionLevel.H;

            try
            {
                var qrCode = new StiQRCode();
                StiQRCode.QREncoder.Encode(code, errorCorrectionLevel2, qrCode, StiQRCodeSize.Automatic, false);
                var bm = qrCode.GetMatrix();

                byte[] matrix = new byte[bm.GetWidth() * bm.GetHeight()];

                for (int y = 0; y < bm.GetHeight(); y++)
                {
                    int offset = y * bm.GetWidth();
                    for (int x = 0; x < bm.GetWidth(); x++)
                    {
                        matrix[offset + x] = (byte)bm.Get(x, y);
                    }
                }

                BarCodeData.MatrixGrid = matrix;
                BarCodeData.MatrixWidth = bm.GetWidth();
                BarCodeData.MatrixHeight = bm.GetHeight();
                BarCodeData.MatrixRatioY = 1;

                int quietZone = 2;
                if (!qr.ShowQuietZones) quietZone = 0;
                int imageWidth = (BarCodeData.MatrixWidth + quietZone * 2) * zoom;
                int imageHeight = (BarCodeData.MatrixHeight + quietZone * 2) * zoom;
                resultImage = new Bitmap(imageWidth, imageHeight);
                using (var gr = Graphics.FromImage(resultImage))
                {
                    (qr.BarCodeType as StiQRCodeBarCodeType).Draw2DBarCode(gr, new Rectangle(0, 0, imageWidth, imageHeight), qr, zoom);
                }
            }
            catch
            {
            }

            return resultImage;
        }        
		#endregion

        #region Methods.override
        public override StiBarCodeTypeService CreateNew() => new StiQRCodeBarCodeType();
        #endregion

        public StiQRCodeBarCodeType()
            : this(40f, StiQRCodeErrorCorrectionLevel.Level1, StiQRCodeSize.Automatic)
		{
		}

        public StiQRCodeBarCodeType(float module, StiQRCodeErrorCorrectionLevel errorCorrectionLevel, StiQRCodeSize matrixSize)
		{
			this.module = module;
            this.ErrorCorrectionLevel = errorCorrectionLevel;
			this.MatrixSize = matrixSize;
		}

        public StiQRCodeBarCodeType(float module, StiQRCodeErrorCorrectionLevel errorCorrectionLevel, StiQRCodeSize matrixSize, Image image)
        {
            this.module = module;
            this.ErrorCorrectionLevel = errorCorrectionLevel;
            this.MatrixSize = matrixSize;
            this.Image = image;
        }

        public StiQRCodeBarCodeType(float module, StiQRCodeErrorCorrectionLevel errorCorrectionLevel, StiQRCodeSize matrixSize, Image image, double imageMultipleFactor)
        {
            this.module = module;
            this.ErrorCorrectionLevel = errorCorrectionLevel;
            this.MatrixSize = matrixSize;
            this.Image = image;
            this.ImageMultipleFactor = imageMultipleFactor;
        }

        public StiQRCodeBarCodeType(float module, StiQRCodeErrorCorrectionLevel errorCorrectionLevel, StiQRCodeSize matrixSize, Image image, double imageMultipleFactor,
            StiQRCodeBodyShapeType bodyShape, StiQRCodeEyeFrameShapeType eyeFrameShape, StiQRCodeEyeBallShapeType eyeBallShape,
            StiBrush bodyBrush, StiBrush eyeFrameBrush, StiBrush eyeBallBrush) :
            this(module, errorCorrectionLevel, matrixSize, image, imageMultipleFactor, bodyShape, eyeFrameShape, eyeBallShape, bodyBrush, eyeFrameBrush, eyeBallBrush, false)
        {
        }

        public StiQRCodeBarCodeType(float module, StiQRCodeErrorCorrectionLevel errorCorrectionLevel, StiQRCodeSize matrixSize, Image image, double imageMultipleFactor,
            StiQRCodeBodyShapeType bodyShape, StiQRCodeEyeFrameShapeType eyeFrameShape, StiQRCodeEyeBallShapeType eyeBallShape,
            StiBrush bodyBrush, StiBrush eyeFrameBrush, StiBrush eyeBallBrush, bool processTilde)
        {
            this.module = module;
            this.ErrorCorrectionLevel = errorCorrectionLevel;
            this.MatrixSize = matrixSize;
            this.Image = image;
            this.ImageMultipleFactor = imageMultipleFactor;
            this.BodyShape = bodyShape;
            this.EyeFrameShape = eyeFrameShape;
            this.EyeBallShape = eyeBallShape;
            this.BodyBrush = bodyBrush;
            this.EyeFrameBrush = eyeFrameBrush;
            this.EyeBallBrush = eyeBallBrush;
            this.ProcessTilde = processTilde;
        }

    }
}