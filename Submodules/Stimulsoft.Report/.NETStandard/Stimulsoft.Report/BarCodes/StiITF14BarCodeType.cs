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
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.BarCodes.Design;
using Stimulsoft.Report.PropertyGrid;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

namespace Stimulsoft.Report.BarCodes
{
	/// <summary>
	/// The class describes the Barcode type - ITF-14.
	/// </summary>	
	[TypeConverter(typeof(StiITF14BarCodeTypeConverter))]
	public class StiITF14BarCodeType : StiBarCodeTypeService
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiITF14BarCodeType
            jObject.AddPropertyFloat("Module", Module, 40f);
            jObject.AddPropertyFloat("Height", Height, 1f);
            jObject.AddPropertyFloat("Ratio", Ratio, 2.5f);
            jObject.AddPropertyBool("PrintVerticalBars", PrintVerticalBars);

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

                    case "Ratio":
                        this.ratio = property.DeserializeFloat();
                        break;

                    case "PrintVerticalBars":
                        this.PrintVerticalBars = property.DeserializeBool();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiITF14BarCodeType;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            var list = new[]
            {
                propHelper.fHeight(),
                propHelper.Module(),
                propHelper.PrintVerticalBars(),
                propHelper.fRatio()
            };
            objHelper.Add(StiPropertyCategories.Main, list);
            
