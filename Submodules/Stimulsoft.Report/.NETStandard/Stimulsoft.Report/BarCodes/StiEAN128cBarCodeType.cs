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
using System.Text;

namespace Stimulsoft.Report.BarCodes
{
    /// <summary>
    /// The class describes the Barcode type - EAN-128.
    /// </summary>
    public class StiEAN128cBarCodeType : StiCode128BarCodeType
	{
        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiEAN128cBarCodeType;

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
		public override string ServiceName => "EAN-128c";
        #endregion

        #region Properties
        public override string DefaultCodeValue => "0123456789012345";
        #endregion

        #region Methods
        public override void Draw(object context, StiBarCode barCode, RectangleF rect, float zoom)
		{
            string code = GetCode(barCode);
            code = CheckCodeSymbols(code, "0123456789" + "\x81");

            #region make barsArray for output
            //scan for FNC1 command and split into separate lines
            var stringList = new List<string>();
            var sbTemp = new StringBuilder();
            string st = string.Empty;
            foreach (char ch in code)
            {
                if (ch == '\x81')
                {
                    stringList.Add(st);
                    st = string.Empty;
                    sbTemp.Append('\x2219');  //or "\x2022"
                }
                else
                {
                    st += ch;
                    sbTemp.Append(ch);
                }
            }
            if (st.Length > 0) stringList.Add(st);
            if (stringList.Count == 0) stringList.Add(st);

            //prepare data string
            var sbCode = new StringBuilder();
            for (int index = 0; index < stringList.Count; index++)
            {
                var stLine = stringList[index];
                int stringLen = stLine.Length / 2;
                for (int index2 = 0; index2 < stringLen; index2++)
                {
                    sbCode.Append((char)int.Parse(stLine.Substring(index2 * 2, 2)));
                }
                if (stLine.Length % 2 == 1)
                {
                    sbCode.Append((char)100);   //switch to CodeB
                    sbCode.Append((char)((int)stLine[stLine.Length - 1] - 32));
                    if (index < stringList.Count - 1)
                    {
                        sbCode.Append((char)99);    //switch back to CodeC
                    }
                }
                if (index < stringList.Count - 1)
                {
                    sbCode.Append((char)102);    //FNC1
                }
            }

            //make barsArray
            var fullCode = new int[sbCode.Length + 4];
            fullCode[0] = 105;
            fullCode[1] = 102;
            for (int index = 0; index < sbCode.Length; index++)
            {
                fullCode[index + 2] = (int)sbCode[index];
            }
            int checkSum = fullCode[0] + fullCode[1];
            for (int index = 0; index < fullCode.Length - 4; index++)
            {
                checkSum += fullCode[index + 2] * (index + 2);
            }
            fullCode[fullCode.Length - 2] = checkSum % 103;
            fullCode[fullCode.Length - 1] = 106;

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
				sbTemp.ToString(),
				barsArray.ToString(),
				rect,
				barCode);

            DrawBarCode(context, rect, barCode); 
		}
		#endregion

        #region Methods.override
        public override StiBarCodeTypeService CreateNew() => new StiEAN128cBarCodeType();
        #endregion

		public StiEAN128cBarCodeType() : this(13f, 1f)
		{
		}

		public StiEAN128cBarCodeType(float module, float height) :
			base(module, height)
		{
		}
	}
}