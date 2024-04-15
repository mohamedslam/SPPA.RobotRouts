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
using System.ComponentModel;
using System.Text;

namespace Stimulsoft.Report.BarCodes
{
    /// <summary>
    /// The class describes the Barcode type - Code-128.
    /// </summary>
    [TypeConverter(typeof(Stimulsoft.Report.BarCodes.Design.StiBarCodeTypeServiceConverter))]
	public abstract class StiCode128BarCodeType : StiBarCodeTypeService
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);
            
            // StiBarCodeTypeService
            jObject.AddPropertyFloat("Module", Module, 13f);
            jObject.AddPropertyFloat("Height", Height, 1f);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Module":
                        this.module = property.DeserializeFloat();
                        break;

                    case "Height":
                        this.height = property.DeserializeFloat();
                        break;
                }
            }
        }
        #endregion

		#region Code128Table
		protected string[] Code128Table = new string[107]
		{
			"212222",
			"222122",
			"222221",
			"121223",
			"121322",
			"131222",
			"122213",
			"122312",
			"132212",
			"221213",
			"221312",
			"231212",
			"112232",
			"122132",
			"122231",
			"113222",
			"123122",
			"123221",
			"223211",
			"221132",
			"221231",
			"213212",
			"223112",
			"312131",
			"311222",
			"321122",
			"321221",
			"312212",
			"322112",
			"322211",
			"212123",
			"212321",
			"232121",
			"111323",
			"131123",
			"131321",
			"112313",
			"132113",
			"132311",
			"211313",
			"231113",
			"231311",
			"112133",
			"112331",
			"132131",
			"113123",
			"113321",
			"133121",
			"313121",
			"211331",
			"231131",
			"213113",
			"213311",
			"213131",
			"311123",
			"311321",
			"331121",
			"312113",
			"312311",
			"332111",
			"314111",
			"221411",
			"431111",
			"111224",
			"111422",
			"121124",
			"121421",
			"141122",
			"141221",
			"112214",
			"112412",
			"122114",
			"122411",
			"142112",
			"142211",
			"241211",
			"221114",
			"413111",
			"241112",
			"134111",
			"111242",
			"121142",
			"121241",
			"114212",
			"124112",
			"124211",
			"411212",
			"421112",
			"421211",
			"212141",
			"214121",
			"412121",
			"111143",
			"111341",
			"131141",
			"114113",
			"114311",
			"411113",
			"411311",
			"113141",
			"114131",
			"311141",
			"411131",
			"211412",
			"211214",
			"211232",
			"2331112"
		};
		#endregion

		#region Properties
		private float module = 13f;
        /// <summary>
        /// Gets or sets width of the most fine element of the bar code.
        /// </summary>
        [Description("Gets or sets width of the most fine element of the bar code.")]
		[DefaultValue(13f)]
        [StiCategory("BarCode")]
        public override float Module
		{
			get
			{
				return module;
			}
			set
			{
				module = value;
				if (value < 9.84f)	module = 9.84f;
				if (value > 40f)	module = 40f;
			}
		}

		private float height = 1f;
        /// <summary>
        /// Gets os sets height factor of the bar code.
        /// </summary>		
        [Description("Gets os sets height factor of the bar code.")]
		[DefaultValue(1f)]
        [StiCategory("BarCode")]
        public override float Height
		{
			get
			{
				return height;
			}
			set
			{
				height = value;
				if (value < 0.5f)	height = 0.5f;
				if (value > 2.0f)	height = 2.0f;
			}
		}

        internal override float LabelFontHeight => Code128TextHeight;

        public override bool[] VisibleProperties
        {
            get
            {
                bool[] props = new bool[visiblePropertiesCount];
                props[11] = true;
                props[13] = true;

                return props;
            }
        }
		#endregion

		#region Consts
		protected const float Code128SpaceLeft			= 10f;
		protected const float Code128SpaceRight			= 10f;
		protected const float Code128SpaceTop			= 0f;
		protected const float Code128SpaceBottom		= 1f;
		protected const float Code128LineHeightShort	= 45f; //maybe
		protected const float Code128LineHeightLong		= Code128LineHeightShort;
		protected const float Code128TextPosition		= Code128LineHeightShort + Code128SpaceBottom;
		protected const float Code128TextHeight			= 8.33f;
		protected const float Code128MainHeight			= 55f;
		protected const float Code128LineHeightForCut	= Code128LineHeightShort;

        protected const CodeSet DefaultCodeSetAB = CodeSet.B;
		#endregion

        #region Enums
        protected enum CodeSet
        {
            None,
            A,
            B,
            C
        }

        protected enum ControlCodes
        {
            FNC1 = 129,
            FNC2 = 130,
            FNC3 = 131,
            FNC4 = 132,
            CodeA = 133,
            CodeB = 134,
            CodeC = 135,
            Shift = 136
        }

        protected enum BarcodeCommands
        {
            FNC1 = 102,
            FNC2 = 97,
            FNC3 = 96,
            FNC4A = 101,
            FNC4B = 100,
            CodeA = 101,
            CodeB = 100,
            CodeC = 99,
            Shift = 98,
            StartA = 103,
            StartB = 104,
            StartC = 105,
            Stop = 106
        }
        #endregion

		#region Methods
		protected string CodeToBar(string inputCode)
		{
			var outputBar = new StringBuilder();
			bool counter = true;
			for (int index = 0; index < inputCode.Length; index++)
			{
				char currentsym;
				if (counter == true)
				{
					switch (inputCode[index])
					{
						case '1': currentsym = '4'; break;
						case '2': currentsym = '5'; break;
						case '3': currentsym = '6'; break;
						case '4': currentsym = '7'; break;
						default: currentsym = '4'; break;
					}
				}
				else
				{
					switch (inputCode[index])
					{
						case '1': currentsym = '0'; break;
						case '2': currentsym = '1'; break;
						case '3': currentsym = '2'; break;
						case '4': currentsym = '3'; break;
						default: currentsym = '0'; break;
					}
				}
				outputBar.Append(currentsym);
				counter = !counter; 
			}
			return outputBar.ToString();
		}
		#endregion

        #region Methods.EncodeAuto
        protected static string EncodeAuto(string inputText, bool encodeAsEan)
        {
            var output = new StringBuilder();
            var sb = new StringBuilder();
            foreach (char ch in inputText)
            {
                int sym = (int)ch;
                //if (sym < 128 || (sym >= (int)ControlCodes.FNC1 && sym <= (int)ControlCodes.FNC4)) sb.Append(ch);
                if (sym < 256) sb.Append(ch);   //now support all Latin1
            }

            //add start symbol
            var state = GetSet(sb, 0, CodeSet.None);
            if (state == CodeSet.A) output.Append((char)BarcodeCommands.StartA);
            else if (state == CodeSet.B) output.Append((char)BarcodeCommands.StartB);
            else output.Append((char)BarcodeCommands.StartC);

            if (encodeAsEan) output.Append((char)BarcodeCommands.FNC1);

            int pos = 0;
            while (pos < sb.Length)
            {
                //get CodeSet of the next symbol
                var newState = GetSet(sb, pos, state);
                if (newState != state)
                {
                    //add CodeSet symbol
                    if (newState == CodeSet.A) output.Append((char)BarcodeCommands.CodeA);
                    else if (newState == CodeSet.B) output.Append((char)BarcodeCommands.CodeB);
                    else output.Append((char)BarcodeCommands.CodeC);
                    state = newState;
                }

                int sym = (int)sb[pos];
                if (sym >= (int)ControlCodes.FNC1 && sym <= (int)ControlCodes.FNC4)
                {
                    //add FNC commands symbol
                    if (sym == (int)ControlCodes.FNC1) output.Append((char)BarcodeCommands.FNC1);
                    else if (sym == (int)ControlCodes.FNC2) output.Append((char)BarcodeCommands.FNC2);
                    else if (sym == (int)ControlCodes.FNC3) output.Append((char)BarcodeCommands.FNC3);
                    else if (state == CodeSet.A) output.Append((char)BarcodeCommands.FNC4A);
                    else output.Append((char)BarcodeCommands.FNC4B);

                    pos++;
                    continue;
                }
                if (sym >= (int)ControlCodes.CodeA && sym <= (int)ControlCodes.CodeC)
                {
                    pos++;
                    continue;
                }

                if (state == CodeSet.A)
                {
                    if (sym < 0x80)
                    {
                        output.Append((char)(sym >= 0x20 ? sym - 0x20 : sym + 0x40));
                    }
                    else
                    {
                        output.Append((char)BarcodeCommands.FNC4A);
                        output.Append((char)(sym >= 0xA0 ? sym - 0xA0 : sym - 0x40));
                    }
                }
                else if (state == CodeSet.B)
                {
                    if (sym < 0x80)
                    {
                        output.Append((char)(sym - 0x20));
                    }
                    else
                    {
                        output.Append((char)BarcodeCommands.FNC4B);
                        output.Append((char)(sym - 0xA0));
                    }
                }
                else
                {
                    int pair = (sym - (int)'0') * 10 + ((int)sb[pos + 1] - (int)'0');
                    output.Append((char)pair);
                    pos++;
                }

                pos++;
            }

            output.Append((char)0); //checksum
            output.Append((char)BarcodeCommands.Stop);

            return output.ToString();
        }

        private static CodeSet GetSet(StringBuilder text, int pos, CodeSet prevSet)
        {
            if (text.Length == 0 || pos > text.Length - 1) return CodeSet.A;

            int sym = (int)text[pos];

            if (sym >= (int)ControlCodes.FNC1 && sym <= (int)ControlCodes.FNC4)
            {
                if (prevSet == CodeSet.None) return DefaultCodeSetAB;
                if (sym == (int)ControlCodes.FNC1) return prevSet;
                if (prevSet == CodeSet.C) return DefaultCodeSetAB;
                else return prevSet;
            }
            if (sym >= (int)ControlCodes.CodeA && sym <= (int)ControlCodes.CodeC)
            {
                if (sym == (int)ControlCodes.CodeA) return CodeSet.A;
                if (sym == (int)ControlCodes.CodeB) return CodeSet.B;
                return CodeSet.C;
            }

            if ((sym < 32) || (sym >= 0x80 && sym < 0xA0))
            {
                return CodeSet.A;
            }
            else
            {
                if ((sym >= 0x60 && sym < 0x80) || (sym >= 0xE0 && sym <= 0xFF))
                {
                    return CodeSet.B;
                }
                else
                {
                    if (char.IsDigit(text[pos]))
                    {
                        if (prevSet == CodeSet.C)
                        {
                            if ((pos + 1 < text.Length) && char.IsDigit(text[pos + 1]))
                            {
                                return CodeSet.C;
                            }
                        }
                        else
                        {
                            if ((pos + 3 < text.Length) && char.IsDigit(text[pos + 1]) && char.IsDigit(text[pos + 2]) && char.IsDigit(text[pos + 3]))
                            {
                                return CodeSet.C;
                            }
                        }
                    }
                    if (prevSet != CodeSet.None && prevSet != CodeSet.C)
                    {
                        return prevSet;
                    }
                    else
                    {
                        return DefaultCodeSetAB;
                    }
                }
            }
        }
        #endregion

        public StiCode128BarCodeType(float module, float height)
		{
			this.module = module;
			this.height = height;
		}
	}
}