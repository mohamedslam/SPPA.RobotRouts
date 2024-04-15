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
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.PropertyGrid;
using System.ComponentModel;
using System.Drawing;

namespace Stimulsoft.Report.BarCodes
{
    public class StiUpcSup2BarCodeType : StiEAN13BarCodeType
	{
		#region IStiPropertyGridObject

		[Browsable(false)]
		public override StiComponentId ComponentId => StiComponentId.StiUpcSup2BarCodeType;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            var list = new[]
            {
                propHelper.fHeight(),
                propHelper.Module(),
                propHelper.ShowQuietZoneIndicator()
            };
            objHelper.Add(StiPropertyCategories.Main, list);

            return objHelper;
        }

        #endregion

		#region ServiceName
		/// <summary>
		/// Gets a service name.
		/// </summary>
		public override string ServiceName => "UPC-Supp2";
        #endregion

        #region Properties
        public override string DefaultCodeValue => "02";

        public override bool[] VisibleProperties
        {
            get
            {
                var props = new bool[visiblePropertiesCount];
                props[11] = true;
                props[13] = true;
                props[17] = true;

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
			var code = GetCode(barCode);
			code = CheckCodeSymbols(code, "0123456789") + new string('0', 2);

            var barsArray = MakeEanAdd2Bars(code, null, true);

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

		#region Properties Browsable (false)
		[Browsable(false)]
		[StiNonSerialized]
		public override StiEanSupplementType SupplementType
		{
			get
			{
				return base.SupplementType;
			}
			set
			{
				base.SupplementType = value;
			}
		}


		[Browsable(false)]
		[StiNonSerialized]
		public override string SupplementCode
		{
			get
			{
				return base.SupplementCode;
			}
			set
			{
				base.SupplementCode = value;
			}
		}
		#endregion

        #region Methods.override
        public override StiBarCodeTypeService CreateNew() => new StiUpcSup2BarCodeType();
        #endregion

		public StiUpcSup2BarCodeType() : this(13f, 1f, StiEanSupplementType.None, null, true)
		{
		}

        public StiUpcSup2BarCodeType(float module, float height, StiEanSupplementType supplementType, string supplementCodeValue, bool showQuietZoneIndicator) :
            base(module, height, supplementType, supplementCodeValue, showQuietZoneIndicator)
		{
		}
		
	}
}
