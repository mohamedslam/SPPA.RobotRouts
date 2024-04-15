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

using Stimulsoft.Base;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report.BarCodes.Design;
using Stimulsoft.Report.PropertyGrid;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

#if STIDRAWING
using Font = Stimulsoft.Drawing.Font;
using StringFormat = Stimulsoft.Drawing.StringFormat;
using Graphics = Stimulsoft.Drawing.Graphics;
#endif

namespace Stimulsoft.Report.BarCodes
{
    /// <summary>
    /// The class describes the Barcode type - ISBN-13.
    /// </summary>
    [TypeConverter(typeof(StiEAN13BarCodeTypeConverter))]
	public class StiIsbn13BarCodeType : StiEAN13BarCodeType
	{
        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiIsbn13BarCodeType;

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
		public override string ServiceName => "ISBN-13";
        #endregion

        #region Properties
        public override string DefaultCodeValue => "978-0-7356-2153-4";
        #endregion

        #region Fields
        protected const float IsbnOffsetY = 10f;
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

            List<EanBarInfo> barsArray;
			if (SupplementType == StiEanSupplementType.None)
			{
				barsArray = MakeEan13Bars(ref code, true);
			}
			else
			{
				barsArray = MakeEan13Bars(ref code, false);
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
				IsbnOffsetY,
				zoom,
				barsArray,
				rect,
				barCode);

			#region draw barcode
            TranslateRect(context, rect, barCode);

            DrawEanBars(context, barsArray, barCode);

            var foreBrush = new StiSolidBrush(barCode.ForeColor);

            bool needPixel = context is Graphics;
            float fontScale = needPixel ? 1 : 0.72f / 0.96f; 
            using (var sf = new StringFormat())			
            using (var barCodeFont = new Font(
                       StiFontCollection.GetFontFamily(barCode.Font.Name),
                       barCode.Font.Size * (barCode.BarCodeType.LabelFontHeight / 8f) * BarCodeData.FullZoomY * fontScale,
                       barCode.Font.Style,
                       needPixel ? GraphicsUnit.Pixel : GraphicsUnit.Point))
			{
				var codeIsbn = "ISBN " + fullCode;
				sf.LineAlignment = StringAlignment.Center;
				sf.Alignment = StringAlignment.Center;
                BaseDrawString(context,
					codeIsbn,
					barCodeFont,
					foreBrush,
					new RectangleF(0, 0, BarCodeData.MainWidth, BarCodeData.SpaceTextTop),
					sf);
			}
            RollbackTransform(context);
			#endregion            
		}
		#endregion

        #region Methods.override
        public override StiBarCodeTypeService CreateNew() => new StiIsbn13BarCodeType();
        #endregion

		public StiIsbn13BarCodeType() : this(13f, 1f, StiEanSupplementType.None, null, true)
		{
		}

        public StiIsbn13BarCodeType(float module, float height, StiEanSupplementType supplementType, string supplementCodeValue, bool showQuietZoneIndicator) :
            base(module, height, supplementType, supplementCodeValue, showQuietZoneIndicator)
		{
		}
	}
}