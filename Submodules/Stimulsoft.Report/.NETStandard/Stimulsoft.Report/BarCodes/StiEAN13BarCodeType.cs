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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.Components;
using Stimulsoft.Report.PropertyGrid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Drawing;
using Stimulsoft.Base.Design;
#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
using StringFormat = Stimulsoft.Drawing.StringFormat;
using Graphics = Stimulsoft.Drawing.Graphics;
#endif

namespace Stimulsoft.Report.BarCodes
{
    /// <summary>
    /// The class describes the Barcode type - EAN-13.
    /// </summary>
    [TypeConverter(typeof(Stimulsoft.Report.BarCodes.Design.StiEAN13BarCodeTypeConverter))]
	public class StiEAN13BarCodeType : StiBarCodeTypeService
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiEAN13BarCodeType
            jObject.AddPropertyFloat("Module", Module, 13f);
            jObject.AddPropertyFloat("Height", Height, 1f);
            jObject.AddPropertyEnum("SupplementType", SupplementType, StiEanSupplementType.None);
            jObject.AddPropertyStringNullOrEmpty("SupplementCode", SupplementCode);
            jObject.AddPropertyBool("ShowQuietZoneIndicator", ShowQuietZoneIndicator, true);
            
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

                    case "SupplementType":
                        this.SupplementType = property.DeserializeEnum<StiEanSupplementType>();
                        break;

                    case "SupplementCode":
                        this.SupplementCode = property.DeserializeString();
                        break;

                    case "ShowQuietZoneIndicator":
                        this.ShowQuietZoneIndicator = property.DeserializeBool();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiEAN13BarCodeType;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            var list = new[]
            {
                propHelper.SupplementCode(),
                propHelper.fHeight(),
                propHelper.Module(),
                propHelper.ShowQuietZoneIndicator(),
                propHelper.SupplementType()
            };
            objHelper.Add(StiPropertyCategories.Main, list);
            
