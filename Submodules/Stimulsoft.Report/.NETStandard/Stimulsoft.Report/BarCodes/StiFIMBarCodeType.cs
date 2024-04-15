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
using System.ComponentModel;
using System.Drawing;
#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

namespace Stimulsoft.Report.BarCodes
{
    /// <summary>
    /// The class describes the Barcode type - FIM.
    /// </summary>
    [TypeConverter(typeof(StiFIMBarCodeTypeConverter))]
	public class StiFIMBarCodeType : StiBarCodeTypeService
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);
            
            // StiFIMBarCodeType
            jObject.AddPropertyFloat("Module", Module, defaultFIMModule);
            jObject.AddPropertyFloat("Height", Height, 1f);
            jObject.AddPropertyBool("AddClearZone", AddClearZone);

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

                    case "AddClearZone":
                        this.AddClearZone = property.DeserializeBool();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiFIMBarCodeType;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            var list = new[]
            {
                propHelper.AddClearZone()
            };
            objHelper.Add(StiPropertyCategories.Main, list);

            return objHelper;
        }
        #endregion

		#region ServiceName
		/// <summary>
		/// Gets a service name.
		/// </summary>
		public override string ServiceName => "FIM";
		#endregion

		#region FIMTable
		protected string FIMSymbols = "abcd";
		protected string[] FIMTable = new string[4]
		{
			"4040114011404",	//a 110010011
			"40140401404014",	//b 101101101
			"40401401401404",	//c 110101011
			"404040140140404",	//d 111010111
		};
		protected const float defaultFIMModule = 31.25f;
        #endregion

        #region Properties
        public override string DefaultCodeValue => "A";

        public override bool[] VisibleProperties
        {
            get
            {
                var props = new bool[visiblePropertiesCount];
                props[22] = true;

                return props;
            }
        }

		private float module = defaultFIMModule;
        /// <summary>
        /// Gets or sets width of the most fine element of the bar code.
        /// </summary>
        [Description("Gets or sets width of the most fine element of the bar code.")]
		[DefaultValue(10f)]
		[Browsable(false)]
		public override float Module
		{
			get
			{
				return module;
			}
			set
			{
				module = defaultFIMModule;
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
				height = 1f;
			}
		}

        /// <summary>
        /// Gets or sets value which indicates will show Clear Zone or no.
        /// </summary>
		[DefaultValue(false)]
        [StiSerializable]
        [Description("Gets or sets value which indicates will show Clear Zone or no.")]
        [TypeConverter(typeof(StiBoolConverter))]
		[Editor(StiEditors.Bool, typeof(UITypeEditor))]
		[StiCategory("BarCode")]
        public bool AddClearZone { get; set; }

		protected override bool PreserveAspectRatio => true;

		internal override float LabelFontHeight
        {
            get
            {
                return FIMTextHeight;
            }
        }
		#endregion

		#region Consts
		protected float FIMSpaceLeft { get {if (AddClearZone) return 16f; else return 1f; } }
		protected float FIMSpaceRight { get {if (AddClearZone) return 7f; else return 1f; } }
		protected const float FIMSpaceTop			= 0f;
		protected const float FIMSpaceBottom		= 0f;
		protected const float FIMLineHeightShort	= 20f;
		protected const float FIMLineHeightLong		= FIMLineHeightShort;
		protected const float FIMTextPosition		= 0f;
		protected const float FIMTextHeight			= 8f;
		protected const float FIMMainHeight			= FIMLineHeightShort;
		protected const float FIMLineHeightForCut	= FIMLineHeightShort;
		#endregion

		#region Methods
        public override void Draw(object context, StiBarCode barCode, RectangleF rect, float zoom)
		{		
			string code = GetCode(barCode).ToLower();
			code = CheckCodeSymbols(code, FIMSymbols) + 'a';

			#region make barsArray for output
			string barsArray = string.Empty;
			switch (code[0])
			{
				case 'a':
					barsArray = FIMTable[0];
					break;
				case 'b':
					barsArray = FIMTable[1];
					break;
				case 'c':
					barsArray = FIMTable[2];
					break;
				case 'd':
					barsArray = FIMTable[3];
					break;
			}
			#endregion

			CalculateSizeFull(
				FIMSpaceLeft,
				FIMSpaceRight,
				FIMSpaceTop,
				FIMSpaceBottom,
				FIMLineHeightShort,
				FIMLineHeightLong,
				FIMTextPosition,
				FIMTextHeight,
				FIMMainHeight,
				FIMLineHeightForCut, 
				2f,
				zoom,
				code,
				string.Empty,
				barsArray,
				rect,
				barCode);

            DrawBarCode(context, rect, barCode); 
		}
		#endregion 
        
        #region Methods.override
        public override StiBarCodeTypeService CreateNew() => new StiFIMBarCodeType();
        #endregion

		public StiFIMBarCodeType() : this(defaultFIMModule, 1f, false)
		{
		}

		public StiFIMBarCodeType(float module, float height, bool addClearZone)
		{
			this.module = module;
			this.height = height;
			this.AddClearZone = addClearZone;
		}
	}
}