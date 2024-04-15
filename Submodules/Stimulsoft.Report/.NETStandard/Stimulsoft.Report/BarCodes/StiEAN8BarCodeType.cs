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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace Stimulsoft.Report.BarCodes
{
    /// <summary>
    /// The class describes the Barcode type - EAN-8.
    /// </summary>
    public class StiEAN8BarCodeType : StiEAN13BarCodeType
	{
        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiEAN8BarCodeType;

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
		public override string ServiceName => "EAN-8";
        #endregion

        #region Properties
        public override string DefaultCodeValue => "12345678";
        #endregion

        #region Consts
        protected override float EanSpaceLeft {			get {return 7f; } }
		protected override float EanSpaceRight {		get {return 7f; } }
		protected override float EanLineHeightShort {	get {return 55.20f; } }
		protected override float EanMainHeight {		get {return 64.58f; } }
		#endregion

		#region Methods
        protected List<EanBarInfo> MakeEan8Bars(string code, bool isLast)
		{
			#region Calculate Ean8 check digit
			var dig = new int[8];
			for (int tempIndex = 0; tempIndex < 7; tempIndex++)
			{
				dig[tempIndex] = int.Parse(code[tempIndex].ToString());
			}
			int sum = (dig[0] + dig[2] + dig[4] + dig[6]) * 3 +
				dig[1] + dig[3] + dig[5];
			int checkDigit = 10 - (sum % 10);
			if (checkDigit == 10)
			{
				checkDigit = 0;
			}
			code = code.Substring(0,7) + (char)(checkDigit + 48);
            #endregion

            var barsArray = new List<EanBarInfo>
            {
                new EanBarInfo(Ean13Symbol.SpaceLeft, (ShowQuietZoneIndicator ? '<' : ' '), false),
                new EanBarInfo(Ean13Symbol.GuardLeft, ' ', false)
            };
            for (int index = 0; index < 4; index++)
			{	
				int currentNumber = int.Parse(code[0 + index].ToString());
                var sym = new EanBarInfo((Ean13Symbol)((int)Ean13Symbol.ComboA0 + currentNumber), code[0 + index], false);
				barsArray.Add(sym);
			}

			barsArray.Add(new EanBarInfo(Ean13Symbol.GuardCenter, ' ', false));
			for (int index = 0; index < 4; index++)
			{	
				int currentNumber = int.Parse(code[4 + index].ToString());
                var sym = new EanBarInfo(Ean13Symbol.ComboC0 + currentNumber, code[4 + index], false);
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
				barsArray = MakeEan8Bars(code, true);
			}
			else
			{
				barsArray = MakeEan8Bars(code, false);
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
        public override StiBarCodeTypeService CreateNew() => new StiEAN8BarCodeType();
        #endregion

		public StiEAN8BarCodeType() : this(13f, 1f, StiEanSupplementType.None, null, true)
		{
		}

        public StiEAN8BarCodeType(float module, float height, StiEanSupplementType supplementType, string supplementCodeValue, bool showQuietZoneIndicator) :
            base(module, height, supplementType, supplementCodeValue, showQuietZoneIndicator)
		{
		}	
	}
}