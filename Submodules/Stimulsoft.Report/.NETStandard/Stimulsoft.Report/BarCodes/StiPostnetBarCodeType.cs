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
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace Stimulsoft.Report.BarCodes
{
    /// <summary>
    /// The class describes the Barcode type - Postnet.
    /// </summary>
    [TypeConverter(typeof(Stimulsoft.Report.BarCodes.Design.StiPostnetBarCodeTypeConverter))]
	public class StiPostnetBarCodeType : StiBarCodeTypeService
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiPostnetBarCodeType
            jObject.AddPropertyFloat("Module", Module, 20f);
            jObject.AddPropertyFloat("Height", Height, 1f);
            jObject.AddPropertyFloat("Space", Space, 26f);

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

                    case "Space":
                        this.space = property.DeserializeFloat();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject

        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiPostnetBarCodeType;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            var list = new[]
            {
                propHelper.fHeight(),
                propHelper.Module(),
                propHelper.Space()
            };
            objHelper.Add(StiPropertyCategories.Main, list);

            return objHelper;
        }
        #endregion

		#region ServiceName
		/// <summary>
		/// Gets a service name.
		/// </summary>
		public override string ServiceName => "Postnet";
		#endregion

		#region PostnetTable
		protected string PostnetSymbols = "0123456789";
		private string[] PostnetTable = new string[10]
		{
			"11000",	//0
			"00011",	//1
			"00101",	//2
			"00110",	//3
			"01001",	//4
			"01010",	//5
			"01100",	//6
			"10001",	//7
			"10010",	//8
			"10100"		//9
		};
		private string PostnetStartCode = "1";
		private string PostnetStopCode = "1";
        #endregion

        #region Properties
        public override string DefaultCodeValue => "12345";

        private float module = 20f;
        /// <summary>
        /// Gets or sets width of the most fine element of the bar code.
        /// </summary>
        [Description("Gets or sets width of the most fine element of the bar code.")]
		[DefaultValue(20f)]
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
				if (value < 20f)	module = 20f;
				if (value > 20f)	module = 20f;
			}
		}

		private float space = 26f;
        /// <summary>
        /// Gets or sets space between elements of bar code.
        /// </summary>
        [Description("Gets or sets space between elements of bar code.")]
		[DefaultValue(26f)]
		[StiSerializable]
        [StiCategory("BarCode")]
        public float Space
		{
			get
			{
				return space;
			}
			set
			{
				space = value;
				if (value < 12f)	space = 12f;
				if (value > 40f)	space = 40f;
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
				if (value < 1f)	height = 1f;
				if (value > 4f)	height = 4f;
			}
		}

        internal override float LabelFontHeight => PostnetTextHeight;

		protected override bool PreserveAspectRatio => true;

		public override bool[] VisibleProperties
        {
            get
            {
                var props = new bool[visiblePropertiesCount];
                props[11] = true;
                props[13] = true;
                props[18] = true;

                return props;
            }
        }
		#endregion

		#region Consts
		protected const float PostnetSpaceLeft = 1f;
		protected const float PostnetSpaceRight = 1f;
		protected const float PostnetSpaceTop = 0f;
		protected const float PostnetSpaceBottom = 1f;
		protected const float PostnetLineHeightShort = PostnetLineHeightLong * 2f / 5f;
		protected const float PostnetLineHeightLong = 6.25f;
		protected const float PostnetTextPosition = PostnetLineHeightLong + PostnetSpaceBottom;
		protected const float PostnetTextHeight = 5.5f;
		protected const float PostnetMainHeight = 14f;
		protected const float PostnetLineHeightForCut = PostnetLineHeightLong;
		#endregion

		#region Methods
		private string CodeToBar(string inputCode)
		{
            var outputBar = new StringBuilder();
			for (int index = 0; index < inputCode.Length; index++)
			{
                var currentsym = new StringBuilder();
				if (inputCode[index] == '1') 
				{
					currentsym.Append("81");	
				}
				else
				{
					currentsym.Append("e1");	
				}
				outputBar.Append(currentsym);
			}
			return outputBar.ToString();
		}

        public override void Draw(object context, StiBarCode barCode, RectangleF rect, float zoom)
		{		
			string code = GetCode(barCode);
			code = CheckCodeSymbols(code, PostnetSymbols);
			
			#region check code for length 5, 9 or 11
			if (code.Length > 11)	code = code.Substring(0,11);
			switch (code.Length)
			{
				case 0:
					code = "00000";
					break;
				case 1:
					code = code + "0000";
					break;
				case 2:
				case 6:
					code = code + "000";
					break;
				case 3:
				case 7:
					code = code + "00";
					break;
				case 4:
				case 8:
				case 10:
					code = code + "0";
					break;
			}
            #endregion

            var codeText = new StringBuilder(code);
			if (code.Length == 11) codeText.Insert(9,"-");
			if (code.Length > 5) codeText.Insert(5,"-");

			#region make barsArray for output 
			var fullCode = new int[code.Length + 1];
			for (int index = 0; index < code.Length; index++) 
			{
                fullCode[index] = PostnetSymbols.IndexOf(code[index]);
			}

			#region calculate checksum Modulo 10
			int sum = 0;
			for (int index = 0; index < code.Length; index++)
			{
				sum += fullCode[index];
			}
			sum = sum % 10;
			if (sum != 0)
			{
				sum = 10 - sum;
			}
			fullCode[code.Length] = sum;
            #endregion

            var tempBarsArray = new StringBuilder();
			tempBarsArray.Append(PostnetStartCode);
			for (int index = 0; index < fullCode.Length; index++)
			{	
				tempBarsArray.Append(PostnetTable[fullCode[index]]);
			}
			tempBarsArray.Append(PostnetStopCode);
            var barsArray = new StringBuilder(CodeToBar(tempBarsArray.ToString()));
			#endregion

			CalculateSizeFull(
				PostnetSpaceLeft,
				PostnetSpaceRight,
				PostnetSpaceTop,
				PostnetSpaceBottom,
				PostnetLineHeightShort,
				PostnetLineHeightLong,
				PostnetTextPosition,
				PostnetTextHeight,
				PostnetMainHeight,
				PostnetLineHeightForCut, 
				Space / Module,
				zoom,
				code,
				codeText.ToString(),
				barsArray.ToString(),
				rect,
				barCode);

            DrawBarCode(context, rect, barCode); 
		}
		#endregion

        #region Methods.override
        public override StiBarCodeTypeService CreateNew() => new StiPostnetBarCodeType();
        #endregion

		public StiPostnetBarCodeType() : this(20f, 1f, 26f)
		{
		}

		public StiPostnetBarCodeType(float module, float height, float space)
		{
			this.module = module;
			this.height = height;
			this.space = space;
		}
	}
}