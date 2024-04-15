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

using System.ComponentModel;
using System.Drawing;
using System.Text;
using Stimulsoft.Base;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.BarCodes.Design;
using Stimulsoft.Report.PropertyGrid;

namespace Stimulsoft.Report.BarCodes
{
	/// <summary>
	/// The class describes the Barcode type - Pharmacode.
	/// </summary>
	[TypeConverter(typeof(StiBarCodeTypeServiceConverter))]
	public class StiPharmacodeBarCodeType : StiBarCodeTypeService
	{
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiBarCodeTypeService
            jObject.AddPropertyFloat("Module", Module, defaultPharmacodeModule);
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
        public override StiComponentId ComponentId => StiComponentId.StiPharmacodeBarCodeType;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            var list = new[]
            {
                propHelper.fHeight()
            };
            objHelper.Add(StiPropertyCategories.Main, list);

            return objHelper;
        }
        #endregion

		#region ServiceName
		/// <summary>
		/// Gets a service name.
		/// </summary>
		public override string ServiceName => "Pharmacode";
		#endregion

		protected string PharmacodeSymbols = "0123456789";
		protected const float defaultPharmacodeModule = 20f;

        #region Properties
        public override string DefaultCodeValue => "1256";

        private float module = defaultPharmacodeModule;
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
				module = defaultPharmacodeModule;
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

        internal override float LabelFontHeight => PharmacodeTextHeight;

        public override bool[] VisibleProperties
        {
            get
            {
                var props = new bool[visiblePropertiesCount];
                props[11] = true;

                return props;
            }
        }
		#endregion

		#region Consts
		protected const float PharmacodeSpaceLeft			= 1f;
		protected const float PharmacodeSpaceRight			= 1f;
		protected const float PharmacodeSpaceTop			= 0f;
		protected const float PharmacodeSpaceBottom			= 0.5f;
		protected const float PharmacodeLineHeightShort		= 16f;
		protected const float PharmacodeLineHeightLong		= PharmacodeLineHeightShort;
		protected const float PharmacodeTextPosition		= PharmacodeLineHeightShort + PharmacodeSpaceBottom;
		protected const float PharmacodeTextHeight			= 5f;
		protected const float PharmacodeMainHeight			= 22f;
		protected const float PharmacodeLineHeightForCut	= PharmacodeLineHeightShort;
		#endregion

		#region Methods
        public override void Draw(object context, StiBarCode barCode, RectangleF rect, float zoom)
		{		
			string code = GetCode(barCode);
			code = CheckCodeSymbols(code, PharmacodeSymbols);
			if (code.Length > 6) code = "131070";
			if (code.Length < 1) code = "3";
			int codeInt = int.Parse(code);
			if (codeInt > 131070) codeInt = 131070;
			if (codeInt < 3) codeInt = 3;

			#region make barsArray for output
			var barsArray = new StringBuilder();
			int dataInt = codeInt - 3;
			int numberOfBars = 2;
			int maxValue = 3;
			while (dataInt > maxValue)
			{
				dataInt -= maxValue + 1;
				numberOfBars++;
				maxValue = (1 << numberOfBars) - 1;
			}
			for (int index = numberOfBars - 1; index >= 0; index--)
			{
				if ((dataInt & (1 << index)) > 0)
				{
					barsArray.Append("6");
				}
				else
				{
					barsArray.Append("4");
				}
				if (index != 0)
				{
					barsArray.Append("1");
				}
			}
			#endregion

			CalculateSizeFull(
				PharmacodeSpaceLeft,
				PharmacodeSpaceRight,
				PharmacodeSpaceTop,
				PharmacodeSpaceBottom,
				PharmacodeLineHeightShort,
				PharmacodeLineHeightLong,
				PharmacodeTextPosition,
				PharmacodeTextHeight,
				PharmacodeMainHeight,
				PharmacodeLineHeightForCut, 
				2f,
				zoom,
				code,
				codeInt.ToString(),
				barsArray.ToString(),
				rect,
				barCode);

            DrawBarCode(context, rect, barCode); 
		}
		#endregion 
        
        #region Methods.override
        public override StiBarCodeTypeService CreateNew() => new StiPharmacodeBarCodeType();
        #endregion

		public StiPharmacodeBarCodeType() : this(defaultPharmacodeModule, 1f)
		{
		}

		public StiPharmacodeBarCodeType(float module, float height)
		{
			this.module = module;
			this.height = height;
		}
	}
}