            return objHelper;
        }
        #endregion

		#region ServiceName
		/// <summary>
		/// Gets a service name.
		/// </summary>
		public override string ServiceName => "ITF-14";
        #endregion

        #region Properties
        public override string DefaultCodeValue => "15400141288763";

        private float module = 40f;
        /// <summary>
        /// Gets or sets width of the most fine element of the bar code.
        /// </summary>
        [Description("Gets or sets width of the most fine element of the bar code.")]
		[DefaultValue(40f)]
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
				if (value < 10f) module = 10f;
				if (value > 40f) module = 40f;
				Ratio = Ratio;
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
				if (value < 0.5f) height = 0.5f;
				if (value > 2.0f) height = 2.0f;
			}
		}


		private float ratio = 2.5f;
        /// <summary>
        /// Get or sets value, which indicates WideToNarrow ratio.
        /// </summary>
		[StiSerializable]
		[DefaultValue(2.5f)]
		[Description("Get or sets value, which indicates WideToNarrow ratio.")]
        [StiCategory("BarCode")]
        public float Ratio
		{
			get
			{
				return ratio;
			}
			set
			{
				ratio = value;
				if (value < 2.25f) ratio = 2.25f;
				if (value > 3.0f) ratio = 3.0f;
			}
		}

        /// <summary>
        /// Get or sets value, which indicates, print or not vertical sections.
        /// </summary>
        [StiSerializable]
        [DefaultValue(false)]
        [Description("Get or sets value, which indicates, print or not vertical sections.")]
        [TypeConverter(typeof(Stimulsoft.Base.Localization.StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiCategory("BarCode")]
        public bool PrintVerticalBars { get; set; }

        internal override float LabelFontHeight => Itf14TextHeight;

        public override bool[] VisibleProperties
        {
            get
            {
                var props = new bool[visiblePropertiesCount];
                props[11] = true;
                props[13] = true;
                props[14] = true;
                props[15] = true;

                return props;
            }
        }
        #endregion

		#region ITF-14 encoding table
		protected string[] symTableSet = {	
            "00110",	//0
			"10001",	//1
			"01001",	//2
			"11000",	//3
			"00101",	//4
			"10100",	//5
			"01100",	//6
			"00011",	//7
			"10010",	//8
			"01010",	//9
			"0000",		//Start
			"100",		//Stop
		};
		#endregion

		#region Consts
        private const float Itf14BearerBarWidth = 2; //4.75f if requiring printing plates
		private const float Itf14SpaceLeft = 10f;
		private const float Itf14SpaceRight = 10f;
        private const float Itf14SpaceTop = Itf14BearerBarWidth;
        private const float Itf14SpaceBottom = Itf14BearerBarWidth + 1f;
		private const float Itf14LineHeightShort = 31.25f;
		private const float Itf14LineHeightLong = Itf14LineHeightShort;
		private const float Itf14TextHeight = 8.33f;
		private const float Itf14MainHeight = 51.75f;
        private const float Itf14TextPosition = Itf14SpaceTop + Itf14LineHeightShort + Itf14SpaceBottom;
		private const float Itf14LineHeightForCut = Itf14LineHeightShort;
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
            code = CheckCodeSymbols(code, "0123456789") + new string('0', 14);

            #region Calculate check digit
            var dig = new int[14];
            for (int tempIndex = 0; tempIndex < 14; tempIndex++)
            {
                dig[tempIndex] = int.Parse(code[tempIndex].ToString());
            }
            int sum = (dig[0] + dig[2] + dig[4] + dig[6] + dig[8] + dig[10] + dig[12]) * 3 +
                       dig[1] + dig[3] + dig[5] + dig[7] + dig[9] + dig[11];
            int checkDigit = 10 - (sum % 10);
            if (checkDigit == 10)
            {
                checkDigit = 0;
            }
            dig[13] = checkDigit;
            code = code.Substring(0, 13) + (char)(checkDigit + 48);
            #endregion

            #region make barsArray for output
            var barsArray = new StringBuilder();
            barsArray.Append(symTableSet[10]);
            for (int index = 0; index < 7; index++)
            {
                string sym1 = symTableSet[dig[index * 2 + 0]];
                string sym2 = symTableSet[dig[index * 2 + 1]];
                var sb = new StringBuilder("0000000000");
                sb[0] = sym1[0];
                sb[2] = sym1[1];
                sb[4] = sym1[2];
                sb[6] = sym1[3];
                sb[8] = sym1[4];
                sb[1] = sym2[0];
                sb[3] = sym2[1];
                sb[5] = sym2[2];
                sb[7] = sym2[3];
                sb[9] = sym2[4];
                barsArray.Append(sb);
            }
            barsArray.Append(symTableSet[11]);
            bool counter = true;
            for (int index = 0; index < barsArray.Length; index++)
            {
                char currentsym;
                if (counter == true)
                {
                    if (barsArray[index] == '0') currentsym = '4'; else currentsym = '5';
                }
                else
                {
                    if (barsArray[index] == '0') currentsym = '0'; else currentsym = '1';
                }
                barsArray[index] = currentsym;
                counter = !counter;
            }
            #endregion

            CalculateSizeFull(
                Itf14SpaceLeft + (PrintVerticalBars ? Itf14BearerBarWidth : 0),
                Itf14SpaceRight + (PrintVerticalBars ? Itf14BearerBarWidth : 0),
                Itf14SpaceTop,
                Itf14SpaceBottom,
                Itf14LineHeightShort,
                Itf14LineHeightLong,
                Itf14TextPosition,
                Itf14TextHeight,
                Itf14MainHeight,
                Itf14LineHeightForCut,
                Ratio,
                zoom,
                code,
                code,
                barsArray.ToString(),
                rect,
                barCode);

            DrawBarCode(context, rect, barCode, DrawBaseLines);

            var foreBrush = new StiSolidBrush(barCode.ForeColor);            
        }

        protected void DrawBaseLines(object context, StiBrush foreBrush)
        {
            BaseFillRectangle(context, foreBrush, 0, 0, BarCodeData.MainWidth, BarCodeData.SpaceTop);
            BaseFillRectangle(context, foreBrush, 0, BarCodeData.SpaceTop + BarCodeData.LineHeightShort, BarCodeData.MainWidth, BarCodeData.SpaceTop);
            if (PrintVerticalBars)
            {
                BaseFillRectangle(context, foreBrush, 0, 0, BarCodeData.SpaceTop, BarCodeData.LineHeightShort + BarCodeData.SpaceTop * 2);
                BaseFillRectangle(context, foreBrush, BarCodeData.MainWidth - BarCodeData.SpaceTop, 0, BarCodeData.SpaceTop, BarCodeData.LineHeightShort + BarCodeData.SpaceTop * 2);
            }
        }
		#endregion

        #region Methods.override
        public override StiBarCodeTypeService CreateNew() => new StiITF14BarCodeType();
        #endregion

		public StiITF14BarCodeType() : this(40f, 1f, 2.5f, false)
		{
		}

        public StiITF14BarCodeType(float module, float height, float ratio, bool printVerticalBars) 
		{
			this.module = module;
			this.height = height;
			this.ratio  = ratio;
            this.PrintVerticalBars = printVerticalBars;
		}
	}
}