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
using Stimulsoft.Report.BarCodes.Design;
using Stimulsoft.Report.PropertyGrid;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace Stimulsoft.Report.BarCodes
{
    /// <summary>
    /// The class describes the Barcode type - Code93.
    /// </summary>
    [TypeConverter(typeof(StiCode93BarCodeTypeConverter))]
	public class StiCode93BarCodeType : StiBarCodeTypeService
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);
            

            // StiCode93BarCodeType
            jObject.AddPropertyFloat("Module", Module, 13f);
            jObject.AddPropertyFloat("Height", Height, 1f);
            jObject.AddPropertyFloat("Ratio", Ratio, 2.2f);

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
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiCode93BarCodeType;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            var list = new[]
            {
                propHelper.fHeight(),
                propHelper.Module(),
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
        public override string ServiceName => "Code93";
		#endregion

		#region Code93Table
		private string Code93Symbols = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ-. $/+%";
		protected string[] Code93Table = new string[49]
		{
			"131112", //0
			"111213", //1
			"111312", //2
			"111411", //3
			"121113", //4
			"121212", //5
			"121311", //6
			"111114", //7
			"131211", //8
			"141111", //9
			"211113", //A
			"211212",
			"211311",
			"221112",
			"221211",
			"231111",
			"112113",
			"112212",
			"112311",
			"122112",
			"132111", //K
			"111123",
			"111222",
			"111321",
			"121122",
			"131121",
			"212112",
			"212211",
			"211122",
			"211221", //T
			"221121",
			"222111",
			"112122",
			"112221",
			"122121",
			"123111", //Z
			"121131", //-
			"311112", //.
			"311211", //space
			"321111", //$
			"112131", ///
			"113121", //+ 
			"211131", //%
			"121221", //only for Code93 Extended
			"312111", //only for Code93 Extended
			"311121", //only for Code93 Extended
			"122211", //only for Code93 Extended
			"111141", //* start
			"1111411" //* stop
		};
        #endregion

        #region Properties
        public override string DefaultCodeValue => "ABC123";

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
				if (value < 2f)	module = 2f;
				if (value > 40f)	module = 40f;
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
				if (value < 0.5f)	height = 0.5f;
				if (value > 2.0f)	height = 2.0f;
			}
		}

		private float ratio = 2.2f;
        /// <summary>
        /// Get or sets value, which indicates WideToNarrow ratio.
        /// </summary>
        [Description("Get or sets value, which indicates WideToNarrow ratio.")]
		[DefaultValue(2.2f)]
		[StiSerializable]
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
				float minRatio = (Module > 20f ? 2.0f : 2.2f);
				if (value < minRatio)	ratio = minRatio;
				if (value > 3.0f)		ratio = 3.0f;
			}
		}

        internal override float LabelFontHeight => Code93TextHeight;

        public override bool[] VisibleProperties
        {
            get
            {
                var props = new bool[visiblePropertiesCount];
                props[11] = true;
                props[13] = true;
                props[15] = true;

                return props;
            }
        }
		#endregion

		#region Consts
		protected const float Code93SpaceLeft			= 10f;
		protected const float Code93SpaceRight			= 10f;
		protected const float Code93SpaceTop			= 0f;
		protected const float Code93SpaceBottom			= 1f;
		protected const float Code93LineHeightShort		= 45f; //maybe
		protected const float Code93LineHeightLong		= Code93LineHeightShort;
		protected const float Code93TextPosition		= Code93LineHeightShort + Code93SpaceBottom;
		protected const float Code93TextHeight			= 8.33f;
		protected const float Code93MainHeight			= 55f;
		protected const float Code93LineHeightForCut	= Code93LineHeightShort;
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

        public override void Draw(object context, StiBarCode barCode, RectangleF rect, float zoom)
		{		
			var code = GetCode(barCode);
			code = CheckCodeSymbols(code, Code93Symbols);

            #region make barsArray for output 
            var fullCode = new int[code.Length + 4];
			for (int index = 0; index < code.Length; index++) 
			{
                fullCode[index + 1] = Code93Symbols.IndexOf(code[index]);
			}

			#region checksum 1
			int sum1 = 0;
			int multiplier = 1;
			for (int index = code.Length - 1; index >= 0; index--)
			{
				sum1 += fullCode[index + 1] * multiplier;
				multiplier++;
				if (multiplier > 20)
				{
					multiplier = 1;
				}
			}
			fullCode[fullCode.Length - 3] = sum1 % 47;
			#endregion

			#region checksum 2
			int sum2 = 0;
			multiplier = 1;
			for (int index = code.Length; index >= 0; index--)
			{
				sum2 += fullCode[index + 1] * multiplier;
				multiplier++;
				if (multiplier > 15)
				{
					multiplier = 1;
				}
			}
			fullCode[fullCode.Length - 2] = sum2 % 47;
			#endregion

			fullCode[0] = Code93Table.Length-2;	//start
			fullCode[fullCode.Length - 1] = Code93Table.Length-1;   //stop

            var barsArray = new StringBuilder();
			for (int index = 0; index < fullCode.Length; index++)
			{	
				barsArray.Append(CodeToBar(Code93Table[fullCode[index]]));
			}
			#endregion

			CalculateSizeFull(
				Code93SpaceLeft,
				Code93SpaceRight,
				Code93SpaceTop,
				Code93SpaceBottom,
				Code93LineHeightShort,
				Code93LineHeightLong,
				Code93TextPosition,
				Code93TextHeight,
				Code93MainHeight,
				Code93LineHeightForCut, 
				Ratio,
				zoom,
				code,
				code,
				barsArray.ToString(),
				rect,
				barCode);

            DrawBarCode(context, rect, barCode); 
		}
		#endregion 
        
        #region Methods.override
        public override StiBarCodeTypeService CreateNew() => new StiCode93BarCodeType();
        #endregion

		public StiCode93BarCodeType() : this(13f, 1f, 2.2f)
		{
		}

		public StiCode93BarCodeType(float module, float height, float ratio)
		{
			this.module = module;
			this.height = height;
			this.ratio  = ratio;
		}
	}
}