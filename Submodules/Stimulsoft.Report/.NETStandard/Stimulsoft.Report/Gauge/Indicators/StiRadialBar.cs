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
using Stimulsoft.Base.Drawing;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Report.Components.Gauge.Primitives;
using Stimulsoft.Report.Gauge;
using Stimulsoft.Report.Gauge.Helpers;
using Stimulsoft.Report.Painters;
using Stimulsoft.Report.PropertyGrid;
using System.ComponentModel;
using System.Drawing;

namespace Stimulsoft.Report.Components.Gauge
{
    public class StiRadialBar : StiBarBase
    {
        #region IStiJsonReportObject.override
        public override JObject SaveToJsonObject(StiJsonSaveMode mode)
        {
            var jObject = base.SaveToJsonObject(mode);

            jObject.AddPropertyIdent("Ident", this.GetType().Name);
            
            return jObject;
        }

        public override void LoadFromJsonObject(JObject jObject)
        {
            base.LoadFromJsonObject(jObject);
        }
        #endregion

        #region IStiPropertyGridObject
        [Browsable(false)]
        public override StiComponentId ComponentId => StiComponentId.StiRadialBar;

        public override StiPropertyCollection GetProperties(IStiPropertyGrid propertyGrid, StiLevel level)
        {
            var propHelper = propertyGrid.PropertiesHelper;
            var checkBoxHelper = new StiPropertyCollection();

            // ValueCategory
            var list = new[]
            {
                propHelper.Value()
            };
            checkBoxHelper.Add(StiPropertyCategories.Value, list);

            // IndicatorCategory
            list = new[]
            {
                propHelper.EndWidth(),
                propHelper.Offset(),
                propHelper.Placement(),
                propHelper.StartWidth(),
                propHelper.UseRangeColor()
            };
            checkBoxHelper.Add(StiPropertyCategories.Indicator, list);

            // AppearanceCategory
            list = new[]
            {
                propHelper.Brush(),
                propHelper.BorderBrush(),
                propHelper.BorderWidth(),
                propHelper.EmptyBrush(),
                propHelper.EmptyBorderBrush(),
                propHelper.EmptyBorderWidth()
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

        #region IStiApplyStyleGauge
        public override void ApplyStyle(IStiGaugeStyle style)
        {
            if (this.AllowApplyStyle)
            {
                this.Brush = style.Core.RadialBarBrush;
                this.BorderBrush = style.Core.RadialBarBorderBrush;
                this.BorderWidth = style.Core.BorderWidth;

                this.EmptyBrush = style.Core.RadialBarEmptyBrush;
                this.EmptyBorderBrush = style.Core.RadialBarEmptyBorderBrush;
                this.EmptyBorderWidth = style.Core.BorderWidth;

                this.StartWidth = style.Core.RadialBarStartWidth;
                this.EndWidth = style.Core.RadialBarEndWidth;

                CheckActualBrushForTopGeometry();
            }
        }
        #endregion

        #region class RadialColorModeHelper
        private class RadialColorModeHelper
        {
            public bool standardBackground = false;
            public int defaultIndex = -99;

            public void Reset()
            {
                standardBackground = false;
                defaultIndex = -99;
            }
        }
        #endregion

        #region Fields
        private StiBrush actualBush = new StiEmptyBrush();
        private RadialColorModeHelper colorModeHelper = new RadialColorModeHelper();
        #endregion

        #region Properties override
        public override StiGaugeElemenType ElementType => StiGaugeElemenType.RadialElement;

        protected override StiBarRangeListType BarType => StiBarRangeListType.RadialBar;

        public override string LocalizeName => "RadialBar";
        #endregion

        #region Methods.RangeBrush
        protected override void CheckActualBrushForTopGeometry()
        {
            if (this.UseRangeColor)
            {
                //RangeList.Sort(new StiRadialIndicatorComparer());

                for (int index = RangeList.Count - 1; index >= 0; index--)
                {
                    var info = RangeList[index] as StiRadialIndicatorRangeInfo;
                    if (this.ValueObj >= info.Value)
                    {
                        if (colorModeHelper.defaultIndex != index)
                        {
                            colorModeHelper.defaultIndex = index;
                            this.actualBush = info.Brush;
                        }
                        break;
                    }
                }
            }
            else
            {
                colorModeHelper.standardBackground = true;
                this.actualBush = this.Brush;
            }
        }
        #endregion

        #region Methods override

        public override StiGaugeElement CreateNew() => new StiRadialBar();

        protected internal override void DrawElement(StiGaugeContextPainter context)
        {
            var radialScale = this.Scale as StiRadialScale;

            var nullValue = this.GetActualValue();
            float actualValue = (nullValue == null)
                ? Scale.Minimum
                : nullValue.GetValueOrDefault();

            if (context.Gauge.IsDesigning) this.CheckActualBrushForTopGeometry();

            if (radialScale != null)
            {
                float from, to;
                if (Scale.IsReversed)
                {
                    from = Scale.Maximum - actualValue;
                    to = Scale.Maximum;
                }
                else
                {
                    from = Scale.Minimum;
                    to = actualValue;

                    if (to > Scale.Maximum)
                        to = Scale.Maximum;
                }

                GetRangeGeometry(context, radialScale, this.EmptyBrush, this.EmptyBorderBrush, this.EmptyBorderWidth, Scale.Minimum, Scale.Maximum);
                GetRangeGeometry(context, radialScale, this.actualBush, this.BorderBrush, this.BorderWidth, from, to);
            }
        }

        protected override void OnRangeColorChanged()
        {
            this.colorModeHelper.Reset();
        }

        protected internal override void InteractiveClick(RectangleF rect, Point p)
        {

        }
        #endregion

        #region Methods
        private void GetRangeGeometry(StiGaugeContextPainter context, StiRadialScale scale, StiBrush bg, 
            StiBrush bb, float bw, float startValue, float endValue)
        {
            #region Temporary Variables
            var centerPoint = scale.barGeometry.Center;
            float radius = scale.barGeometry.Radius;
            float diameter = scale.barGeometry.Diameter;

            float startWidth, endWidth;
            if (this.StartWidth >= 0)
                startWidth = (scale.IsReversed) ? (this.EndWidth * diameter) : (this.StartWidth * diameter);
            else
                startWidth = 0;

            if (this.EndWidth >= 0)
                endWidth = (scale.IsReversed) ? this.StartWidth * diameter : this.EndWidth * diameter;
            else
                endWidth = 0;

            float startAngle = scale.StartAngle + scale.GetPosition(startValue) * scale.SweepAngle;
            float endAngle = (scale.GetPosition(endValue) - scale.GetPosition(startValue)) * scale.SweepAngle;

            float scaleStartWidth = (scale.IsReversed) ? scale.EndWidth * diameter : scale.StartWidth * diameter;
            float scaleEndWidth = (scale.IsReversed) ? scale.StartWidth * diameter : scale.EndWidth * diameter;
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

            if (float.IsNaN(radius1) || float.IsNaN(radius2) || float.IsNaN(radius3) || float.IsNaN(radius4))
                return;

            float maxRadius = StiMathHelper.GetMax(radius1, radius2, radius3, radius4);
            var finalRect = new RectangleF(centerPoint.X - maxRadius, centerPoint.Y - maxRadius, maxRadius * 2, maxRadius * 2);

            context.AddRadialRangeGaugeGeom(finalRect, bg, bb, bw, centerPoint, 
                startAngle, endAngle, radius1, radius2, radius3, radius4);
        }
        #endregion
    }
}