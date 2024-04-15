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
using System.ComponentModel;
using System.Drawing;
using System.Text;
using Stimulsoft.Base;
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.PropertyGrid;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

namespace Stimulsoft.Report.BarCodes
{
	/// <summary>
	/// The class describes the Barcode type - Plessey.
	/// </summary>
	[TypeConverter(typeof(Stimulsoft.Report.BarCodes.Design.StiPlesseyBarCodeTypeConverter))]
	public class StiPlesseyBarCodeType : StiBarCodeTypeService
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiPlesseyBarCodeType
            jObject.AddPropertyFloat("Module", Module, 8f);
            jObject.AddPropertyFloat("Height", Height, 1f);
            jObject.AddPropertyEnum("CheckSum1", CheckSum1, StiPlesseyCheckSum.None);
            jObject.AddPropertyEnum("CheckSum2", CheckSum2, StiPlesseyCheckSum.None);

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

                    case "CheckSum1":
                        this.checkSum1 = property.DeserializeEnum<StiPlesseyCheckSum>();
                        break;

                    case "CheckSum2":
                        this.checkSum2 = property.DeserializeEnum<StiPlesseyCheckSum>();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiPlesseyBarCodeType;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            var list = new[]
            {
                propHelper.CheckSum1(),
                propHelper.CheckSum2(),
                propHelper.fHeight(),
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
		public override string ServiceName => "Plessey";
		#endregion

		#region PlesseyTable
		protected string PlesseySymbols = "0123456789ABCDEF";
		private string[] PlesseyTable = new string[16]
		{
			"0000",	//0
			"1000",	//1
			"0100",	//2
			"1100",	//3
			"0010",	//4
			"1010",	//5
			"0110",	//6
			"1110",	//7
			"0001",	//8
			"1001",	//9
			"0101",	//A
			"1101",	//B
			"0011",	//C
			"1011",	//D
			"0111",	//E
			"1111"	//F
		};
		private string PlesseyStartCode = "1101";
		private string PlesseyStopCode = "11";
        #endregion

        #region Properties
        public override string DefaultCodeValue => "1234567";

        private float module = 8f;
        /// <summary>
        /// Gets or sets width of the most fine element of the bar code.
        /// </summary>
        [Description("Gets or sets width of the most fine element of the bar code.")]
		[DefaultValue(8f)]
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
				if (value < 1f)	module = 1f;
				if (value > 40f)	module = 40f;
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
				if (value < 0.5f)	height = 0.5f;
				if (value > 2.0f)	height = 2.0f;
			}
		}

		private StiPlesseyCheckSum checkSum1 = StiPlesseyCheckSum.None;
        /// <summary>
        /// Gets or sets mode of CheckSum1.
        /// </summary>
		[DefaultValue(StiPlesseyCheckSum.None)]
		[StiSerializable]
        [Description("Gets or sets mode of CheckSum1.")]
        [TypeConverter(typeof(StiEnumConverter))]
		[Editor(StiEditors.Enum, typeof(UITypeEditor))]
		[StiCategory("BarCode")]
        public StiPlesseyCheckSum CheckSum1
		{
			get
			{
				return checkSum1;
			}
			set
			{
				checkSum1 = value;
				if (checkSum1 == StiPlesseyCheckSum.None)
				{
					CheckSum2 = StiPlesseyCheckSum.None;
				}
			}
		}

		private StiPlesseyCheckSum checkSum2 = StiPlesseyCheckSum.None;
        /// <summary>
        /// Gets or sets mode of CheckSum2.
        /// </summary>
		[DefaultValue(StiPlesseyCheckSum.None)]
		[StiSerializable]
        [Description("Gets or sets mode of CheckSum2.")]
        [TypeConverter(typeof(StiEnumConverter))]
		[Editor(StiEditors.Enum, typeof(UITypeEditor))]
		[StiCategory("BarCode")]
        public StiPlesseyCheckSum CheckSum2
		{
			get
			{
				return checkSum2;
			}
			set
			{
				checkSum2 = value;
				if ((checkSum2 != StiPlesseyCheckSum.None) && (CheckSum1 == StiPlesseyCheckSum.None))
				{
					CheckSum1 = StiPlesseyCheckSum.Modulo10;
				}
			}
		}

        internal override float LabelFontHeight => PlesseyTextHeight;

        public override bool[] VisibleProperties
        {
            get
            {
                var props = new bool[visiblePropertiesCount];
                props[4] = true;
                props[5] = true;
                props[11] = true;
                props[13] = true;

                return props;
            }
        }
		#endregion

		#region Consts
		protected const float PlesseySpaceLeft = 4*5f;
		protected const float PlesseySpaceRight = 4*5f;
		protected const float PlesseySpaceTop = 0f;
		protected const float PlesseySpaceBottom = 1f;
		protected const float PlesseyLineHeightShort = 70f; //maybe
		protected const float PlesseyLineHeightLong = PlesseyLineHeightShort;
		protected const float PlesseyTextPosition = PlesseyLineHeightShort + PlesseySpaceBottom;
		protected const float PlesseyTextHeight = 11.5f;
		protected const float PlesseyMainHeight = 84f;
		protected const float PlesseyLineHeightForCut = PlesseyLineHeightShort;
		#endregion

		#region Methods
		private string CodeToBar(string inputCode)
		{
			var outputBar = new StringBuilder();
			for (int index = 0; index < inputCode.Length; index++)
			{
                var currentsym = new StringBuilder();
				if (inputCode[index] == '1') 
				{
					currentsym.Append("71");	//maybe "61" for standard Plessy
				}
				else
				{
					currentsym.Append("53");	//maybe "43" for standard Plessy
				}
				outputBar.Append(currentsym);
			}
			return outputBar.ToString();
		}

        public override void Draw(object context, StiBarCode barCode, RectangleF rect, float zoom)
		{
			var code = GetCode(barCode);
			code = CheckCodeSymbols(code, PlesseySymbols);
            var codeText = new StringBuilder(code);

			#region make barsArray for output 
			int codeLength = code.Length;
			if (CheckSum1 != StiPlesseyCheckSum.None)codeLength++;
			if (CheckSum2 != StiPlesseyCheckSum.None)codeLength++;
			var fullCode = new int[codeLength];
			for (int index = 0; index < code.Length; index++) 
			{
                fullCode[index] = PlesseySymbols.IndexOf(code[index]);
			}

			#region calculate checksum 1
			if (CheckSum1 != StiPlesseyCheckSum.None) 
			{
				int sum = 0;

				if (CheckSum1 == StiPlesseyCheckSum.Modulo10) 
				{
					#region checksum Modulo10
					long sum1 = 0;
					long sum2 = 0;
					long multiplier = 1;
					bool odd = false;
					for (int index = code.Length - 1; index >= 0; index--)
					{
						if (odd == false)
						{
							sum1 += fullCode[index] * multiplier;
							multiplier *= 10;
						}
						else
						{
							sum2 += fullCode[index];
						}
						odd = !odd;
					}
					sum1 = sum1 * 2;
					long sum3 = 0;
					while (sum1 >=10)
					{
						sum3 += sum1 % 10;
						sum1 /= 10;
					}
					sum3 += sum1;
					sum = (int)((sum3 + sum2) % 10);
					if (sum != 0)
					{
						sum = 10 - sum;
					}
					#endregion
				}

				if (CheckSum1 == StiPlesseyCheckSum.Modulo11) 
				{
					#region checksum Modulo11
					int sum1 = 0;
					int multiplier = 2;
					for (int index = code.Length - 1; index >= 0; index--)
					{
						sum1 += fullCode[index] * multiplier;
						multiplier++;
						if (multiplier > 7)
						{
							multiplier = 2;
						}
					}
					sum = sum1 % 11;
					if (sum != 0)
					{
						sum = 11 - sum;
					}
					#endregion
				}

				fullCode[code.Length] = sum;
				codeText.Append(PlesseySymbols[sum]);
			}
			#endregion

			#region calculate checksum 2
			if (CheckSum2 != StiPlesseyCheckSum.None) 
			{
				int sum = 0;

				if (CheckSum2 == StiPlesseyCheckSum.Modulo10) 
				{
					#region checksum Modulo10
					long sum1 = 0;
					long sum2 = 0;
					long multiplier = 1;
					bool odd = false;
					for (int index = code.Length; index >= 0; index--)	//include first checksum
					{
						if (odd == false)
						{
							sum1 += fullCode[index] * multiplier;
							multiplier *= 10;
						}
						else
						{
							sum2 += fullCode[index];
						}
						odd = !odd;
					}
					sum1 = sum1 * 2;
					long sum3 = 0;
					while (sum1 >=10)
					{
						sum3 += sum1 % 10;
						sum1 /= 10;
					}
					sum3 += sum1;
					sum = (int)((sum3 + sum2) % 10);
					if (sum != 0)
					{
						sum = 10 - sum;
					}
					#endregion
				}

				if (CheckSum2 == StiPlesseyCheckSum.Modulo11) 
				{
					#region checksum Modulo11
					int sum1 = 0;
					int multiplier = 2;
					for (int index = code.Length; index >= 0; index--)	//include first checksum
					{
						sum1 += fullCode[index] * multiplier;
						multiplier++;
						if (multiplier > 7)
						{
							multiplier = 2;
						}
					}
					sum = sum1 % 11;
					if (sum != 0)
					{
						sum = 11 - sum;
					}
					#endregion
				}

				fullCode[code.Length + 1] = sum;
				codeText.Append(PlesseySymbols[sum]);
			}
            #endregion

            var tempBarsArray = new StringBuilder();
			tempBarsArray.Append(PlesseyStartCode);
			for (int index = 0; index < fullCode.Length; index++)
			{	
				tempBarsArray.Append(PlesseyTable[fullCode[index]]);
			}
			tempBarsArray.Append(PlesseyStopCode);
            var barsArray = new StringBuilder(CodeToBar(tempBarsArray.ToString()));
			#endregion

			CalculateSizeFull(
				PlesseySpaceLeft,
				PlesseySpaceRight,
				PlesseySpaceTop,
				PlesseySpaceBottom,
				PlesseyLineHeightShort,
				PlesseyLineHeightLong,
				PlesseyTextPosition,
				PlesseyTextHeight,
				PlesseyMainHeight,
				PlesseyLineHeightForCut, 
				1.667f,	//maybe 2 for standard Plessy
				zoom,
				code,
				codeText.ToString(),
				barsArray.ToString(),
				rect,
				barCode);

            DrawBarCode(context, rect, barCode); 
		}
		#endregion

        #region Methods.override
        public override StiBarCodeTypeService CreateNew() => new StiPlesseyBarCodeType();
        #endregion

		public StiPlesseyBarCodeType() : this(8f, 1f, StiPlesseyCheckSum.None, StiPlesseyCheckSum.None)
		{
		}

		public StiPlesseyBarCodeType(float module, float height, StiPlesseyCheckSum checkSum1, StiPlesseyCheckSum checkSum2)
		{
			this.module = module;
			this.height = height;
			this.checkSum1 = checkSum1;
			this.checkSum2 = checkSum2;
		}
	}
}