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

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Stimulsoft.Base;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.PropertyGrid;

namespace Stimulsoft.Report.BarCodes
{
	/// <summary>
	/// The class describes the Barcode type - UPC-E.
	/// </summary>
	public class StiUpcEBarCodeType : StiEAN13BarCodeType
	{
        #region IStiPropertyGridObject

        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiUpcEBarCodeType;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            var list = new[]
            {
                propHelper.SupplementCode(),
                propHelper.fHeight(),
                propHelper.Module(),
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
		public override string ServiceName => "UPC-E";
		#endregion

		#region Consts
		protected string[] symParitySet = {	"eeeooo",	//0
											"eeoeoo",	//1
											"eeooeo",	//2
											"eeoooe",	//3
											"eoeeoo",	//4
											"eooeeo",	//5
											"eoooee",	//6
											"eoeoeo",	//7
											"eoeooe",	//8
											"eooeoe"	//9
										  };

		protected override float EanSpaceLeft {	 get {return 9f; } }
		protected override float EanSpaceRight { get {return 7f; } }
        #endregion

        #region Properties
        public override string DefaultCodeValue => "01234567";

        public override bool[] VisibleProperties
        {
            get
            {
                var props = new bool[visiblePropertiesCount];
                props[11] = true;
                props[13] = true;
                props[19] = true;
                props[20] = true;

                return props;
            }
        }
        #endregion

        #region Properties Browsable (false)
        [Browsable(false)]
        [StiNonSerialized]
        public override bool ShowQuietZoneIndicator
        {
            get
            {
                return false;
            }
            set
            {
            }
        }
        #endregion

		#region Methods
        protected List<EanBarInfo> MakeUpcEBars(string code, bool isLast)
		{
            var barsArray = new List<EanBarInfo>
            {
                new EanBarInfo(Ean13Symbol.SpaceLeft, code[0], false),
                new EanBarInfo(Ean13Symbol.GuardLeft, ' ', false)
            };

            var symbolParity = symParitySet[int.Parse(code[7].ToString())];
			for (int index = 0; index < 6; index++)
			{	
				int currentNumber = int.Parse(code[1 + index].ToString());
				char parity = symbolParity[index];
				if (code[0] != '0') 
				{
					if (parity == 'o')	parity = 'e';
					else parity = 'o';
				}

                var sym = new EanBarInfo((Ean13Symbol)((int)Ean13Symbol.ComboA0 + currentNumber), code[1 + index], false);
				if (parity != 'o')
				{
					sym.SymbolType = Ean13Symbol.ComboB0 + currentNumber;
				}
				barsArray.Add(sym);
			}

			barsArray.Add(new EanBarInfo(Ean13Symbol.GuardSpecial, ' ', false));
			barsArray.Add(new EanBarInfo((isLast ? Ean13Symbol.SpaceRight : Ean13Symbol.SpaceBeforeAdd), code[7], false));
			return barsArray;
		}

		/// <summary>
		/// Draws the bar code with the specified parameters.
		/// </summary>
		/// <param name="context">Context for drawing.</param>
        /// <param name="barCode">Component that invokes drawing.</param>
		/// <param name="rect">The rectangle that shows coordinates for drawing.</param>
		/// <param name="zoom">Zoom of drawing.</param>
        public override void Draw(object context, StiBarCode barCode, RectangleF rect, float zoom)
		{
			string code = GetCode(barCode);
			code = CheckCodeSymbols(code, "0123456789") + new string('0', 8);
			string suppCode = CheckCodeSymbols(SupplementCode, "0123456789") + new string('0', 5);

            List<EanBarInfo> barsArray;
			if (SupplementType == StiEanSupplementType.None)
			{
				barsArray = MakeUpcEBars(code, true);
			}
			else
			{
				barsArray = MakeUpcEBars(code, false);
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
        public override StiBarCodeTypeService CreateNew() => new StiUpcEBarCodeType();
        #endregion

		public StiUpcEBarCodeType() : this(13f, 1f, StiEanSupplementType.None, null, false)
		{
		}

		public StiUpcEBarCodeType(float module, float height, StiEanSupplementType supplementType, string supplementCodeValue, bool showQuietZoneIndicator) :
            base(module, height, supplementType, supplementCodeValue, showQuietZoneIndicator)
		{
		}	
	}
}