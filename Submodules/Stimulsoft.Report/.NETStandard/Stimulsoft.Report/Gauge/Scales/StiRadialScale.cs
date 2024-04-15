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

using System;
using System.ComponentModel;
using System.Drawing;
using Stimulsoft.Base.Serializing;
using Stimulsoft.Base.Localization;
using Stimulsoft.Base.Design;
using Stimulsoft.Base;
using Stimulsoft.Report.Components.Gauge.Primitives;
using Stimulsoft.Report.Gauge;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Report.PropertyGrid;

#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
using Stimulsoft.System.Windows.Forms;
#else
using System.Drawing.Design;
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Components.Gauge
{
    public class StiRadialScale : StiScaleBase
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyIdent("Ident", this.GetType().Name);

            jObject.AddPropertyFloat(nameof(Radius), Radius);
            jObject.AddPropertyEnum(nameof(RadiusMode), RadiusMode);
            jObject.AddPropertyPointF(nameof(Center), Center);
            jObject.AddPropertyFloat(nameof(StartAngle), StartAngle, 45f);
            jObject.AddPropertyFloat(nameof(SweepAngle), SweepAngle, 300f);
            jObject.AddPropertyEnum(nameof(Skin), Skin);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(Radius):
                        this.Radius = property.DeserializeFloat();
                        break;

                    case nameof(RadiusMode):
                        this.RadiusMode = property.DeserializeEnum<StiRadiusMode>();
                        break;

                    case nameof(Center):
                        this.Center = property.DeserializePointF();
                        break;

                    case nameof(StartAngle):
                        this.StartAngle = property.DeserializeFloat();
                        break;

                    case nameof(SweepAngle):
                        this.SweepAngle = property.DeserializeFloat();
                        break;

                    case nameof(Skin):
                        this.Skin = property.DeserializeEnum<StiRadialScaleSkin>();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiRadialScale;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var checkBoxHelper = new StiPropertyCollection();

            // ScaleCategory
            var list = new[]
            {
                propHelper.Minimum(),
                propHelper.Maximum(),
                propHelper.MajorInterval(),
                propHelper.MinorInterval(),
                propHelper.StartWidth(),
                propHelper.EndWidth(),
                propHelper.IsReversed(),
                propHelper.StartAngle(),
                propHelper.SweepAngle(),
                propHelper.Radius(),
                propHelper.RadiusMode(),
                propHelper.Skin()
            };
            checkBoxHelper.Add(StiPropertyCategories.Scale, list);

            // PositionCategory
            list = new[]
            {
                propHelper.Center(),
                propHelper.LeftF(),
                propHelper.TopF(),
            };
            checkBoxHelper.Add(StiPropertyCategories.Position, list);

            // AppearanceCategory
            list = new[]
            {
                propHelper.Brush(),
                propHelper.BorderBrush()
            };
            checkBoxHelper.Add(StiPropertyCategories.Appearance, list);

            // MiscCategory
            list = new[]
            {
                propHelper.AllowApplyStyle()
            };
            checkBoxHelper.Add(StiPropertyCategories.Misc, list);

            return checkBoxHelper;
        }

        #endregion

        #region ICloneable
        public override object Clone() => base.Clone();
        #endregion

        #region IStiApplyStyleGauge
        public override void ApplyStyle(IStiGaugeStyle style)
        {
            foreach (StiGaugeElement item in this.Items)
            {
                item.ApplyStyle(style);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets outer radius of the radial scale. Should be from 0 to 1.
        /// </summary>
        [StiSerializable]
        [DefaultValue(0.75f)]
        [StiCategory("Scale")]
        [Description("Gets or sets outer radius of the radial scale. Should be from 0 to 1.")]
        [StiOrder(StiPropertyOrder.ScaleRadius)]
        [StiPropertyLevel(StiLevel.Basic)]
        public float Radius { get; set; } = 0.75f;

        /// <summary>
        /// Gets or sets the mode of calculating the radius RadialScale.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiRadiusMode.Auto)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiCategory("Scale")]
        [StiOrder(StiPropertyOrder.ScaleRadiusMode)]
        [StiPropertyLevel(StiLevel.Basic)]
        [Description("Gets or sets the mode of calculating the radius RadialScale.")]
        public StiRadiusMode RadiusMode { get; set; } = StiRadiusMode.Auto;

        /// <summary>
        /// Gets or sets center of the radial scale.
        /// </summary>
        [StiSerializable]
        [StiPropertyLevel(StiLevel.Basic)]
        [StiCategory("Position")]
        [Description("Gets or sets center of the radial scale.")]
        [TypeConverter(typeof(StiPointFConverter))]
        public PointF Center { get; set; } = new PointF(0.5f, 0.5f);

        /// <summary>
        /// Gets or sets start angle of the radial scale.
        /// </summary>
        [StiSerializable]
        [DefaultValue(45f)]
        [StiCategory("Scale")]
        [Description("Gets or sets start angle of the radial scale.")]
        [StiOrder(StiPropertyOrder.ScaleStartAngle)]
        [StiPropertyLevel(StiLevel.Basic)]
        public float StartAngle { get; set; } = 45f;

        /// <summary>
        /// Gets or sets sweep angle of the radial scale.
        /// </summary>
        [StiSerializable]
        [DefaultValue(300f)]
        [StiCategory("Scale")]
        [Description("Gets or sets sweep angle of the radial scale.")]
        [StiOrder(StiPropertyOrder.ScaleSweepAngle)]
        [StiPropertyLevel(StiLevel.Basic)]
        public float SweepAngle { get; set; } = 300f;

        /// <summary>
        /// Gets or sets the skin of the component rendering.
        /// </summary>
        [StiSerializable]
        [DefaultValue(StiRadialScaleSkin.Default)]
        [TypeConverter(typeof(StiEnumConverter))]
        [Editor(StiEditors.Enum, typeof(UITypeEditor))]
        [StiCategory("Scale")]
        [StiOrder(StiPropertyOrder.ScaleSkin)]
        [StiPropertyLevel(StiLevel.Basic)]
        [Description("Gets or sets the skin of the component rendering.")]
        public StiRadialScaleSkin Skin { get; set; } = StiRadialScaleSkin.Default;
        #endregion

        #region Properties overrider
        public override StiGaugeElemenType ScaleType => StiGaugeElemenType.RadialElement;
        #endregion

        #region Methods
        protected internal float GetRadius()
        {
            return (this.Radius < 0)
                ? 0
                : this.Radius;
        }

        protected internal float GetStartWidth()
        {
            float value = this.StartWidth;

            if (value < 0)
                value = 0;
            else if (value > 1)
                value = 1;

            return value;
        }

        protected internal float GetEndWidth()
        {
            float value = this.EndWidth;

            if (value < 0)
                value = 0;
            else if (value > 1)
                value = 1;

            return value;
        }

        protected internal float GetSweepAngle()
        {
            float value = this.SweepAngle;

            if (value < 0)
                value = 0;
            else if (value > 360)
                value = 360;

            return value;
        }

        internal float GetCurrentAngle(float angle)
        {
            return GetPosition(angle) * this.SweepAngle + this.StartAngle;
        }
        #endregion

        #region Methods override
        protected override void InteractiveClick(MouseEventArgs e)
        {

        }

        public override StiScaleBase CreateNew() => new StiRadialScale();
        #endregion

        public StiRadialScale()
        {
            barGeometry = new StiRadialBarGeometry(this);
        }
    }
}