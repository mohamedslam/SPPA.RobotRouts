#region Copyright (C) 2003-2022 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports												}
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
    /// The class describes the Barcode type - Code11.
    /// </summary>
    [TypeConverter(typeof(StiCode11BarCodeTypeConverter))]
	public class StiCode11BarCodeType : StiBarCodeTypeService
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);
            
            // StiBarCodeTypeService
            jObject.AddPropertyFloat("Module", Module, 8f);
            jObject.AddPropertyFloat("Height", Height, 1f);
            jObject.AddPropertyEnum("Checksum", Checksum, StiCode11CheckSum.Auto);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Checksum":
                        this.Checksum = property.DeserializeEnum<StiCode11CheckSum>();
                        break;

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

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiCode11BarCodeType;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            var list = new[]
            {
                propHelper.BarCodeChecksum(),
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
		public override string ServiceName => "Code11";
		#endregion

		#region Code11Table
		protected string Code11Symbols = "0123456789-";
		protected int Code11StartStopSymbolIndex = 11;
		protected string[] Code11Table = new string[12]
		{
			"40405",	//0
			"50405",	//1
			"41405",	//2
			"51404",	//3
			"40505",	//4
			"50504",	//5
			"41504",	//6
			"40415",	//7
			"50414",	//8
			"50404",	//9
			"40504",	//-
			"40514"		//start/stop symbol only
		};
        #endregion

        #region Properties
        public override string DefaultCodeValue => "A12345678B";

        private float module = defaultCode11Module;
        /// <summary>
        /// Gets or sets width of the most fine element of the bar code.
        /// </summary>
        [Description("Gets or sets width of the most fine element of the bar code.")]
		[DefaultValue(defaultCode11Module)]
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
				if (value < 4f)module = 4f;
				if (value > 40f) module = 40f;
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
				if (value < 0.2f)	height = 0.2f;
				if (value > 2.0f)	height = 2.0f;
			}
		}

        /// <summary>
        /// Gets or sets mode of checksum.
        /// </summary>
        [Description("Gets or sets mode of checksum.")]
        [DefaultValue(StiCode11CheckSum.Auto)]
        [TypeConverter(typeof(StiEnumConverter))]
		[Editor(StiEditors.Enum, typeof(UITypeEditor))]
		[StiCategory("BarCode")]
        [StiSerializable]
        public StiCode11CheckSum Checksum { get; set; } = StiCode11CheckSum.Auto;

        internal override float LabelFontHeight => Code11TextHeight;

        public override bool[] VisibleProperties
        {
            get
            {
                var props = new bool[visiblePropertiesCount];
                props[3] = true;
                props[11] = true;
                props[13] = true;

                return props;
            }
        }
		#endregion

		#region Consts
		protected const float Code11SpaceLeft			= 5f;
		protected const float Code11SpaceRight			= 5f;
		protected const float Code11SpaceTop			= 0f;
		protected const float Code11SpaceBottom			= 1f;
		protected const float Code11LineHeightShort		= 40f;
		protected const float Code11LineHeightLong		= Code11LineHeightShort;
		protected const float Code11TextPosition		= Code11LineHeightShort + Code11SpaceBottom;
		protected const float Code11TextHeight			= 8f;
		protected const float Code11MainHeight			= 50f;
		protected const float Code11LineHeightForCut	= Code11LineHeightShort;
		protected const float defaultCode11Module = 8f;
		#endregion

		#region Methods
        public override void Draw(object context, StiBarCode barCode, RectangleF rect, float zoom)
		{		
			var code = GetCode(barCode);
			code = CheckCodeSymbols(code, Code11Symbols);

			#region make barsArray for output
			int checksumLength = 0;
			if (Checksum == StiCode11CheckSum.Auto) checksumLength = (code.Length >= 10 ? 2 : 1);
			if (Checksum == StiCode11CheckSum.OneDigit) checksumLength = 1;
			if (Checksum == StiCode11CheckSum.TwoDigits) checksumLength = 2;
			var fullCode = new int[code.Length + 2 + checksumLength];
			fullCode[0] = Code11StartStopSymbolIndex;
			for (int index = 0; index < code.Length; index++) 
			{
                fullCode[index + 1] = Code11Symbols.IndexOf(code[index]);
			}
            //calculate Checksum "C"
            int checkSum = 0;
			int weight = 1;
			for (int index = code.Length; index > 0; index--) 
			{
				checkSum += fullCode[index] * weight;
				weight++;
				if (weight > 10) weight = 1;
			}
			fullCode[code.Length + 1] = checkSum % 11;
			if (checksumLength == 2)
			{
				//calculate checksum "K"
				checkSum = 0;
				weight = 1;
				for (int index = code.Length + 1; index > 0; index--) 
				{
					checkSum += fullCode[index] * weight;
					weight++;
					if (weight > 9) weight = 1;
				}
				fullCode[code.Length + 2] = checkSum % 9;
			}
			fullCode[fullCode.Length - 1] = fullCode[0]; //start/stop

			var barsArray = new StringBuilder();
			for (int index = 0; index < fullCode.Length; index++)
			{	
				barsArray.Append(Code11Table[fullCode[index]]);
				if (index != (fullCode.Length - 1))
				{
                    barsArray.Append("0");	//interspace
				}
			}
			#endregion

			CalculateSizeFull(
				Code11SpaceLeft,
				Code11SpaceRight,
				Code11SpaceTop,
				Code11SpaceBottom,
				Code11LineHeightShort,
				Code11LineHeightLong,
				Code11TextPosition,
				Code11TextHeight,
				Code11MainHeight,
				Code11LineHeightForCut, 
				2f,
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
        public override StiBarCodeTypeService CreateNew() => new StiCode11BarCodeType();
        #endregion

		public StiCode11BarCodeType() : this(defaultCode11Module, 1f, StiCode11CheckSum.Auto)
		{
		}

		public StiCode11BarCodeType(float module, float height, StiCode11CheckSum checksum)
		{
			this.module = module;
			this.height = height;
			this.Checksum = checksum;
		}
	}
}