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
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.PropertyGrid;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text;

namespace Stimulsoft.Report.BarCodes
{
    /// <summary>
    /// The class describes the Barcode type - Serial Shipping Container Code (SSCC-18).
    /// </summary>
    public class StiSSCC18BarCodeType : StiCode128cBarCodeType
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiSSCC18BarCodeType
            jObject.AddPropertyString("CompanyPrefix", CompanyPrefix, "0123456");
            jObject.AddPropertyString("SerialNumber", SerialNumber, "000000001");
            jObject.AddPropertyString("ExtensionDigit", ExtensionDigit, "0");

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "CompanyPrefix":
                        this.CompanyPrefix = property.DeserializeString();
                        break;

                    case "SerialNumber":
                        this.SerialNumber = property.DeserializeString();
                        break;

                    case "ExtensionDigit":
                        this.ExtensionDigit = property.DeserializeString();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiSSCC18BarCodeType;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            var list = new[]
            {
                propHelper.CompanyPrefix(),
                propHelper.ExtensionDigit(),
                propHelper.fHeight(),
                propHelper.Module(),
                propHelper.SerialNumber()
            };
            objHelper.Add(StiPropertyCategories.Main, list);

            return objHelper;
        }
        #endregion

		#region ServiceName
		/// <summary>
		/// Gets a service name.
		/// </summary>
		public override string ServiceName => "SSCC";
        #endregion

        #region Properties
        public override string DefaultCodeValue => "(00)~0~0123456~000000001~8";

        /// <summary>
        /// Gets or sets the GS1 Company Prefix (7-10 digits).
        /// </summary>
        [Description("Gets or sets the GS1 Company Prefix (7-10 digits).")]
        [DefaultValue("0123456")]
        [StiSerializable]
        [StiCategory("BarCode")]
        public string CompanyPrefix { get; set; } = "0123456";

        /// <summary>
        /// Gets or sets the Serial Reference Numbers (6-9 digits).
        /// </summary>
        [Description("Gets or sets the Serial Reference Numbers (6-9 digits).")]
        [DefaultValue("000000001")]
        [StiSerializable]
        [StiCategory("BarCode")]
        public string SerialNumber { get; set; } = "000000001";

        /// <summary>
        /// Gets or sets the extension digit.
        /// </summary>
        [Description("Gets or sets the extension digit.")]
        [DefaultValue("0")]
        [StiSerializable]
        [StiCategory("BarCode")]
        public string ExtensionDigit { get; set; } = "0";

        protected override bool TextSpacing => false;


        public override bool[] VisibleProperties
        {
            get
            {
                var props = new bool[visiblePropertiesCount];
                props[11] = true;
                props[13] = true;
                props[24] = true;
                props[25] = true;
                props[26] = true;
                return props;
            }
        }
        #endregion

        #region Methods
        public override string GetCombinedCode()
        {
            return string.Format("(00){0}{1}{0}{2}{0}{3}{0}{4}",
                '~',
                ExtensionDigit,
                CompanyPrefix,
                SerialNumber,
                GetCheckDigit(CheckCodeSymbols(ExtensionDigit + CompanyPrefix + SerialNumber + new string('0', 17), "0123456789")));
        }

        private string GetCheckDigit(string input)
        {
            var dig = new int[17];
            for (int tempIndex = 0; tempIndex < 17; tempIndex++)
            {
                dig[tempIndex] = int. Parse(input[tempIndex].ToString(CultureInfo.InvariantCulture));
            }

            int sum = (dig[1] + dig[3] + dig[5] + dig[7] + dig[9] + dig[11] + dig[13] + dig[15]) +
                (dig[0] + dig[2] + dig[4] + dig[6] + dig[8] + dig[10] + dig[12] + dig[14] + dig[16]) * 3;
            int checkDigit = 10 - (sum % 10);
            if (checkDigit == 10)
            {
                checkDigit = 0;
            }
            return ((char)(checkDigit + 48)).ToString(CultureInfo.InvariantCulture);
        }

        private bool CheckContens(string inputCode, string[] arr)
        {
            if (arr.Length != 5) return true;
            if (arr[0] != "00") return true;
            if (arr[1].Length != 1) return true;
            int companyPrefixLen = arr[2].Length;
            if ((companyPrefixLen < 7) || (companyPrefixLen > 10)) return true;
            int serialLen = arr[3].Length;
            if ((serialLen + companyPrefixLen) != 16) return true;
            if (inputCode.Length != 24) return true;
            return false;
        }

        public override void Draw(object context, StiBarCode barCode, RectangleF rect, float zoom)
        {
            var inputString = GetCode(barCode);
            var inputCode = CheckCodeSymbols(inputString.Replace('\x1f', '~'), "0123456789" + "~");
            var arr = inputCode.Split(new char[] { '~' });

            bool error = CheckContens(inputCode, arr);
            if (error)
            {
                #region Correct code
                var newArr = new string[arr.Length > 5 ? arr.Length : 5];
                for (int index = 0; index < arr.Length; index++)
                {
                    newArr[index] = arr[index];
                }

                arr = newArr;
                arr[0] = "00";
                if (arr[1].Length < 1) arr[1] = "0";
                if (arr[1].Length > 1) arr[1] = arr[1].Substring(0, 1);
                if (arr[2].Length < 7) arr[2] = arr[2] + new string('0', 7 - arr[2].Length);
                if (arr[2].Length > 10) arr[2] = arr[2].Substring(0, 10);
                if (arr[3].Length < 16 - arr[2].Length) arr[3] = arr[3] + new string('0', 16 - arr[2].Length);
                if (arr[3].Length > 16 - arr[2].Length) arr[3] = arr[3].Substring(0, 16 - arr[2].Length);
                #endregion
            }

            //calculate check digit
            string code = arr[1] + arr[2] + arr[3];
            string checkDigit = GetCheckDigit(code);
            code = arr[0] + code + checkDigit;
            var text = string.Format("({0}) {1} {2} {3} {4}{5}",
                arr[0],
                arr[1],
                arr[2],
                arr[3],
                checkDigit,
                error ? " *" : "");

            if ((barCode.Report != null) && (barCode.Report.IsDesigning))
            {
                text = inputString.Replace('~', ' ');
            }

            #region make barsArray for output
            var stringList = new List<string>
            {
                code
            };

            //prepare data string
            var sbCode = new StringBuilder();
            for (int index = 0; index < stringList.Count; index++)
            {
                string stLine = stringList[index];
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
            for (int index = 0; index < fullCode.Length - 3; index++)
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
                text,
                barsArray.ToString(),
                rect,
                barCode);

            DrawBarCode(context, rect, barCode);
        }
		#endregion

        #region Methods.override
        public override StiBarCodeTypeService CreateNew() => new StiSSCC18BarCodeType();
        #endregion

		public StiSSCC18BarCodeType() : this(13f, 1f)
		{
		}

        public StiSSCC18BarCodeType(float module, float height) :
			base(module, height)
		{
		}
	}
}