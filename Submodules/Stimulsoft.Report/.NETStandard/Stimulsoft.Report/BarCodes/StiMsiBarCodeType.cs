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

using System.ComponentModel;
using System.Drawing;
using System.Text;
using Stimulsoft.Base;
using Stimulsoft.Report.PropertyGrid;

namespace Stimulsoft.Report.BarCodes
{
	/// <summary>
	/// The class describes the Barcode type - Msi.
	/// </summary>
	public class StiMsiBarCodeType : StiPlesseyBarCodeType
	{
        #region IStiPropertyGridObject

        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiMsiBarCodeType;

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
		public override string ServiceName => "Msi";
        #endregion

        #region Properties
        public override string DefaultCodeValue => "1234567";

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

		#region MsiTable
		private string[] MsiTable = new string[16]
		{
			"0000",	//0
			"0001",	//1
			"0010",	//2
			"0011",	//3
			"0100",	//4
			"0101",	//5
			"0110",	//6
			"0111",	//7
			"1000",	//8
			"1001",	//9
			"1010",	//A
			"1011",	//B
			"1100",	//C
			"1101",	//D
			"1110",	//E
			"1111"	//F
		};
		private string MsiStartCode = "1";
		private string MsiStopCode = "00";
		#endregion

		#region Methods
		protected string CodeToBarMsi(string inputCode)
		{
            var outputBar = new StringBuilder();
			for (int index = 0; index < inputCode.Length; index++)
			{
                var currentsym = new StringBuilder();
				if (inputCode[index] == '1') 
				{
					currentsym.Append("71");
				}
				else
				{
					currentsym.Append("53");
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
			tempBarsArray.Append(MsiStartCode);
			for (int index = 0; index < fullCode.Length; index++)
			{	
				tempBarsArray.Append(MsiTable[fullCode[index]]);
			}
			tempBarsArray.Append(MsiStopCode);
            var barsArray = new StringBuilder(CodeToBarMsi(tempBarsArray.ToString()));
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
				1.667f,
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
        public override StiBarCodeTypeService CreateNew() => new StiMsiBarCodeType();
        #endregion

		public StiMsiBarCodeType() : this(8f, 1f, StiPlesseyCheckSum.Modulo10, StiPlesseyCheckSum.None)
		{
		}

		public StiMsiBarCodeType(float module, float height, StiPlesseyCheckSum checkSum1, StiPlesseyCheckSum checkSum2) :
			base(module, height, checkSum1, checkSum2)
		{
		}
	}
}