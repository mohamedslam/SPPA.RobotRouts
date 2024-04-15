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
using Stimulsoft.Base;
using Stimulsoft.Base.Services;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Report.Painters;
using Stimulsoft.Report.PropertyGrid;
using Stimulsoft.Base.Json.Linq;
using System;
using System.Drawing.Design;
using Stimulsoft.Base.Design;

#if NETSTANDARD
using UITypeEditor = Stimulsoft.System.Drawing.Design.UITypeEditor;
#endif

namespace Stimulsoft.Report.Components.ShapeTypes
{
	/// <summary>
	/// The class describes the shape type - Arrow.
	/// </summary>
	[StiServiceBitmap(typeof(StiShapeTypeService), "Stimulsoft.Report.Bmp.ShapeTypes.Arrow.png")]
	[TypeConverter(typeof(Stimulsoft.Report.Components.ShapeTypes.Design.StiArrowShapeTypeConverter))]
    [StiGdiShapeTypePainter(typeof(Stimulsoft.Report.Painters.StiArrowGdiShapeTypePainter))]
    [StiWpfShapeTypePainter("Stimulsoft.Report.Painters.StiArrowWpfShapeTypePainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
	public class StiArrowShapeType : StiShapeTypeService
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiArrowShapeType
            jObject.AddPropertyEnum("Direction", Direction, StiShapeDirection.Up);
            jObject.AddPropertyFloat("ArrowWidth", ArrowWidth, 0.3f);
            jObject.AddPropertyFloat("ArrowHeight", ArrowHeight, 0.4f);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "Direction":
                        this.Direction = property.DeserializeEnum<StiShapeDirection>();
                        break;

                    case "ArrowWidth":
                        this.arrowWidth = property.DeserializeFloat();
                        break;

                    case "ArrowHeight":
                        this.arrowHeight = property.DeserializeFloat();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiArrowShapeType;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            var list = new[]
            {
                propHelper.Direction(),
                propHelper.ArrowWidth(),
                propHelper.ArrowHeight()
            };
            objHelper.Add(StiPropertyCategories.Main, list);

            return objHelper;
        }
        #endregion

        #region Properties.override
        /// <summary>
		/// Gets a service name.
		/// </summary>
		public override string ServiceName => StiLocalization.Get("Shapes", "Arrow");
        #endregion

        #region Properties
		/// <summary>
		/// Gets or sets the arrow direction.
		/// </summary>
		[StiCategory("Behavior")]
		[StiOrder(100)]
		[StiSerializable]
		[TypeConverter(typeof(StiEnumConverter))]
		[Editor(StiEditors.Enum, typeof(UITypeEditor))]
		[Description("Gets or sets the arrow direction.")]
		public virtual StiShapeDirection Direction { get; set; } = StiShapeDirection.Up;

		private float arrowWidth = 0.3f;
		/// <summary>
		/// Gets or sets the arrow width factor.
		/// </summary>
		[DefaultValue(0.3f)]
		[StiCategory("Behavior")]
		[StiOrder(110)]
		[StiSerializable]
        [Description("Gets or sets the arrow width factor.")]
		public virtual float ArrowWidth
		{
			get
			{
				return arrowWidth;
			}
			set
			{
				if (value >= 0 && value <= 0.5f)arrowWidth = value;
			}
		}


		private float arrowHeight = 0.4f;
		/// <summary>
		/// Gets or sets the arrow height factor.
		/// </summary>
		[DefaultValue(0.4f)]
		[StiCategory("Behavior")]
		[StiOrder(120)]
		[StiSerializable]
        [Description("Gets or sets the arrow height factor.")]
		public virtual float ArrowHeight
		{
			get
			{
				return arrowHeight;
			}
			set
			{
				if (value >= 0 && value <= 1f)arrowHeight = value;
			}
		}
        #endregion

        #region Methods.override
        public override StiShapeTypeService CreateNew() => new StiArrowShapeType();
        #endregion

        /// <summary>
		/// Creates a new arrow.
		/// </summary>
		public StiArrowShapeType() : this(StiShapeDirection.Up, 0.3f, 0.4f)
		{
		}

		
		/// <summary>
		/// Creates a new arrow with specified arguments.
		/// </summary>
		/// <param name="direction">Arrow direction.</param>
		/// <param name="arrowWidth">Arrow width factor.</param>
		/// <param name="arrowHeight">Arrow height factor.</param>
		public StiArrowShapeType(StiShapeDirection direction, float arrowWidth, float arrowHeight)
		{
			this.Direction = direction;
			this.arrowWidth = arrowWidth;
			this.arrowHeight = arrowHeight;
		}
	}
}