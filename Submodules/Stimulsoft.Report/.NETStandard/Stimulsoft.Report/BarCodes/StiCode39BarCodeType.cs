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
using Stimulsoft.Base.Design;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Report.BarCodes.Design;
using Stimulsoft.Report.PropertyGrid;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

namespace Stimulsoft.Report.BarCodes
{
    /// <summary>
    /// The class describes the Barcode type - Code-39.
    /// </summary>
    [TypeConverter(typeof(StiCode39BarCodeTypeConverter))]
	public class StiCode39BarCodeType : StiBarCodeTypeService
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);
            
            // StiCode39BarCodeType
            jObject.AddPropertyFloat("Module", Module, 13f);
            jObject.AddPropertyFloat("Height", Height, 1f);
            jObject.AddPropertyEnum("CheckSum", CheckSum, StiCheckSum.Yes);
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

                    case "CheckSum":
                        this.CheckSum = property.DeserializeEnum<StiCheckSum>();
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
        public override StiComponentId ComponentId => StiComponentId.StiCode39BarCodeType;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            var list = new[]
            {
                propHelper.CheckSum(),
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
		public override string ServiceName => "Code39";
		#endregion

		#region Code39Table
		protected string Code39Symbols = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ-. $/+%";	//without '*'
		protected int Code39StartStopSymbolIndex = 43;
		protected string[] Code39Table = new string[44]
		{
			"1112212111",	//0
			"2112111121",	//1
			"1122111121",	//2
			"2122111111",	//3
			"1112211121",	//4
			"2112211111",	//5
			"1122211111",	//6
			"1112112121",	//7
			"2112112111",	//8
			"1122112111",	//9
			"2111121121",	//A
			"1121121121",
			"2121121111",
			"1111221121",
			"2111221111",
			"1121221111",
			"1111122121",
			"2111122111",
			"1121122111",
			"1111222111",
			"2111111221",
			"1121111221",
			"2121111211",
			"1111211221",
			"2111211211",
			"1121211211",
			"1111112221",
			"2111112211",
			"1121112211",
			"1111212211",
			"2211111121",
			"1221111121",
			"2221111111",
			"1211211121",
			"2211211111",
			"1221211111",	//Z
			"1211112121",	//-
			"2211112111",	//.
			"1221112111",	//
			"1212121111",	//$
			"1212111211",	// /
			"1211121211",	//+
			"1112121211",	//%
			"1211212111"	//*		start/stop symbol only
		};
        #endregion

        #region Properties
        public override string DefaultCodeValue => "ABC123";

        /// <summary>
        /// Gets or sets a mode of the checksum.
        /// </summary>
        [Description("Gets or sets a mode of the checksum.")]
        [DefaultValue(StiCheckSum.Yes)]
        [StiSerializable]
        [TypeConverter(typeof(StiEnumConverter))]
		[Editor(StiEditors.Enum, typeof(UITypeEditor))]
		[StiCategory("BarCode")]
        public StiCheckSum CheckSum { get; set; } = StiCheckSum.Yes;

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
				if (value < 2f) module = 2f;
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
				if (value < 0.5f)	height = 0.5f;
				if (value > 2.0f)	height = 2.0f;
			}
		}

		private float ratio = 2.2f;
        /// <summary>
        /// Get or sets value, which indicates WideToNarrow ratio.
        /// </summary>
		[DefaultValue(2.2f)]
		[StiSerializable]
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
				if (value < minRatio)	ratio = minRatio;
				if (value > 3.0f)		ratio = 3.0f;
			}
		}

        internal override float LabelFontHeight => Code39TextHeight;

        public override bool[] VisibleProperties
        {
            get
            {
                var props = new bool[visiblePropertiesCount];
                props[11] = true;
                props[13] = true;
                props[15] = true;
                props[23] = true;

                return props;
            }
        }
		#endregion

		#region Consts
		protected const float Code39SpaceLeft			= 10f;
		protected const float Code39SpaceRight			= 10f;
		protected const float Code39SpaceTop			= 0f;
		protected const float Code39SpaceBottom			= 1f;
		protected const float Code39LineHeightShort		= 45f; //maybe
		protected const float Code39LineHeightLong		= Code39LineHeightShort;
		protected const float Code39TextPosition		= Code39LineHeightShort + Code39SpaceBottom;
		protected const float Code39TextHeight			= 8.33f;
		protected const float Code39MainHeight			= 55f;
		protected const float Code39LineHeightForCut	= Code39LineHeightShort;
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
			code = CheckCodeSymbols(code, Code39Symbols);

            #region make barsArray for output 
            var fullCode = new int[code.Length + (CheckSum == StiCheckSum.Yes ? 3 : 2)];
			fullCode[0] = Code39StartStopSymbolIndex;
			int checkSum = 0;
			for (int index = 0; index < code.Length; index++) 
			{
                fullCode[index + 1] = Code39Symbols.IndexOf(code[index]);
				checkSum += fullCode[index + 1];
			}
			if (CheckSum == StiCheckSum.Yes)
			{
				fullCode[fullCode.Length - 2] = checkSum % 43;
			}
			fullCode[fullCode.Length - 1] = fullCode[0]; // "*"

            var barsArray = new StringBuilder();
			for (int index = 0; index < fullCode.Length; index++)
			{	
				barsArray.Append(CodeToBar(Code39Table[fullCode[index]]));
			}
			#endregion

			CalculateSizeFull(
				Code39SpaceLeft,
				Code39SpaceRight,
				Code39SpaceTop,
				Code39SpaceBottom,
				Code39LineHeightShort,
				Code39LineHeightLong,
				Code39TextPosition,
				Code39TextHeight,
				Code39MainHeight,
				Code39LineHeightForCut, 
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
        public override StiBarCodeTypeService CreateNew() => new StiCode39BarCodeType();
        #endregion

		public StiCode39BarCodeType() : this(13f, 1f, 2.2f, StiCheckSum.Yes)
		{
		}

		public StiCode39BarCodeType(float module, float height, float ratio, StiCheckSum checkSum)
		{
			this.module = module;
			this.height = height;
			this.ratio  = ratio;
			this.CheckSum = checkSum;
		}
	}
}