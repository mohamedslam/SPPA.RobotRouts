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
	/// The class describes the Barcode type - UPC-A.
	/// </summary>
	//[TypeConverter(typeof(StiUpcABarCodeTypeConverter))]
	public class StiUpcABarCodeType : StiEAN13BarCodeType
	{
        #region IStiPropertyGridObject

        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiUpcABarCodeType;

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
		public override string ServiceName => "UPC-A";
		#endregion

		#region Consts
		protected override float EanSpaceLeft {	 get {return 9f; } }
		protected override float EanSpaceRight { get {return 9f; } }
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
        protected List<EanBarInfo> MakeUpcABars(string code, bool isLast)
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
                new EanBarInfo(Ean13Symbol.SpaceLeft, code[1], false),
                new EanBarInfo(Ean13Symbol.GuardLeft, ' ', false),
                new EanBarInfo((Ean13Symbol)((int)Ean13Symbol.ComboA0 + int.Parse(code[1].ToString())), ' ', false, true)
            };
            for (int index = 0; index < 5; index++)
			{	
				int currentNumber = int.Parse(code[2 + index].ToString());
                var sym = new EanBarInfo((Ean13Symbol)((int)Ean13Symbol.ComboA0 + currentNumber), code[2 + index], false);
				barsArray.Add(sym);
			}

			barsArray.Add(new EanBarInfo(Ean13Symbol.GuardCenter, ' ', false));
			for (int index = 0; index < 5; index++)
			{	
				int currentNumber = int.Parse(code[7 + index].ToString());
                var sym = new EanBarInfo(Ean13Symbol.ComboC0 + currentNumber, code[7 + index], false);
				barsArray.Add(sym);
			}

			barsArray.Add(new EanBarInfo(Ean13Symbol.ComboC0 + int.Parse(code[12].ToString()), ' ', false, true));
			barsArray.Add(new EanBarInfo(Ean13Symbol.GuardRight, ' ', false));

			if (isLast)
			{
				barsArray.Add(new EanBarInfo(Ean13Symbol.SpaceRight, code[12], false));
			}
			else
			{
				barsArray.Add(new EanBarInfo(Ean13Symbol.SpaceBeforeAdd, code[12], false));
			}

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
			code = '0' + CheckCodeSymbols(code, "0123456789") + new string('0', 12);
			string suppCode = CheckCodeSymbols(SupplementCode, "0123456789") + new string('0', 5);

            List<EanBarInfo> barsArray;
			if (SupplementType == StiEanSupplementType.None)
			{
				barsArray = MakeUpcABars(code, true);
			}
			else
			{
				barsArray = MakeUpcABars(code, false);
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
        public override StiBarCodeTypeService CreateNew() => new StiUpcABarCodeType();
        #endregion

		public StiUpcABarCodeType() : this(13f, 1f, StiEanSupplementType.None, null, false)
		{
		}

		public StiUpcABarCodeType(float module, float height, StiEanSupplementType supplementType, string supplementCodeValue, bool showQuietZoneIndicator) :
            base(module, height, supplementType, supplementCodeValue, showQuietZoneIndicator)
		{
		}
	}
}