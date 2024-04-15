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
	/// The class describes the Barcode type - EAN-128.
	/// </summary>
	public class StiEAN128AutoBarCodeType : StiCode128BarCodeType
	{
        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiEAN128AutoBarCodeType;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            var list = new[]
            {
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
		public override string ServiceName => "EAN-128 Auto";
        #endregion

        #region Properties
        public override string DefaultCodeValue => "1234567890123";
        #endregion

        #region Methods
        public override void Draw(object context, StiBarCode barCode, RectangleF rect, float zoom)
		{		
			string code = GetCode(barCode);

            #region prepare data
            var sbTemp = new StringBuilder();
            for (int index = 0; index < code.Length; index++)
            {
                int ch = (int)code[index];
                sbTemp.Append(ch >= 32 && ch != 0x81 ? (char)ch : '\x2219');  //or "\x2022"
            }

            var encodedText = EncodeAuto(code, true);

            var fullCode = new int[encodedText.Length];
            for (int index = 0; index < encodedText.Length; index++)
            {
                fullCode[index] = (int)encodedText[index];
            }

            int checkSum = fullCode[0] + fullCode[1];
            for (int index = 0; index < encodedText.Length - 4; index++)
            {
                checkSum += fullCode[index + 2] * (index + 2);
            }
            fullCode[fullCode.Length - 2] = checkSum % 103;
            #endregion

            #region make barsArray for output
            var barsArray = new StringBuilder();
			for (int index = 0; index < fullCode.Length; index++)
			{	
				barsArray.Append(CodeToBar(Code128Table[fullCode[index]]));
			}
            var text = sbTemp.ToString();
			#endregion

			CalculateSizeFull(
				Code128SpaceLeft,
				Code128SpaceRight,
				Code128SpaceTop,
				Code128SpaceBottom,
				Code128LineHeightShort,
				Code128LineHeightLong,
				Code128TextPosition,
				Code128TextHeight,
				Code128MainHeight,
				Code128LineHeightForCut, 
				2f,
				zoom,
				code, 
				text,
				barsArray.ToString(),
				rect,
				barCode);

            DrawBarCode(context, rect, barCode); 
		}
		#endregion

        #region Methods.override
        public override StiBarCodeTypeService CreateNew() => new StiEAN128AutoBarCodeType();
        #endregion

		public StiEAN128AutoBarCodeType() : this(13f, 1f)
		{
		}

		public StiEAN128AutoBarCodeType(float module, float height) :
			base(module, height)
		{
		}
	}
}