            return objHelper;
        }
        #endregion

		#region ServiceName
		/// <summary>
		/// Gets a service name.
		/// </summary>
		public override string ServiceName => "EAN-13";
		#endregion

		#region Class EanBarInfo
		public class EanBarInfo
		{
			public Ean13Symbol SymbolType;
			public char SymbolText;
			public bool TextAtTop;
			public bool MakeLonger;

			public EanBarInfo(Ean13Symbol symbolType, char symbolText, bool textAtTop) :
				this(symbolType, symbolText, textAtTop, false)
			{
			}

			public EanBarInfo(Ean13Symbol symbolType, char symbolText, bool textAtTop, bool makeLonger)
			{
				this.SymbolType = symbolType;
				this.SymbolText = symbolText;
				this.TextAtTop = textAtTop;
				this.MakeLonger = makeLonger;
			}
		}
        #endregion

        #region Properties
        public override string DefaultCodeValue => "0123456789012";

        public override bool[] VisibleProperties
        {
            get
            {
                var props = new bool[visiblePropertiesCount];

                props[11] = true;
                props[13] = true;
                props[17] = true;
                props[19] = true;
                props[20] = true;

                return props;
            }
        }

		private float module = 13f;
        /// <summary>
        /// Gets or sets width of the most fine element of the bar code.
        /// </summary>
        [Description("Gets or sets width of the most fine element of the bar code.")]
		[DefaultValue(13f)]
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

				if (value < 10.4f)	
					module = 10.4f;

				if (value > 26f)	
					module = 26f;
			}
		}

		private float height = 1f;
        /// <summary>
        /// Gets os sets height factor of the bar code.
        /// </summary>		
        [Description("Gets os sets height factor of the bar code.")]
		[DefaultValue(1f)]
        [StiCategory("BarCode")]
        public override float Height
		{
			get
			{
				return height;
			}
			set
			{
				height = value;

				if (value < 0.5f)	
					height = 0.5f;

				if (value > 1.0f)	
					height = 1.0f;
			}
		}		

        /// <summary>
        /// Gets or sets type of supplement code.
        /// </summary>
		[DefaultValue(StiEanSupplementType.None)]
        [StiSerializable]
        [Description("Gets or sets type of supplement code.")]
        [TypeConverter(typeof(StiEnumConverter))]
		[Editor(StiEditors.Enum, typeof(UITypeEditor))]
		[StiCategory("BarCode")]
        public virtual StiEanSupplementType SupplementType { get; set; } = StiEanSupplementType.None;

		/// <summary>
		/// Gets or sets the component supplement bar code.
		/// </summary>
		[StiCategory("BarCode")]
		[DefaultValue(null)]
        [StiSerializable]
        [Description("Gets or sets the component supplement bar code.")]
        public virtual string SupplementCode { get; set; } = null;

        /// <summary>
        /// Gets or sets value which indicates will show Quiet Zone Indicator or no.
        /// </summary>
        [DefaultValue(true)]
        [StiSerializable]
        [Description("Gets or sets value which indicates will show Quiet Zone Indicator or no.")]
        [TypeConverter(typeof(Stimulsoft.Base.Localization.StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		[StiCategory("BarCode")]
        public virtual bool ShowQuietZoneIndicator { get; set; } = true;

        internal override float LabelFontHeight => EanTextHeight;
		#endregion

		#region Consts
		protected virtual float EanSpaceLeft {			get {return 11f; } }
		protected virtual float EanSpaceRight {			get {return 8f; } }
		protected virtual float EanSpaceTop {			get {return 0f; } }
		protected virtual float EanSpaceBottom {		get {return 1f; } }
		protected virtual float EanSpaceBeforeAdd {		get {return 10f; } }
		protected virtual float EanSpaceTextTop {		get {return 10f; } }
		protected virtual float EanLineHeightShort {	get {return 69.20f; } }
		protected virtual float EanLineHeightLong {		get {return EanLineHeightShort + 5f; } }
		protected virtual float EanTextPositionTop {	get {return 0.5f; } }
		protected virtual float EanTextPositionBottom { get {return EanLineHeightShort + EanSpaceBottom; } }
		protected virtual float EanTextHeight {			get {return 8.33f; } }
		protected virtual float EanMainHeight {			get {return 78.58f; } }
		protected virtual float EanLineHeightForCut {	get {return EanLineHeightShort; } }
		protected virtual float EanWideToNarrowRatio {	get {return 2f; } }

		protected string[] symComboSet = {	
			"000000",
			"001011",
			"001101",
			"001110",
			"010011",
			"011001",
			"011100",
			"010101",
			"010110",
			"011010"	
		};

		protected string[] symParitySetAdd2 = 
		{	
			"oo",	//0
			"oe",	//1
			"eo",	//2
			"ee"	//3
		};

		protected string[] symParitySetAdd5 = 
		{	
			"eeooo",	//0
			"eoeoo",	//1
			"eooeo",	//2
			"eoooe",	//3
			"oeeoo",	//4
			"ooeeo",	//5
			"oooee",	//6
			"oeoeo",	//7
			"oeooe",	//8
			"ooeoe"	//9
		};

		public enum Ean13Symbol
		{
			ComboA0 = 0,
			ComboA1,
			ComboA2,
			ComboA3,
			ComboA4,
			ComboA5,
			ComboA6,
			ComboA7,
			ComboA8,
			ComboA9,
			ComboB0,
			ComboB1,
			ComboB2,
			ComboB3,
			ComboB4,
			ComboB5,
			ComboB6,
			ComboB7,
			ComboB8,
			ComboB9,
			ComboC0,
			ComboC1,
			ComboC2,
			ComboC3,
			ComboC4,
			ComboC5,
			ComboC6,
			ComboC7,
			ComboC8,
			ComboC9,
			GuardLeft,
			GuardCenter,
			GuardRight,
			GuardSpecial,
			GuardAddLeft,
			GuardAddDelineator,
			SpaceLeft,
			SpaceRight,
			SpaceBeforeAdd
		}

		protected string[] ean13SymData = 
		{	  
			"2504",	//"0001101", comboA
			"1514",	//"0011001",
			"1415",	//"0010011",
			"0704",	//"0111101",
			"0425",	//"0100011",
			"0524",	//"0110001",
			"0407",	//"0101111",
			"0605",	//"0111011",
			"0506",	//"0110111",
			"2405",	//"0001011"	
			"0416",	//"0100111", comboB
			"0515",	//"0110011",
			"1505",	//"0011011",
			"0434",	//"0100001",
			"1604",	//"0011101",
			"0614",	//"0111001",
			"3404",	//"0000101",
			"1424",	//"0010001",
			"2414",	//"0001001",
			"1406",	//"0010111"
			"6140",	//"1110010", comboC
			"5150",	//"1100110",
			"5051",	//"1101100",
			"4340",	//"1000010",
			"4061",	//"1011100",
			"4160",	//"1001110",
			"4043",	//"1010000",
			"4241",	//"1000100",
			"4142",	//"1001000",
			"6041",	//"1110100"
			"808",	//guard left
			"08080",	//guard center
			"808",	//guard right
			"080808",//guard special
			"809",	//guard add-on left
			"08",	//guard add-on delineator
			"x",		//space left
			"y",		//space right
			"z",		//space before add
		};
		#endregion

		#region Base methods
		protected void CalculateSizeEan(
			float offsetY,
			float Zoom,
            List<EanBarInfo> BarsArray,
			RectangleF rect,
			StiBarCode barCode)
		{
			BarCodeData.WideToNarrowRatio = 2f;
			BarCodeData.EanBarsArray = BarsArray;
            var sb = new StringBuilder();
			foreach (EanBarInfo barInfo in BarsArray)
			{
				sb.Append(ean13SymData[(int)barInfo.SymbolType]);
			}
			float barsWidth = GetSymbolsStringWidth(sb.ToString());

			RectWindow = new RectangleF(0, 0, rect.Width, rect.Height);
			if ((barCode.Angle == StiAngle.Angle90) || (barCode.Angle == StiAngle.Angle270))
			{
				RectWindow = new RectangleF(0, 0, rect.Height, rect.Width);
			}

            float fontAddSize = barCode.Font.Size - 8 + 0.5f;
            var MainHeight = EanMainHeight + fontAddSize;
            var TextHeight = EanTextHeight + fontAddSize;

            float fullZoomX = (Module / 10) * Zoom;
			float fullZoomY = fullZoomX;
			float cutHeight = EanLineHeightForCut * (1f - Height);

            float spaceTextTop = EanSpaceTextTop + fontAddSize;

			if (barCode.AutoScale)
			{
				fullZoomX = (float)RectWindow.Width / barsWidth;
				cutHeight = - (float)(RectWindow.Height / fullZoomY - (MainHeight + offsetY));
                if (!barCode.ShowLabelText)
                {
                    cutHeight -= EanTextHeight;
                    spaceTextTop -= EanTextHeight;
                }
			}

            BarCodeData.FullZoomY = fullZoomY;

			BarCodeData.SpaceLeft = EanSpaceLeft * fullZoomX;
			BarCodeData.SpaceRight = EanSpaceRight * fullZoomX;
			BarCodeData.SpaceBeforeAdd = EanSpaceBeforeAdd * fullZoomX;
			BarCodeData.LineWidth = 1f * fullZoomX;
			BarCodeData.MainWidth = barsWidth * fullZoomX;
			BarCodeData.SpaceTop = EanSpaceTop * fullZoomY;
			BarCodeData.SpaceBottom = EanSpaceBottom * fullZoomY;
			BarCodeData.SpaceTextTop = spaceTextTop * fullZoomY;
			BarCodeData.LineHeightShort = (EanLineHeightShort - cutHeight) * fullZoomY;
			BarCodeData.LineHeightLong = (EanLineHeightLong - cutHeight) * fullZoomY;
			BarCodeData.TextPositionTop = EanTextPositionTop * fullZoomY;
			BarCodeData.TextPositionBottom = (EanTextPositionBottom - cutHeight) * fullZoomY;
			BarCodeData.MainHeight = (MainHeight + offsetY - cutHeight) * fullZoomY;
			BarCodeData.TextHeight = TextHeight * fullZoomY;
			BarCodeData.OffsetY = offsetY * fullZoomY;
		}

        protected void DrawEanBars(object context, List<EanBarInfo> barsArray, StiBarCode barCode)
		{
            var backBrush = new StiSolidBrush(barCode.BackColor);
            var foreBrush = new StiSolidBrush(barCode.ForeColor);
			BaseFillRectangle(context, backBrush, 0, 0, BarCodeData.MainWidth, BarCodeData.MainHeight);			

			float binOffsetX = 0;
			float binOffsetY = BarCodeData.OffsetY + BarCodeData.SpaceTop;

            bool needPixel = context is Graphics;
            float fontScale = needPixel ? 1 : 0.72f / 0.96f;
            using (var sf = new StringFormat())
            using (var barCodeFont = new Font(
                       StiFontCollection.GetFontFamily(barCode.Font.Name),
                       barCode.Font.Size * (barCode.BarCodeType.LabelFontHeight / 8f) * BarCodeData.FullZoomY * fontScale,
                       barCode.Font.Style,
                       needPixel ? GraphicsUnit.Pixel : GraphicsUnit.Point))
            {
				sf.Alignment = StringAlignment.Center;
				sf.LineAlignment = StringAlignment.Near;

				foreach (var barInfo in barsArray)
				{
					float inSymPosition = 0;
					var sym = ean13SymData[(int)barInfo.SymbolType];
					if (barInfo.MakeLonger) sym = makeLonger(sym);
					for (int index = 0; index < sym.Length; index++)
					{
						char currentChar = sym[index];
						if (IsSymbolSpace(currentChar) == false)
						{
							float binHeight = BarCodeData.LineHeightShort;
							if (IsSymbolLong(currentChar) || barInfo.TextAtTop)
							{
								binHeight = BarCodeData.LineHeightLong;
							}
							float binTopOffset = 0;
							if (barInfo.TextAtTop) 
							{
								binTopOffset = BarCodeData.SpaceTextTop;
							}
							BaseFillRectangle(context, foreBrush,
								(binOffsetX + BarCodeData.LineWidth * inSymPosition),
								(binOffsetY + binTopOffset),
								(BarCodeData.LineWidth * GetSymbolWidth(currentChar)),
								(binHeight - binTopOffset));
						}
						inSymPosition += GetSymbolWidth(currentChar);
					}
					if ((barCode.ShowLabelText) && (barInfo.SymbolText != ' '))
					{
						RectangleF rect;
						if (barInfo.TextAtTop)
						{
							rect = new RectangleF(
								binOffsetX,
								binOffsetY + BarCodeData.TextPositionTop,
								inSymPosition * BarCodeData.LineWidth,
								BarCodeData.SpaceTextTop);
						}
						else
						{
							rect = new RectangleF(
								binOffsetX,
								binOffsetY + BarCodeData.TextPositionBottom,
								inSymPosition * BarCodeData.LineWidth,
								BarCodeData.SpaceTextTop);
						}
						BaseDrawString(context,
							barInfo.SymbolText.ToString(),
							barCodeFont,
							foreBrush,
							rect,
							sf);
					}
					binOffsetX += BarCodeData.LineWidth * inSymPosition;
				}
			}
		}

        protected List<EanBarInfo> MakeEan13Bars(ref string code, bool isLast)
		{
			#region Calculate Ean13 check digit
			var dig = new int[12];
			for (int tempIndex = 0; tempIndex < 12; tempIndex++)
			{
				dig[tempIndex] = int.Parse(code[tempIndex].ToString());
			}
			int sum = (dig[1] + dig[3] + dig[5] + dig[7] + dig[9] + dig[11]) * 3 +
				dig[0] + dig[2] + dig[4] + dig[6] + dig[8] + dig[10];
			int checkDigit = 10 - (sum % 10);
			if (checkDigit == 10)
			{
				checkDigit = 0;
			}
			code = code.Substring(0,12) + (char)(checkDigit + 48);
            #endregion

            var barsArray = new List<EanBarInfo>
            {
                new EanBarInfo(Ean13Symbol.SpaceLeft, code[0], false),
                new EanBarInfo(Ean13Symbol.GuardLeft, ' ', false)
            };
            int firstNumber = int.Parse(code[0].ToString());

			for (int index = 0; index < 6; index++)
			{	
				int currentNumber = int.Parse(code[1 + index].ToString());
                var sym = new EanBarInfo((Ean13Symbol)((int)Ean13Symbol.ComboA0 + currentNumber), code[1 + index], false);
				if (symComboSet[firstNumber][index] != '0')
				{
					sym.SymbolType = Ean13Symbol.ComboB0 + currentNumber;
				}
				barsArray.Add(sym);
			}
			barsArray.Add(new EanBarInfo(Ean13Symbol.GuardCenter, ' ', false));

			for (int index = 0; index < 6; index++)
			{	
				int currentNumber = int.Parse(code[7 + index].ToString());
                var sym = new EanBarInfo(Ean13Symbol.ComboC0 + currentNumber, code[7 + index], false);
				barsArray.Add(sym);
			}
			barsArray.Add(new EanBarInfo(Ean13Symbol.GuardRight, ' ', false));

			if (isLast)
			{
				barsArray.Add(new EanBarInfo(Ean13Symbol.SpaceRight, (ShowQuietZoneIndicator ? '>' : ' '), false));
			}
			else
			{
				barsArray.Add(new EanBarInfo(Ean13Symbol.SpaceBeforeAdd, ' ', false));
			}

			return barsArray;
		}

        protected List<EanBarInfo> MakeEanAdd2Bars(string code, List<EanBarInfo> baseArray, bool isLast)
		{
            var barsArray = (baseArray ?? new List<EanBarInfo>());
			barsArray.Add(new EanBarInfo(Ean13Symbol.GuardAddLeft, ' ', true));
			int numberDigits = 2;
			string symbolParity = symParitySetAdd2[int.Parse(code.Substring(0,2)) % 4];	//cheksum
			for (int index = 0; index < numberDigits; index++)
			{	
				int currentNumber = int.Parse(code[index].ToString());
				char parity = symbolParity[index];
                var sym = new EanBarInfo((Ean13Symbol)((int)Ean13Symbol.ComboA0 + currentNumber), code[index], true);
				if (parity != 'o')
				{
					sym.SymbolType = Ean13Symbol.ComboB0 + currentNumber;
				}
				barsArray.Add(sym);
				if (index < numberDigits - 1)
				{
					barsArray.Add(new EanBarInfo(Ean13Symbol.GuardAddDelineator, ' ', true));
				}
			}

            if (isLast)
            {
                barsArray.Add(new EanBarInfo(Ean13Symbol.SpaceRight, (ShowQuietZoneIndicator ? '>' : ' '), true));
            }
            else
            {
                barsArray.Add(new EanBarInfo(Ean13Symbol.SpaceBeforeAdd, ' ', true));
            }
			return barsArray;
		}

        protected List<EanBarInfo> MakeEanAdd5Bars(string code, List<EanBarInfo> baseArray, bool isLast)
		{
            var barsArray = (baseArray ?? new List<EanBarInfo>());
			barsArray.Add(new EanBarInfo(Ean13Symbol.GuardAddLeft, ' ', true));
			int checkSum =	int.Parse(code[0].ToString()) * 3 + int.Parse(code[1].ToString()) * 9 +
				int.Parse(code[2].ToString()) * 3 + int.Parse(code[3].ToString()) * 9 + 
				int.Parse(code[4].ToString()) * 3;
			var symbolParity = symParitySetAdd5[checkSum % 10];
			int numberDigits = 5;
			for (int index = 0; index < numberDigits; index++)
			{	
				int currentNumber = int.Parse(code[index].ToString());
				char parity = symbolParity[index];
                var sym = new EanBarInfo((Ean13Symbol)((int)Ean13Symbol.ComboA0 + currentNumber), code[index], true);
				if (parity != 'o')
				{
					sym.SymbolType = Ean13Symbol.ComboB0 + currentNumber;
				}
				barsArray.Add(sym);
				if (index < numberDigits - 1)
				{
					barsArray.Add(new EanBarInfo(Ean13Symbol.GuardAddDelineator, ' ', true));
				}
			}

            if (isLast)
            {
                barsArray.Add(new EanBarInfo(Ean13Symbol.SpaceRight, (ShowQuietZoneIndicator ? '>' : ' '), true));
            }
            else
            {
                barsArray.Add(new EanBarInfo(Ean13Symbol.SpaceBeforeAdd, ' ', true));
            }

            return barsArray;
		}

		protected string makeLonger(string symString)
		{
            var sb = new StringBuilder();
			for (int index = 0; index < symString.Length; index++)
			{
				char sym = symString[index];
				switch (sym)
				{
					case '4': sym = '8'; break;
					case '5': sym = '9'; break;
					case '6': sym = 'a'; break;
					case '7': sym = 'b'; break;
				}
				sb.Append(sym);
			}
			return sb.ToString();
		}

		/// <summary>
		/// Returns width of one symbol in module.
		/// </summary>
		protected override float GetSymbolWidth(char symbol)
		{
			float symbolWidth;
			switch (symbol)
			{
				case '0':
				case '4':
				case '8':
				case 'c':
				case 'd':
				case 'e':
				case 'f':
					symbolWidth = 1f;
					break;
				case '1':
				case '5':
				case '9':
					symbolWidth = 1f * EanWideToNarrowRatio;
					break;
				case '2':
				case '6':
				case 'a':
					symbolWidth = 1.5f * EanWideToNarrowRatio;
					break;
				case '3':
				case '7':
				case 'b':
					symbolWidth = 2f * EanWideToNarrowRatio;
					break;
				case 'x':
					symbolWidth = EanSpaceLeft;
					break;
				case 'y':
					symbolWidth = EanSpaceRight;
					break;
				case 'z':
					symbolWidth = EanSpaceBeforeAdd;
					break;
				default:
					symbolWidth = 1f;
					break;
			}
			return symbolWidth;
		}

		/// <summary>
		/// Returns true is char is space.
		/// </summary>
		protected override bool IsSymbolSpace(char symbol)
		{
			bool isSpace;
			switch (symbol)
			{
				case '0':
				case '1':
				case '2':
				case '3':
					isSpace = true;
					break;
				case 'x':
				case 'y':
				case 'z':
					isSpace = true;
					break;
				default:
					isSpace = false;
					break;
			}
			return isSpace;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Draws the bar code with the specified parameters.
		/// </summary>
		/// <param name="context">Context for drawing.</param>
        /// <param name="barCode">Component that invokes drawing.</param>
		/// <param name="rect">The rectangle that shows coordinates for drawing.</param>
		/// <param name="zoom">Zoom of drawing.</param>
        public override void Draw(object context, StiBarCode barCode, RectangleF rect, float zoom)
		{
			var code = GetCode(barCode);
			code = CheckCodeSymbols(code, "0123456789") + new string('0', 13);
			var suppCode = CheckCodeSymbols(SupplementCode, "0123456789") + new string('0', 5);

            List<EanBarInfo> barsArray;
			if (SupplementType == StiEanSupplementType.None)
			{
				barsArray = MakeEan13Bars(ref code, true);
			}
			else
			{
				barsArray = MakeEan13Bars(ref code, false);
				if (SupplementType == StiEanSupplementType.TwoDigit)
				{
					MakeEanAdd2Bars(suppCode, barsArray, true);
				}
				else
				{
					MakeEanAdd5Bars(suppCode, barsArray, true);
				}
			}

			CalculateSizeEan(
				0,
				zoom,
				barsArray,
				rect,
				barCode);

			TranslateRect(context, rect, barCode);

			DrawEanBars(context, barsArray, barCode);

            RollbackTransform(context);
		}
		#endregion

        #region Methods.override
        public override StiBarCodeTypeService CreateNew() => new StiEAN13BarCodeType();
        #endregion

		public StiEAN13BarCodeType() : this(13f, 1f, StiEanSupplementType.None, null, true)
		{
		}

		public StiEAN13BarCodeType(float module, float height, 
            StiEanSupplementType supplementType, string supplementCodeValue, bool showQuietZoneIndicator)
		{
			this.module = module;
			this.height = height;
			this.SupplementType = supplementType;
			this.SupplementCode = supplementCodeValue;
            this.ShowQuietZoneIndicator = showQuietZoneIndicator;
		}
	}
}