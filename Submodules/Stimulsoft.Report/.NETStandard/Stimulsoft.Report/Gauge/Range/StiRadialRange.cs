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
using Stimulsoft.Report.Components.Gauge.Primitives;
using Stimulsoft.Report.Gauge;
using Stimulsoft.Report.Gauge.Helpers;
using Stimulsoft.Report.Painters;
using Stimulsoft.Report.PropertyGrid;
using System.ComponentModel;
#if NETSTANDARD
using Stimulsoft.System.Drawing.Design;
#else
using System.Drawing.Design;
#endif

namespace Stimulsoft.Report.Components.Gauge
{
    public class StiRadialRange : StiRangeBase
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyIdent("Ident", this.GetType().Name);

            jObject.AddPropertyBool(nameof(UseValuesFromTheSpecifiedRange), UseValuesFromTheSpecifiedRange, true);

            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);

            foreach (var property in jObject.Properties())
            {
                switch (property.Name)
                {
                    case nameof(UseValuesFromTheSpecifiedRange):
                        this.UseValuesFromTheSpecifiedRange = property.DeserializeBool();
                        break;
                }
            }
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiRadialRange;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var checkBoxHelper = new StiPropertyCollection();

            // ValueCategory
            var list = new[]
            {
                propHelper.EndValue(),
                propHelper.StartValue()
            };
            checkBoxHelper.Add(StiPropertyCategories.Value, list);

            // AppearanceCategory
            list = new[]
            {
                propHelper.Brush(),
                propHelper.BorderBrush(),
                propHelper.BorderWidth()
            };
            checkBoxHelper.Add(StiPropertyCategories.Appearance, list);

            // BehaviorCategory
            list = new[]
            {
                propHelper.EndWidth(),
                propHelper.Offset(),
                propHelper.Placement(),
                propHelper.StartWidth(),
                propHelper.UseValuesFromTheSpecifiedRange()
            };
            checkBoxHelper.Add(StiPropertyCategories.Behavior, list);

            return checkBoxHelper;
        }
        #endregion

        #region Properties
        [DefaultValue(true)]
        [StiSerializable]
        [StiCategory("Behavior")]
        [TypeConverter(typeof(StiBoolConverter))]
        [Editor(StiEditors.Bool, typeof(UITypeEditor))]
        [StiPropertyLevel(StiLevel.Basic)]
        public bool UseValuesFromTheSpecifiedRange { get; set; } = true;
        #endregion

        #region Properties override
        public override string LocalizeName => "RadialRange";
        #endregion

        #region Methods
        protected internal override void DrawRange(StiGaugeContextPainter context, StiScaleBase scale)
        {
            var radialScale = scale as StiRadialScale;
            if (radialScale == null || this.StartWidth == 0 || this.EndWidth == 0) return;

            #region Temporary Variables
            float startValue = this.StartValue;
            if (this.UseValuesFromTheSpecifiedRange && startValue < scale.Minimum) startValue = scale.Minimum;
            else if (startValue > scale.Maximum) return;

            float endValue = this.EndValue;
            if (this.UseValuesFromTheSpecifiedRange && endValue > scale.Maximum) endValue = scale.Maximum;
            else if (endValue < scale.Minimum) return;

            float diameter = scale.barGeometry.Diameter;
            float scaleStartWidth, scaleEndWidth;

            if (scale.IsReversed)
            {
                startValue = scale.Maximum - startValue;
                endValue = scale.Maximum - endValue;

                scaleStartWidth = scale.EndWidth * diameter;
                scaleEndWidth = scale.StartWidth * diameter;
            }
            else
            {
                scaleStartWidth = scale.StartWidth * diameter;
                scaleEndWidth = scale.EndWidth * diameter;
            }

            var rect = scale.barGeometry.RectGeometry;
            var centerPoint = scale.barGeometry.Center;
            float radius = scale.barGeometry.Radius;

            float startWidth = (this.StartWidth >= 0) ? this.StartWidth * diameter : 0;
            float endWidth = (this.EndWidth >= 0) ? this.EndWidth * diameter : 0;
            float startAngle = radialScale.StartAngle + scale.GetPosition(startValue) * radialScale.SweepAngle;
            float endAngle = (scale.GetPosition(endValue) - scale.GetPosition(startValue)) * radialScale.SweepAngle;

            float value1 = scale.GetPosition(startValue);
            float value2 = scale.GetPosition(endValue);

            float scaleRest = StiMathHelper.MaxMinusMin(scaleStartWidth, scaleEndWidth);
            #endregion

            float radius1, radius2, radius3, radius4;

            if (this.Placement == StiPlacement.Outside)
            {
                radius1 = radius;
                radius2 = radius;
                radius3 = radius + startWidth;
                radius4 = radius + endWidth;
            }
            else if (this.Placement == StiPlacement.Overlay)
            {
                if (scale.IsUp)
                {
                    radius1 = radius - ((scaleStartWidth + (scaleRest * value1) - startWidth) / 2);
                    radius2 = radius - ((scaleStartWidth + (scaleRest * value2) - endWidth) / 2);
                    radius3 = radius - ((scaleStartWidth + (scaleRest * value1) + startWidth) / 2);
                    radius4 = radius - ((scaleStartWidth + (scaleRest * value2) + endWidth) / 2);
                }
                else
                {
                    radius1 = radius - ((scaleStartWidth - (scaleRest * value1) - startWidth) / 2);
                    radius2 = radius - ((scaleStartWidth - (scaleRest * value2) - endWidth) / 2);
                    radius3 = radius - ((scaleStartWidth - (scaleRest * value1) + startWidth) / 2);
                    radius4 = radius - ((scaleStartWidth - (scaleRest * value2) + endWidth) / 2);
                }
            }
            else
            {
                if (scale.IsUp)
                {
                    radius1 = radius - (scaleStartWidth + (scaleRest * value1));
                    radius2 = radius - (scaleStartWidth + (scaleRest * value2));
                    radius3 = radius - (scaleStartWidth + (scaleRest * value1) + startWidth);
                    radius4 = radius - (scaleStartWidth + (scaleRest * value2) + endWidth);
                }
                else
                {
                    radius1 = radius - (scaleStartWidth - (scaleRest * value1));
                    radius2 = radius - (scaleStartWidth - (scaleRest * value2));
                    radius3 = radius - (scaleStartWidth - (scaleRest * value1) + startWidth);
                    radius4 = radius - (scaleStartWidth - (scaleRest * value2) + endWidth);
                }
            }

            context.AddRadialRangeGaugeGeom(rect, this.Brush, this.BorderBrush, this.BorderWidth,
                centerPoint, startAngle, endAngle, radius1, radius2, radius3, radius4);
        }
        #endregion

        #region Methods override
        public override StiRangeBase CreateNew() => new StiRadialRange();
        #endregion
    }
}