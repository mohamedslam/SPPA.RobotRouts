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
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.PropertyGrid;
using System.ComponentModel;
using System.Text;
using System.Drawing;

namespace Stimulsoft.Report.BarCodes
{
    /// <summary>
    /// The class describes the Barcode type - AustraliaPost4State.
    /// </summary>
    [TypeConverter(typeof(Stimulsoft.Report.BarCodes.Design.StiBarCodeTypeServiceConverter))]
	public class StiAustraliaPost4StateBarCodeType : StiBarCodeTypeService
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiBarCodeTypeService
            jObject.AddPropertyFloat("Module", Module, 20f);
            jObject.AddPropertyFloat("Height", Height, 1f);

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
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject

        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiAustraliaPost4StateBarCodeType;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            var list = new[]
            {
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
		public override string ServiceName => "Australia Post 4-state";
		#endregion

		#region AustraliaPost4State Tables
		protected string AustraliaPost4StateSymbolsC = "ABC DEF#GHIabcdeJKLfMNOgPQRhijklSTUmVWXnYZ0opqrs123t456u789vwxyz";
		protected string AustraliaPost4StateSymbolsN = "012_345_678_9";
		private string AustraliaPost4StateStartCode = "13";
		private string AustraliaPost4StateStopCode = "13";
        #endregion

        #region Properties
        public override string DefaultCodeValue => "1139987520";

        private float module = 20f;
        /// <summary>
        /// Gets or sets width of the most fine element of the bar code.
        /// </summary>
		[DefaultValue(20f)]
        [Description("Gets or sets width of the most fine element of the bar code.")]
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
				if (value < 20f)	module = 20f;
				if (value > 20f)	module = 20f;
			}
		}

		private float height = 1f;
        /// <summary>
        /// Gets os sets height factor of the bar code.
        /// </summary>
		[DefaultValue(1f)]
		[Browsable(false)]
        [Description("Gets os sets height factor of the bar code.")]
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
				if (value < 1f)	height = 1f;
				if (value > 1f)	height = 1f;
			}
		}

        internal override float LabelFontHeight => AustraliaPost4StateTextHeight;

		protected override bool PreserveAspectRatio => true;

		public override bool[] VisibleProperties
        {
            get
            {
                var props = new bool[visiblePropertiesCount];
                props[13] = true;

                return props;
            }
        }
		#endregion

        #region Consts
        protected const float AustraliaPost4StateSpaceLeft = 11.8f;
		protected const float AustraliaPost4StateSpaceRight = 11.8f;
		protected const float AustraliaPost4StateSpaceTop = 3.9f + 7f;
		protected const float AustraliaPost4StateSpaceBottom = 3.9f;
		protected const float AustraliaPost4StateLineHeightShort = AustraliaPost4StateLineHeightLong * 0.62f;
		protected const float AustraliaPost4StateLineHeightLong = 10f;
		protected const float AustraliaPost4StateTextPosition = 1f;
		protected const float AustraliaPost4StateTextHeight = 5f;
		protected const float AustraliaPost4StateMainHeight = AustraliaPost4StateSpaceTop + AustraliaPost4StateLineHeightLong + AustraliaPost4StateSpaceBottom;
		protected const float AustraliaPost4StateLineHeightForCut = AustraliaPost4StateLineHeightLong;
		protected override StringAlignment TextAlignment { get { return StringAlignment.Near; } }
		#endregion

		#region Methods

		#region Reed-Solomon Parity
		private static uint[,] mult = new uint[64,64];
		private static int[] gen = {0,0,0,0,0};

		/// <summary>
		/// Initialises the Reed-Solomon Parity generator.
		/// </summary>
		private static void RSInitialise() 
		{
			//Set up the constants required. primpoly is the binary representation of the primitive polynomial used
			//to construct the Galois field GF(64). Test is used to check when an element must be reduced modulo primpoly.
			uint primpoly = 67;
			uint test = 64;

			//The mult[,] array provides lookup table multiplication in GF(64). The two array indices are the elements to be 
			//multiplied, and the corresponding value is the product. Mult a field element by 0 is 0 and mult by 1 is itself
			for (int i = 0; i < 64; i++) 
			{
				mult[0, i] = 0;
				mult[1, i] = (uint)i;
			}

			//Multiplication by elements other than 0 or 1 requires the corresponding powers of alpha which is a root of primpoly.
			//Beginning with the zero power of alpha, which is 1, successive powers of alpha are obtained by shifting to the 
			//left. If the result exceeds 6 bits, then it is reduced modulo primpoly.  The rows of mult[,] are filled iteratively 
			//according to these powers.  Note that the `powers of alpha' representation is logarithmic, so that multiplication 
			//requires just an addition.  prev is the previous power of alpha, and next is the next power.  Because of the above
			//mentioned property of logarithms, the next row is just the prev row shifted to the left.
			uint prev = 1;
			for (int i = 1; i < 64; i++) 
			{
				uint next = prev << 1;
				if ((next & test) != 0)
				{
					next ^= primpoly;
				}
				for (int j = 0; j < 64; j++) 
				{
					mult[next, j] = mult[prev, j] << 1;
					if ((mult[next, j] & test) != 0)
					{
						mult[next, j] ^= primpoly;
					}
				}
				prev = next;
			}

			//Initialise the generator polynomial
			gen[0] = 48;   
			gen[1] = 17;
			gen[2] = 29;
			gen[3] = 30;
			gen[4] = 1;
		}

		/// <summary>
		/// Generates the Reed-Solomon Parity Symbols.
		/// </summary>
		private static uint[] RSEncode(uint[] infosymbols)
		{
			if (gen[0] == 0) RSInitialise();

			//The temp array is initialised with x^(n-k) m(x).  After division by g(x), temp will contain the parity symbols, 
			//p(x), in locations 0 to 4. Note that p(x) is the remainder after the division operation; inversion added for AustralianPost4State
			var temp = new uint[31];
			int len = infosymbols.Length;
			for (int i = 0; i < 4; i++) temp[i] = 0;
			for (int i = 0; i < len; i++) temp[i + 4] = infosymbols[len - 1 - i];

			//Perform the division by the generator polynomial g(x). This is accomplished using k iterations of long division, 
			//where g(x) times the most significant symbol in the dividend are subtracted.
			for (int i = len - 1; i >= 0; i--) 
			{
				for (int j = 0; j <= 4; j++) 
				{
					temp[i + j] = temp[i + j] ^ (mult[gen[j], temp[4 + i]]);
				}
			}

			//Place the parity symbols in the array; inversion added for AustralianPost4State
			var parity4 = new uint[4];
			for (int i = 0; i < 4; i++)
			{
				parity4[i] = temp[3 - i];
			}
			return parity4;
		}
		#endregion

		private string CharTo4State(char inputChar, bool useTableC)
		{
			var outputString = new StringBuilder();
			if (useTableC)
			{
                int inputNumber = AustraliaPost4StateSymbolsC.IndexOf(inputChar);
				outputString.Append((char)(((inputNumber >> 4) & 0x03) + '0'));
				outputString.Append((char)(((inputNumber >> 2) & 0x03) + '0'));
				outputString.Append((char)((inputNumber & 0x03) + '0'));
			}
			else
			{
                int inputNumber = AustraliaPost4StateSymbolsN.IndexOf(inputChar);
				outputString.Append((char)(((inputNumber >> 2) & 0x03) + '0'));
				outputString.Append((char)((inputNumber & 0x03) + '0'));
			}
			return outputString.ToString();
		}

		private string StateToBar(string inputCode)
		{
			var outputBar = new StringBuilder();
			for (int index = 0; index < inputCode.Length; index++)
			{
				switch (inputCode[index])
				{
					case '0':
						outputBar.Append("c");
						break;
					case '1':
						outputBar.Append("d");
						break;
					case '2':
						outputBar.Append("e");
						break;
					case '3':
						outputBar.Append("f");
						break;
				}
				outputBar.Append("0");
			}
			return outputBar.ToString();
		}

		private bool MakeBarsArray(ref string code, ref string barsArray, ref string errorString)
		{
			if (code.Length < 10)
			{
				errorString = "Data too short";
				return false;
			}

			string FCC = code.Substring(0, 2);
			string DPID = code.Substring(2, 8);
			string CustInfo = code.Substring(10);

			#region Check FCC
			int custInfoLength = -1;
			switch (FCC)
			{
				case "11":
				case "87":
				case "45":
				case "92":
					custInfoLength = 0;
					break;

				case "59":
					custInfoLength = 16;
					break;

				case "62":
				case "44":
					custInfoLength = 31;
					break;
			}
			if (custInfoLength < 0)
			{
				errorString = "Unknown FCC";
				return false;
			}
			#endregion

			int custInfoSymbols = custInfoLength / 3;
			if (CustInfo.Length > custInfoSymbols)
			{
				errorString = "CustomerInfo too long";
				return false;
			}
			if (custInfoSymbols > CustInfo.Length)
			{
				custInfoSymbols = CustInfo.Length;
			}

			#region Make tempBarsArray
			var tempBarsArray = new StringBuilder();
			tempBarsArray.Append(CharTo4State(FCC[0], false));
			tempBarsArray.Append(CharTo4State(FCC[1], false));
			tempBarsArray.Append(CharTo4State(DPID[0], false));
			tempBarsArray.Append(CharTo4State(DPID[1], false));
			tempBarsArray.Append(CharTo4State(DPID[2], false));
			tempBarsArray.Append(CharTo4State(DPID[3], false));
			tempBarsArray.Append(CharTo4State(DPID[4], false));
			tempBarsArray.Append(CharTo4State(DPID[5], false));
			tempBarsArray.Append(CharTo4State(DPID[6], false));
			tempBarsArray.Append(CharTo4State(DPID[7], false));
			for (int index = 0; index < custInfoSymbols; index++)
			{	
				tempBarsArray.Append(CharTo4State(CustInfo[index], true));
			}
			if (tempBarsArray.Length < 21 + custInfoLength)
			{
				tempBarsArray.Append('3', 21 + custInfoLength - tempBarsArray.Length);
			}
			#endregion

			#region Calculate Reed Solomon parity
			int triplesCount = tempBarsArray.Length / 3;
			var triples = new uint[triplesCount];
			for (int index = 0; index < triplesCount; index++)
			{
				triples[index] = (((uint)tempBarsArray[index * 3 + 0] - (uint)'0') << 4) +
								(((uint)tempBarsArray[index * 3 + 1] - (uint)'0') << 2) +
								((uint)tempBarsArray[index * 3 + 2] - (uint)'0');
			}
			var rs = RSEncode(triples);
			for (int index = 0; index < 4; index++)
			{
				tempBarsArray.Append((char)(((rs[index] >> 4) & 0x03) + '0'));
				tempBarsArray.Append((char)(((rs[index] >> 2) & 0x03) + '0'));
				tempBarsArray.Append((char)((rs[index] & 0x03) + '0'));
			}
			#endregion

			barsArray = StateToBar(AustraliaPost4StateStartCode + tempBarsArray.ToString() + AustraliaPost4StateStopCode);
			code = string.Format("{0} {1} {2} {3} {4} {5} {6}",
                FCC,
				DPID,
				CustInfo.Substring(0, custInfoSymbols),
				rs[0],
				rs[1],
				rs[2],
				rs[3]);
			return true;
		}

        public override void Draw(object context, StiBarCode barCode, RectangleF rect, float zoom)
		{
			var code = GetCode(barCode);
			code = CheckCodeSymbols(code, AustraliaPost4StateSymbolsC);

			var barsArray = string.Empty;
			var errorString = string.Empty;
			if (MakeBarsArray(ref code, ref barsArray, ref errorString))
			{
				CalculateSizeFull(
					AustraliaPost4StateSpaceLeft,
					AustraliaPost4StateSpaceRight,
					AustraliaPost4StateSpaceTop,
					AustraliaPost4StateSpaceBottom,
					AustraliaPost4StateLineHeightShort,
					AustraliaPost4StateLineHeightLong,
					AustraliaPost4StateTextPosition,
					AustraliaPost4StateTextHeight,
					AustraliaPost4StateMainHeight,
					AustraliaPost4StateLineHeightForCut, 
					1,
					zoom,
					code,
					code,
					barsArray,
					rect,
					barCode);

                DrawBarCode(context, rect, barCode); 
			}
			else
			{
				if (errorString.Length > 0)
				{
                    DrawBarCodeError(context, rect, barCode, errorString);
				}
				else
				{
                    DrawBarCodeError(context, rect, barCode);
				}
			}
		}
		#endregion

        #region Methods.override
        public override StiBarCodeTypeService CreateNew() => new StiAustraliaPost4StateBarCodeType();
        #endregion

		public StiAustraliaPost4StateBarCodeType() : this(20f, 1f)
		{
		}

		public StiAustraliaPost4StateBarCodeType(float module, float height)
		{
			this.module = module;
			this.height = height;
		}
	}
}