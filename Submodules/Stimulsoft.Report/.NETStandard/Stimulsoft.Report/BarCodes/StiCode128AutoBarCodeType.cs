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
    /// The class describes the Barcode type - Code-128.
    /// </summary>
    public class StiCode128AutoBarCodeType : StiCode128BarCodeType
	{
        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiCode128AutoBarCodeType;

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
		public override string ServiceName => "Code128 Auto";
        #endregion

        #region Properties
        public override string DefaultCodeValue => "1234567890123";
        #endregion

        #region Methods
        public override void Draw(object context, StiBarCode barCode, RectangleF rect, float zoom)
		{
            var code = GetCode(barCode);

            #region prepare data
            var sbTemp = new StringBuilder();
            for (int index = 0; index < code.Length; index++)
            {
                int ch = (int)code[index];
                if (ch >= 32 && ch < (int)ControlCodes.FNC1 || ch > (int)ControlCodes.CodeC && ch <= 0xFF)
                {
                    sbTemp.Append((char)ch);
                }
                else
                {
                    if (!(ch >= (int)ControlCodes.CodeA && ch <= (int)ControlCodes.CodeC))
                    {
                        sbTemp.Append('\x2219');  //or "\x2022"
                    }
                }
            }

            var encodedText = EncodeAuto(code, false);

            var fullCode = new int[encodedText.Length];
            for (int index = 0; index < encodedText.Length; index++)
            {
                fullCode[index] = (int)encodedText[index];
            }

            int checkSum = fullCode[0];
            for (int index = 0; index < encodedText.Length - 3; index++)
            {
                checkSum += fullCode[index + 1] * (index + 1);
            }
            fullCode[fullCode.Length - 2] = checkSum % 103;
            #endregion

            #region make barsArray for output
            var barsArray = new StringBuilder();
			for (int index = 0; index < fullCode.Length; index++)
			{	
				barsArray.Append(CodeToBar(Code128Table[fullCode[index]]));
			}
            string text = sbTemp.ToString();
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
        public override StiBarCodeTypeService CreateNew() => new StiCode128AutoBarCodeType();
        #endregion

		public StiCode128AutoBarCodeType() : this(13f, 1f)
		{
		}

		public StiCode128AutoBarCodeType(float module, float height) :
			base(module, height)
		{
		}
	}
}