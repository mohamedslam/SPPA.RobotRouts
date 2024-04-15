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
using System;
using System.ComponentModel;
using System.Text;
using System.Drawing;
using Stimulsoft.Base.Design;
#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

namespace Stimulsoft.Report.BarCodes
{
    /// <summary>
    /// The class describes the Barcode type - RoyalMail4State.
    /// </summary>
    [TypeConverter(typeof(Stimulsoft.Report.BarCodes.Design.StiRoyalMail4StateBarCodeTypeConverter))]
	public class StiRoyalMail4StateBarCodeType : StiBarCodeTypeService
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiRoyalMail4StateBarCodeType
            jObject.AddPropertyFloat("Module", Module, 20f);
            jObject.AddPropertyFloat("Height", Height, 1f);
            jObject.AddPropertyEnum("CheckSum", CheckSum, StiCheckSum.Yes);

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
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject

        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiRoyalMail4StateBarCodeType;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            var list = new[]
            {
                propHelper.CheckSum(),
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
		public override string ServiceName => "Royal Mail 4-state";
		#endregion

		#region RoyalMail4State Tables
		protected string RoyalMail4StateSymbols = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		private string RoyalMail4StateStartCode = "1";
		private string RoyalMail4StateStopCode = "0";
        private string[] RoyalMail4StateCodes =
        {
            "3300",
            "3210",
            "3201",
            "2310",
            "2301",
            "2211",
            "3120",
            "3030",
            "3021",
            "2130",
            "2121",
            "2031",
            "3102",
            "3012",
            "3003",
            "2112",
            "2103",
            "2013",
            "1320",
            "1230",
            "1221",
            "0330",
            "0321",
            "0231",
            "1302",
            "1212",
            "1203",
            "0312",
            "0303",
            "0213",
            "1122",
            "1032",
            "1023",
            "0132",
            "0123",
            "0033"
        };
        #endregion

        #region Properties
        public override string DefaultCodeValue => "529508A";

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

		private float height = 1f;
        /// <summary>
        /// Gets os sets height factor of the bar code.
        /// </summary>		
        [Description("Gets os sets height factor of the bar code.")]
		[DefaultValue(1f)]
		[Browsable(false)]
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
				if (value > 1f)	height = 1f;
			}
		}

        /// <summary>
        /// Gets or sets mode of checksum.
        /// </summary>
        [Description("Gets or sets mode of checksum.")]
        [DefaultValue(StiCheckSum.Yes)]
        [StiSerializable]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiCategory("BarCode")]
        public StiCheckSum CheckSum { get; set; } = StiCheckSum.Yes;

        internal override float LabelFontHeight => RoyalMail4StateTextHeight;

        protected override bool PreserveAspectRatio => true;

        public override bool[] VisibleProperties
        {
            get
            {
                var props = new bool[visiblePropertiesCount];
                props[13] = true;
                props[23] = true;

                return props;
            }
        }
		#endregion

		#region Consts
		protected const float RoyalMail4StateSpaceLeft = 3.9f;
		protected const float RoyalMail4StateSpaceRight = 3.9f;
		protected const float RoyalMail4StateSpaceTop = 3.9f;
		protected const float RoyalMail4StateSpaceBottom = 3.9f;
		protected const float RoyalMail4StateLineHeightShort = RoyalMail4StateLineHeightLong * 0.62f;
		protected const float RoyalMail4StateLineHeightLong = 10f;
        protected const float RoyalMail4StateTextPosition = RoyalMail4StateSpaceTop + RoyalMail4StateLineHeightLong + RoyalMail4StateSpaceBottom + 1f;
		protected const float RoyalMail4StateTextHeight = 5f;
        protected const float RoyalMail4StateMainHeight = RoyalMail4StateSpaceTop + RoyalMail4StateLineHeightLong + RoyalMail4StateSpaceBottom + 7f;
		protected const float RoyalMail4StateLineHeightForCut = RoyalMail4StateLineHeightLong;
		protected override StringAlignment TextAlignment { get { return StringAlignment.Center; } }
		#endregion

		#region Methods

		private string CharTo4State(char inputChar)
		{
            int inputNumber = RoyalMail4StateSymbols.IndexOf(inputChar);
            return RoyalMail4StateCodes[inputNumber];
		}

		private string StateToBar(string inputCode)
		{
			var outputBar = new StringBuilder();
			for (int index = 0; index < inputCode.Length; index++)
			{
				switch (inputCode[index])
				{
					case '0':
						outputBar.Append("c");
						break;
					case '1':
						outputBar.Append("d");
						break;
					case '2':
						outputBar.Append("e");
						break;
					case '3':
						outputBar.Append("f");
						break;
				}
				outputBar.Append("0");
			}
			return outputBar.ToString();
		}

		private bool MakeBarsArray(ref string code, ref string barsArray, ref string errorString)
		{
			#region Make tempBarsArray
			var tempBarsArray = new StringBuilder();
            int sum1 = 0;
            int sum2 = 0;
            for (int index = 0; index < code.Length; index++)
            {
                string symb = CharTo4State(code[index]);
                if (symb[0] == '0' || symb[0] == '1') sum1 += 4;
                if (symb[0] == '0' || symb[0] == '2') sum2 += 4;
                if (symb[1] == '0' || symb[1] == '1') sum1 += 2;
                if (symb[1] == '0' || symb[1] == '2') sum2 += 2;
                if (symb[2] == '0' || symb[2] == '1') sum1 += 1;
                if (symb[2] == '0' || symb[2] == '2') sum2 += 1;
                tempBarsArray.Append(symb);
            }
            sum1 = sum1 % 6;
            sum2 = sum2 % 6;
            if (sum1 == 0) sum1 = 6;
            if (sum2 == 0) sum2 = 6;
            int checksumValue = (sum1 - 1) * 6 + (sum2 - 1);
            if (CheckSum == StiCheckSum.Yes)
            {
                tempBarsArray.Append(CharTo4State(RoyalMail4StateSymbols[checksumValue]));
            }
			#endregion

			barsArray = StateToBar(RoyalMail4StateStartCode + tempBarsArray.ToString() + RoyalMail4StateStopCode);

            return true;
		}

        public override void Draw(object context, StiBarCode barCode, RectangleF rect, float zoom)
		{		
			var code = GetCode(barCode);
			code = CheckCodeSymbols(code, RoyalMail4StateSymbols);

            var barsArray = string.Empty;
            var errorString = string.Empty;
			if (MakeBarsArray(ref code, ref barsArray, ref errorString))
			{
				CalculateSizeFull(
					RoyalMail4StateSpaceLeft,
					RoyalMail4StateSpaceRight,
					RoyalMail4StateSpaceTop,
					RoyalMail4StateSpaceBottom,
					RoyalMail4StateLineHeightShort,
					RoyalMail4StateLineHeightLong,
					RoyalMail4StateTextPosition,
					RoyalMail4StateTextHeight,
					RoyalMail4StateMainHeight,
					RoyalMail4StateLineHeightForCut, 
					1,
					zoom,
					code,
					code,
					barsArray,
					rect,
					barCode);

                DrawBarCode(context, rect, barCode); 
			}
			else
			{
				if (errorString.Length > 0)
				{
                    DrawBarCodeError(context, rect, barCode, errorString);
				}
				else
				{
                    DrawBarCodeError(context, rect, barCode);
				}
			}

		}
		#endregion

        #region Methods.override
        public override StiBarCodeTypeService CreateNew() => new StiRoyalMail4StateBarCodeType();
        #endregion

        public StiRoyalMail4StateBarCodeType() : this(20f, 1f, StiCheckSum.Yes)
		{
		}

        public StiRoyalMail4StateBarCodeType(float module, float height, StiCheckSum checkSum)
		{
			this.module = module;
			this.height = height;
            this.CheckSum = checkSum;
		}

	}
}
