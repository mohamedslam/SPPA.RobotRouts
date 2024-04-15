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
using Stimulsoft.Base.Context.Animation;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Report.Components.Gauge.Primitives;
using Stimulsoft.Report.Gauge;
using Stimulsoft.Report.Gauge.Helpers;
using Stimulsoft.Report.Painters;
using Stimulsoft.Report.PropertyGrid;
using System;
using System.ComponentModel;
using System.Drawing;

namespace Stimulsoft.Report.Components.Gauge
{
    public class StiRadialMarker : StiMarkerBase
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
        public override StiComponentId ComponentId => StiComponentId.StiRadialMarker;

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
                propHelper.Offset(),
                propHelper.Placement(),
                propHelper.RelativeHeight(),
                propHelper.RelativeWidth(),
                propHelper.Skin(),
            };
            checkBoxHelper.Add(StiPropertyCategories.Indicator, list);

            // TextAdditionalCategory
            list = new[]
            {
                propHelper.Font(),
                propHelper.Format(),
                propHelper.ShowValue(),
                propHelper.TextBrush()
            };
            checkBoxHelper.Add(StiPropertyCategories.TextAdditional, list);

            // AppearanceCategory
            list = new[]
            {
                propHelper.Brush(),
                propHelper.BorderBrush(),
                propHelper.BorderWidth(),
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
                this.Brush = style.Core.MarkerBrush;
                this.BorderBrush = style.Core.MarkerBorderBrush;

                this.BorderWidth = style.Core.MarkerBorderWidth;
                this.Skin = style.Core.MarkerSkin;
            }
        }
        #endregion

        #region Properties override
        public override StiGaugeElemenType ElementType => StiGaugeElemenType.RadialElement;

        public override string LocalizeName => "RadialMarker";
        #endregion

        #region Methods override
        public override StiGaugeElement CreateNew() => new StiRadialMarker();

        protected internal override void DrawElement(StiGaugeContextPainter context)
        {
            var radialScale = this.Scale as StiRadialScale;
            if (radialScale == null) return;

            var nullValue = GetActualValue();
            var valueTemp = (nullValue == null)
                ? 0f
                : nullValue.GetValueOrDefault();

            var value = radialScale.GetPosition(valueTemp);
            var centerPoint = radialScale.barGeometry.Center;
            var actualWidth = Scale.barGeometry.Size.Width * this.RelativeWidth;
            var actualHeight = Scale.barGeometry.Size.Height * this.RelativeHeight;

            var diameter = Scale.barGeometry.Diameter;
            var rest = StiMathHelper.MaxMinusMin(Scale.StartWidth, Scale.EndWidth) * diameter * value;
            var currentRadius = Scale.barGeometry.Radius - (this.Offset * diameter);

            if (this.Placement == StiPlacement.Overlay)
            {
                if (Scale.IsReversed)
                {
                    currentRadius -= (Scale.IsUp) ? ((Scale.EndWidth * diameter + actualWidth + rest) / 2) :
                        ((Scale.EndWidth * diameter + actualWidth - rest) / 2);
                }
                else
                {
                    currentRadius -= (Scale.IsUp) ? ((Scale.StartWidth * diameter + actualWidth + rest) / 2) :
                        ((Scale.StartWidth * diameter + actualWidth - rest) / 2);
                }
            }
            else if (this.Placement == StiPlacement.Inside)
            {
                if (Scale.IsReversed)
                {
                    currentRadius -= (Scale.IsUp) ? ((Scale.EndWidth * diameter) + actualWidth - rest) :
                        ((Scale.EndWidth * diameter) + actualWidth + rest);
                }
                else
                {
                    currentRadius -= (Scale.IsUp) ? ((Scale.StartWidth * diameter) + actualWidth - rest) :
                        ((Scale.StartWidth * diameter) + actualWidth + rest);
                }
            }

            var rect = new RectangleF(currentRadius + centerPoint.X, centerPoint.Y - actualHeight / 2, actualWidth, actualHeight);
            float angle = radialScale.StartAngle + radialScale.GetSweepAngle() * value;

            float rotationAngle = radialScale.StartAngle - angle;

            //IsReversed - ?

            if (this.Scale.Gauge.IsAnimation)
            {
                var animation = new StiRotationAnimation(rotationAngle, 0, centerPoint, StiGaugeHelper.GlobalDurationElement, TimeSpan.Zero);
                animation.Id = "radialMarker" + radialScale.Items.IndexOf(this);
                animation.ApplyPreviousAnimation(context.Gauge.PreviousAnimations);

                this.Animation = animation;
            }

            this.GetActualSkin().Draw(context, this, rect, angle, centerPoint);
        }

        protected internal override void InteractiveClick(RectangleF rect, Point p)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}