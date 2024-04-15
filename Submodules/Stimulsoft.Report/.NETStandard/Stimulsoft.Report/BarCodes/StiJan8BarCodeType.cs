#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Stimulsoft.Base;
using Stimulsoft.Report.BarCodes.Design;
using Stimulsoft.Report.PropertyGrid;

namespace Stimulsoft.Report.BarCodes
{
	/// <summary>
	/// The class describes the Barcode type - JAN-8.
	/// </summary>
	[TypeConverter(typeof(StiEAN13BarCodeTypeConverter))]
	public class StiJan8BarCodeType : StiEAN8BarCodeType
	{
        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiJan8BarCodeType;

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
		public override string ServiceName => "JAN-8";
        #endregion

        #region Properties
        public override string DefaultCodeValue => "4523456789012";

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
			string fullCode = GetCode(barCode);
			string code = CheckCodeSymbols(fullCode, "0123456789") + new string('0', 13);
			string suppCode = CheckCodeSymbols(SupplementCode, "0123456789") + new string('0', 5);

            if (!(code.StartsWith("45", StringComparison.InvariantCulture) || code.StartsWith("49", StringComparison.InvariantCulture)))
			{
                DrawBarCodeError(context, rect, barCode);
				return;
			}

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
				0f,
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
        public override StiBarCodeTypeService CreateNew() => new StiJan8BarCodeType();
        #endregion

		public StiJan8BarCodeType() : this(13f, 1f, StiEanSupplementType.None, null, true)
		{
		}

		public StiJan8BarCodeType(float module, float height, StiEanSupplementType supplementType, string supplementCodeValue, bool showQuietZoneIndicator) :
            base(module, height, supplementType, supplementCodeValue, showQuietZoneIndicator)
		{
		}
	}
}