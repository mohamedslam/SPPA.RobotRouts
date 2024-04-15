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

using Stimulsoft.Base.Context;
using Stimulsoft.Base.Context.Animation;
using Stimulsoft.Base.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Stimulsoft.Report.Chart.Geoms.Series.Pie
{
    public class StiPie3dMainSideSeriesElementGeom : StiSeriesElementGeom
    {
        #region Fields
        private Color borderColor;
        private float startAngle;
        private float sweepAngle;
        private float realStartAngle;
        private float realSweepAngle;
        private int count;
        #endregion

        #region Properties
        public override StiSeriesInteractionData Interaction
        {
            get
            {
                return this.Pie3DSlice.Interaction;
            }
        }

        public StiPie3dSlice Pie3DSlice { get; private set; }
        #endregion

        #region Methods
        public override bool Contains(float px, float py)
        {
            if (Invisible) return false;

            double x = px - this.ClientRectangle.X - this.ClientRectangle.Width / 2;
            double y = py - this.ClientRectangle.Y - this.ClientRectangle.Height / 2;
            double angle = Math.Atan2(y, x);
            if (angle < 0)
                angle += (2 * Math.PI);
            double angleDegrees = angle * 180 / Math.PI;
            // point is inside the pie slice only if between start and end angle
            if ((angleDegrees >= startAngle && angleDegrees <= (startAngle + sweepAngle)) ||
                (startAngle + sweepAngle > 360) && ((angleDegrees + 360) <= (startAngle + sweepAngle)))
            {
                // distance of the point from the ellipse centre
                double r = Math.Sqrt(y * y + x * x);
                return GetEllipseRadius(angle) > r;
            }
            return false;
        }

        private double GetEllipseRadius(double angle)
        {
            double a = this.ClientRectangle.Width / 2;
            double b = this.ClientRectangle.Height / 2;
            double a2 = a * a;
            double b2 = b * b;
            double cosFi = Math.Cos(angle);
            double sinFi = Math.Sin(angle);
            // distance of the ellipse perimeter point
            return (a * b) / Math.Sqrt(b2 * cosFi * cosFi + a2 * sinFi * sinFi);
        }

        public override void Draw(StiContext context)
        {
            var pen = new StiPenGeom(borderColor, 1);

            var path = new List<StiSegmentGeom>();
            path.Add(
                new StiPieSegmentGeom(new RectangleF(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height),
                startAngle, sweepAngle,
                this.realStartAngle, this.realSweepAngle, null)
                );

            context.PushSmoothingModeToAntiAlias();

            var brush = IsSelected
                    ? StiSelectedHelper.GetSelectedBrush(this.SeriesBrush)
                    : this.SeriesBrush;

            var animation = GetAnimation();
            if (animation != null)
            {
                context.DrawAnimationPathElement(brush, brush, pen, path, this.ClientRectangle, this.GetToolTip(), this, animation, GetInteractionData());
            }
            else
            {
                context.FillPath(brush, path, this.ClientRectangle);
                context.DrawPath(pen, path, this.ClientRectangle);
            }

            if (this.IsMouseOver)
                context.FillPath(StiMouseOverHelper.GetMouseOverColor(), path, this.ClientRectangle);

            context.PopSmoothingMode();
        }

        private StiAnimation GetAnimation()
        {
            if (!((StiChart)Series.Chart).IsAnimation) return null;

            var duration = StiChartHelper.GlobalDurationElement;
            var beginTime = StiChartHelper.GlobalBeginTimeElement;

            StiAnimation animation = null;
            StiPieSegmentAnimation pieAnimation = null;

            if (animation == null)
            {
                var beginTimeAnimationOpacity = new TimeSpan(beginTime.Ticks / count * Index);
                if (this.Series.Chart.PreviousAnimations.Count > 0) beginTimeAnimationOpacity = TimeSpan.FromMilliseconds(300);
                var opacityAnimation = new StiOpacityAnimation(duration, beginTimeAnimationOpacity);

                opacityAnimation.AnotherAnimation = pieAnimation;
                opacityAnimation.Id = $"series_{Series.Chart.Series.IndexOf(Series)}_{Index}";

                animation = opacityAnimation;
            }

            return animation;
        }
        #endregion

        public StiPie3dMainSideSeriesElementGeom(StiPie3dSlice pie3DSlice, StiAreaGeom areaGeom, double value, int index, int count, IStiSeries series, RectangleF clientRectangle,
           Color borderColor, StiBrush brush, float startAngle, float sweepAngle)
           : base(areaGeom, value, index, series, clientRectangle, brush)
        {
            this.Pie3DSlice = pie3DSlice;
            this.count = count;
            this.borderColor = borderColor;
            this.realStartAngle = startAngle;
            this.realSweepAngle = sweepAngle;
            this.startAngle = StiPie3dHelper.TransformAngle(this.ClientRectangle, startAngle);
            this.sweepAngle = sweepAngle;

            this.sweepAngle = sweepAngle;

            if (this.sweepAngle % 180 != 0F)
                this.sweepAngle = StiPie3dHelper.TransformAngle(this.ClientRectangle, startAngle + sweepAngle) - this.startAngle;

            if (this.sweepAngle < 0)
                this.sweepAngle += 360;
        }
    }
}
