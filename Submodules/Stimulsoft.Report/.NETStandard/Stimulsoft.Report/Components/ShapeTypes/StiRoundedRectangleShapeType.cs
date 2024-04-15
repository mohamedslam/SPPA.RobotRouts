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
using Stimulsoft.Base.Services;
using Stimulsoft.Report.Painters;
using Stimulsoft.Report.PropertyGrid;
using System.ComponentModel;

namespace Stimulsoft.Report.Components.ShapeTypes
{
    /// <summary>
    /// The class describes the shape type - Rounded Rectangle.
    /// </summary>
    [StiServiceBitmap(typeof(StiShapeTypeService), "Stimulsoft.Report.Bmp.ShapeTypes.RoundedRectangle.png")]
	[TypeConverter(typeof(Stimulsoft.Report.Components.ShapeTypes.Design.StiRoundedRectangleShapeTypeConverter))]
    [StiGdiShapeTypePainter(typeof(Stimulsoft.Report.Painters.StiRoundedRectangleGdiShapeTypePainter))]
    [StiWpfShapeTypePainter("Stimulsoft.Report.Painters.StiRoundedRectangleWpfShapeTypePainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
	public class StiRoundedRectangleShapeType : StiShapeTypeService
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiRoundedRectangleShapeType
            jObject.AddPropertyFloat("Round", Round, 0.2f);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Round":
                        this.round = property.DeserializeFloat();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiRoundedRectangleShapeType;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            var list = new[]
            {
                propHelper.ShapeRound()
            };
            objHelper.Add(StiPropertyCategories.Main, list);

            return objHelper;
        }
        #endregion

        #region Properties.override
        /// <summary>
		/// Gets a service name.
		/// </summary>
		public override string ServiceName => StiLocalization.Get("Shapes", "RoundedRectangle");
        #endregion

        #region Properties
        private float round = 0.2f;
		/// <summary>
		/// Gets or sets the factor of rounding.
		/// </summary>
		[StiCategory("Behavior")]
		[StiOrder(150)]
		[StiSerializable]
        [Description("Gets or sets the factor of rounding.")]
		public virtual float Round
		{
			get
			{
				return round;
			}
			set
			{
				if (value > 0 && value <= 0.5)round = value;
			}
		}
        #endregion

        #region Methods.override
        public override StiShapeTypeService CreateNew() => new StiRoundedRectangleShapeType();
        #endregion

        /// <summary>
		/// Creates a new rounded rectangle.
		/// </summary>
		public StiRoundedRectangleShapeType() : this(0.2f)
		{
		}

		/// <summary>
		/// Creates a new rounded rectangle with the specified factor of rounding.
		/// </summary>
		/// <param name="round">Factor of rounding.</param>
		public StiRoundedRectangleShapeType(float round)
		{
			this.round = round;
		}
	}
}