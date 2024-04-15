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
    /// The class describes the shape type - Octagon.
    /// </summary>
    [StiServiceBitmap(typeof(StiShapeTypeService), "Stimulsoft.Report.Bmp.ShapeTypes.Octagon.png")]
	[TypeConverter(typeof(Stimulsoft.Report.Components.ShapeTypes.Design.StiOctagonShapeTypeConverter))]
    [StiGdiShapeTypePainter(typeof(Stimulsoft.Report.Painters.StiOctagonGdiShapeTypePainter))]
    [StiWpfShapeTypePainter("Stimulsoft.Report.Painters.StiOctagonWpfShapeTypePainter, Stimulsoft.Report.Wpf, " + StiVersion.VersionInfo)]
	public class StiOctagonShapeType : StiShapeTypeService
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            // StiOctagonShapeType
            jObject.AddPropertyBool("AutoSize", AutoSize, true);
            jObject.AddPropertyFloat("Bevel", Bevel, 0f);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case "AutoSize":
                        this.AutoSize = property.DeserializeBool();
                        break;

                    case "Bevel":
                        this.bevel = property.DeserializeFloat();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiOctagonShapeType;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var objHelper = new StiPropertyCollection();

            var list = new[]
            {
                propHelper.AutoSize(),
                propHelper.Bevel()
            };
            objHelper.Add(StiPropertyCategories.Main, list);

            return objHelper;
        }
        #endregion

        #region Properties.override
        /// <summary>
		/// Gets a service name.
		/// </summary>
		public override string ServiceName => StiLocalization.Get("Shapes", "Octagon");
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the octagon autosize.
        /// </summary>
        [DefaultValue(true)]
        [StiCategory("Behavior")]
        [StiOrder(100)]
        [StiSerializable]
        [Description("Gets or sets the octagon autosize.")]
        public virtual bool AutoSize { get; set; } = true;

		private float bevel = 0f;
		/// <summary>
		/// Gets or sets the bevel size.
		/// </summary>
		[DefaultValue(0f)]
		[StiCategory("Behavior")]
		[StiOrder(110)]
		[StiSerializable]
        [Description("Gets or sets the bevel size.")]
        public virtual float Bevel
		{
			get
			{
                return bevel;
			}
			set
			{
                if (value >= 0) bevel = value;
			}
		}
        #endregion

        #region Methods.override
        public override StiShapeTypeService CreateNew() => new StiOctagonShapeType();
        #endregion

        /// <summary>
		/// Creates a new octagon.
		/// </summary>
		public StiOctagonShapeType() : this(true, 0f)
		{
		}

        /// <summary>
        /// Creates a new octagon with specified arguments.
        /// </summary>
        /// <param name="autoSize">Allow autosize</param>
		/// <param name="bevel">Bevel size</param>
        public StiOctagonShapeType(bool autoSize, float bevel)
		{
            this.AutoSize = autoSize;
            this.bevel = bevel;
		}
	}
}