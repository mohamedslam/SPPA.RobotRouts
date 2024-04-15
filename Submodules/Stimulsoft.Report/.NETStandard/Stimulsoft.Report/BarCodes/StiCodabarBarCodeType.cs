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
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.BarCodes.Design;
using Stimulsoft.Report.PropertyGrid;

namespace Stimulsoft.Report.BarCodes
{
	/// <summary>
	/// The class describes the Barcode type - Codabar.
	/// </summary>
	[TypeConverter(typeof(StiCodabarBarCodeTypeConverter))]
	public class StiCodabarBarCodeType : StiBarCodeTypeService
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiCodabarBarCodeType
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
        public override StiComponentId ComponentId => StiComponentId.StiCodabarBarCodeType;

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
		public override string ServiceName => "Codabar";
		#endregion

		#region CodabarTable
		private string CodabarSymbols = "0123456789-$:/.+ABCD";
		private string[] CodabarTable = new string[20]
		{
			"11111221",	//0
			"11112211",	//1
			"11121121",	//2
			"22111111",	//3
			"11211211",	//4
			"21111211",	//5
			"12111121",	//6
			"12112111",	//7
			"12211111",	//8
			"21121111",	//9
			"11122111",	//-
			"11221111",	//$
			"21112121",	//:
			"21211121",	// /
			"21212111",	//.
			"11222221",	//+
			"11221211",	//A
			"11121221",	//B
			"12121121",	//C
			"11122211"	//D
		};
        #endregion

        #region Properties
        public override string DefaultCodeValue => "A12345678B";

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
				if (value < 7.5f)	module = 7.5f;
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

        internal override float LabelFontHeight => CodabarTextHeight;

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
		protected const float CodabarSpaceLeft = 7f;
		protected const float CodabarSpaceRight = 7f;
		protected const float CodabarSpaceTop = 0f;
		protected const float CodabarSpaceBottom = 1f;
		protected const float CodabarLineHeightShort = 45f; //maybe
		protected const float CodabarLineHeightLong = CodabarLineHeightShort;
		protected const float CodabarTextPosition = CodabarLineHeightShort + CodabarSpaceBottom;
		protected const float CodabarTextHeight = 8.33f;
		protected const float CodabarMainHeight = 55f;
		protected const float CodabarLineHeightForCut = CodabarLineHeightShort;
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
					if (inputCode[index] == '1') currentsym = '4'; else currentsym = '5';
				}
				else
				{
					if (inputCode[index] == '1') currentsym = '0';	else currentsym = '1';
				}
				outputBar.Append(currentsym);
				counter = !counter; 
			}
			return outputBar.ToString();
		}

        public override void Draw(object context, StiBarCode barCode, RectangleF rect, float zoom)
		{
			var code = GetCode(barCode);
			code = CheckCodeSymbols(code, CodabarSymbols);

			#region make barsArray for output 
			var fullCode = new int[code.Length];
			for (int index = 0; index < code.Length; index++) 
			{
                fullCode[index] = CodabarSymbols.IndexOf(code[index]);
			}
			var barsArray = new StringBuilder();
			for (int index = 0; index < fullCode.Length; index++)
			{	
				barsArray.Append(CodeToBar(CodabarTable[fullCode[index]]));
			}
			#endregion

			CalculateSizeFull(
				CodabarSpaceLeft,
				CodabarSpaceRight,
				CodabarSpaceTop,
				CodabarSpaceBottom,
				CodabarLineHeightShort,
				CodabarLineHeightLong,
				CodabarTextPosition,
				CodabarTextHeight,
				CodabarMainHeight,
				CodabarLineHeightForCut, 
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
        public override StiBarCodeTypeService CreateNew() => new StiCodabarBarCodeType();
        #endregion

		public StiCodabarBarCodeType() : this(13f, 1f, 2.2f)
		{
		}

		public StiCodabarBarCodeType(float module, float height, float ratio)
		{
			this.module = module;
			this.height = height;
			this.ratio  = ratio;
		}
	}
}