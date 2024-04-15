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
	/// The class describes the Barcode type - Code-39 Extended.
	/// </summary>
	public class StiCode39ExtBarCodeType : StiCode39BarCodeType
	{
        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiCode39ExtBarCodeType;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            var list = new[]
            {
                propHelper.CheckSum(),
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
		public override string ServiceName => "Code39 Extended";
        #endregion

        #region Properties
        public override string DefaultCodeValue => "Abc123";
        #endregion

        #region Code39ExtendedTable
        protected string[] Code39ExtTable = new string[128]
		{
			"%U", "$A", "$B", "$C", "$D", "$E", "$F", "$G",
			"$H", "$I", "$J", "$K", "$L", "$M", "$N", "$O",
			"$P", "$Q", "$R", "$S", "$T", "$U", "$V", "$W",
			"$X", "$Y", "$Z", "%A", "%B", "%C", "%D", "%E",
			 " ", "/A", "/B", "/C", "/D", "/E", "/F", "/G",
			"/H", "/I", "/J", "/K", "/L",  "-",  ".", "/O",
			 "0",  "1",  "2",  "3",  "4",  "5",  "6",  "7",
			 "8",  "9", "/Z", "%F", "%G", "%H", "%I", "%J",
			"%V",  "A",  "B",  "C",  "D",  "E",  "F",  "G",
			 "H",  "I",  "J",  "K",  "L",  "M",  "N",  "O",
			 "P",  "Q",  "R",  "S",  "T",  "U",  "V",  "W",
			 "X",  "Y",  "Z", "%K", "%L", "%M", "%N", "%O",
			"%W", "+A", "+B", "+C", "+D", "+E", "+F", "+G",
			"+H", "+I", "+J", "+K", "+L", "+M", "+N", "+O",
			"+P", "+Q", "+R", "+S", "+T", "+U", "+V", "+W",
			"+X", "+Y", "+Z", "%P", "%Q", "%R", "%S", "%T" 
		};
		#endregion

		#region Methods
        public override void Draw(object context, StiBarCode barCode, RectangleF rect, float zoom)
		{		
			var code = GetCode(barCode);
		//	string checkedCode = CheckCodeSymbols(code, Code39Symbols);
			if (code == null)	code = string.Empty;
            var checkedCode = new StringBuilder();
            var checkedString = new StringBuilder();
			for (int index = 0; index < code.Length; index++)
			{
				int sym = (int)code[index];
				if (sym < 128) 
				{
					checkedCode.Append(Code39ExtTable[sym]);
					checkedString.Append((char)sym);
				}
			}
			code = checkedCode.ToString();

            #region make barsArray for output 
            var fullCode = new int[code.Length + (CheckSum == StiCheckSum.Yes ? 3 : 2)];
			fullCode[0] = Code39StartStopSymbolIndex;
			int checkSum = 0;
			for (int index = 0; index < code.Length; index++) 
			{
				fullCode[index + 1] = Code39Symbols.IndexOf(code[index]);
				checkSum += fullCode[index + 1];
			}
			if (CheckSum == StiCheckSum.Yes)
			{
				fullCode[fullCode.Length - 2] = checkSum % 43;
			}
			fullCode[fullCode.Length - 1] = fullCode[0]; // "*"

            var barsArray = new StringBuilder();
			for (int index = 0; index < fullCode.Length; index++)
			{	
				barsArray.Append(CodeToBar(Code39Table[fullCode[index]]));
			}
			#endregion

			CalculateSizeFull(
				Code39SpaceLeft,
				Code39SpaceRight,
				Code39SpaceTop,
				Code39SpaceBottom,
				Code39LineHeightShort,
				Code39LineHeightLong,
				Code39TextPosition,
				Code39TextHeight,
				Code39MainHeight,
				Code39LineHeightForCut, 
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
        public override StiBarCodeTypeService CreateNew() => new StiCode39ExtBarCodeType();
        #endregion

		public StiCode39ExtBarCodeType() : this(13f, 1f, 2.2f, StiCheckSum.Yes)
		{
		}

		public StiCode39ExtBarCodeType(float module, float height, float ratio, StiCheckSum checkSum) :
			base(module, height, ratio, checkSum)
		{
		}
	}
}