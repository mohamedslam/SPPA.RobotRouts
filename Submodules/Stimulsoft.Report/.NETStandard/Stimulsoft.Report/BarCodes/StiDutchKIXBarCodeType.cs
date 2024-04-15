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
using Stimulsoft.Report.PropertyGrid;
using System.ComponentModel;
using System.Text;
using System.Drawing;

namespace Stimulsoft.Report.BarCodes
{
    /// <summary>
    /// The class describes the Barcode type - Royal TPG Post KIX 4-State Barcode.
    /// </summary>
    public class StiDutchKIXBarCodeType : StiBarCodeTypeService
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);
            
            // StiBarCodeTypeService
            jObject.AddPropertyFloat("Module", Module, 20f);
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

        #region IStiPropertyGridObject

        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiDutchKIXBarCodeType;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            var list = new[]
            {
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
		public override string ServiceName => "Royal TPG Post KIX 4-State";
		#endregion

		#region DutchKIX Tables
		protected string DutchKIXSymbols = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private string[] DutchKIXCodes =
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
        public override string DefaultCodeValue => "2500GG30250";

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
				if (value < 15f)	module = 15f;
				if (value > 25f)	module = 25f;
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

        internal override float LabelFontHeight => DutchKIXTextHeight;

        protected override bool PreserveAspectRatio => true;

        public override bool[] VisibleProperties
        {
            get
            {
                var props = new bool[visiblePropertiesCount];
                props[13] = true;

                return props;
            }
        }
		#endregion

		#region Consts
		protected const float DutchKIXSpaceLeft = 3.9f;
		protected const float DutchKIXSpaceRight = 3.9f;
		protected const float DutchKIXSpaceTop = 3.9f;
		protected const float DutchKIXSpaceBottom = 2f;
		protected const float DutchKIXLineHeightShort = DutchKIXLineHeightLong * 0.62f;
		protected const float DutchKIXLineHeightLong = 10f;
        protected const float DutchKIXTextPosition = DutchKIXSpaceTop + DutchKIXLineHeightLong + DutchKIXSpaceBottom + 1f;
		protected const float DutchKIXTextHeight = 5f;
        protected const float DutchKIXMainHeight = DutchKIXSpaceTop + DutchKIXLineHeightLong + DutchKIXSpaceBottom + 7f;
		protected const float DutchKIXLineHeightForCut = DutchKIXLineHeightLong;
		protected override StringAlignment TextAlignment { get { return StringAlignment.Center; } }
		#endregion

		#region Methods

		private string CharTo4State(char inputChar)
		{
            int inputNumber = DutchKIXSymbols.IndexOf(inputChar);
            return DutchKIXCodes[inputNumber];
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
            for (int index = 0; index < code.Length; index++)
            {
                tempBarsArray.Append(CharTo4State(code[index]));
            }
			#endregion

			barsArray = StateToBar(tempBarsArray.ToString());

            return true;
		}

        public override void Draw(object context, StiBarCode barCode, RectangleF rect, float zoom)
		{		
			var code = GetCode(barCode);
			code = CheckCodeSymbols(code, DutchKIXSymbols);

            var barsArray = string.Empty;
            var errorString = string.Empty;
			if (MakeBarsArray(ref code, ref barsArray, ref errorString))
			{
				CalculateSizeFull(
					DutchKIXSpaceLeft,
					DutchKIXSpaceRight,
					DutchKIXSpaceTop,
					DutchKIXSpaceBottom,
					DutchKIXLineHeightShort,
					DutchKIXLineHeightLong,
					DutchKIXTextPosition,
					DutchKIXTextHeight,
					DutchKIXMainHeight,
					DutchKIXLineHeightForCut, 
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
        public override StiBarCodeTypeService CreateNew() => new StiDutchKIXBarCodeType();
        #endregion

        public StiDutchKIXBarCodeType() : this(20f, 1f)
		{
		}

        public StiDutchKIXBarCodeType(float module, float height)
		{
			this.module = module;
			this.height = height;
		}
	}
}