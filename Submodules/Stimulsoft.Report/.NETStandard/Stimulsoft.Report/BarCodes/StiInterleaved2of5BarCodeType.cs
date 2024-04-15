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
    /// The class describes the Barcode type - 2of5 Interleaved.
    /// </summary>	
    [TypeConverter(typeof(StiInterleaved2of5BarCodeTypeConverter))]
	public class StiInterleaved2of5BarCodeType : StiBarCodeTypeService
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiInterleaved2of5BarCodeType
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
        public override StiComponentId ComponentId => StiComponentId.StiInterleaved2of5BarCodeType;

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
		public override string ServiceName => "2of5 Interleaved";
        #endregion

        #region Properties
        public override string DefaultCodeValue => "12345678";

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
				if (value < 7.5f)module = 7.5f;
				if (value > 40f)module = 40f;
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
				if (value < 0.5f)height = 0.5f;
				if (value > 2.0f)height = 2.0f;
			}
		}


		private float ratio = 2.2f;
        /// <summary>
        /// Get or sets value, which indicates WideToNarrow ratio.
        /// </summary>
		[StiSerializable]
		[DefaultValue(2.2f)]
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
				float minRatio = (Module > 20f ? 2.0f : 2.2f);
				if (value < minRatio)ratio = minRatio;
				if (value > 3.0f)ratio = 3.0f;
			}
		}
        internal override float LabelFontHeight => Interleaved2of5TextHeight;

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

		#region 2of5 Interleaved encoding table
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
		private const float Interleaved2of5SpaceLeft = 10f;
		private const float Interleaved2of5SpaceRight = 10f;
		private const float Interleaved2of5SpaceTop = 0f;
		private const float Interleaved2of5SpaceBottom = 1f;
		private const float Interleaved2of5LineHeightShort = 40f;
		private const float Interleaved2of5LineHeightLong = Interleaved2of5LineHeightShort;
		private const float Interleaved2of5TextHeight = 8.33f;
		private const float Interleaved2of5MainHeight = 50f;
		private const float Interleaved2of5TextPosition = Interleaved2of5LineHeightShort + Interleaved2of5SpaceBottom;
		private const float Interleaved2of5LineHeightForCut = Interleaved2of5LineHeightShort;
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
			code = CheckCodeSymbols(code, "0123456789");
			if (code.Length % 2 == 1)
			{
				code = '0' + code;
			}

			#region make barsArray for output 
            var barsArray = new StringBuilder();
			barsArray.Append(symTableSet[10]);
			if (code.Length > 0) 
			{
				for (int index = 0; index < code.Length/2; index++)
				{
					var sym1 = symTableSet[int.Parse(code[index*2+0].ToString())];
					var sym2 = symTableSet[int.Parse(code[index*2+1].ToString())];
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
					if (barsArray[index] == '0') currentsym = '0';	else currentsym = '1';
				}
				barsArray[index] = currentsym;
				counter = !counter; 
			}
			#endregion

			CalculateSizeFull(
				Interleaved2of5SpaceLeft,
                Interleaved2of5SpaceRight,
				Interleaved2of5SpaceTop,
				Interleaved2of5SpaceBottom,
				Interleaved2of5LineHeightShort,
				Interleaved2of5LineHeightLong,
				Interleaved2of5TextPosition,
				Interleaved2of5TextHeight,
				Interleaved2of5MainHeight,
				Interleaved2of5LineHeightForCut,
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
        public override StiBarCodeTypeService CreateNew() => new StiInterleaved2of5BarCodeType();
        #endregion

		public StiInterleaved2of5BarCodeType() : this(13f, 1f, 2.2f)
		{
		}

		public StiInterleaved2of5BarCodeType(float module, float height, float ratio) 
		{
			this.module = module;
			this.height = height;
			this.ratio  = ratio;
		}
	}
}