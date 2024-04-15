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
    public class StiCode128bBarCodeType : StiCode128BarCodeType
	{
        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiCode128bBarCodeType;

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
		public override string ServiceName => "Code128b";
        #endregion

        #region Properties
        public override string DefaultCodeValue => "ABCabc123";
        #endregion

        #region Methods
        public override void Draw(object context, StiBarCode barCode, RectangleF rect, float zoom)
		{
			var code = GetCode(barCode);
			code = CheckCodeSymbols(code,
				" !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~" + "\x7F" + "\x81" +
				"\xA0\xA1\xA2\xA3\xA4\xA5\xA6\xA7\xA8\xA9\xAA\xAB\xAC\xAD\xAE\xAF" +
				"\xB0\xB1\xB2\xB3\xB4\xB5\xB6\xB7\xB8\xB9\xBA\xBB\xBC\xBD\xBE\xBF" +
				"\xC0\xC1\xC2\xC3\xC4\xC5\xC6\xC7\xC8\xC9\xCA\xCB\xCC\xCD\xCE\xCF" +
				"\xD0\xD1\xD2\xD3\xD4\xD5\xD6\xD7\xD8\xD9\xDA\xDB\xDC\xDD\xDE\xDF" +
				"\xE0\xE1\xE2\xE3\xE4\xE5\xE6\xE7\xE8\xE9\xEA\xEB\xEC\xED\xEE\xEF" +
				"\xF0\xF1\xF2\xF3\xF4\xF5\xF6\xF7\xF8\xF9\xFA\xFB\xFC\xFD\xFE\xFF" );

			#region Process ISO-8859-1
			var bullet = '\x2219';  //or "\x2022"
			var sb = new StringBuilder();
			var sbTemp = new StringBuilder();
			foreach (char ch in code)
			{
				if (ch < 128)
				{
					sb.Append((char)(ch - 32));
					sbTemp.Append(ch);
				}
				else
				{
					if (ch == (int)ControlCodes.FNC1)
					{
						sb.Append((char)BarcodeCommands.FNC1);
						sbTemp.Append(bullet);
					}
					else
					{
						sb.Append((char)BarcodeCommands.FNC4B);
						sb.Append((char)(ch - 0xA0));
						sbTemp.Append(ch);
					}
				}
			}
			code = sb.ToString();
			var text = sbTemp.ToString();
			#endregion

			#region make barsArray for output
			var fullCode = new int[code.Length + 3];
			fullCode[0] = (int)BarcodeCommands.StartB;
			int checkSum = fullCode[0];
			for (int index = 0; index < code.Length; index++) 
			{
                int ch = code[index];
				fullCode[index + 1] = ch;
				checkSum += ch * (index + 1);
			}
			fullCode[fullCode.Length - 2] = checkSum % 103;
			fullCode[fullCode.Length - 1] = (int)BarcodeCommands.Stop;

			var barsArray = new StringBuilder();
			for (int index = 0; index < fullCode.Length; index++)
			{	
				barsArray.Append(CodeToBar(Code128Table[fullCode[index]]));
			}
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
        public override StiBarCodeTypeService CreateNew() => new StiCode128bBarCodeType();
        #endregion

		public StiCode128bBarCodeType() : this(13f, 1f)
		{
		}

		public StiCode128bBarCodeType(float module, float height) :
			base(module, height)
		{
		}
	}
}