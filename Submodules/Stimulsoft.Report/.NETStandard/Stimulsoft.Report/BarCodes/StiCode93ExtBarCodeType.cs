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
using Stimulsoft.Report.PropertyGrid;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace Stimulsoft.Report.BarCodes
{
    /// <summary>
    /// The class describes the Barcode type - Code93 Extended.
    /// </summary>
    public class StiCode93ExtBarCodeType : StiCode93BarCodeType
	{
        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiCode93ExtBarCodeType;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            var list = new[]
            {
                propHelper.fHeight(),
                propHelper.Module(),
                propHelper.fRatio()
            };
            objHelper.Add(StiPropertyCategories.Main, list);

            return objHelper;
        }
        #endregion

		#region ServiceName
		/// <summary>
		/// Gets a service name.
		/// </summary>
		public override string ServiceName => "Code93 Extended";
        #endregion

        #region Properties
        public override string DefaultCodeValue => "Abc123";
        #endregion

        #region Code93ExtendedTable
        private string Code93ExtSymbols = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ-. $/+%<]>[";
		private string[] Code93ExtTable = new string[128]
		{
			"]U", "<A", "<B", "<C", "<D", "<E", "<F", "<G",
			"<H", "<I", "<J", "<K", "<L", "<M", "<N", "<O",
			"<P", "<Q", "<R", "<S", "<T", "<U", "<V", "<W",
			"<X", "<Y", "<Z", "]A", "]B", "]C", "]D", "]E",
			 " ", ">A", ">B", ">C", ">D", ">E", ">F", ">G",
			">H", ">I", ">J", ">K", ">L",  "-",  ".", ">O",
			 "0",  "1",  "2",  "3",  "4",  "5",  "6",  "7",
			 "8",  "9", ">Z", "]F", "]G", "]H", "]I", "]J",
			"]V",  "A",  "B",  "C",  "D",  "E",  "F",  "G",
			 "H",  "I",  "J",  "K",  "L",  "M",  "N",  "O",
			 "P",  "Q",  "R",  "S",  "T",  "U",  "V",  "W",
			 "X",  "Y",  "Z", "]K", "]L", "]M", "]N", "]O",
			"]W", "[A", "[B", "[C", "[D", "[E", "[F", "[G",
			"[H", "[I", "[J", "[K", "[L", "[M", "[N", "[O",
			"[P", "[Q", "[R", "[S", "[T", "[U", "[V", "[W",
			"[X", "[Y", "[Z", "]P", "]Q", "]R", "]S", "]T" 
		};
		#endregion

		#region Methods
        public override void Draw(object context, StiBarCode barCode, RectangleF rect, float zoom)
		{		
			var code = GetCode(barCode);
		//	code = CheckCodeSymbols(code, Code93Symbols);
			if (code == null)	code = string.Empty;
            var checkedCode = new StringBuilder();
            var checkedString = new StringBuilder();
			for (int index = 0; index < code.Length; index++)
			{
				int sym = (int)code[index];
				if (sym < 128) 
				{
					checkedCode.Append(Code93ExtTable[sym]); 
					checkedString.Append((char)sym);
				}
			}
			code = checkedCode.ToString();

            #region make barsArray for output 
            var fullCode = new int[code.Length + 4];
			for (int index = 0; index < code.Length; index++) 
			{
                fullCode[index + 1] = Code93ExtSymbols.IndexOf(code[index]);
			}

			#region checksum 1
			int sum1 = 0;
			int multiplier = 1;
			for (int index = code.Length - 1; index >= 0; index--)
			{
				sum1 += fullCode[index + 1] * multiplier;
				multiplier++;
				if (multiplier > 20)
				{
					multiplier = 1;
				}
			}
			fullCode[fullCode.Length - 3] = sum1 % 47;
			#endregion

			#region checksum 2
			int sum2 = 0;
			multiplier = 1;
			for (int index = code.Length; index >= 0; index--)
			{
				sum2 += fullCode[index + 1] * multiplier;
				multiplier++;
				if (multiplier > 15)
				{
					multiplier = 1;
				}
			}
			fullCode[fullCode.Length - 2] = sum2 % 47;
			#endregion

			fullCode[0] = Code93Table.Length-2;	//start
			fullCode[fullCode.Length - 1] = Code93Table.Length-1;   //stop

            var barsArray = new StringBuilder();
			for (int index = 0; index < fullCode.Length; index++)
			{	
				barsArray.Append(CodeToBar(Code93Table[fullCode[index]]));
			}
			#endregion

			CalculateSizeFull(
				Code93SpaceLeft,
				Code93SpaceRight,
				Code93SpaceTop,
				Code93SpaceBottom,
				Code93LineHeightShort,
				Code93LineHeightLong,
				Code93TextPosition,
				Code93TextHeight,
				Code93MainHeight,
				Code93LineHeightForCut, 
				Ratio,
				zoom,
				code,
				checkedString.ToString(),
				barsArray.ToString(),
				rect,
				barCode);

            DrawBarCode(context, rect, barCode); 
		}
		#endregion 
        
        #region Methods.override
        public override StiBarCodeTypeService CreateNew() => new StiCode93ExtBarCodeType();
        #endregion

		public StiCode93ExtBarCodeType() : this(13f, 1f, 2.2f)
		{
		}

		public StiCode93ExtBarCodeType(float module, float height, float ratio) :
			base(module, height, ratio)
		{
		}
	}
}