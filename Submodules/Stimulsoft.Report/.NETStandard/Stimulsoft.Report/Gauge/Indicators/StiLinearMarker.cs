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
using System.ComponentModel;
using System.Drawing;

#if NETSTANDARD
using Stimulsoft.System.Windows.Forms;
#else
using System.Windows.Forms;
#endif

namespace Stimulsoft.Report.Components.Gauge
{
    public class StiLinearMarker : StiMarkerBase
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
        public override StiComponentId ComponentId => StiComponentId.StiLinearMarker;

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
                propHelper.Skin()
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
                propHelper.BorderWidth()
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
                //this.Skin = style.Core.MarkerSkin;
            }

            if (!StiGaugeV2InitHelper.AllowOldEditor)
            {
                this.BorderBrush = style.Core.LinearMarkerBorder;
            }
        }
        #endregion

        #region Properties override
        public override string LocalizeName => "LinearMarker";
        #endregion

        #region Methods override
        public override StiGaugeElement CreateNew() => new StiLinearMarker();

        protected internal override void DrawElement(StiGaugeContextPainter context)
        {
            var rect = GetRectangle(this.ValueObj);
            var rectZero = GetRectangle(0);

            if (context.Gauge.IsAnimation)
            {
                var animation = new StiTranslationAnimation(new PointF(rectZero.X, rectZero.Y), new PointF(rect.X, rect.Y), StiGaugeHelper.GlobalDurationElement, StiGaugeHelper.GlobalBeginTimeElement);
                animation.Id = "linearMarker_" + Scale.Items.IndexOf(this);
                animation.ApplyPreviousAnimation(context.Gauge.PreviousAnimations);

                this.Animation = animation;
            }

            var skin = this.GetActualSkin();
            skin.Draw(context, this, rect);
        }

        private RectangleF GetRectangle(float value)
        {
            var linearScale = this.Scale as StiLinearScale;

            var size = linearScale.barGeometry.Size;
            var barRect = linearScale.barGeometry.RectGeometry;
            float offset = GetBarPosition(linearScale.Orientation, value);
            float actualWidth = size.Width * this.RelativeWidth;
            float actualHeight = size.Height * this.RelativeHeight;
            float heightDiv2 = actualHeight / 2;
            float x = 0;
            float y = 0;

            if (linearScale.Orientation == Orientation.Horizontal)
            {
                if (this.Placement != StiPlacement.Overlay)
                {
                    float rest = Scale.barGeometry.GetRestToLenght();
                    float position = Scale.GetPosition(value);
                    float restValue = (Scale.StartWidth < Scale.EndWidth) ? ((1 - position) * rest) : (rest * position);

                    y = (this.Placement == StiPlacement.Outside) ? (barRect.Top - actualHeight + restValue) : (barRect.Bottom - restValue);
                }
                else
                {
                    y = StiRectangleHelper.CenterY(Scale.barGeometry.RectGeometry) - heightDiv2 + (this.Offset * size.Height);
                }

                if (Scale.IsReversed)
                {
                    offset += actualWidth / 2;
                    x = barRect.Right - offset;
                }
                else
                {
                    offset -= actualWidth / 2;
                    x = barRect.Left + offset;
                }
            }
            else
            {
                if (this.Placement != StiPlacement.Overlay)
                {
                    float rest = Scale.barGeometry.GetRestToLenght();
                    float position = Scale.GetPosition(value);
                    float restValue = (Scale.StartWidth < Scale.EndWidth) ? ((1 - position) * rest) : (rest * position);

                    x = (this.Placement == StiPlacement.Outside) ? (barRect.Left - actualWidth + restValue) : (barRect.Right - restValue);
                }
                else
                {
                    x = StiRectangleHelper.CenterX(Scale.barGeometry.RectGeometry) - (actualWidth / 2) + (this.Offset * size.Width);
                }

                if (Scale.IsReversed)
                {
                    offset -= heightDiv2;
                    y = barRect.Top + offset;
                }
                else
                {
                    offset += heightDiv2;
                    y = barRect.Bottom - offset;
                }
            }

            return new RectangleF(x, y, actualWidth, actualHeight);
        }

        protected internal override void InteractiveClick(RectangleF rect, Point p)
        {
        }
        #endregion

        #region Methods
        private float GetBarPosition(Orientation orientation, float value)
        {
            float minimum = this.Scale.ScaleHelper.ActualMinimum;
            float maximum = this.Scale.ScaleHelper.ActualMaximum;

            #region Check minimum,maximum
            if (float.IsNaN(value))
            {
                value = minimum;
            }
            else
            {
                if (value < minimum) value = minimum;
                else if (value > maximum) value = maximum;
            }
            #endregion

            float distance = StiMathHelper.Length(minimum, value);
            float width = (orientation == Orientation.Horizontal) ?
                this.Scale.barGeometry.RectGeometry.Width :
                this.Scale.barGeometry.RectGeometry.Height;

            return (distance / this.Scale.ScaleHelper.TotalLength) * width;
        }
        #endregion
    }
}