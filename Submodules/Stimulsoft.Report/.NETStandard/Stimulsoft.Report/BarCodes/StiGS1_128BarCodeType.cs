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
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text;

namespace Stimulsoft.Report.BarCodes
{
    /// <summary>
    /// The class describes the Barcode type - GS1-128.
    /// </summary>
    public class StiGS1_128BarCodeType : StiCode128BarCodeType
	{
        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiGS1_128BarCodeType;

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
		public override string ServiceName => "GS1-128";
		#endregion

		#region Methods
        public override void Draw(object context, StiBarCode barCode, RectangleF rect, float zoom)
		{		
			var code = GetCode(barCode);

            code = code.Replace((char)BarcodeCommandCode.Fnc1, (char)ControlCodes.FNC1);

            if (code.Trim().StartsWith("("))    //for compatibility with old versions
                code = code.Replace("(", "[").Replace(")", "]");

            var sbCode = new StringBuilder();
            var sbText = new StringBuilder();
            string errorMessage = StiGS1ApplicationIdentifiers.ParseCode(code, sbCode, sbText, (char)((int)ControlCodes.FNC1), false);

            if (errorMessage != null && (barCode.CodeValue == null) && barCode.Code.Value.Contains("{"))
            {
                errorMessage = null;
                sbCode = new StringBuilder(barCode.Code.Value);
                sbText = new StringBuilder(barCode.Code.Value);
            }

            if (errorMessage == null)
            {
                #region Encode data
                var encodedText = EncodeAuto(sbCode.ToString(), true);

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

                #region Make barsArray for output
                var barsArray = new StringBuilder();
                for (int index = 0; index < fullCode.Length; index++)
                {
                    barsArray.Append(CodeToBar(Code128Table[fullCode[index]]));
                }
                #endregion

                #region Prepare text
                StringBuilder sbb2 = new StringBuilder();
                foreach (char ch in sbText.ToString())
                {
                    if (ch < 32 || ch > 255)
                        sbb2.Append('\x2219');
                    else
                        sbb2.Append(ch);
                }
                sbText = sbb2;
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
                    sbCode.ToString(),
                    sbText.ToString(),
                    barsArray.ToString(),
                    rect,
                    barCode);

                DrawBarCode(context, rect, barCode);
            }
            else
            {
                DrawBarCodeError(context, rect, barCode, errorMessage);
            }
		}
        #endregion

        #region Properties.override
        public override string DefaultCodeValue => "[21]012345[3103]000123";

        protected override bool TextSpacing => false;
        #endregion

        #region Methods.override
        public override StiBarCodeTypeService CreateNew() => new StiGS1_128BarCodeType();
        #endregion

        public StiGS1_128BarCodeType() : this(13f, 1f)
		{
		}

        public StiGS1_128BarCodeType(float module, float height) :
			base(module, height)
		{
		}
	}
}